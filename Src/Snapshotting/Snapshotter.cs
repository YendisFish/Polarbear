using System.Data.Common;
using System.Threading.Tasks.Sources;
using Polarbear;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json;

namespace Polarbear.Snapshotting;

public static class Snapshotter
{
    internal static bool isSaving { get; set; } = false;

    internal static void ThreadedSnapshot(PolarbearDB db)
    {
        if(isSaving == false)
        {
            Task t = new(() => Snapshot(db));
            t.Start();
        }
    }

    internal static void ForceSave(PolarbearDB db)
    {
        while(isSaving);
        Thread.Sleep(500);
        Snapshot(db);
    }
    
    public static void Snapshot(PolarbearDB db)
    {
        isSaving = true;
        
        DirectoryInfo dir = (Directory.Exists(db.saveLocation)) ? new DirectoryInfo(db.saveLocation) : /* vv Next Line vv*/
            Directory.CreateDirectory(db.saveLocation);

        FileInfo dbFile = (dir.GetFiles().Where(x => x.Name == "polarbear.bson").Count() > 0) ? /* vv Next Line vv*/
            GetFileByName(dir.GetFiles(), "polarbear.bson") : CreateFileInDir(dir, "polarbear.bson");
        
        FileInfo rvFile = (dir.GetFiles().Where(x => x.Name == "reverse.bson").Count() > 0) ? /* vv Next Line vv*/
            GetFileByName(dir.GetFiles(), "reverse.bson") : CreateFileInDir(dir, "reverse.bson");

        JsonSerializer serializer = new() { TypeNameHandling = TypeNameHandling.All };
        
        using(FileStream stream = dbFile.OpenWrite()) using(BsonDataWriter wrt = new BsonDataWriter(stream))
        {
            serializer.Serialize(wrt, db.dbMap);
        }
        
        using(FileStream stream = rvFile.OpenWrite()) using(BsonDataWriter wrt = new BsonDataWriter(stream))
        {
            serializer.Serialize(wrt, db.reverseLookup);
        }

        isSaving = false;
    }

    internal static FileInfo GetFileByName(FileInfo[] files, string name)
    {
        foreach(FileInfo f in files)
        {
            if(f.Name == name || f.FullName == name)
            {
                return f;
            }
        }

        throw new Exception("File was not found!");
    }

    internal static FileInfo CreateFileInDir(DirectoryInfo dir, string filename)
    {
        File.Create(Path.Join(dir.FullName, filename)).Close();
        return new FileInfo(Path.Join(dir.FullName, filename));
    }

    public static PolarbearDB LoadFrom(string path)
    {
        DirectoryInfo dir = (Directory.Exists(path)) ? new DirectoryInfo(path) : /* vv Next Line vv*/
            Directory.CreateDirectory(path);

        FileInfo dbFile = (dir.GetFiles().Where(x => x.Name == "polarbear.bson").Count() > 0) ? /* vv Next Line vv*/
            GetFileByName(dir.GetFiles(), "polarbear.bson") : CreateFileInDir(dir, "polarbear.bson");
        
        FileInfo rvFile = (dir.GetFiles().Where(x => x.Name == "reverse.bson").Count() > 0) ? /* vv Next Line vv*/
            GetFileByName(dir.GetFiles(), "reverse.bson") : CreateFileInDir(dir, "reverse.bson");

        PolarbearDB db = new();

        using(FileStream fs = dbFile.OpenRead()) using(BsonDataReader rdr = new(fs))
        {
            JsonSerializer serializer = new() { TypeNameHandling = TypeNameHandling.All };
            db.dbMap = serializer.Deserialize<Dictionary<string, IDictionary<string, Enterable>>>(rdr) ?? /* vv Next Line vv*/
                       throw new Exception("Could not load DB from file!");
        }
        
        using(FileStream fs = rvFile.OpenRead()) using(BsonDataReader rdr = new(fs))
        {
            JsonSerializer serializer = new() { TypeNameHandling = TypeNameHandling.All };
            db.reverseLookup = serializer.Deserialize<Dictionary<string, Dictionary<string, List<Enterable>>>>(rdr) ?? /* vv Next Line vv*/
                               throw new Exception("Could not load DB from file!");
        }

        db.saveLocation = path;

        return db;
    }
}