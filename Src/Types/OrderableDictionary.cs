using System.Collections;
using System.Collections.Specialized;

namespace Polarbear;

internal class OrderableDictionary<TKey, TVal> : IDictionary<TKey, TVal>
{
    private OrderedDictionary dict { get; set; } = new();
    public int Count => dict.Count;
    public bool IsReadOnly => dict.IsReadOnly;
    public ICollection<TKey> Keys => (ICollection<TKey>)dict.Keys;
    public ICollection<TVal> Values => (ICollection<TVal>)dict.Values;

    public TVal this[TKey key]
    {
        get => (TVal)dict[key ?? throw new NullReferenceException()]!;
        set => dict[key ?? throw new NullReferenceException()] = value;
    }

    public IEnumerator<KeyValuePair<TKey, TVal>> GetEnumerator()
    {
        foreach(DictionaryEntry entry in dict)
        {
            yield return new KeyValuePair<TKey, TVal>((TKey)entry.Key, (TVal)entry.Value!);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(TKey key, TVal val) => dict.Add(key!, val);
    public void Add(KeyValuePair<TKey, TVal> kvp) => dict.Add(kvp.Key!, kvp.Value);
    
    public bool Remove(TKey key) 
    {
        dict.Remove(key!);
        return true; //what is this????
    }

    public bool Remove(KeyValuePair<TKey, TVal> kvp)
    {
        dict.Remove(kvp);
        return true; // what is this????
    }

    public bool ContainsKey(TKey key) => dict.Contains(key!);
    public bool Contains(TKey key) => dict.Contains(key!);
    public bool Contains(KeyValuePair<TKey, TVal> kvp) => dict.Contains(kvp);

    public bool TryGetValue(TKey key, out TVal val)
    {
        try
        {
            if(key is null) { throw new NullReferenceException(); }

            if(dict.Contains(key))
            {
                val = (TVal)dict[key]!;
                return true;
            } else {
                val = default!;
                return false;
            }
        } catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            val = default!;
            return false;
        }
    }

    public void Clear() => dict.Clear();
    public void CopyTo(KeyValuePair<TKey, TVal>[] array, int index) => dict.CopyTo(array, index);
}