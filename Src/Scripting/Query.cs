using System.Reflection;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;

namespace Polarbear.Scripting;

public class Query<T> where T : Enterable
{
    internal T obj { get; set; }
    public PolarbearDB db { get; set; }
    public List<T> Outcome { get; set; }

    public Query(List<T> baseObjs, PolarbearDB parentDb)
    {
        db = parentDb;
        Outcome = baseObjs;
    }

    public void Where(Func<T, bool> condition, T? template = null,
        params string[] toInclude) // query the database and live compare here
    {
#warning Polarbear.Scripting.Query.Where() is experimental! Please be carefule while using this function!

        List<T> newOutcome = new();

        if (template is not null)
        {
            IEnumerable<PropertyInfo> props = typeof(T).GetProperties().Where(x => toInclude.Contains(x.Name));
            IEnumerable<FieldInfo> fields = typeof(T).GetFields().Where(x => toInclude.Contains(x.Name));

            foreach (PropertyInfo prop in props)
            {
                object? o = prop.GetValue(template);
                string stringRep = JsonConvert.SerializeObject(o);

                if (!db.reverseLookup.ContainsKey(stringRep) ||
                    !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
                {
                    break;
                }

                CompareLogic logic = new();
                logic.Config.MembersToInclude = toInclude.ToList();

                for (int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
                {
                    ComparisonResult result = logic.Compare(template, db.reverseLookup[stringRep][typeof(T).Name][i]);
                    if (result.AreEqual)
                    {
                        if (condition(template))
                        {
                            newOutcome.Add((T)db.reverseLookup[stringRep][typeof(T).Name][i]);
                        }
                    }
                }
            }

            foreach (FieldInfo prop in fields)
            {
                object? o = prop.GetValue(template);
                string stringRep = JsonConvert.SerializeObject(o);

                if (!db.reverseLookup.ContainsKey(stringRep) ||
                    !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
                {
                    break;
                }

                CompareLogic logic = new();
                logic.Config.MembersToInclude = toInclude.ToList();

                for (int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
                {
                    ComparisonResult result = logic.Compare(template, db.reverseLookup[stringRep][typeof(T).Name][i]);
                    if (result.AreEqual)
                    {
                        if (condition(template))
                        {
                            newOutcome.Add((T)db.reverseLookup[stringRep][typeof(T).Name][i]);
                        }
                    }
                }
            }

            Outcome = newOutcome;
        }
        else
        {
            IEnumerable<PropertyInfo> props = typeof(T).GetProperties().Where(x => toInclude.Contains(x.Name));
            IEnumerable<FieldInfo> fields = typeof(T).GetFields().Where(x => toInclude.Contains(x.Name));

            foreach (PropertyInfo prop in props)
            {
                object? o = prop.GetValue(template);
                string stringRep = JsonConvert.SerializeObject(o);

                if (!db.reverseLookup.ContainsKey(stringRep) ||
                    !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
                {
                    break;
                }

                CompareLogic logic = new();
                logic.Config.MembersToInclude = toInclude.ToList();

                for (int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
                {
                    ComparisonResult result = logic.Compare(template, db.reverseLookup[stringRep][typeof(T).Name][i]);
                    if (result.AreEqual)
                    {
                        if (condition(obj))
                        {
                            newOutcome.Add((T)db.reverseLookup[stringRep][typeof(T).Name][i]);
                        }
                    }
                }
            }

            foreach (FieldInfo prop in fields)
            {
                object? o = prop.GetValue(template);
                string stringRep = JsonConvert.SerializeObject(o);

                if (!db.reverseLookup.ContainsKey(stringRep) ||
                    !db.reverseLookup[stringRep].ContainsKey(typeof(T).Name))
                {
                    break;
                }

                CompareLogic logic = new();
                logic.Config.MembersToInclude = toInclude.ToList();

                for (int i = 0; i < db.reverseLookup[stringRep][typeof(T).Name].Count; i++)
                {
                    ComparisonResult result = logic.Compare(template, db.reverseLookup[stringRep][typeof(T).Name][i]);
                    if (result.AreEqual)
                    {
                        if (condition(obj))
                        {
                            newOutcome.Add((T)db.reverseLookup[stringRep][typeof(T).Name][i]);
                        }
                    }
                }
            }

            Outcome = newOutcome;
        }
    }

    public void WhereOutcome(Func<T, bool> condition)
    {
        List<T> newOutcome = new();

        foreach(T element in Outcome)
        {
            if(condition(element))
            {
                newOutcome.Add(element);
            }
        }

        Outcome = newOutcome;
    }
}