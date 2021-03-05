using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stryker.Core.Options.Inputs
{
    public class IgnoredMethodsInput : OptionDefinition<IEnumerable<string>, IEnumerable<Regex>>
    {
        public override IEnumerable<string> DefaultInput => Enumerable.Empty<string>();

        protected override string Description => @"Ignore mutations on method parameters.";

        public IgnoredMethodsInput() { }
        public IgnoredMethodsInput(IEnumerable<string> ignoredMethods)
        {
            if (ignoredMethods is { })
            {
                var ignoredMethodPatterns = new List<Regex>();
                foreach (var methodPattern in ignoredMethods.Where(pattern => !string.IsNullOrWhiteSpace(pattern)))
                {
                    ignoredMethodPatterns.Add(new Regex("^" + Regex.Escape(methodPattern).Replace("\\*", ".*") + "$", RegexOptions.IgnoreCase));
                }
                Value = ignoredMethodPatterns;
            }
        }
    }
}
