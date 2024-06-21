using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Generator
{
    [Generator]
    public sealed class ExampleGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx =>
            {
                var sourceText = $$"""
                namespace GeneratedNamespace
                {
                    public static class GeneratedClass
                    {
                        public static int GeneratedMath(int one, int two) => one + two;
                    }
                }
                """;
                ctx.AddSource("ExampleGenerator.g.cs", SourceText.From(sourceText, Encoding.UTF8));
            });
        }
    }
}
