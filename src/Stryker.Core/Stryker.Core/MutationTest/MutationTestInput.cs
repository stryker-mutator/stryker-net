﻿using Microsoft.CodeAnalysis;
using Stryker.Core.Initialisation;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.IO;

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
        public Initialisation.ProjectInfo ProjectInfo { get; set; }

        /// <summary>
        /// The testrunner that will be used for the mutation test run
        /// </summary>
        public ITestRunner TestRunner { get; set; }

        public int TimeoutMS { get; set; }

        /// <summary>
        /// All the needed references for compiling the input project
        /// </summary>
        public IEnumerable<PortableExecutableReference> AssemblyReferences { get; set; }

        public string GetInjectionPath()
        {
            if (ProjectInfo.AppendTargetFrameworkToOutputPath)
            {
                return Path.Combine(ProjectInfo.TestProjectPath, "bin", "Debug", ProjectInfo.TargetFramework, ProjectInfo.ProjectUnderTestAssemblyName + ".dll");
            }
            return Path.Combine(ProjectInfo.TestProjectPath, "bin", "Debug", ProjectInfo.ProjectUnderTestAssemblyName + ".dll");
        }
    }
}
