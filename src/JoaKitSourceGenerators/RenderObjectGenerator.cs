using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JoaKitSourceGenerators;

[Generator]
public class RenderObjectGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var components = context.SyntaxProvider.CreateSyntaxProvider(IsClass, GetClassInfo)
            .Where(info => info is not null)
            .Collect()
            .SelectMany((infos, _) => infos.Distinct());
        
        context.RegisterSourceOutput(components, GenerateCode!);
    }

    private static void GenerateCode(SourceProductionContext ctx, ComponentInfo componentInfo)
    {
        var code = GenerateCode(componentInfo);
        ctx.AddSource($"{componentInfo.Name}Component_generated.cs", code);
    }

    private static string GenerateCode(ComponentInfo componentInfo)
    {
        var newTypeName = componentInfo.Name + "Component";
        
        return $$"""
            #nullable enable

            using {{componentInfo.Namespace}};
            using JoaKit;
            using SkiaSharp;
            using System;
            using System.Runtime.CompilerServices;
            
            namespace {{componentInfo.Namespace}}
            {
                public class {{newTypeName}} : CustomRenderObject
                {
                    {{GetFields(componentInfo.Parameters)}}
           
                    public {{newTypeName}}({{GetConstructorArguments(componentInfo.Parameters)}})
                    {
                        {{GetConstructorBody(componentInfo.Parameters)}}

                        ComponentType = typeof({{componentInfo.Name}});
                    }
            
                    public override RenderObject Build(IComponent component)
                    {
                        {{GetParameterUpdateCalls(componentInfo.Parameters, componentInfo.Name)}}
                        RenderObject = component.Build();
                        PWidth = RenderObject.PWidth;
                        PHeight = RenderObject.PHeight;
                        return RenderObject;
                    }

                    public override void Render(SKCanvas canvas)
                    {
                        RenderObject.Render(canvas);
                    }

                    public {{newTypeName}} Key(string key)
                    {
                        PKey = key;
                        return this;
                    }
                }
            }
            """;
    }

    private static ComponentInfo? GetClassInfo(GeneratorSyntaxContext ctx, CancellationToken cancellationToken)
    {
        if (ctx.Node is not ClassDeclarationSyntax classDeclarationSyntax)
            return null;

        var type = ctx.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) as ITypeSymbol;

        if (type is null)
            return null;
        
        if (!IsUiComponent(type))
            return null;

        return new ComponentInfo(type);
    }

    private static bool IsClass(SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        return syntaxNode is ClassDeclarationSyntax;
    }

    private static bool IsUiComponent(ITypeSymbol type)
    {
        return type.AllInterfaces.Any(x => x is
        {
            Name: "IComponent",
            ContainingNamespace:
            {
                Name: "JoaKit",
                ContainingNamespace.IsGlobalNamespace: true
            }
        });
    }

    private static string GetParameterUpdateCalls(IReadOnlyList<(string name, string type)> parameters, string componentName)
    {
        return string.Join("\n", parameters.Select(x => $"(({componentName})component).{x.name} = _{x.name.ToLowerInvariant()};"));
    }

    private static string GetConstructorBody(IReadOnlyList<(string name, string type)> parameters)
    {
        var defaultBody = "PLineNumber = lineNumer;\n            PFilePath = filePath;";

        if (parameters.Count != 0)
            defaultBody += "\n            ";
        
        return defaultBody + string.Join("\n            ",
            parameters.Select(x => $"_{x.name.ToLowerInvariant()} = {x.name.ToLowerInvariant()};"));
    }

    private static string GetFields(IReadOnlyList<(string name, string type)> parameters)
    {
        return string.Join("\n        ",
            parameters.Select(x => $"private readonly {x.type} _{x.name.ToLowerInvariant()};"));
    }

    private static string GetConstructorArguments(IReadOnlyList<(string name, string type)> parameters)
    {
        var defaultArguments = "[CallerLineNumber] int lineNumer = -1, [CallerFilePath] string filePath = \"\"";

        if (parameters.Count != 0)
            defaultArguments = ", " + defaultArguments;
        
        return string.Join(", ", parameters.Select(x => $"{x.type} {x.name.ToLowerInvariant()}")) + defaultArguments;
    }
}