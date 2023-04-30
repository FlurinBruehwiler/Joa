using System.Text;
using JoaKit;
using JoaKitSourceGenerators;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using SkiaSharp;
using Xunit.Abstractions;
using VerifyCS = DemoSourceGenTest.SourceGeneratorVerifier<JoaKitSourceGenerators.RenderObjectGenerator>;


namespace DemoSourceGenTest;

public class RenderObjectGeneratorTest
{
    private readonly ITestOutputHelper _output;

    public RenderObjectGeneratorTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Test1()
    {
        var code = """
using JoaKit;
namespace Test;
public class TestComponent : Component
{
    [Parameter]
    public string Test { get; set; }
    
    public override RenderObject Build()
    {
        return new Div();
    }
}
""";
        
        var generated = """
#nullable enable
using Test;
using JoaKit;
using SkiaSharp;
using System;
namespace Test
{
    public class TestComponentComponent : RenderObject
    {
        private readonly String _test;
        public TestComponent UiComponent { get; init; } = null!;
        public RenderObject? RenderObject { get; private set; }
        public TestComponentComponent(String test)
        {
            _test = test;
        }
        public override void Render(SKCanvas canvas)
        {
            UiComponent.Test = _test;
            RenderObject = UiComponent.Render();
        }
    }
}
""";

        var test = new VerifyCS.Test
        {
            TestState =
            {
                Sources = { code },
                GeneratedSources =
                {
                    (typeof(RenderObjectGenerator),
                        "TestComponentComponent_generated.cs",
                        SourceText.From(generated, Encoding.UTF8)),
                },
                ReferenceAssemblies = new ReferenceAssemblies("net7.0", 
                    new PackageIdentity("Microsoft.NETCore.App.Ref", "7.0.0"), 
                    Path.Combine("ref", "7.0.0"))
            },
        };

        test.TestState.AdditionalReferences.Add(typeof(Component).Assembly);
        test.TestState.AdditionalReferences.Add(typeof(SKCanvas).Assembly);
        
        await test.RunAsync();
    }
}