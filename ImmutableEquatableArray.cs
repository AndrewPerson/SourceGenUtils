// Based off of https://github.com/thenameless314159/SourceGeneratorUtils/blob/main/src/SourceGeneratorUtils/Infrastructure/ImmutableEquatableArray.cs

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace SourceGenUtils;

[DebuggerDisplay("Length = {Length}")]
[DebuggerTypeProxy(typeof(ImmutableEquatableArray<>.DebugView))]
public sealed class ImmutableEquatableArray<T> :
    IEquatable<ImmutableEquatableArray<T>>, IReadOnlyList<T>, IList<T>, IList
    where T : IEquatable<T>
{
    public static ImmutableEquatableArray<T> Empty { get; } = new([]);

    private readonly T[] values;
    public ref readonly T this[int index] => ref values[index];
    public int Length => values.Length;

    private ImmutableEquatableArray(T[] values)
        => this.values = values;

    public bool Equals(ImmutableEquatableArray<T> other)
        => ReferenceEquals(this, other) || ((ReadOnlySpan<T>)values).SequenceEqual(other.values);

    public override bool Equals(object? obj)
        => obj is ImmutableEquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        int hash = 0;
        foreach (T value in values)
        {
            hash = HashCode.Combine(hash, value?.GetHashCode() ?? 0);
        }

        return hash;
    }

    public Enumerator GetEnumerator() => new(values);

    public struct Enumerator
    {
        private readonly T[] values;
        private int index;

        internal Enumerator(T[] values)
        {
            this.values = values;
            index = -1;
        }

        public bool MoveNext() => ++index < values.Length;
        public readonly ref T Current => ref values[index];
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static ImmutableEquatableArray<T> UnsafeCreateFromArray(T[] values)
        => new(values);

    #region Explicit interface implementations
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)values).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)values).GetEnumerator();
    bool ICollection<T>.IsReadOnly => true;
    bool IList.IsFixedSize => true;
    bool IList.IsReadOnly => true;
    T IReadOnlyList<T>.this[int index] => values[index];

    T IList<T>.this[int index]
    {
        get => values[index];
        set => throw new InvalidOperationException();
    }

    object IList.this[int index]
    {
        get => values[index];
        set => throw new InvalidOperationException();
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => values.CopyTo(array, arrayIndex);
    void ICollection.CopyTo(Array array, int index) => values.CopyTo(array, index);
    int IList<T>.IndexOf(T item) => values.AsSpan().IndexOf(item);
    int IList.IndexOf(object value) => ((IList)values).IndexOf(value);
    bool ICollection<T>.Contains(T item) => values.AsSpan().IndexOf(item) >= 0;
    bool IList.Contains(object? value) => ((IList)values).Contains(value);
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => this;

    int IReadOnlyCollection<T>.Count => Length;
    int ICollection<T>.Count => Length;
    int ICollection.Count => Length;

    void ICollection<T>.Add(T item) => throw new InvalidOperationException();
    bool ICollection<T>.Remove(T item) => throw new InvalidOperationException();
    void ICollection<T>.Clear() => throw new InvalidOperationException();
    void IList<T>.Insert(int index, T item) => throw new InvalidOperationException();
    void IList<T>.RemoveAt(int index) => throw new InvalidOperationException();
    int IList.Add(object? value) => throw new InvalidOperationException();
    void IList.Clear() => throw new InvalidOperationException();
    void IList.Insert(int index, object value) => throw new InvalidOperationException();
    void IList.Remove(object value) => throw new InvalidOperationException();
    void IList.RemoveAt(int index) => throw new InvalidOperationException();
    #endregion

    private sealed class DebugView(ImmutableEquatableArray<T> array)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => array.ToArray();
    }
}

public static class ImmutableEquatableArray
{
    public static ImmutableEquatableArray<T> ToImmutableEquatableArray<T>(this IEnumerable<T> values)
        where T : IEquatable<T>
        => values is ICollection<T> { Count: 0 }
            ? ImmutableEquatableArray<T>.Empty
            : ImmutableEquatableArray<T>.UnsafeCreateFromArray(values.ToArray());

    public static ImmutableEquatableArray<T> Create<T>(ReadOnlySpan<T> values) where T : IEquatable<T>
        => values.IsEmpty
            ? ImmutableEquatableArray<T>.Empty
            : ImmutableEquatableArray<T>.UnsafeCreateFromArray(values.ToArray());
}