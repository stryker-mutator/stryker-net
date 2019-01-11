﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Reporters
{
    public class JsonReporter : IReporter
    {
        private readonly StrykerOptions _options;
        public JsonReporter(StrykerOptions options)
        {
            _options = options;
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
            var jsonReport = JsonReportComponent.FromProjectComponent(reportComponent, _options.ThresholdOptions);
            jsonReport.Name = _options.ProjectUnderTestNameFilter;

            using (var file = File.CreateText(_options.BasePath + "/StrykerLogs/mutation-report.json"))
            {
                file.WriteLine(SerializeJsonReport(jsonReport));
            }
        }

        private static string SerializeJsonReport(JsonReportComponent jsonReport)
        {
            return JsonConvert.SerializeObject(jsonReport, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            });
        }

        public sealed class JsonReportComponent
        {
            public string Name { get; set; }
            public string Language { get; set; } = "cs";
            public string Health { get; set; }
            public string Source { get; set; }

            // All possible mutations we found including those skipped by the user but not including compile errors
            public int PossibleMutants { get; set; }
            // All mutants that were caught by a test
            public int DetectedMutants { get; set; }
            // All mutants that were tested
            public int TotalMutants { get; set; }
            // All mutants that were directly caught by a test
            public int KilledMutants { get; set; }
            // All mutants that were not caught by a test
            public int SurvivedMutants { get; set; }
            // All mutants that were skipped by the user
            public int SkippedMutants { get; set; }
            //All mutants that resulted in a timeout in the test and are counted as caught mutants
            public int TimeoutMutants { get; set; }
            // All mutants we tried to place, but were unable to due to compile errors
            public int CompileErrors { get; set; }

            public int ThresholdHigh { get; set; }
            public int ThresholdLow { get; set; }
            public int ThresholdBreak { get; set; }
            public decimal? MutationScore { get; set; }
            public IList<JsonMutant> Mutants { get; } = new List<JsonMutant>();

            public IList<JsonReportComponent> ChildResults { get; } = new List<JsonReportComponent>();

            public static JsonReportComponent FromProjectComponent(IReadOnlyInputComponent component, ThresholdOptions thresholdOptions)
            {
                var report = new JsonReportComponent
                {
                    Name = component.Name,
                    DetectedMutants = component.DetectedMutants.Count(),
                    TotalMutants = component.TotalMutants.Count(),
                    KilledMutants = component.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.Killed).Count(),
                    SurvivedMutants = component.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.Survived).Count(),
                    SkippedMutants = component.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.Skipped).Count(),
                    TimeoutMutants = component.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.Timeout).Count(),
                    CompileErrors = component.ReadOnlyMutants.Where(m => m.ResultStatus == MutantStatus.BuildError).Count(),
                    MutationScore = component.GetMutationScore(),
                    ThresholdHigh = thresholdOptions.ThresholdHigh,
                    ThresholdLow = thresholdOptions.ThresholdLow,
                    ThresholdBreak = thresholdOptions.ThresholdBreak,
                };
                report.PossibleMutants = report.TotalMutants + report.SkippedMutants;

                if (report.MutationScore >= report.ThresholdHigh)
                {
                    report.Health = "ok";
                }
                else if (report.MutationScore <= report.ThresholdBreak)
                {
                    report.Health = "danger";
                }
                else
                {
                    report.Health = "warning";
                }

                if (component is FolderComposite folderComponent)
                {
                    foreach (var child in folderComponent.Children)
                    {
                        report.ChildResults.Add(FromProjectComponent(child, thresholdOptions));
                    }
                }
                else if (component is FileLeaf fileComponent)
                {
                    report.Source = fileComponent.SourceCode;
                    foreach (var mutant in fileComponent.Mutants)
                    {
                        var jsonMutant = new JsonMutant
                        {
                            Id = mutant.Id,
                            MutatorName = mutant.Mutation.DisplayName,
                            Replacement = mutant.Mutation.ReplacementNode.ToFullString(),
                            Span = new[] { mutant.Mutation.OriginalNode.SpanStart, mutant.Mutation.OriginalNode.Span.End },
                            Status = StrykerStatusToMutationStatus(mutant.ResultStatus)
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

            private static string StrykerStatusToMutationStatus(MutantStatus status)
            {
                switch (status)
                {
                    case MutantStatus.Survived:
                    case MutantStatus.Killed:
                    case MutantStatus.Timeout:
                    case MutantStatus.Skipped:
                        return status.ToString();
                    case MutantStatus.BuildError:
                        return "CompileError";
                    default:
                        return MutantStatus.NotRun.ToString();
                }
            }

            public sealed class JsonMutant
            {
                public int Id { get; set; }
                public string MutatorName { get; set; }
                public string Replacement { get; set; }
                public int[] Span { get; set; }
                public string Status { get; set; }
            }
        }
        public void OnMutantsCreated(IReadOnlyInputComponent reportComponent)
        {
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
        }

        public void OnStartMutantTestRun(IEnumerable<Mutant> mutantsToBeTested)
        {
        }
    }
}
