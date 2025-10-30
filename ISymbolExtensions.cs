using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SourceGenUtils;

public static class ISymbolExtensions
{
    public static List<AttributeData> GetAttributes<T>(this ISymbol symbol) where T : Attribute
    {
        return symbol
            .GetAttributes()
            .Where
            (
                a => a.AttributeClass?.ContainingNamespace.ToString() == typeof(T).Namespace
                     && a.AttributeClass?.Name == typeof(T).Name
            )
            .ToList();
    }
    
    public static bool HasAttribute<T>(this ISymbol symbol) where T : Attribute
    {
        return symbol.GetAttributes<T>().Any();
    }
}