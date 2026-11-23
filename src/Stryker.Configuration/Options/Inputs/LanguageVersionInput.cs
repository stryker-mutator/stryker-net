using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Exceptions;
using Stryker.Utilities.Logging;

namespace Stryker.Configuration.Options.Inputs;

public class LanguageVersionInput : Input<string>
{
    private const string DeprecationWarning = "The language-version option is deprecated and ignored. Configure LangVersion in the project file instead.";

    public override string Default => null;

    protected override string Description => "Deprecated: configure the C# language version in the project file. This option is ignored.";
    protected override IEnumerable<string> AllowedOptions => Enum.GetNames(typeof(LanguageVersion)).Where(l => LanguageVersion.CSharp1.ToString() != l);

    public LanguageVersion Validate(ILogger<LanguageVersionInput> logger = null)
    {
        if (SuppliedInput is { })
        {
            if (!Enum.TryParse(SuppliedInput, true, out LanguageVersion result) || result == LanguageVersion.CSharp1)
            {
                throw new InputException($"The given c# language version ({SuppliedInput}) is invalid. Valid options are: [{string.Join(", ", ((IEnumerable<LanguageVersion>)Enum.GetValues(typeof(LanguageVersion))).Where(l => l != LanguageVersion.CSharp1))}]");
            }

            logger ??= ApplicationLogging.LoggerFactory.CreateLogger<LanguageVersionInput>();
            logger.LogWarning(DeprecationWarning);
        }

        return LanguageVersion.Default;
    }
}
