using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class OutputHtmlReportNameInput : Input<string>
    {
        protected override string Description => string.Empty;

        public override string Default => null;

        public string Validate()
        {
            if (string.IsNullOrWhiteSpace(SuppliedInput))
            {
                return "mutation-report.html";
            }
            if (SuppliedInput.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                throw new InputException("Invalid HTML Report Name supplied");
            }
            if (!SuppliedInput.EndsWith(".html"))
            {
                return SuppliedInput + ".html";
            }
            return SuppliedInput;
        }
    }
}
