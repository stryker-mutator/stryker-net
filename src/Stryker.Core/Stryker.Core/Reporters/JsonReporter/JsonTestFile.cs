using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;

namespace Stryker.Core.Reporters.Json
{
    public class JsonTestFile
    {
        public string Language { get; init; }
        public string Source { get; init; }
        public ISet<JsonTest> Tests { get; set; }

        public JsonTestFile()
        {
        }

        public JsonTestFile(ReadOnlyFileLeaf file)
        {
            Source = file.SourceCode;
            Language = "cs";
        }
    }
}
