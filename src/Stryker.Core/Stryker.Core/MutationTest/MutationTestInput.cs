using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Testing;
using Stryker.Core.Initialisation;
using Stryker.Core.ProjectComponents.SourceProjects;

namespace Stryker.Core.MutationTest;

/// <summary>
/// Represents the state the application under test is in.
/// </summary>
public class MutationTestInput
{
    /// <summary>
    /// Contains all information about the project to mutate
    /// </summary>
    public SourceProjectInfo SourceProjectInfo { get; init; }

    /// <summary>
    /// The testrunner that will be used for the mutation test run
    /// </summary>
    public ITestRunner TestRunner { get; init; }

    /// <summary>
    /// Get/Set the initial test
    /// </summary>
    public InitialTestRun InitialTestRun { get; set; }
}
