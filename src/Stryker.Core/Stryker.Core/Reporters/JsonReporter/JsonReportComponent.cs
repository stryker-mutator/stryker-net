using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System;
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
            public Dictionary<string, decimal> Totals { get; } = new Dictionary<string, decimal>();

            // All possible mutations we found including those skipped by the user but not including compile errors
            [JsonIgnore]
            public int PossibleMutants
            {
                get
                {
                    return (int)Totals["PossibleMutants"];
                }
                set
                {
                    Totals["PossibleMutants"] = value;
                }
            }

            // All mutants that were caught by a test
            [JsonIgnore]
            public int DetectedMutants
            {
                get
                {
                    return (int)Totals["DetectedMutants"];
                }
                set
                {
                    Totals["DetectedMutants"] = value;
                }
            }

            // All mutants that were tested
            [JsonIgnore]
            public int TotalMutants
            {
                get
                {
                    return (int)Totals["TotalMutants"];
                }
                set
                {
                    Totals["TotalMutants"] = value;
                }
            }

            // All mutants that were directly caught by a test
            [JsonIgnore]
            public int KilledMutants
            {
                get
                {
                    return (int)Totals["KilledMutants"];
                }
                set
                {
                    Totals["KilledMutants"] = value;
                }
            }

            // All mutants that were not caught by a test
            [JsonIgnore]
            public int SurvivedMutants
            {
                get
                {
                    return (int)Totals["SurvivedMutants"];
                }
                set
                {
                    Totals["SurvivedMutants"] = value;
                }
            }

            // All mutants that were skipped by the user
            [JsonIgnore]
            public int SkippedMutants
            {
                get
                {
                    return (int)Totals["SkippedMutants"];
                }
                set
                {
                    Totals["SkippedMutants"] = value;
                }
            }

            //All mutants that resulted in a timeout in the test and are counted as caught mutants
            [JsonIgnore]
            public int TimeoutMutants
            {
                get
                {
                    return (int)Totals["TimeoutMutants"];
                }
                set
                {
                    Totals["TimeoutMutants"] = value;
                }
            }

            // All mutants we tried to place, but were unable to due to compile errors
            [JsonIgnore]
            public int CompileErrors
            {
                get
                {
                    return (int)Totals["CompileErrors"];
                }
                set
                {
                    Totals["CompileErrors"] = value;
                }
            }

            [JsonIgnore]
            public int? ThresholdHigh
            {
                get
                {
                    if (!Totals.ContainsKey("ThresholdHigh"))
                    {
                        return null;
                    }
                    return (int)Totals["ThresholdHigh"];
                }
                set
                {
                    if (!(value is null))
                    {
                        Totals["ThresholdHigh"] = (int)value;
                    }
                }
            }

            [JsonIgnore]
            public int? ThresholdLow
            {
                get
                {
                    if (!Totals.ContainsKey("ThresholdLow"))
                    {
                        return null;
                    }
                    return (int)Totals["ThresholdLow"];
                }
                set
                {
                    if (!(value is null))
                    {
                        Totals["ThresholdLow"] = (int)value;
                    }
                }
            }

            [JsonIgnore]
            public int? ThresholdBreak
            {
                get
                {
                    if (!Totals.ContainsKey("ThresholdBreak"))
                    {
                        return null;
                    }
                    return (int)Totals["ThresholdBreak"];
                }
                set
                {
                    if (!(value is null))
                    {
                        Totals["ThresholdBreak"] = (int)value;
                    }
                }
            }

            [JsonIgnore]
            public decimal? MutationScore
            {
                get
                {
                    if (!Totals.ContainsKey("MutationScore"))
                    {
                        return null;
                    }
                    return Totals["MutationScore"];
                }
                set
                {
                    if (!(value is null))
                    {
                        Totals["MutationScore"] = (decimal)value;
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
                    DetectedMutants = component.DetectedMutants.Count(),
                    TotalMutants = component.TotalMutants.Count(),
                    KilledMutants = Where(MutantStatus.Killed),
                    SurvivedMutants = Where(MutantStatus.Survived),
                    SkippedMutants = Where(MutantStatus.Skipped),
                    TimeoutMutants = Where(MutantStatus.Timeout),
                    CompileErrors = Where(MutantStatus.BuildError),
                    MutationScore = component.GetMutationScore(),
                };
                report.PossibleMutants = report.TotalMutants + report.SkippedMutants;

                if (report.MutationScore >= options.ThresholdOptions.ThresholdHigh)
                {
                    report.Health = "ok";
                }
                else if (report.MutationScore <= options.ThresholdOptions.ThresholdBreak)
                {
                    report.Health = "danger";
                }
                else
                {
                    report.Health = "warning";
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
                            Column = location.StartLinePosition.Character
                        };
                        End = new JsonMutantLocationPoint
                        {
                            Line = location.EndLinePosition.Line + 1,
                            Column = location.EndLinePosition.Character
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
