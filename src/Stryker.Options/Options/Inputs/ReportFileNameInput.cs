using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Stryker.Abstractions.Exceptions;

namespace Stryker.Abstractions.Options.Inputs;

public class ReportFileNameInput : Input<string>
{
    protected override string Description => string.Empty;

    public override string Default => "mutation-report";

    public string Validate()
    {
        if (string.IsNullOrWhiteSpace(SuppliedInput))
        {
            return Default;
        }
        if (SuppliedInput.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
        {
            throw new InputException("Invalid Report Name supplied");
        }
        string Extension = Path.GetExtension(SuppliedInput);
        if (Extension == ".json")
        {
            SuppliedInput = SuppliedInput.Replace(".json", "");
        }
        else if (Extension == ".html")
        {
            SuppliedInput = SuppliedInput.Replace(".html", "");
        }
        return SuppliedInput;
    }
}
