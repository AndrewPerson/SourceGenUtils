using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace SourceGenUtils;

public readonly record struct LocationTagged<T>(
    T Value,
    StringyLocation Location
)
{
    public LocationTagged(T value, SyntaxNode node) : this(value, new StringyLocation(node)) { }
    public LocationTagged(T value, SyntaxReference syntaxRef) : this(value, new StringyLocation(syntaxRef)) { }

    public LocationTagged<U> Select<U>(Func<T, U> selector) => new(selector(Value), Location);

    public class ValueComparer(IEqualityComparer<T>? inner) : IEqualityComparer<LocationTagged<T>>
    {
        public IEqualityComparer<T> Inner { get; } = inner ?? EqualityComparer<T>.Default;
        
        public bool Equals(LocationTagged<T> x, LocationTagged<T> y)
        {
            return Inner.Equals(x.Value, y.Value);
        }

        public int GetHashCode(LocationTagged<T> obj)
        {
            return Inner.GetHashCode(obj.Value);
        }
    }
}