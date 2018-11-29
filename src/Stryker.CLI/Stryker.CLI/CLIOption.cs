using System;
using System.Runtime.Serialization;

namespace Stryker.CLI
{
    public class CLIOption<T>
    {
        public string ArgumentName { get; set; }
        public string ArgumentShortName { get; set; }
        public string ArgumentDescription { get; set; }
        public T DefaultValue { get; set; }
        public string JsonKey { get; set; }
    }
}
