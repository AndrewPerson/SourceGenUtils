using Microsoft.CodeAnalysis;

namespace SourceGenUtils;

public readonly record struct LocationTagged<T>(
    T Value,
    StringyLocation Location
)
{
    public LocationTagged(T value, SyntaxNode node) : this(value, new StringyLocation(node)) { }
    public LocationTagged(T value, SyntaxReference syntaxRef) : this(value, new StringyLocation(syntaxRef)) { }
}