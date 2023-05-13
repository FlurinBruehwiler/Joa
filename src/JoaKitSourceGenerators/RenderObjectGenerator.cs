using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace JoaKitSourceGenerators;

[Generator]
public class RenderObjectGenerator : ISourceGenerator
{
    private INamedTypeSymbol? _actionType;

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            return;

        var parameterAttributeType = context.Compilation.GetTypeByMetadataName("JoaKit.ParameterAttribute");
        var extensionAttributeType = context.Compilation.GetTypeByMetadataName("JoaKit.ExtensionAttribute");
        var componentInterface = context.Compilation.GetTypeByMetadataName("JoaKit.Component");
        _actionType = context.Compilation.GetTypeByMetadataName(typeof(Action).FullName);

        if (parameterAttributeType is null)
            throw new Exception("JoaKit.ParameterAttribute not found");

        if (extensionAttributeType is null)
            throw new Exception("JoaKit.InheritAttribute not found");

        if (componentInterface is null)
            throw new Exception("JoaKit.Component not found");

        foreach (var classDeclarationSyntax in receiver.Candidates)
        {
            var model = context.Compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);

            if (model.GetDeclaredSymbol(classDeclarationSyntax) is not ITypeSymbol type)
                continue;

            if (!IsUiComponent(type, componentInterface))
                continue;

            var newTypeName = type.Name + "Component";

            var parameters = GetParameters(type, parameterAttributeType);
            var extensions = GetExtensions(type, extensionAttributeType);

            var source = $$"""
            #nullable enable
            using {{type.ContainingNamespace}};
            using JoaKit;
            using SkiaSharp;
            using System;
            using Joa.PluginCore;
            using System.Runtime.CompilerServices;
            using Joa.Settings;
            
            namespace {{type.ContainingNamespace}}
            {
                public class {{newTypeName}} : CustomRenderObject
                {
                    {{GetFields(parameters, extensions)}}
            
                    public {{type.Name}} UiComponent { get; init; } = null!;
            
                    public {{newTypeName}}({{GetConstructorArguments(parameters)}})
                    {
                        {{GetConstructorBody(parameters)}}

                        PLineNumber = lineNumer;
                        PFilePath = filePath;

                        ComponentType = typeof({{type.ToDisplayString()}});
                    }
            
                    public override RenderObject Build(Component component)
                    {
            {{GetParameterUpdateCalls(parameters, extensions, type.ToDisplayString())}}

                        RenderObject = component.Build();
                        PWidth = RenderObject.PWidth;
                        PHeight = RenderObject.PHeight;
                        return RenderObject;
                    }

            {{GetExtensionMethods(extensions, newTypeName)}}

                    public {{newTypeName}} Key(string key)
                    {
                        PKey = key;
                        return this;
                    }
                }
            }
            """;

