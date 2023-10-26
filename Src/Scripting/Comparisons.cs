using System.Reflection;
using KellermanSoftware.CompareNetObjects;

namespace Polarbear.Scripting;

internal class Comparisons
{
    public bool Compare<T>(T obj, T obj2, params string[] toInclude)
    {
        CompareLogic logic = new();
        logic.Config.MembersToInclude = toInclude.ToList();

        ComparisonResult res = logic.Compare(obj, obj2);

        return res.AreEqual;
    }
}