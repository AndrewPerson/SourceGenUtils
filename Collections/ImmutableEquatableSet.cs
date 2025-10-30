using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SourceGenUtils.Collections;

public class ImmutableEquatableSet<T> : IImmutableSet<T>, IEquatable<ImmutableEquatableSet<T>> where T : IEquatable<T>
{
    public static readonly ImmutableEquatableSet<T> Empty = new(ImmutableHashSet<T>.Empty);
    
    private readonly ImmutableHashSet<T> set;
    
    public int Count => set.Count;

    internal ImmutableEquatableSet(ImmutableHashSet<T> set)
    {
        this.set = set;
    }

    IImmutableSet<T> IImmutableSet<T>.Clear() => Clear();
    public ImmutableEquatableSet<T> Clear() => new(set.Clear());

    public bool Contains(T value) => set.Contains(value);

    IImmutableSet<T> IImmutableSet<T>.Add(T value) => Add(value);
    public ImmutableEquatableSet<T> Add(T value) => new(set.Add(value));

    IImmutableSet<T> IImmutableSet<T>.Remove(T value) => Remove(value);
    public ImmutableEquatableSet<T> Remove(T value) => new(set.Remove(value));

    public bool TryGetValue(T equalValue, out T actualValue) => set.TryGetValue(equalValue, out actualValue);

    IImmutableSet<T> IImmutableSet<T>.Intersect(IEnumerable<T> other) => Intersect(other);
    public ImmutableEquatableSet<T> Intersect(IEnumerable<T> other) => new(set.Intersect(other));

    IImmutableSet<T> IImmutableSet<T>.Except(IEnumerable<T> other) => Except(other);
    public ImmutableEquatableSet<T> Except(IEnumerable<T> other) => new(set.Except(other));

    IImmutableSet<T> IImmutableSet<T>.SymmetricExcept(IEnumerable<T> other) => SymmetricExcept(other);
    public ImmutableEquatableSet<T> SymmetricExcept(IEnumerable<T> other) => new(set.SymmetricExcept(other));

    IImmutableSet<T> IImmutableSet<T>.Union(IEnumerable<T> other) => Union(other);
    public ImmutableEquatableSet<T> Union(IEnumerable<T> other) => new(set.Union(other));

    public bool SetEquals(IEnumerable<T> other) => set.SetEquals(other);
    public bool IsProperSubsetOf(IEnumerable<T> other) => set.IsProperSubsetOf(other);
    public bool IsProperSupersetOf(IEnumerable<T> other) => set.IsProperSupersetOf(other);
    public bool IsSubsetOf(IEnumerable<T> other) => set.IsSubsetOf(other);
    public bool IsSupersetOf(IEnumerable<T> other) => set.IsSupersetOf(other);
    public bool Overlaps(IEnumerable<T> other) => set.Overlaps(other);
    
    public IEnumerator<T> GetEnumerator() => set.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)set).GetEnumerator();

    public bool Equals(ImmutableEquatableSet<T> other) => set.SetEquals(other);
    public override bool Equals(object? obj) => obj is ImmutableEquatableSet<T> otherSet && Equals(otherSet);

    public override int GetHashCode() => set.Aggregate(0, (hash, v) => hash ^ v.GetHashCode());
}

public static class ImmutableEquatableSet
{
    public static ImmutableEquatableSet<T> ToImmutableEquatableSet<T>(this IEnumerable<T> values)
        where T : IEquatable<T>
        => values is ICollection<T> { Count: 0 }
            ? ImmutableEquatableSet<T>.Empty
            : new(values.ToImmutableHashSet());
}