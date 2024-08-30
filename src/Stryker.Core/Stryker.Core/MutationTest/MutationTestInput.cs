using Stryker.Abstractions.Initialisation;
using Stryker.Abstractions.ProjectComponents.SourceProjects;
using Stryker.Abstractions.ProjectComponents.TestProjects;
using Stryker.Abstractions.TestRunners;
using Stryker.Abstractions.TestRunners;

namespace Stryker.Abstractions.MutationTest
{
    /// <summary>
    /// Represents the state the application under test is in.
    /// </summary>
    public class MutationTestInput
    {
        /// <summary>
        /// Contains all information about the project to mutate
        /// </summary>
        public SourceProjectInfo SourceProjectInfo { get; set; }

        /// <summary>
        /// Contains all information about the tests to run
        /// </summary>
        public TestProjectsInfo TestProjectsInfo { get; set; }

        /// <summary>
        /// The testrunner that will be used for the mutation test run
        /// </summary>
        public ITestRunner TestRunner { get; set; }

        /// <summary>
        /// Get/Set the initial test
        /// </summary>
        public InitialTestRun InitialTestRun { get; set; }
    }
}
