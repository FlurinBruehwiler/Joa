using System.Reflection;
using DemoSourceGen;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit.Abstractions;

namespace DemoSourceGenTest;

public class UnitTest1
{
    private readonly ITestOutputHelper _output;

    public UnitTest1(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void Test1()
    {
        var source = CreateCompilation("""
using JoaKit;
using JoaKit.RenderObjects;
using Modern.WindowKit.Input;

namespace Test;

public class TestComponent : UiComponent
{
    [Parameter]
    public string Test { get; set; }
    
    public override RenderObject Render()
    {
        return new Div()
    }
}
""");
        
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new DemoSourceGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(source, out _, out var diagnostics);
        var runResult = driver.GetRunResult();
        
        foreach (var diagnostic in diagnostics)
        {
            _output.WriteLine(diagnostic.GetMessage());
        }
        
        Assert.NotNull(runResult);
    }
    
    private static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText(source) },
            new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
}