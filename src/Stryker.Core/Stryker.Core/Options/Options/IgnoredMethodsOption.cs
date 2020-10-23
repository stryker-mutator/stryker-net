using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Stryker.Core.Options.Options
{
    class IgnoredMethodsOption : BaseStrykerOption<IEnumerable<Regex>>
    {
        public IgnoredMethodsOption(string[] ignoredMethods)
        {
            var list = new List<Regex>();
            foreach (var methodPattern in ignoredMethods.Where(x => !string.IsNullOrEmpty(x)))
            {
                list.Add(new Regex("^" + Regex.Escape(methodPattern).Replace("\\*", ".*") + "$", RegexOptions.IgnoreCase));
            }
            Value = list;
        }

        public override StrykerOption Type => StrykerOption.IgnoredMethods;
        public override string HelpText => "Mutations that would affect parameters that are directly passed into methods with given names are ignored. Example: ['ConfigureAwait', 'ToString']";
        public override IEnumerable<Regex> DefaultValue => null;
    }
}
