using Stryker.Core.Options;
using System.Collections.Generic;

namespace Stryker.CLI
{
    public class JsonOption
    {
        public StrykerInput InputType { get; set; }
        public string JsonKey { get; set; }
    }

    public class JsonOptionsParser
    {
        private readonly IDictionary<StrykerInput, JsonOption> _jsonOptions = new Dictionary<StrykerInput, JsonOption>();

        public JsonOptionsParser()
        {

        }

        public T Parse<T>(StrykerInput inputType)
        {
            if (_jsonOptions.TryGetValue(inputType, out var jsonOption))
            {

            }
            return default;
        }
    }
}
