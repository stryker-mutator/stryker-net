using System;

namespace Stryker.Core.Options
{
    using System.Linq;

    public class FileMutateOptions
    {
        private readonly string _configurationValue;

        public string[] DefaultFoldersToExclude { get; } = { "obj", "bin", "node_modules" };

        public string[] FilesToInclude { get; set; } = { };

        public string[] FilesToExclude { get; set; } = { };

        public bool IncludeAllFiles => IsConfigurationValueOptional() || !FilesToInclude.Any();

        public FileMutateOptions(string configurationValue)
        {
            _configurationValue = configurationValue;

            ProcessConfigurationValue();
        }

        private void ProcessConfigurationValue()
        {
            if (IsConfigurationValueOptional())
                return;

            var files = _configurationValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            FilesToInclude = files.Where(p => !p.StartsWith("!")).ToArray();
            FilesToExclude = files.Where(p => p.StartsWith("!")).ToArray();
        }

        private bool IsConfigurationValueOptional()
        {
            return string.IsNullOrWhiteSpace(_configurationValue) || _configurationValue.Equals("*.cs");
        }
    }
}