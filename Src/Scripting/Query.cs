using System.Reflection;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;

namespace Polarbear.Scripting;

public class Query<T> where T : Enterable
{
    internal T obj { get; set; }
    public PolarbearDB db { get; set; }
    
    internal List<QueryClause> query { get; set; }
    internal List<T> outcome { get; set; }

    public Query(List<T> baseObjs, PolarbearDB parentDb)
    {
        db = parentDb;
        query = new();
        outcome = baseObjs;
    }

    public List<T> Result()
    {
        return outcome;
    }

    public void Where(Func<T, bool> condition, T? template = null, params string[] toInclude) // query the database and live compare here
    {
        #warning Polarbear.Scripting.Query.Where() is experimental! Please be carefule while using this function!
        
        List<T> newOutcome = new();

        if(template is not null)
        {
            IEnumerable<PropertyInfo> props = typeof(T).GetProperties().Where(x => toInclude.Contains(x.Name));
            IEnumerable<FieldInfo> fields = typeof(T).GetFields().Where(x => toInclude.Contains(x.Name));
        
            foreach(PropertyInfo prop in props)
            {
                object? o = prop.GetValue(template);
                string stringRep = JsonConvert.SerializeObject(o);
            
                if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
                {
                    break;
                }

                CompareLogic logic = new();
                logic.Config.MembersToInclude = toInclude.ToList();
            
                for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
                {
                    ComparisonResult result = logic.Compare(template, db.reverseLookup[stringRep][typeof(T).Name][i]);
                    if(result.AreEqual)
                    {
                        if(condition(template))
                        {
                            newOutcome.Add((T)db.reverseLookup[stringRep][typeof(T).Name][i]);
                        }
                    }
                }
            }
        
            foreach(FieldInfo prop in fields)
            {
                object? o = prop.GetValue(template);
                string stringRep = JsonConvert.SerializeObject(o);
            
                if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
                {
                    break;
                }

                CompareLogic logic = new();
                logic.Config.MembersToInclude = toInclude.ToList();
            
                for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
                {
                    ComparisonResult result = logic.Compare(template, db.reverseLookup[stringRep][typeof(T).Name][i]);
                    if(result.AreEqual)
                    {
                        if(condition(template))
                        {
                            newOutcome.Add((T)db.reverseLookup[stringRep][typeof(T).Name][i]);
                        }
                    }
                }
            }

            outcome = newOutcome;
        } else {
            IEnumerable<PropertyInfo> props = typeof(T).GetProperties().Where(x => toInclude.Contains(x.Name));
            IEnumerable<FieldInfo> fields = typeof(T).GetFields().Where(x => toInclude.Contains(x.Name));
        
            foreach(PropertyInfo prop in props)
            {
                object? o = prop.GetValue(template);
                string stringRep = JsonConvert.SerializeObject(o);
            
                if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
                {
                    break;
                }

                CompareLogic logic = new();
                logic.Config.MembersToInclude = toInclude.ToList();
            
                for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
                {
                    ComparisonResult result = logic.Compare(template, db.reverseLookup[stringRep][typeof(T).Name][i]);
                    if(result.AreEqual)
                    {
                        if(condition(obj))
                        {
                            newOutcome.Add((T)db.reverseLookup[stringRep][typeof(T).Name][i]);
                        }
                    }
                }
            }
        
            foreach(FieldInfo prop in fields)
            {
                object? o = prop.GetValue(template);
                string stringRep = JsonConvert.SerializeObject(o);
            
                if(!db.reverseLookup.ContainsKey(stringRep) || !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
                {
                    break;
                }

                CompareLogic logic = new();
                logic.Config.MembersToInclude = toInclude.ToList();
            
                for(int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
                {
                    ComparisonResult result = logic.Compare(template, db.reverseLookup[stringRep][typeof(T).Name][i]);
                    if(result.AreEqual)
                    {
                        if(condition(obj))
                        {
                            newOutcome.Add((T)db.reverseLookup[stringRep][typeof(T).Name][i]);
                        }
                    }
                }
            }

            outcome = newOutcome;
        }
    }
}

internal record QueryClause();