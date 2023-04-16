using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace DemoSourceGen;

[Generator]
public class RenderObjectGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            return;

        var parameterAttributeType = context.Compilation.GetTypeByMetadataName("JoaKit.ParameterAttribute");
        var componentInterface = context.Compilation.GetTypeByMetadataName("JoaKit.IComponent");
        
        foreach (var classDeclarationSyntax in receiver.Candidates)
        {
            var model = context.Compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);

            if (model.GetDeclaredSymbol(classDeclarationSyntax) is not ITypeSymbol type)
                continue;

            if (!IsUiComponent(type, componentInterface))
                continue;

            var newTypeName = type.Name + "Component";

            var parameters = GetParameters(type, parameterAttributeType);

            var source = $$"""
            #nullable enable

            using {{type.ContainingNamespace}};
            using JoaKit;
            using SkiaSharp;
            using System;
            
            namespace {{type.ContainingNamespace}}
            {
                public class {{newTypeName}} : RenderObject
                {
                    {{GetFields(parameters)}}
            
                    public {{type.Name}} UiComponent { get; init; } = null!;
                    public RenderObject? RenderObject { get; private set; }
            
                    public {{newTypeName}}({{GetConstructorArguments(parameters)}})
                    {
                        {{GetConstructorBody(parameters)}}
                    }
            
                    public override void Render(SKCanvas canvas)
                    {
                        {{GetParameterUpdateCalls(parameters)}}
                        RenderObject = UiComponent.Render();
                    }

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

    private static string GetParameterUpdateCalls(List<IPropertySymbol> parameters)
    {
        return string.Join("\n", parameters.Select(x => $"UiComponent.{x.Name} = _{x.Name.ToLowerInvariant()};"));
    }

    private static string GetConstructorBody(List<IPropertySymbol> parameters)
    {
        return string.Join("\n",
            parameters.Select(x => $"_{x.Name.ToLowerInvariant()} = {x.Name.ToLowerInvariant()};"));
    }

    private static string GetFields(List<IPropertySymbol> parameters)
    {
        return string.Join("\n",
            parameters.Select(x => $"private readonly {x.Type.Name} _{x.Name.ToLowerInvariant()};"));
    }

    private static string GetConstructorArguments(List<IPropertySymbol> parameters)
    {
        return string.Join(", ", parameters.Select(x => $"{x.Type.Name} {x.Name.ToLowerInvariant()}"));
    }

    private static List<IPropertySymbol> GetParameters(ITypeSymbol type, INamedTypeSymbol parameterAttributeType)
    {
        return type.GetMembers().Where(x => IsParameter(x, parameterAttributeType)).Select(x => (IPropertySymbol)x).ToList();
    }

    private static bool IsParameter(ISymbol symbol, INamedTypeSymbol parameterAttributeType)
    {
        if (symbol.DeclaredAccessibility != Accessibility.Public)
            return false;

        if (symbol is not IPropertySymbol)
            return false;

        if (!symbol.GetAttributes().Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, parameterAttributeType)))
            return false;
        
        return true;
    }

    private static bool IsUiComponent(ITypeSymbol type, INamedTypeSymbol componentInterface)
    {
        return type.AllInterfaces.Any(x => SymbolEqualityComparer.Default.Equals(x, componentInterface));
    }
}