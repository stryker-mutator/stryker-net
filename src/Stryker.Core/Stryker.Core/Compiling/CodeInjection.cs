using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Stryker.Core.Compiling
{
    public static class CodeInjection
    {
        // files to be injected into the mutated assembly
        private static readonly string[] Files = {"Stryker.Core.InjectedHelpers.MutantControl.cs", 
            "Stryker.Core.InjectedHelpers.Coverage.CommunicationChannel.cs"};
        private static readonly IList<SyntaxTree> Helpers = new List<SyntaxTree>();

        private const string PatternForCheck = "\\/\\/ *check with: *([^\\r\\n]+)";
        public static readonly string SelectorExpression;

        static CodeInjection()
        {
            var helper = GetSourceFromResource("Stryker.Core.InjectedHelpers.MutantControl.cs");
            var extractor = new Regex(PatternForCheck);
            var result = extractor.Match(helper);
            if (!result.Success)
            {
                throw new InvalidDataException("Internal error: failed to find expression for mutant selection.");
            }
            SelectorExpression = result.Groups[1].Value;
            foreach (var file in Files)
            {
                Helpers.Add(CSharpSyntaxTree.ParseText(GetSourceFromResource(file), new CSharpParseOptions(LanguageVersion.Latest)));
            }
        }

        public static IEnumerable<SyntaxTree> MutantHelpers => Helpers;

        private static string GetSourceFromResource(string sourceResourceName)
        {
            string helper;
            using (var stream =
                typeof(CodeInjection).Assembly.GetManifestResourceStream(sourceResourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    helper = reader.ReadToEnd();
                }
            }

            return helper;
        }

    }
}
