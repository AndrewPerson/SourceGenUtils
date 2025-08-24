using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SourceGenUtils
{
    public readonly struct StringyType : IEquatable<StringyType>
    {
        public string? Namespace { get; }
        public string Type { get; }
        public StringyType[] GenericParams { get; }

        public string NamespaceQualifiedType => Namespace == null ? $"global::{GenericType}" : $"global::{Namespace}.{GenericType}";
        public string GenericType => GenericParams.Length == 0 ? Type : $"{Type}<{string.Join(", ", GenericParams.Select(p => p.NamespaceQualifiedType))}>";

        public StringyType(string? @namespace, string type, StringyType[] genericParams)
        {
            Namespace = @namespace;
            Type = type;
            GenericParams = genericParams;
        }

        public StringyType(ITypeSymbol type) : this
        (
            type.ContainingNamespace.IsGlobalNamespace ? null : type.ContainingNamespace.ToString(),
            type.Name,
            type is INamedTypeSymbol namedType
                ? namedType.TypeArguments.Select(t => new StringyType(t)).ToArray()
                : new StringyType[0]
        )
        { }

        public override string ToString() => NamespaceQualifiedType;

        public bool Equals(StringyType other)
        {
            return Namespace == other.Namespace
                && Type == other.Type
                && Enumerable.SequenceEqual(GenericParams, other.GenericParams);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Namespace, Type, GenericParams.Aggregate(0, HashCode.Combine));
        }
    }
}