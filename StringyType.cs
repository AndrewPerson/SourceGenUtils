﻿using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using SourceGenUtils.Collections;

namespace SourceGenUtils;

public readonly record struct StringyType
(
    string? Namespace,
    string Type,
    ImmutableEquatableArray<StringyType> GenericParams
)
{
    public string NamespaceQualifiedType => Namespace == null ? $"global::{GenericType}" : $"global::{Namespace}.{GenericType}";
    public string GenericType => GenericParams.Length == 0 ? Type : $"{Type}<{string.Join(", ", GenericParams.Select(p => p.NamespaceQualifiedType))}>";

    public StringyType(ITypeSymbol type) : this
    (
        type.ContainingNamespace.IsGlobalNamespace ? null : type.ContainingNamespace.ToString(),
        type.Name,
        type is INamedTypeSymbol namedType
            ? namedType.TypeArguments.Select(t => new StringyType(t)).ToImmutableEquatableArray()
            : ImmutableEquatableArray<StringyType>.Empty
    )
    { }

    public override string ToString() => NamespaceQualifiedType;
}