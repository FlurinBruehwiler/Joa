using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace JoaKitSourceGenerators;

internal class ComponentInfo : IEquatable<ComponentInfo>
{
    public string Name { get; }
    public string Namespace { get; }
    public IReadOnlyList<(string name, string type)> Parameters { get; }

    public ComponentInfo(ITypeSymbol type)
    {
        Name = type.Name;
        Parameters = GetParameters(type);
        Namespace = GetFullNameSpace(type);
    }

    private static string GetFullNameSpace(ITypeSymbol typeSymbol)
    {
        var currentNamespace = typeSymbol.ContainingNamespace;
        var output = string.Empty;
        
        while (!currentNamespace.IsGlobalNamespace)
        {
            if (output == string.Empty)
            {
                output = currentNamespace.Name;
            }
            else
            {
                output = $"{currentNamespace.Name}.{output}";
            }

            currentNamespace = currentNamespace.ContainingNamespace;
        }

        return output;
    }
    
    private static List<(string, string)> GetParameters(ITypeSymbol type)
    {
        return type.GetMembers().Where(IsParameter).Select(x => (((IPropertySymbol)x).Name, ((IPropertySymbol)x).Type.Name))
            .ToList();
    }
    
    private static bool IsParameter(ISymbol symbol)
    {
        if (symbol.DeclaredAccessibility != Accessibility.Public)
            return false;

        if (symbol is not IPropertySymbol)
            return false;

        if (!symbol.GetAttributes()
                .Any(x => x is
                {
                    AttributeClass:
                    {
                        Name:"ParameterAttribute",
                        ContainingNamespace:
                        {
                            Name:"JoaKit",
                            ContainingNamespace.IsGlobalNamespace: true
                        }
                    }
                }))
            return false;

        return true;
    }
    
    public bool Equals(ComponentInfo other)
    {
        if (ReferenceEquals(null, other))
            return false;
        
        if (ReferenceEquals(this, other))
            return true;
        
        return Name == other.Name && Namespace == other.Namespace && Parameters.SequenceEqual(Parameters);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        
        if (ReferenceEquals(this, obj))
            return true;
        
        if (obj.GetType() != GetType())
            return false;
        
        return Equals((ComponentInfo)obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}