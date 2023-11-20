using System.Collections.Generic;
using DeepCopy;

namespace Polarbear;

public abstract class Enterable
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
}

public static class EnterableExtensions
{
    public static Foreign<T> ToForeign<T>(this T obj) where T : Enterable
    {
        return new(obj.Id);
    }
}