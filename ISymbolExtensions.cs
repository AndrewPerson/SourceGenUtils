using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SourceGenUtils;

public static class ISymbolExtensions
{
    public static bool HasAttribute<T>(this ISymbol symbol) where T : Attribute
    {
        return symbol
            .GetAttributes()
            .Any
            (
                a => a.AttributeClass?.ContainingNamespace.ToString() == typeof(T).Namespace
                     && a.AttributeClass?.Name == typeof(T).Name
            );
    }
}