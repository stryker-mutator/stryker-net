using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Core.ProjectComponents.TestProjects;
using System;

namespace Stryker.Core.Reporters.Json.TestFiles;

public class JsonTestFile : IJsonTestFile
{
    public string Language { get; init; }
    public string Source { get; init; }
    public ISet<IJsonTest> Tests { get; set; }

    public JsonTestFile() { }

    public JsonTestFile(ITestFile testFile)
    {
        Source = testFile?.Source;
        Language = "cs";
        Tests = new HashSet<IJsonTest>();

        AddTestFile(testFile);
    }

    public void AddTestFile(ITestFile testFile)
    {
        foreach (var test in testFile?.Tests ?? Enumerable.Empty<ITestCase>())
        {
            Tests.Add(new JsonTest(test.Id.ToString())
            {
                Name = test.Name,
                Location = new Location(test.Node.GetLocation().GetMappedLineSpan())
            });
        }
    }
}

public class JsonTestFileConverter : JsonConverter<IJsonTestFile>
{
    public override IJsonTestFile Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Deserialize the JSON into the concrete type
        var jsonTestFile = JsonSerializer.Deserialize<JsonTestFile>(ref reader, options);
        return jsonTestFile;
    }

    public override void Write(Utf8JsonWriter writer, IJsonTestFile value, JsonSerializerOptions options)
    {
        // Serialize the concrete type
        JsonSerializer.Serialize(writer, (JsonTestFile)value, options);
    }
}