            context.AddSource($"{newTypeName}_generated.cs", source);
        }
    }

    private string GetExtensionMethods(List<IPropertySymbol> extensions, string newTypeName)
    {
        var output = string.Empty;

        foreach (var property in extensions)
        {
            var name = property.Name.ToLowerInvariant();

            output += $$""" 
                    public {{newTypeName}} {{property.Name}}({{property.Type.ToDisplayString()}} {{name}})
                    {
            {{GetExtensionsBody(property, name)}}
                        return this;
                    }


            """;
        }

        return output;
    }

    private string GetExtensionsBody(IPropertySymbol property, string name)
    {
        var propertyType = (INamedTypeSymbol)property.Type;

        if (IsAction(propertyType))
        {
            return GetActionBody(name);
        }

        if (IsGenericAction(propertyType))
        {
            return GetGenericActionBody(propertyType, name);
        }

        if (IsFunc(propertyType))
        {
            return GetFuncBody(propertyType, name);
        }

        return $"           _{property.Name.ToLowerInvariant()} = {property.Name.ToLowerInvariant()};";
    }

    private string GetActionBody(string name)
    {
        return $$""" 
                    _{{name}} = () => {
                        Component.Parent.StateHasChanged();
                        {{name}}();
                    };
        """;
    }

    private string GetGenericActionBody(INamedTypeSymbol type, string name)
    {
        Dictionary<string, string> parameters = new();

        foreach (var parameter in type.TypeArguments)
        {
            parameters.Add(parameter.ToDisplayString(), "n" + Guid.NewGuid().ToString("N"));
        }

        var parametersString = string.Join(", ", parameters.Select(x => $"{x.Key} {x.Value}"));
        var callBackCallParametersString = string.Join(", ", parameters.Select(x => x.Value));

        return $$""" 
                    _{{name}} = ({{parametersString}}) => {
                        Component.Parent.StateHasChanged();
                        {{name}}({{callBackCallParametersString}});
                    };
        """;
    }

    private string GetFuncBody(INamedTypeSymbol type, string name)
    {
        Dictionary<string, string> parameters = new();

        for (var i = 0; i < type.TypeArguments.Length - 1; i++)
        {
            var parameter = type.TypeArguments[i];
            parameters.Add(parameter.ToDisplayString(), "n" + Guid.NewGuid().ToString("N"));
        }

        var parametersString = string.Join(", ", parameters.Select(x => $"{x.Key} {x.Value}"));
        var callBackCallParametersString = string.Join(", ", parameters.Select(x => x.Value));

        return $$""" 
                    _{{name}} = ({{parametersString}}) => {
                        Component.Parent.StateHasChanged();
                        return {{name}}({{callBackCallParametersString}});
                    };
        """;
    }

    private bool IsFunc(INamedTypeSymbol typeSymbol)
    {
        if (!typeSymbol.IsGenericType)
            return false;

        return typeSymbol.Name == "Func" && typeSymbol.ContainingNamespace.ToString() == "System";
    }

    private bool IsAction(INamedTypeSymbol typeSymbol)
    {
        return SymbolEqualityComparer.Default.Equals(typeSymbol, _actionType);
    }

    private bool IsGenericAction(INamedTypeSymbol typeSymbol)
    {
        if (!typeSymbol.IsGenericType)
            return false;

        return typeSymbol.Name == "Action" && typeSymbol.ContainingNamespace.ToString() == "System";
    }


    private static string GetParameterUpdateCalls(List<IPropertySymbol> parameters,
        List<IPropertySymbol> extensions, string typeName)
    {
        var updates = new List<IPropertySymbol>();
        updates.AddRange(parameters);
        updates.AddRange(extensions);

        return string.Join("\n\n",
            updates.Select(x => $$"""
                        if(_{{x.Name.ToLowerInvariant()}} != default)
                        {
                            (({{typeName}})component).{{x.Name}} = _{{x.Name.ToLowerInvariant()}};        
                        }
            """));
    }

    private static string GetConstructorBody(List<IPropertySymbol> parameters)
    {
        return string.Join("\n",
            parameters.Select(x => $"_{x.Name.ToLowerInvariant()} = {x.Name.ToLowerInvariant()};"));
    }

    private static string GetFields(List<IPropertySymbol> parameters, List<IPropertySymbol> extensions)
    {
        var fields = string.Join("\n",
            parameters.Select(x => $"        private readonly {x.Type.ToDisplayString()} _{x.Name.ToLowerInvariant()};"));

        fields += "\n";

        fields += string.Join("\n",
            extensions.Select(x => $"        private {x.Type.ToDisplayString()} _{x.Name.ToLowerInvariant()};"));

        return fields;
    }

    private static string GetConstructorArguments(List<IPropertySymbol> parameters)
    {
        var arguments = "[CallerLineNumber] int lineNumer = -1, [CallerFilePath] string filePath = \"\"";

        if (parameters.Count != 0)
        {
            arguments = ", " + arguments;
            var extraArguments =
                string.Join(", ", parameters.Select(x => $"{x.Type.ToDisplayString()} {x.Name.ToLowerInvariant()}"));
            arguments = extraArguments + arguments;
        }

        return arguments;
    }

    private static List<IPropertySymbol> GetParameters(ITypeSymbol type, INamedTypeSymbol parameterAttributeType)
    {
        return type.GetMembers().Where(x => HasAttributeOfType(x, parameterAttributeType))
            .Select(x => (IPropertySymbol)x)
            .ToList();
    }

    private List<IPropertySymbol> GetExtensions(ITypeSymbol type, INamedTypeSymbol extensionAttributeType)
    {
        return type.GetMembers().Where(x => HasAttributeOfType(x, extensionAttributeType))
            .Select(x => (IPropertySymbol)x)
            .ToList();
    }

    private static bool HasAttributeOfType(ISymbol symbol, INamedTypeSymbol attributeType)
    {
        if (symbol.DeclaredAccessibility != Accessibility.Public)
            return false;

        if (symbol is not IPropertySymbol)
            return false;

        if (!symbol.GetAttributes()
                .Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, attributeType)))
            return false;

        return true;
    }

    private static bool IsUiComponent(ITypeSymbol type, INamedTypeSymbol componentClass)
    {
        return SymbolEqualityComparer.Default.Equals(type.BaseType, componentClass);
    }
}