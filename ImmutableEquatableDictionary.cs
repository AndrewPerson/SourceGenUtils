using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SourceGenUtils;

public class ImmutableEquatableDictionary<TKey, TValue> :
    IEquatable<ImmutableEquatableDictionary<TKey, TValue>>, IImmutableDictionary<TKey, TValue>,
    IDictionary<TKey, TValue>, IDictionary
    where TKey : notnull where TValue : IEquatable<TValue>
{
    public static ImmutableEquatableDictionary<TKey, TValue> Empty = new(ImmutableDictionary<TKey, TValue>.Empty);
    
    ICollection IDictionary.Keys => ((IDictionary)dictionary).Keys;
    ICollection IDictionary.Values => ((IDictionary)dictionary).Values;

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => ((IDictionary<TKey, TValue>)dictionary).Keys;
    ICollection<TValue> IDictionary<TKey, TValue>.Values => ((IDictionary<TKey, TValue>)dictionary).Values;

    public IEnumerable<TKey> Keys => dictionary.Keys;
    public IEnumerable<TValue> Values => dictionary.Values;

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;

    bool IDictionary.IsFixedSize => true;
    bool IDictionary.IsReadOnly => true;

    bool ICollection.IsSynchronized => true;
    object ICollection.SyncRoot { get; } = new();

    public int Count => dictionary.Count;

    object IDictionary.this[object key]
    {
#nullable disable
        get => ((IReadOnlyDictionary<TKey, TValue>)this)[(TKey)key];
#nullable enable

        set => throw new NotImplementedException();
    }

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get => ((IReadOnlyDictionary<TKey, TValue>)this)[key];
        set => throw new NotImplementedException();
    }

    public TValue this[TKey key] => dictionary[key];

    private readonly ImmutableDictionary<TKey, TValue> dictionary;

    internal ImmutableEquatableDictionary(ImmutableDictionary<TKey, TValue> dictionary)
    {
        this.dictionary = dictionary;
    }

    bool IEquatable<ImmutableEquatableDictionary<TKey, TValue>>.Equals(ImmutableEquatableDictionary<TKey, TValue> other)
    {
        if (Count != other.Count) return false;

        return this.All(kv =>
        {
            var otherValue = other[kv.Key];

            return (kv.Value is null && otherValue is null) || (kv.Value?.Equals(otherValue) ?? false);
        });
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => dictionary.Contains(item);
    bool IDictionary.Contains(object key) => ((IDictionary)dictionary).Contains(key);
    bool IImmutableDictionary<TKey, TValue>.Contains(KeyValuePair<TKey, TValue> pair) => dictionary.Contains(pair);

    bool IDictionary<TKey, TValue>.ContainsKey(TKey key) => dictionary.ContainsKey(key);
    bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key) => dictionary.ContainsKey(key);

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => throw new NotImplementedException();

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) =>
        throw new NotImplementedException();

    void IDictionary.Add(object key, object value) => throw new NotImplementedException();

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Add(TKey key, TValue value) =>
        Add(key, value);
    
    public ImmutableEquatableDictionary<TKey, TValue> Add(TKey key, TValue value) => new(dictionary.Add(key, value));

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.AddRange(
        IEnumerable<KeyValuePair<TKey, TValue>> pairs) => AddRange(pairs);
    
    public ImmutableEquatableDictionary<TKey, TValue> AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs) =>
        new(dictionary.AddRange(pairs));

    void ICollection<KeyValuePair<TKey, TValue>>.Clear() => throw new NotImplementedException();
    void IDictionary.Clear() => throw new NotImplementedException();
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Clear() => Clear();
    public ImmutableEquatableDictionary<TKey, TValue> Clear() => new(dictionary.Clear());

    void ICollection.CopyTo(Array array, int index) => ((ICollection)dictionary).CopyTo(array, index);

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
    
    IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)dictionary).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
        dictionary.GetEnumerator();

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.SetItem(TKey key, TValue value)
        => SetItem(key, value);
    
    public ImmutableEquatableDictionary<TKey, TValue> SetItem(TKey key, TValue value) =>
        new(dictionary.SetItem(key, value));

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.SetItems(
        IEnumerable<KeyValuePair<TKey, TValue>> items) => SetItems(items);
    
    public ImmutableEquatableDictionary<TKey, TValue> SetItems(IEnumerable<KeyValuePair<TKey, TValue>> items) =>
        new(dictionary.SetItems(items));
    
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) =>
        throw new NotImplementedException();

    void IDictionary.Remove(object key) => throw new NotImplementedException();
    bool IDictionary<TKey, TValue>.Remove(TKey key) => throw new NotImplementedException();
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Remove(TKey key) => Remove(key);
    public ImmutableEquatableDictionary<TKey, TValue> Remove(TKey key) => new(dictionary.Remove(key));
    
    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.RemoveRange(IEnumerable<TKey> keys)
        => RemoveRange(keys);
    
    public ImmutableEquatableDictionary<TKey, TValue> RemoveRange(IEnumerable<TKey> keys) =>
        new(dictionary.RemoveRange(keys));

    bool IImmutableDictionary<TKey, TValue>.TryGetKey(TKey equalKey, out TKey actualKey) =>
        dictionary.TryGetKey(equalKey, out actualKey);

#nullable disable
    bool IDictionary<TKey, TValue>.TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) =>
#nullable enable
        dictionary.TryGetValue(key, out value);

#nullable disable
    bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) =>
#nullable enable
        dictionary.TryGetValue(key, out value);
    
    public override int GetHashCode()
    {
        return dictionary.Aggregate(0, (hash, kv) => hash ^ kv.GetHashCode());
    }
}

public static class ImmutableEquatableDictionary
{
    public static ImmutableEquatableDictionary<TKey, TValue> ToImmutableEquatableDictionary<TKey, TValue>(
        this IEnumerable<KeyValuePair<TKey, TValue>> self) where TKey : notnull where TValue : IEquatable<TValue>
        => self is ICollection<KeyValuePair<TKey, TValue>> { Count: 0 }
            ? ImmutableEquatableDictionary<TKey, TValue>.Empty
            : new(self.ToImmutableDictionary());
}