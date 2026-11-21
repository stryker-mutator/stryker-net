using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stryker.Configuration.Options.Inputs;

/// <summary>
/// Method calls whose arguments should not be mutated. Takes the same patterns as <see cref="IgnoreMethodsInput"/>.
/// </summary>
public class IgnoreMethodArgumentsInput : Input<IEnumerable<string>>
{
    public override IEnumerable<string> Default => Enumerable.Empty<string>();

    protected override string Description => "Ignore mutations in the arguments of the given method calls, but not in the rest of the call.";

    public IEnumerable<Regex> Validate() => IgnoreMethodsInput.ParseRegex(SuppliedInput ?? Default);
}
