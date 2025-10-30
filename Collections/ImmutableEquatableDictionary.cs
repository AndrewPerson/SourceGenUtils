using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SourceGenUtils.Collections;

public class ImmutableEquatableDictionary<TKey, TValue> : IEquatable<ImmutableEquatableDictionary<TKey, TValue>>, IImmutableDictionary<TKey, TValue>
    where TKey : notnull where TValue : IEquatable<TValue>
{
    public static readonly ImmutableEquatableDictionary<TKey, TValue> Empty = new(ImmutableDictionary<TKey, TValue>.Empty);
    
    public IEnumerable<TKey> Keys => dictionary.Keys;
    public IEnumerable<TValue> Values => dictionary.Values;

    public int Count => dictionary.Count;

    public IEqualityComparer<TKey> KeyComparer => dictionary.KeyComparer;
    public IEqualityComparer<TValue> ValueComparer => dictionary.ValueComparer;

    public TValue this[TKey key] => dictionary[key];

    private readonly ImmutableDictionary<TKey, TValue> dictionary;

    internal ImmutableEquatableDictionary(ImmutableDictionary<TKey, TValue> dictionary)
    {
        this.dictionary = dictionary;
    }

    public ImmutableEquatableDictionary<TKey, TValue> WithComparers(IEqualityComparer<TKey>? keyComparer) => new(dictionary.WithComparers(keyComparer));
    public ImmutableEquatableDictionary<TKey, TValue> WithComparers(IEqualityComparer<TKey>? keyComparer, IEqualityComparer<TValue>? valueComparer)
        => new(dictionary.WithComparers(keyComparer, valueComparer));

    public bool Contains(KeyValuePair<TKey, TValue> pair) => dictionary.Contains(pair);
    public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

    public ImmutableEquatableDictionary<TKey, TValue> Add(TKey key, TValue value) => new(dictionary.Add(key, value));
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Add(TKey key, TValue value) =>
        Add(key, value);
    
    public ImmutableEquatableDictionary<TKey, TValue> AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs) => new(dictionary.AddRange(pairs));
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.AddRange(
        IEnumerable<KeyValuePair<TKey, TValue>> pairs) => AddRange(pairs);

    public ImmutableEquatableDictionary<TKey, TValue> Clear() => new(dictionary.Clear());
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Clear() => Clear();
    
    IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
        dictionary.GetEnumerator();
    
    public ImmutableEquatableDictionary<TKey, TValue> SetItem(TKey key, TValue value) => new(dictionary.SetItem(key, value));
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.SetItem(TKey key, TValue value)
        => SetItem(key, value);
    
    public ImmutableEquatableDictionary<TKey, TValue> SetItems(IEnumerable<KeyValuePair<TKey, TValue>> items) => new(dictionary.SetItems(items));
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.SetItems(
        IEnumerable<KeyValuePair<TKey, TValue>> items) => SetItems(items);
    
    public ImmutableEquatableDictionary<TKey, TValue> Remove(TKey key) => new(dictionary.Remove(key));
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Remove(TKey key) => Remove(key);
    
    public ImmutableEquatableDictionary<TKey, TValue> RemoveRange(IEnumerable<TKey> keys) => new(dictionary.RemoveRange(keys));
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.RemoveRange(IEnumerable<TKey> keys)
        => RemoveRange(keys);

    public bool TryGetKey(TKey equalKey, out TKey actualKey) =>
        dictionary.TryGetKey(equalKey, out actualKey);

#nullable disable
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) =>
#nullable enable
        dictionary.TryGetValue(key, out value);

    public override bool Equals(object? obj) => obj is ImmutableEquatableDictionary<TKey, TValue> dict && Equals(dict);
    public bool Equals(ImmutableEquatableDictionary<TKey, TValue> other)
    {
        if (Count != other.Count) return false;
        if (!Equals(ValueComparer, other.ValueComparer)) return false;

        return this.All(kv => other.TryGetValue(kv.Key, out var otherValue) && ValueComparer.Equals(kv.Value, otherValue));
    }
    
    public override int GetHashCode()
        => dictionary.Aggregate(0, (hash, kv) => hash ^ HashCode.Combine(KeyComparer.GetHashCode(kv.Key), ValueComparer.GetHashCode(kv.Value)));
}

public static class ImmutableEquatableDictionary
{
    public static ImmutableEquatableDictionary<TKey, TValue> ToImmutableEquatableDictionary<TKey, TValue>(
        this IEnumerable<KeyValuePair<TKey, TValue>> self) where TKey : notnull where TValue : IEquatable<TValue>
        => self is ICollection<KeyValuePair<TKey, TValue>> { Count: 0 }
            ? ImmutableEquatableDictionary<TKey, TValue>.Empty
            : new(self.ToImmutableDictionary());
}