using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Stryker.Core.Initialisation;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.TestRunners;

namespace Stryker.Core.MutationTest
{
    /// <summary>
    /// Represents the state the application under test is in.
    /// </summary>
    public class MutationTestInput
    {
        /// <summary>
        /// Contains all information about the project the framework was called on
        /// </summary>
        public SourceProjectInfo ProjectInfo { get; set; }

        /// <summary>
        /// The testrunner that will be used for the mutation test run
        /// </summary>
        public ITestRunner TestRunner { get; set; }

        /// <summary>
        /// Get/Set the initial test
        /// </summary>
        public InitialTestRun InitialTestRun { get; set; }

        /// <summary>
        /// All the needed references for compiling the input project
        /// </summary>
        public IEnumerable<PortableExecutableReference> AssemblyReferences { get; set; }
    }
}
