using Polarbear;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json;

namespace Polarbear.Snapshotting;

public class Snapshotter
{
    public static void Snapshot(PolarbearDB db)
    {
        DirectoryInfo dir = (Directory.Exists(db.saveLocation)) ? new DirectoryInfo(db.saveLocation) : /* vv Next Line vv*/
            Directory.CreateDirectory(db.saveLocation);

        FileInfo dbFile = (dir.GetFiles().Where(x => x.Name == "polarbear.bson").Count() > 0) ? /* vv Next Line vv*/
            GetFileByName(dir.GetFiles(), "polarbear.bson") : CreateFileInDir(dir, "polarbear.bson");
        
        FileInfo rvFile = (dir.GetFiles().Where(x => x.Name == "reverse.bson").Count() > 0) ? /* vv Next Line vv*/
            GetFileByName(dir.GetFiles(), "reverse.bson") : CreateFileInDir(dir, "reverse.bson");

        using(FileStream stream = dbFile.OpenWrite()) using(BsonDataWriter wrt = new BsonDataWriter(stream))
        {
            JsonSerializer serializer = new() { TypeNameHandling = TypeNameHandling.All };
            serializer.Serialize(wrt, db.dbMap);
        }
        
        using(FileStream stream = rvFile.OpenWrite()) using(BsonDataWriter wrt = new BsonDataWriter(stream))
        {
            JsonSerializer serializer = new() { TypeNameHandling = TypeNameHandling.All };
            serializer.Serialize(wrt, db.reverseLookup);
        }
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
}