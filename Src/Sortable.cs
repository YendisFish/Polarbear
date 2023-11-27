namespace Polarbear;

public abstract class Sortable : Enterable
{
    public abstract IComparer<string> Comparer { get; init; }
}