using Newtonsoft.Json;
using Polarbear;

PolarbearDB db = new();

Test t = new Test("Hi!");
Test t1 = new Test("Hi!");
t1.x = 100;

db.Insert(t);

Test[]? tests = db.Query(t1, "Id");
Console.WriteLine(tests.Length);
Console.WriteLine(tests[0].Id);

public class Test : Enterable
{
    public int x = 5;
    
    public Test(string id)
    {
        Id = id;
    }
}