using FSharp.Compiler.Syntax;
using FSharp.Compiler.Text;
using Stryker.Core.Helpers;
using Stryker.Core.Mutants;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.ProjectComponents
{
    public class FsharpFileLeaf : ExcludableProjectComponent<ParsedInput, Range>, IFileLeaf<ParsedInput>
    {
        public string SourceCode { get; set; }

        /// <summary>
        /// The original unmutated syntaxtree
        /// </summary>
        public ParsedInput SyntaxTree { get; set; }

        /// <summary>
        /// The mutated syntax tree
        /// </summary>
        public ParsedInput MutatedSyntaxTree { get; set; }

        public override IEnumerable<Mutant> Mutants { get; set; }

        public override IEnumerable<ParsedInput> CompilationSyntaxTrees => MutatedSyntaxTrees;

        public override IEnumerable<ParsedInput> MutatedSyntaxTrees => new List<ParsedInput> { MutatedSyntaxTree };

        public override IEnumerable<IFileLeaf<ParsedInput>> GetAllFiles()
        {
            yield return this;
        }

        public override void Display()
        {
            DisplayFile(this);
        }

        public override bool IsMatch(FilePattern pattern, MutantSpan span)
        {
            var range = FromMutantSpan(span);

            return pattern.MutantSpans.Any(span => RangeModule.rangeContainsRange(FromMutantSpan(span), range));
        }

        public override IEnumerable<Range> Reduce(IEnumerable<Range> spans)
        {
            // TODO
            return spans;
        }

        public override IEnumerable<Range> RemoveOverlap(IEnumerable<Range> left, IEnumerable<Range> right)
        {
            // TODO
            return left;
        }

        public override MutantSpan ToMutantSpan(Range range)
        {
            var startIndex = RangeHelper.GetIndex(SourceCode, range.Start);
            var endIndex = RangeHelper.GetIndex(SourceCode, range.End);
            return new (startIndex, endIndex);
        }

        public override Range FromMutantSpan(MutantSpan span)
        {
            return RangeHelper.FromBounds(FullPath, SourceCode, span.Start, span.End);
        }
    }
}
