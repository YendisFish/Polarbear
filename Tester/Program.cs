using Newtonsoft.Json;
using Polarbear;
using System.Diagnostics;

PolarbearDB db = new("../../a/");

for(int i = 0; i < 5000; i++)
{
    db.InsertSorted(new ToSort());
}

DatabaseTable<ToSort> sorted = DatabaseTable<ToSort>.GetTable(db);

for(int i = 0; i < sorted.Count; i++)
{
    Console.WriteLine(sorted[i].Id);
}

public class Comparer : IComparer<string>
{
    public int Compare(string x, string y)
    {
        return DateTime.Compare(DateTime.Parse(x), DateTime.Parse(y));
    }
}

class ToSort : Sortable
{
    public override IComparer<string> Comparer { get; init; }

    public ToSort()
    {
        Id = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.ffff");
        this.Comparer = new Comparer();
    }
}