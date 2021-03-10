using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.UnitTest.Reporters
{
    public static class JsonReportTestHelper
    {
        public static IProjectComponent CreateProjectWith(bool duplicateMutant = false, int mutationScore = 60)
        {
            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = Mutator.Arithmetic
            };

            var folder = new CsharpFolderComposite { FullPath = "/home/user/src/project/", RelativePath = "" };
            int mutantCount = 0;
            for (var i = 1; i <= 2; i++)
            {
                var addedFolder = new CsharpFolderComposite
                {
                    RelativePath = $"{i}",
                    FullPath = $"/home/user/src/project/{i}",
                };
                folder.Add(addedFolder);

                for (var y = 0; y <= 4; y++)
                {
                    var m = new Collection<Mutant>();
                    addedFolder.Add(new CsharpFileLeaf()
                    {
                        RelativePath = $"{i}/SomeFile{y}.cs",
                        FullPath = $"/home/user/src/project/{i}/SomeFile{y}.cs",
                        Mutants = m,
                        SourceCode = "void M(){ int i = 0 + 8; }"
                    });

                    for (var z = 0; z <= 5; z++)
                    {
                        m.Add(new Mutant()
                        {
                            Id = duplicateMutant ? 2 : ++mutantCount,
                            ResultStatus = 100 / 6 * z < mutationScore ? MutantStatus.Killed : MutantStatus.Survived,
                            Mutation = mutation,
                            MustRunAgainstAllTests = true
                        });
                    }
                }
            }

            return folder;
        }
    }
}
