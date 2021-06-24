using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.DiffProviders;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutantFilters
{
    public class DiffMutantFilter : IMutantFilter
    {
        private readonly DiffResult _diffResult;
        private readonly ILogger<DiffMutantFilter> _logger;
        private readonly IFileSystem _fileSystem;

        public string DisplayName => "git diff file filter";

        public DiffMutantFilter(IDiffProvider diffProvider = null, IFileSystem fileSystem = null)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DiffMutantFilter>();
            _fileSystem = fileSystem ?? new FileSystem();
            _diffResult = diffProvider.ScanDiff();

            if (_diffResult != null)
            {
                _logger.LogInformation("{0} files changed", _diffResult.ChangedSourceFiles?.Count ?? 0 + _diffResult.ChangedTestFiles?.Count ?? 0);

                if (_diffResult.ChangedSourceFiles != null)
                {
                    foreach (var changedFile in _diffResult.ChangedSourceFiles)
                    {
                        _logger.LogInformation("Changed file {0}", changedFile);
                    }
                }
                if (_diffResult.ChangedTestFiles != null)
                {
                    foreach (var changedFile in _diffResult.ChangedTestFiles)
                    {
                        _logger.LogInformation("Changed test file {0}", changedFile);
                    }
                }
            }
        }

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, ReadOnlyFileLeaf file, IStrykerOptions options)
        {
            // Since all mutants are passed by reference the original array can be returned. All mutants that are not flagged as either changed nor being covered by changed tests are set to ignored. 

            // A non-csharp file is flagged by the diff result as modified. We cannot determine which mutants will be affected by this, thus all mutants have to be tested.
            if (_diffResult.ChangedTestFiles is { } && _diffResult.ChangedTestFiles.Any(x => !x.Path.EndsWith(".cs")))
            {
                _logger.LogDebug("Returning all mutants in {0} because a non-source file is modified", file.RelativePath);
                return SetMutantStatusForNonCSharpFileChanged(mutants);
            }

            // Check wether a mutant is appearing in changed lines. If this isn't the case the mutant is set to ignored.
            var mutantsToTestByLinesChanged = SetMutantStatusForAppearingInLinesChanged(mutants, file);


            // If any of the tests have been changed, we want to return all mutants covered by these testfiles.
            // Only check for changed c# files. Other files have already been handled.
         
            var mutantsToTestByChangedTests = ResetMutantStatusForChangedTestsAsync(mutants).Result;


            var allMutantsToTest = mutantsToTestByLinesChanged.Concat(mutantsToTestByChangedTests.Where(kvp => !mutantsToTestByLinesChanged.ContainsKey(kvp.Key))).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return allMutantsToTest.Values.ToList();
        }

        private IDictionary<int, Mutant> SetMutantStatusForAppearingInLinesChanged(IEnumerable<Mutant> mutants, ReadOnlyFileLeaf file)
        {
            IDictionary<int, Mutant> mutantsToTest = new Dictionary<int, Mutant>();
            var currentFileChanges = _diffResult?.ChangedSourceFiles.FirstOrDefault(changedFile => changedFile.Path == file.FullPath);

            if (currentFileChanges is { })
            {
                _logger.LogDebug("Returning certain mutants in {0} because the file is modified", file.RelativePath);

                foreach(var mutant in mutants)
                {
                    if (currentFileChanges.AddedLines.Any(addedLine => addedLine.LineNumber == mutant.Line)) {
                        mutant.ResultStatus = MutantStatus.NotRun;
                        mutant.ResultStatusReason = "Mutant changed compared to target commit";
                        mutantsToTest.Add(mutant.Id, mutant);
                    }
                }
            }

            return mutantsToTest;
           
        }

        private IEnumerable<Mutant> SetMutantStatusForNonCSharpFileChanged(IEnumerable<Mutant> mutants)
        {
            foreach (var mutant in mutants)
            {
                mutant.ResultStatus = MutantStatus.NotRun;
                mutant.ResultStatusReason = "Non-CSharp files in test project were changed";
            }

            return mutants;
        }

        private async Task<IDictionary<int, Mutant>> ResetMutantStatusForChangedTestsAsync(IEnumerable<Mutant> mutants)
        {
            if (_diffResult.ChangedTestFiles == null || !_diffResult.ChangedTestFiles.Any(changedFile => changedFile.Path.EndsWith(".cs")))
            {
                // No tests are changed so no mutants to test
                return null;
            }

            var mutantsToTest = new Dictionary<int, Mutant>();

            foreach (var mutant in mutants)
            {
                var changedMutant = await DetermineSingleMutantStatusForChangedTestsAsync(mutant);

                if (changedMutant is { })
                {
                    mutantsToTest.Add(changedMutant.Id, changedMutant);

                }
            }

            return mutantsToTest;
        }

        private async Task<Mutant> DetermineSingleMutantStatusForChangedTestsAsync(Mutant mutant)
        {
            var coveringTests = mutant.CoveringTests.Tests;


            foreach (var changedTestFile in _diffResult.ChangedTestFiles)
            {
                var coveringTest = coveringTests.FirstOrDefault(coveringTestFile => coveringTestFile.TestfilePath == changedTestFile.Path);

                // TODO: This means there is no corresponding test file with the file from the diff result. Might want to throw an error.
                if (changedTestFile is null)
                {
                    continue;
                }

                CompilationUnitSyntax root = await GetCompilationUnitSyntaxForTestFileAsync(coveringTest.TestfilePath);

                // Get the member associated with the current coveringTest.
                MemberDeclarationSyntax coveringTestSyntax = root.Members.FirstOrDefault(member => member.FullSpan.Contains(coveringTest.LineNumber));

                var allChangedLines = changedTestFile.AddedLines.Concat(changedTestFile.DeletedLines);
                var overlapsWithAnyChangedLine = allChangedLines.Any(line => coveringTestSyntax.Span.Contains(line.LineNumber));

                // Any of the changed lines overlaps with the span of the current test. This means the test has changed and thus we want to test the mutant it covers.
                if (overlapsWithAnyChangedLine)
                {
                    mutant.ResultStatus = MutantStatus.NotRun;
                    mutant.ResultStatusReason = "One or more covering tests changed";
                    return mutant;
                }
            }

            return null;
        }

        private async Task<CompilationUnitSyntax> GetCompilationUnitSyntaxForTestFileAsync(string testFilePath)
        {
            if (!_fileSystem.File.Exists(testFilePath))
            {
                return null;
            }

            using StreamReader inputReader = _fileSystem.File.OpenText(testFilePath);

            var testFileContent = await inputReader.ReadToEndAsync();

            SyntaxTree tree = CSharpSyntaxTree.ParseText(testFileContent);

            return tree.GetCompilationUnitRoot();
        }
    }
}
