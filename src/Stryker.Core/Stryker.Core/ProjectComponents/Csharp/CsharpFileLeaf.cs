using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Stryker.Core.Helpers;
using Stryker.Core.Mutants;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.ProjectComponents
{
    public class CsharpFileLeaf : ExcludableProjectComponent<SyntaxTree, TextSpan>, IFileLeaf<SyntaxTree>
    {
        // only needed for tests
        internal CsharpFileLeaf() : base() { }

        public CsharpFileLeaf(IEnumerable<ExcludableString> strings) : base(strings) { }

        public string SourceCode { get; set; }

        /// <summary>
        /// The original unmutated syntax tree
        /// </summary>
        public SyntaxTree SyntaxTree { get; set; }

        /// <summary>
        /// The mutated syntax tree
        /// </summary>
        public SyntaxTree MutatedSyntaxTree { get; set; }

        public override IEnumerable<Mutant> Mutants { get; set; }

        public override IEnumerable<SyntaxTree> CompilationSyntaxTrees => MutatedSyntaxTrees;

        public override IEnumerable<SyntaxTree> MutatedSyntaxTrees => new List<SyntaxTree> { MutatedSyntaxTree };

        public override IEnumerable<IFileLeaf<SyntaxTree>> GetAllFiles()
        {
            yield return this;
        }

        public override void Display()
        {
            DisplayFile(this);
        }

        public override bool IsMatch(FilePattern pattern, MutantSpan mutantSpan)
        {
            var textSpan = FromMutantSpan(mutantSpan);

            return pattern.MutantSpans.Any(span => FromMutantSpan(span).Contains(textSpan));
        }

        public override IEnumerable<TextSpan> Reduce(IEnumerable<TextSpan> spans)
        {
            return spans.Reduce();
        }

        public override IEnumerable<TextSpan> RemoveOverlap(IEnumerable<TextSpan> left, IEnumerable<TextSpan> right)
        {
            return left.RemoveOverlap(right);
        }

        public override MutantSpan ToMutantSpan(TextSpan span)
        {
            return new(span.Start, span.End);
        }

        public override TextSpan FromMutantSpan(MutantSpan span)
        {
            return TextSpan.FromBounds(span.Start, span.End);
        }
    }
}
