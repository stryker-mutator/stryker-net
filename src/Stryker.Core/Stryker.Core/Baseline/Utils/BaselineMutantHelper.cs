using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Reporting;

namespace Stryker.Core.Baseline.Utils;

public class BaselineMutantHelper : IBaselineMutantHelper
{
    public IEnumerable<IMutant> GetMutantMatchingSourceCode(IEnumerable<IMutant> mutants, IJsonMutant baselineMutant, string baselineMutantSourceCode)
    {
        return mutants.Where(x =>
           x.Mutation.OriginalNode.ToString() == baselineMutantSourceCode &&
           x.Mutation.DisplayName == baselineMutant.MutatorName);
    }

    public string GetMutantSourceCode(string source, IJsonMutant baselineMutant)
    {
        var tree = CSharpSyntaxTree.ParseText(source);

        var beginLinePosition = new LinePosition(baselineMutant.Location.Start.Line - 1, baselineMutant.Location.Start.Column - 1);
        var endLinePosition = new LinePosition(baselineMutant.Location.End.Line - 1, baselineMutant.Location.End.Column - 1);

        var span = new LinePositionSpan(beginLinePosition, endLinePosition);

        var textSpan = tree.GetText().Lines.GetTextSpan(span);
        var originalNode = tree.GetRoot().DescendantNodes(textSpan).FirstOrDefault(n => textSpan.Equals(n.Span));
        return originalNode?.ToString();
    }
}
