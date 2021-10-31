using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class ReportFileNameInput : Input<string>
    {
        protected override string Description => string.Empty;

        public override string Default => null;

        public string Validate()
        {
            if (string.IsNullOrWhiteSpace(SuppliedInput))
            {
                return "mutation-report";
            }
            if (SuppliedInput.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                throw new InputException("Invalid HTML Report Name supplied");
            }
            return SuppliedInput;
        }
    }
}
