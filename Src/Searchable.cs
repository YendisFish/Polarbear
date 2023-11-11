using System.Reflection;
using DeepCopy;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;

namespace Polarbear;

public class Searchable<T> where T: Enterable
{
    internal PolarbearDB db { get; set; }
    internal List<T> raw { get; set; }

    public T this[int index]
    {
        get
        {
            return raw[index];
        }
    }
    
    public Searchable(PolarbearDB db)
    {
        this.db = db;
        raw = new();
    }

    public IEnumerator<T > GetEnumerator()
    {
        for(int i = 0; i < raw.Count; i++)
        {
            yield return raw[i];
        }
    }

    //simply selects a value from the database based on the template object
    public Searchable<T> Select(T obj, params string[] toInclude)
    {
        List<T> ret = new();

        IEnumerable<PropertyInfo> props = typeof(T).GetProperties().Where(x => toInclude.Contains(x.Name));
        IEnumerable<FieldInfo> fields = typeof(T).GetFields().Where(x => toInclude.Contains(x.Name));
        
        foreach(PropertyInfo prop in props)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            
            if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                return this;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
            {
                ComparisonResult result = logic.Compare(obj, db.reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual)
                {
                    ret.Add(DeepCopier.Copy((T)db.reverseLookup[stringRep][typeof(T).Name][i]));
                }
            }
        }
        
