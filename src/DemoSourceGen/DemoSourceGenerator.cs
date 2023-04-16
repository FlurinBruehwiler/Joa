using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace DemoSourceGen;

[Generator]
public class DemoSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new DemoSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not DemoSyntaxReceiver receiver)
            return;

        foreach (var classDeclarationSyntax in receiver.Candidates)
        {
            var model = context.Compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);

            if (model.GetDeclaredSymbol(classDeclarationSyntax) is not ITypeSymbol type)
                continue;

            Console.WriteLine("Found Type: " + type.Name);

            if (!IsUiComponent(type))
                continue;

            Console.WriteLine("Found UiComponent: " + type.Name);

            var newTypeName = type.Name + "Component";

            var parameters = GetParameters(type);

            var source = $$"""
            using {{type.ContainingNamespace}};
            using JoaKit;
                
            namespace {{type.ContainingNamespace}}
            {
                public class {{newTypeName}} : RenderObject
                {
                    {{GetFields(parameters)}}

                    public required {{type.Name}} UiComponent { get; init; }
                    public RenderObject? RenderObject { get; private set; }
                    
                    public {{newTypeName}}({{GetConstructorArguments(parameters)}})
                    {
                        {{GetConstructorBody(parameters)}}
                    }

                    public override void Render(SkiaSharp.SKCanvas canvas)
                    {
                        {{GetParameterUpdateCalls(parameters)}}
                        RenderObject = UiComponent.Render();
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

    private static List<IPropertySymbol> GetParameters(ITypeSymbol type)
    {
        return type.GetMembers().Where(IsParameter).Select(x => (IPropertySymbol)x).ToList();
    }

    private static bool IsParameter(ISymbol symbol)
    {
        if (symbol.DeclaredAccessibility != Accessibility.Public)
            return false;

        if (symbol is not IPropertySymbol)
            return false;

        return true;
    }

    private static bool IsUiComponent(ITypeSymbol type)
    {
        return type.BaseType?.MetadataName == "UiComponent";
    }
}