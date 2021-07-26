using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stryker.Core.Options.Inputs
{
    public class IgnoreMethodsInput : InputDefinition<IEnumerable<string>>
    {
        public override IEnumerable<string> Default => Enumerable.Empty<string>();

        protected override string Description => @"Ignore mutations on method parameters.";

        public IEnumerable<Regex> Validate()
        {
            if (SuppliedInput is { })
            {
                var ignoredMethodPatterns = new List<Regex>();
                foreach (var methodPattern in SuppliedInput.Where(pattern => !string.IsNullOrWhiteSpace(pattern)))
                {
                    ignoredMethodPatterns.Add(new Regex("^" + Regex.Escape(methodPattern).Replace("\\*", ".*") + "$", RegexOptions.IgnoreCase));
                }
                return ignoredMethodPatterns;
            }
            return Enumerable.Empty<Regex>();
        }
    }
}
