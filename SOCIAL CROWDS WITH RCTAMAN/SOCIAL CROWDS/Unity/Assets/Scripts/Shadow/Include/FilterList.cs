using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class FilterList<T>
{
    public enum ListType { Whitelist, Blacklist };
    public readonly ListType type;

    private readonly HashSet<T> elements;

    public FilterList(ListType type, params T[] entries)
    {
        this.elements = new HashSet<T>(entries);
        this.type = type;
    }

    public bool Allowed(T item)
    {
        return (elements.Contains(item) ^ this.type == ListType.Blacklist);
    }

    public bool IsWhitelist()
    {
        return this.type == ListType.Whitelist;
    }
}

public class Blacklist<T> : FilterList<T>
{
    public Blacklist(params T[] ent) : base(ListType.Blacklist, ent) { }
}

public class Whitelist<T> : FilterList<T>
{
    public Whitelist(params T[] ent) : base(ListType.Whitelist, ent) { }
}
