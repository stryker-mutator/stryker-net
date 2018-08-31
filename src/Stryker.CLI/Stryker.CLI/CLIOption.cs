using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.CLI
{
    public class CLIOption<T> where T : IConvertible
    {
        public string ArgumentName { get; set; }
        public string ArgumentShortName { get; set; }
        public string ArgumentDescription { get; set; }
        public T DefaultValue { get; set; }
        public string JsonKey { get; set; }
    }
}
