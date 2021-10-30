using System.IO.Abstractions;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class OutputHTMLReportNameInput : Input<string>
    {
        protected override string Description => string.Empty;

        public override string Default => null;

        public string Validate()
        {
            if (string.IsNullOrWhiteSpace(SuppliedInput))
            {
                return "mutation-report.html";
            }
            if(!SuppliedInput.EndsWith(".html"))
            {
                return SuppliedInput + ".html";
            }
            return SuppliedInput;
        }
    }
}
