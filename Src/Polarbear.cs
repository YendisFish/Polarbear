using System.Reflection;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace Polarbear;

public class PolarbearDB
{
    internal Dictionary<string, Dictionary<string, Enterable>> dbMap { get; set; } = new();
    internal Dictionary<string, Dictionary<string, List<Enterable>>> reverseLookup { get; set; } = new();
    
    public void Insert<T>(T obj) where T: Enterable
    {
        if(!dbMap.ContainsKey(typeof(T).Name))
        {
            dbMap.Add(typeof(T).Name, new());
        }
        
        if(!dbMap[typeof(T).Name].ContainsKey(obj.Id))
        {
            dbMap[typeof(T).Name].Add(obj.Id, obj);
        } else {
            dbMap[typeof(T).Name][obj.Id] = obj;
        }
        
        ReverseInsert(obj);
    }

    public T? QueryById<T>(T obj) where T : Enterable
    {
        Dictionary<string, Enterable>? entries = new();
        if (dbMap.TryGetValue(typeof(T).Name, out entries))
        {
            Enterable? ret = null;
            if (!entries.TryGetValue(obj.Id, out ret))
            {
                return null;
            }

            return (T?)ret;
        }

        return null;
    }

    public T[]? Query<T>(T obj, params string[] toInclude) where T: Enterable
    {
        List<T> ret = new();

        IEnumerable<PropertyInfo> props = typeof(T).GetProperties().Where(x => toInclude.Contains(x.Name)).ToArray();
        IEnumerable<FieldInfo> fields = typeof(T).GetFields().Where(x => toInclude.Contains(x.Name)).ToArray();
        
        Console.WriteLine(props.Count());
        Console.WriteLine(fields.Count());
        
        foreach(PropertyInfo prop in props)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            Console.WriteLine(stringRep);
            
            if(!reverseLookup.ContainsKey(stringRep) || !reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                return null;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < reverseLookup[stringRep][typeof(T).Name].Count; i++)
            {
                Console.WriteLine(reverseLookup[stringRep][typeof(T).Name][i]);
                
                ComparisonResult result = logic.Compare(obj, reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual)
                {
                    ret.Add((T)reverseLookup[stringRep][typeof(T).Name][i]);
                }
            }
        }
        
        foreach(FieldInfo prop in fields)
        {
            object? o = prop.GetValue(obj);
            string stringRep = JsonConvert.SerializeObject(o);
            Console.WriteLine(stringRep);
            
            if(!reverseLookup.ContainsKey(stringRep) || !reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                return null;
            }

            CompareLogic logic = new();
            logic.Config.MembersToInclude = toInclude.ToList();
            
            for(int i = 0; i < reverseLookup[stringRep][typeof(T).Name].Count; i++)
            {
                Console.WriteLine(reverseLookup[stringRep][typeof(T).Name][i]);
                
                ComparisonResult result = logic.Compare(obj, reverseLookup[stringRep][typeof(T).Name][i]);
                if(result.AreEqual)
                {
                    ret.Add((T)reverseLookup[stringRep][typeof(T).Name][i]);
                }
            }
        }
        
        return ret.ToArray();
    }

    public void ReverseInsert<T>(T obj) where T: Enterable
    {
        PropertyInfo[] props = typeof(T).GetProperties();
        FieldInfo[] fields = typeof(T).GetFields();
        
        foreach(PropertyInfo prop in props)
        {
            object? o = prop.GetValue(obj);

            string stringRep = JsonConvert.SerializeObject(o);
            Console.WriteLine(stringRep);

            if(!reverseLookup.ContainsKey(stringRep))
            {
                reverseLookup.Add(stringRep, new());
            }

            if(!reverseLookup[stringRep].ContainsKey(typeof(T).Name))
            {
                reverseLookup[stringRep].Add(typeof(T).Name, new());
                reverseLookup[stringRep][typeof(T).Name].Add(obj);
                return;
            }

            bool contains = false;
            for(int i = 0; i < reverseLookup[stringRep][typeof(T).Name].Count; i++)
            {
                if(reverseLookup[stringRep][typeof(T).Name][i].Id == obj.Id)
                {
                    reverseLookup[stringRep][typeof(T).Name][i] = obj;
                    contains = true;
                    break;
                }
            }

            if(!contains)
            {
                reverseLookup[stringRep][typeof(T).Name].Add(obj);
            }
        }
    }

    public void RemoveById()
    {
        
    }

    public void Remove()
    {
        
    }
}
