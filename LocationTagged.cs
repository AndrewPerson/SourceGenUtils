using Microsoft.CodeAnalysis;

namespace SourceGenUtils;

public readonly record struct LocationTagged<T>(
    T Value,
    StringyLocation Location
)
{
    public LocationTagged(T value, SyntaxNode node) : this(value, new StringyLocation(node)) { }
}