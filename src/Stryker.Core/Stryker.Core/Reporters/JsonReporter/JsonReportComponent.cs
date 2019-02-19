using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Reporters.Json
{
    public partial class JsonReporter
    {
        public sealed class JsonReportComponent
        {
            public string Name { get; set; }
            public string Language { get; set; }
            public string Health { get; set; }
            public string Source { get; set; }
            public decimal MutationScore { get; set; }
            public Dictionary<string, decimal> Totals { get; } = new Dictionary<string, decimal>();

            // All possible mutations we found including those skipped by the user but not including compile errors
            [JsonIgnore]
            public int ValidMutations
            {
                get
                {
                    return (int)Totals["Valid Mutations"];
                }
                set
                {
                    Totals["Valid Mutations"] = value;
                }
            }

            // All mutants that were caught by a test
            [JsonIgnore]
            public int DetectedMutations
            {
                get
                {
                    return (int)Totals["Killed + Timeout"];
                }
                set
                {
                    Totals["Killed + Timeout"] = value;
                }
            }

            // All mutants that were tested
            [JsonIgnore]
            public int TotalMutants
            {
                get
                {
                    return (int)Totals["Tested"];
                }
                set
                {
                    Totals["Tested"] = value;
                }
            }

            // All mutants that were directly caught by a test
            [JsonIgnore]
            public int KilledMutants
            {
                get
                {
                    return (int)Totals["Killed"];
                }
                set
                {
                    Totals["Killed"] = value;
                }
            }

            // All mutants that were not caught by a test
            [JsonIgnore]
            public int SurvivedMutants
            {
                get
                {
                    return (int)Totals["Survived"];
                }
                set
                {
                    Totals["Survived"] = value;
                }
            }

            // All mutants that were skipped by the user
            [JsonIgnore]
            public int SkippedMutants
            {
                get
                {
                    return (int)Totals["Skipped"];
                }
                set
                {
                    Totals["Skipped"] = value;
                }
            }

            //All mutants that resulted in a timeout in the test and are counted as caught mutants
            [JsonIgnore]
            public int TimeoutMutants
            {
                get
                {
                    return (int)Totals["Timeout"];
                }
                set
                {
                    Totals["Timeout"] = value;
                }
            }

            // All mutants we tried to place, but were unable to due to compile errors
            [JsonIgnore]
            public int CompileErrors
            {
                get
                {
                    return (int)Totals["Compile Error"];
                }
                set
                {
                    Totals["Compile Error"] = value;
                }
            }

            [JsonIgnore]
            public int? ThresholdHigh
            {
                get
                {
                    if (!Totals.ContainsKey("Good %"))
                    {
                        return null;
                    }
                    return (int)Totals["Good %"];
                }
                set
                {
                    if (!(value is null))
                    {
                        Totals["Good %"] = (int)value;
                    }
                }
            }

            [JsonIgnore]
            public int? ThresholdLow
            {
                get
                {
                    if (!Totals.ContainsKey("Warning %"))
                    {
                        return null;
                    }
                    return (int)Totals["Warning %"];
                }
                set
                {
                    if (!(value is null))
                    {
                        Totals["Warning %"] = (int)value;
                    }
                }
            }

            [JsonIgnore]
            public int? ThresholdBreak
            {
                get
                {
                    if (!Totals.ContainsKey("Error %"))
                    {
                        return null;
                    }
                    return (int)Totals["Error %"];
                }
                set
                {
                    if (!(value is null))
                    {
                        Totals["Error %"] = (int)value;
                    }
                }
            }

            public IList<JsonMutant> Mutants { get; set; }

            public IList<JsonReportComponent> ChildResults { get; set; }

            public static JsonReportComponent FromProjectComponent(IReadOnlyInputComponent component, StrykerOptions options)
            {
                int Where(MutantStatus MutantStatus) => component.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus).Count();

                var report = new JsonReportComponent
                {
                    DetectedMutations = component.DetectedMutants.Count(),
                    TotalMutants = component.TotalMutants.Count(),
                    KilledMutants = Where(MutantStatus.Killed),
                    SurvivedMutants = Where(MutantStatus.Survived),
                    SkippedMutants = Where(MutantStatus.Skipped),
                    TimeoutMutants = Where(MutantStatus.Timeout),
                    CompileErrors = Where(MutantStatus.BuildError),
                    MutationScore = component.GetMutationScore() ?? 0,
                };
                report.ValidMutations = report.TotalMutants + report.SkippedMutants;

                if (report.MutationScore >= options.ThresholdOptions.ThresholdHigh)
                {
                    report.Health = "Good";
                }
                else if (report.MutationScore <= options.ThresholdOptions.ThresholdBreak)
                {
                    report.Health = "Danger";
                }
                else
                {
                    report.Health = "Warning";
                }

                if (component is FolderComposite folderComponent)
                {
                    report.Name = component.Name is null ? options.ProjectUnderTestNameFilter : folderComponent.RelativePath;
                    report.ChildResults = new List<JsonReportComponent>();

                    foreach (var child in folderComponent.Children)
                    {
                        report.ChildResults.Add(FromProjectComponent(child, options));
                    }
                }
                else if (component is FileLeaf fileComponent)
                {
                    report.Name = fileComponent.Name;
                    report.Source = fileComponent.SourceCode;
                    report.Language = "cs";
                    report.Mutants = new List<JsonMutant>();

                    foreach (var mutant in fileComponent.Mutants)
                    {
                        var jsonMutant = new JsonMutant
                        {
                            Id = mutant.Id,
                            MutatorName = mutant.Mutation.DisplayName,
                            Replacement = mutant.Mutation.ReplacementNode.ToFullString(),
                            Location = new JsonMutant.JsonMutantLocation(mutant.Mutation.OriginalNode.SyntaxTree.GetLineSpan(mutant.Mutation.OriginalNode.FullSpan)),
                            Status = mutant.ResultStatus.ToString()
                        };

                        report.Mutants.Add(jsonMutant);
                    }
                }
                else
                {
                    throw new System.Exception("Unknown IReadOnlyInputComponent implementation");
                }

                return report;
            }

            public sealed class JsonMutant
            {
                public int Id { get; set; }
                public string MutatorName { get; set; }
                public string Replacement { get; set; }
                public JsonMutantLocation Location { get; set; }
                public string Status { get; set; }

                public sealed class JsonMutantLocation
                {
                    public JsonMutantLocation(FileLinePositionSpan location)
                    {
                        Start = new JsonMutantLocationPoint
                        {
                            Line = location.StartLinePosition.Line + 1,
                            Column = location.StartLinePosition.Character + 1
                        };
                        End = new JsonMutantLocationPoint
                        {
                            Line = location.EndLinePosition.Line + 1,
                            Column = location.EndLinePosition.Character + 1
                        };
                    }

                    public JsonMutantLocationPoint Start { get; set; }
                    public JsonMutantLocationPoint End { get; set; }
                    public sealed class JsonMutantLocationPoint
                    {
                        public int Line { get; set; }
                        public int Column { get; set; }
                    }
                }
            }
        }
    }
}
