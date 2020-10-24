using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stryker.Core.Options.Options
{
    public class IgnoredMethodsOption : ComplexStrykerInput<IEnumerable<Regex>, IEnumerable<string>>
    {
        static IgnoredMethodsOption()
        {
            HelpText = "Mutations that would affect parameters that are directly passed into methods with given names are ignored. Example: ['ConfigureAwait', 'ToString']";
            DefaultInput = Enumerable.Empty<string>();
            DefaultValue = new IgnoredMethodsOption(DefaultInput).Value;
        }

        public IgnoredMethodsOption(IEnumerable<string> ignoredMethods)
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

        public override StrykerOption Type => StrykerOption.IgnoredMethods;
    }
}