        foreach(FieldInfo prop in fields)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            
            if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                return this;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
            {
                ComparisonResult result = logic.Compare(obj, db.reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual)
                {
                    ret.Add(DeepCopier.Copy((T)db.reverseLookup[stringRep][typeof(T).Name][i]));
                }
            }
        }

        Searchable<T> retr = new(db);
        retr.raw = ret;

        return retr;
    }

    //does the same as select but also applies a custom condition on all objects before adding them to the return values
    public Searchable<T> SelectWhere(Func<T, bool> condition, T obj, params string[] toInclude)
    {
        List<T> ret = new();

        IEnumerable<PropertyInfo> props = typeof(T).GetProperties().Where(x => toInclude.Contains(x.Name));
        IEnumerable<FieldInfo> fields = typeof(T).GetFields().Where(x => toInclude.Contains(x.Name));
        
        foreach(PropertyInfo prop in props)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            
            if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                return this;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
            {
                ComparisonResult result = logic.Compare(obj, db.reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual && condition((T)db.reverseLookup[stringRep][typeof(T).Name][i]))
                {
                    ret.Add(DeepCopier.Copy((T)db.reverseLookup[stringRep][typeof(T).Name][i]));
                }
            }
        }
        
        foreach(FieldInfo prop in fields)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            
            if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                return this;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
            {
                ComparisonResult result = logic.Compare(obj, db.reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual && condition((T)db.reverseLookup[stringRep][typeof(T).Name][i]))
                {
                    ret.Add(DeepCopier.Copy((T)db.reverseLookup[stringRep][typeof(T).Name][i]));
                }
            }
        }

        Searchable<T> retr = new(db);
        retr.raw = ret;

        return retr;
    }

    //select but returns an IEnumerable
    public IEnumerable<T> SelectAsEnumerable(T obj, params string[] toInclude)
    {
        IEnumerable<PropertyInfo> props = typeof(T).GetProperties().Where(x => toInclude.Contains(x.Name));
        IEnumerable<FieldInfo> fields = typeof(T).GetFields().Where(x => toInclude.Contains(x.Name));
        
        foreach(PropertyInfo prop in props)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            
            if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                yield break;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
            {
                ComparisonResult result = logic.Compare(obj, db.reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual)
                {
                    yield return DeepCopier.Copy((T)db.reverseLookup[stringRep][typeof(T).Name][i]);
                }
            }
        }
        
        foreach(FieldInfo prop in fields)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            
            if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                yield break;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
            {
                ComparisonResult result = logic.Compare(obj, db.reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual)
                {
                    yield return DeepCopier.Copy((T)db.reverseLookup[stringRep][typeof(T).Name][i]);
                }
            }
        }
    }
    
    //select where but returns an IEnumerable
    public IEnumerable<T> SelectWhereAsEnumerable(Func<T, bool> condition, T obj, params string[] toInclude)
    {
        IEnumerable<PropertyInfo> props = typeof(T).GetProperties().Where(x => toInclude.Contains(x.Name));
        IEnumerable<FieldInfo> fields = typeof(T).GetFields().Where(x => toInclude.Contains(x.Name));
        
        foreach(PropertyInfo prop in props)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            
            if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                yield break;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
            {
                ComparisonResult result = logic.Compare(obj, db.reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual && condition((T)db.reverseLookup[stringRep][typeof(T).Name][i]))
                {
                    yield return DeepCopier.Copy((T)db.reverseLookup[stringRep][typeof(T).Name][i]);
                }
            }
        }
        
        foreach(FieldInfo prop in fields)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            
            if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                yield break;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
            {
                ComparisonResult result = logic.Compare(obj, db.reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual && condition((T)db.reverseLookup[stringRep][typeof(T).Name][i]))
                {
                    yield return DeepCopier.Copy((T)db.reverseLookup[stringRep][typeof(T).Name][i]);
                }
            }
        }
    }

    //maxes out the return value to a certain number
    public Searchable<T> SelectLimit(int limit, T obj, params string[] toInclude)
    {
        List<T> ret = new();

        IEnumerable<PropertyInfo> props = typeof(T).GetProperties().Where(x => toInclude.Contains(x.Name));
        IEnumerable<FieldInfo> fields = typeof(T).GetFields().Where(x => toInclude.Contains(x.Name));
        
        foreach(PropertyInfo prop in props)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            
            if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                return this;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count && ret.Count <= 5; i++)
            {
                ComparisonResult result = logic.Compare(obj, db.reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual)
                {
                    ret.Add(DeepCopier.Copy((T)db.reverseLookup[stringRep][typeof(T).Name][i]));
                }
            }
        }
        
        foreach(FieldInfo prop in fields)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            
            if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                return this;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count && ret.Count <= 5; i++)
            {
                ComparisonResult result = logic.Compare(obj, db.reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual)
                {
                    ret.Add(DeepCopier.Copy((T)db.reverseLookup[stringRep][typeof(T).Name][i]));
                }
            }
        }

        Searchable<T> retr = new(db);
        retr.raw = ret;

        return retr;
    }

    //gets the index of a value in the database (BY ID ONLY!!!!) and searches the database downwards or upwards
    public Searchable<T> SelectFromLimited(T obj, int limit, SelectFromDirection direction)
    {
        T? safe = db.QueryById(obj);
        Searchable<T> ret = new Searchable<T>(db);
        
        if(safe is null)
        {
            return this;
        }

        switch(direction)
        {
            case SelectFromDirection.DOWN:
            {
                ret.raw = DownwardsSearch(safe, limit);
                break;
            }

            case SelectFromDirection.UP:
            {
                ret.raw = UpwardsSearch(safe, limit);
                break;
            }    
        }

        return ret;
    }

    internal List<T> DownwardsSearch(T safe, int limit)
    {
        List<T> ret = new();
        
        int start = db.dbMap[db.GetTable<T>()].Keys.ToList().IndexOf(safe.Id);
        for(int i = start; i < start + limit; i++)
        {
            ret.Add(DeepCopier.Copy((T)db.dbMap[db.GetTable<T>()].ElementAt(i).Value));
        }

        return ret;
    }
    
    internal List<T> UpwardsSearch(T safe, int limit)
    {
        List<T> ret = new();
        
        int start = db.dbMap[db.GetTable<T>()].Keys.ToList().IndexOf(safe.Id);
        for(int i = start; i > start - limit; i--)
        {
            ret.Add(DeepCopier.Copy((T)db.dbMap[db.GetTable<T>()].ElementAt(i).Value));
        }

        return ret;
    }
}

public enum SelectFromDirection
{
    UP,
    DOWN
}