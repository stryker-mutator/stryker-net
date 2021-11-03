using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
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
            if(Path.GetExtension(SuppliedInput) != string.Empty)
            {
                throw new InputException("Filenames cannot contain an extension");
            }
            return SuppliedInput;
        }
    }
}
