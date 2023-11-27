using System.Reflection;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Polarbear.Snapshotting;
using DeepCopy;

namespace Polarbear;

public class PolarbearDB
{
    internal Dictionary<string, IDictionary<string, Enterable>> dbMap { get; set; } = new();
    internal Dictionary<string, Dictionary<string, List<Enterable>>> reverseLookup { get; set; } = new();
    internal string saveLocation { get; set; }

    public PolarbearDB(string save)
    {
        saveLocation = save;
    }

    internal PolarbearDB(Dictionary<string, IDictionary<string, Enterable>> db, Dictionary<string, Dictionary<string, List<Enterable>>> rv, string sl)
    {
        this.dbMap = db;
        this.reverseLookup = rv;
        this.saveLocation = sl;
    }
    
    internal PolarbearDB() { }

    public void Insert<T>(T obj) where T: Enterable
    {
        // perform type checks
        if(typeof(T) == typeof(Orderable))
        {
            throw new NotSupportedException("Insert does not support type Orderable! Please use InsertOrdered!");
        }

        if(typeof(T).IsGenericType)
        {
            throw new NotSupportedException("Generic tables are not supported by Polarbear! Please wrap them in a class!");
        }

        lock(dbMap)
        {
            if(!dbMap.ContainsKey(typeof(T).Name))
            {
                dbMap.Add(typeof(T).Name, new Dictionary<string, Enterable>());
            }
        
            if(!dbMap[typeof(T).Name].ContainsKey(obj.Id))
            {
                dbMap[typeof(T).Name].Add(obj.Id, DeepCopier.Copy(obj));
            } else {
                dbMap[typeof(T).Name][obj.Id] = DeepCopier.Copy(obj);
            }
        
            ReverseInsert(obj);
            
            //Snapshotter.Snapshot(this);
        }
    }

    public void InsertOrdered<T>(T obj) where T: Orderable
    {
        if(typeof(T).IsGenericType)
        {
            throw new NotSupportedException("Generic tables are not supported by Polarbear! Please wrap them in a class!");
        }
        
        lock(dbMap)
        {
            if(!dbMap.ContainsKey(typeof(T).Name))
            {
                dbMap.Add(typeof(T).Name, new OrderableDictionary<string, Enterable>());
            }
        
            if(!dbMap[typeof(T).Name].ContainsKey(obj.Id))
            {
                dbMap[typeof(T).Name].Add(obj.Id, DeepCopier.Copy(obj));
            } else {
                dbMap[typeof(T).Name][obj.Id] = DeepCopier.Copy(obj);
            }
        
            ReverseInsert(obj);
        }
    }

    public void InsertSorted<T>(T obj) where T: Sortable
    {
        if(typeof(T).IsGenericType)
        {
            throw new NotSupportedException("Generic tables are not supported by Polarbear! Please wrap them in a class!");
        }

        lock(dbMap)
        {
            if(!dbMap.ContainsKey(typeof(T).Name))
            {
                dbMap.Add(typeof(T).Name, new SortedDictionary<string, Enterable>(obj.Comparer));
            }
        
            if(!dbMap[typeof(T).Name].ContainsKey(obj.Id))
            {
                dbMap[typeof(T).Name].Add(obj.Id, DeepCopier.Copy(obj));
            } else {
                dbMap[typeof(T).Name][obj.Id] = DeepCopier.Copy(obj);
            }
        
            ReverseInsert(obj);
        }
    }

    public T? QueryById<T>(T obj) where T : Enterable
    {
        IDictionary<string, Enterable> entries;
        if (dbMap.TryGetValue(typeof(T).Name, out entries!))
        {
            Enterable ret;
            if (!entries.TryGetValue(obj.Id, out ret!))
            {
                return null;
            }

            return DeepCopier.Copy((T?)ret);
        }

        return null;
    }

