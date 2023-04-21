using Xunit.Abstractions;

namespace JoaKitSourceGeneratorsTest;

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

public class TestComponent : IComponent
{
    [Parameter]
    public string Test { get; set; }
    
    public RenderObject Render()
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

        // var test = new VerifyCS.Test
        // {
        //     TestState =
        //     {
        //         Sources = { code },
        //         GeneratedSources =
        //         {
        //             (typeof(RenderObjectGenerator),
        //                 "TestComponentComponent_generated.cs",
        //                 SourceText.From(generated, Encoding.UTF8)),
        //         },
        //         ReferenceAssemblies = new ReferenceAssemblies("net7.0", 
        //             new PackageIdentity("Microsoft.NETCore.App.Ref", "7.0.0"), 
        //             Path.Combine("ref", "7.0.0"))
        //     },
        // };
        //
        // test.TestState.AdditionalReferences.Add(typeof(IComponent).Assembly);
        // test.TestState.AdditionalReferences.Add(typeof(SKCanvas).Assembly);
        //
        // await test.RunAsync();
    }
}