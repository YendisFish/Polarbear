using Newtonsoft.Json;
using Polarbear;
using System.Diagnostics;

PolarbearDB db = new("./save/");

for(int i = 0; i < 500; i++)
{
    Test t = new();
    db.Insert(t);
}

Test tt = new Test() { Id = "Tester" };
Test t2 = new Test() { Id = "Tester2" };

tt.x = 10;
t2.x = 10;

db.Insert(tt);
db.Insert(t2);

Searchable<Test> s = new(db);
Searchable<Test> s2 = s.SelectFromLimited(t2, 5, SelectFromDirection.UP);

foreach(Test t in s2)
{
    Console.WriteLine(t.Id);
}

public class Test : Enterable
{
    public int x { get; set; } = 5;

    public Test()
    {
        Id = Guid.NewGuid().ToString();
    }
}