using DeepCopy;

namespace Polarbear;

public struct Foreign<T> where T: Enterable
{
    public string Id { get; set; }

    public Foreign(string id)
    {
        Id = id;
    }

    public Foreign(T obj)
    {
        Id = obj.Id;
    }

    public T? ToEnterable(PolarbearDB db)
    {
        Dictionary<string, Enterable> entries;
        if (db.dbMap.TryGetValue(typeof(T).Name, out entries!))
        {
            Enterable ret;
            if (!entries.TryGetValue(this.Id, out ret!))
            {
                return null;
            }

            return DeepCopier.Copy((T?)ret);
        }

        return null;
    }
    
    public static implicit operator string(Foreign<T> foreign) => foreign.Id;
    public static implicit operator Foreign<T>(string id) => new(id);
    public static implicit operator Foreign<T>(T obj) => new(obj);
}