    public T[]? Query<T>(T obj, params string[] toInclude) where T: Enterable
    {
        List<T> ret = new();

        IEnumerable<PropertyInfo> props = typeof(T).GetProperties().Where(x => toInclude.Contains(x.Name));
        IEnumerable<FieldInfo> fields = typeof(T).GetFields().Where(x => toInclude.Contains(x.Name));
        
        foreach(PropertyInfo prop in props)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            
            if(!reverseLookup.ContainsKey(stringRep) || !reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                return null;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < reverseLookup[stringRep][typeof(T).Name].Count; i++)
            {
                ComparisonResult result = logic.Compare(obj, reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual)
                {
                    ret.Add(DeepCopier.Copy((T)reverseLookup[stringRep][typeof(T).Name][i]));
                }
            }
        }
        
        foreach(FieldInfo prop in fields)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            
            if(!reverseLookup.ContainsKey(stringRep) || !reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                return null;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < reverseLookup[stringRep][typeof(T).Name].Count; i++)
            {
                ComparisonResult result = logic.Compare(obj, reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual)
                {
                    ret.Add(DeepCopier.Copy((T)reverseLookup[stringRep][typeof(T).Name][i]));
                }
            }
        }
        
        return ret.ToArray();
    }

    public T[]? QueryAll<T>(string tableName) where T: Enterable
    {
        List<T> ret = new();

        IDictionary<string, Enterable>? table;
        if(!dbMap.TryGetValue(tableName, out table))
        {
            return new T[1];
        }

        foreach(KeyValuePair<string, Enterable> entry in table)
        {
            ret.Add((T)entry.Value);
        }

        return ret.ToArray();
    }

    public string GetTable<T>() where T : Enterable => typeof(T).Name;

    public void ReverseInsert<T>(T obj) where T: Enterable
    {
        lock(reverseLookup)
        {
            PropertyInfo[] props = typeof(T).GetProperties();
            FieldInfo[] fields = typeof(T).GetFields();
        
            foreach(PropertyInfo prop in props)
            {
                object? o = prop.GetValue(obj);
                string stringRep = JsonConvert.SerializeObject(o);

                if(!reverseLookup.ContainsKey(stringRep))
                {
                    reverseLookup.Add(stringRep, new());
                }

                if(!reverseLookup[stringRep].ContainsKey(typeof(T).Name))
                {
                    reverseLookup[stringRep].Add(typeof(T).Name, new());
                    reverseLookup[stringRep][typeof(T).Name].Add(DeepCopier.Copy(obj));
                    return;
                }

                bool contains = false;
                for(int i = 0; i < reverseLookup[stringRep][typeof(T).Name].Count; i++)
                {
                    if(reverseLookup[stringRep][typeof(T).Name][i].Id == obj.Id)
                    {
                        reverseLookup[stringRep][typeof(T).Name][i] = DeepCopier.Copy(obj);
                        contains = true;
                        break;
                    }
                }

                if(!contains)
                {
                    reverseLookup[stringRep][typeof(T).Name].Add(DeepCopier.Copy(obj));
                }
            }
        }
    }

    internal void RemoveById<T>(T obj) where T: Enterable
    {
        lock(dbMap)
        {
            string key = typeof(T).Name;

            if(!dbMap.ContainsKey(key) || !dbMap[key].ContainsKey(obj.Id))
            {
                return;
            }

            dbMap[key].Remove(obj.Id);
        }
    }

    public void Remove<T>(T obj, params string[] toInclude) where T: Enterable
    {
        lock(reverseLookup)
        {
            List<T> toRemove = new();
        
            IEnumerable<PropertyInfo> props = typeof(T).GetProperties().Where(x => toInclude.Contains(x.Name));
            IEnumerable<FieldInfo> fields = typeof(T).GetFields().Where(x => toInclude.Contains(x.Name));
            
            foreach(PropertyInfo prop in props)
            {
                object? o = prop.GetValue(obj);
                string stringRep = JsonConvert.SerializeObject(o);
                
                if(!reverseLookup.ContainsKey(stringRep) || !reverseLookup[stringRep].ContainsKey(typeof(T).Name))
                {
                    break;
                }

                CompareLogic logic = new();
                logic.Config.MembersToInclude = toInclude.ToList();
                
                for(int i = 0; i < reverseLookup[stringRep][typeof(T).Name].Count; i++)
                {
                    ComparisonResult result = logic.Compare(obj, reverseLookup[stringRep][typeof(T).Name][i]);
                    if(result.AreEqual)
                    {
                        toRemove.Add((T)reverseLookup[stringRep][typeof(T).Name][i]);
                        reverseLookup[stringRep][typeof(T).Name].RemoveAt(i);
                    }
                }
            }
            
            foreach(FieldInfo prop in fields)
            {
                object? o = prop.GetValue(obj);
                string stringRep = JsonConvert.SerializeObject(o);
                
                if(!reverseLookup.ContainsKey(stringRep) || !reverseLookup[stringRep].ContainsKey(typeof(T).Name))
                {
                    break;
                }

                CompareLogic logic = new();
                logic.Config.MembersToInclude = toInclude.ToList();
                
                for(int i = 0; i < reverseLookup[stringRep][typeof(T).Name].Count; i++)
                {
                    ComparisonResult result = logic.Compare(obj, reverseLookup[stringRep][typeof(T).Name][i]);
                    if(result.AreEqual)
                    {
                        toRemove.Add((T)reverseLookup[stringRep][typeof(T).Name][i]);
                        reverseLookup[stringRep][typeof(T).Name].RemoveAt(i);
                    }
                }
            }
            
            if(!dbMap.ContainsKey(typeof(T).Name))
            {
                throw new Exception("Data inconsistency!");
            }

            foreach(T removeable in toRemove)
            {
                if(dbMap[typeof(T).Name].ContainsKey(removeable.Id))
                {
                    RemoveById(removeable);
                }
            }
        }
    }

    internal void ReverseTableRemove<T>() where T: Enterable
    {
        lock(reverseLookup)
        {
            foreach(KeyValuePair<string, Dictionary<string, List<Enterable>>> rVal in reverseLookup)
            {
                foreach(KeyValuePair<string, List<Enterable>> entries in rVal.Value)
                {
                    if(entries.Key == typeof(T).Name)
                    {
                        rVal.Value.Remove(entries.Key);
                    }
                }
            }
        }
    }

    public void Drop<T>(T obj) where T: Enterable
    {
        string key = typeof(T).Name;

        if(dbMap.ContainsKey(key))
        {
            dbMap.Remove(key);
        }
        
        ReverseTableRemove<T>();
    }
    
    public void Snapshot()
    {
        Snapshotter.Snapshot(this);
    }
    
    public static PolarbearDB Load(string path)
    {
        return Snapshotter.LoadFrom(path);
    }
}

public class DatabaseTable<T> where T: Enterable
{
    internal IDictionary<string, Enterable> raw { get; set; }
    public int Count => raw.Count;

    public T this[int index]
    {
        get => (T)raw.ElementAt(index).Value;
        set => raw[raw.ElementAt(index).Key] = value;
    }
    
    internal DatabaseTable(IDictionary<string, Enterable> raw)
    {
        this.raw = raw;
    }

    public IEnumerator<KeyValuePair<string, Enterable>> GetEnumerator()
    {
        return raw.GetEnumerator();
    }
    
    public static DatabaseTable<T> GetTable<T>(PolarbearDB db) where T: Enterable
    {
#warning this gives an unsafe handle to the database table! please use with caution because you will be responsible for all thread safety!
        
        IDictionary<string, Enterable> ret;
        if(!db.dbMap.TryGetValue(db.GetTable<T>(), out ret!))
        {
            throw new Exception("Table not found!");
        }

        return new(ret);
    }
}