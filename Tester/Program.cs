using Newtonsoft.Json;
using Polarbear;
using System.Diagnostics;

PolarbearDB db = new("./save/");
Stopwatch watch = new();

Console.WriteLine("Loaded");

db.Insert(new Test(Guid.NewGuid().ToString()));

/*
watch.Start();
db.Snapshot();
watch.Stop();*/

//Console.WriteLine(watch.Elapsed.Seconds);

for(int i = 0; i < 5000000; i++)
{
    db.Insert(new Enterable());
    
    watch.Start();
    //db.Snapshot();
    watch.Stop();
    
    //Console.WriteLine(watch.Elapsed.Microseconds);
    watch.Reset();
}

Console.WriteLine("Finished Inserts");
db.Snapshot();

public class Test : Enterable
{
    public int x = 5;
    public byte[] y = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
    
    public Test(string id)
    {
        Id = id;
    }
}