﻿using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Testing;
using Stryker.Core.ToolHelpers;
using System;
using System.IO;
using System.Linq;

namespace Stryker.Core.Initialisation
{
    public interface INugetRestoreProcess
    {
        void RestorePackages(string solutionPath);
    }

    /// <summary>
    /// Restores nuget packages for a given solution file
    /// </summary>
    public class NugetRestoreProcess : INugetRestoreProcess
    {
        private IProcessExecutor _processExecutor { get; set; }
        private ILogger _logger { get; set; }

        public NugetRestoreProcess(IProcessExecutor processExecutor = null)
        {
            _processExecutor = processExecutor ?? new ProcessExecutor();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<NugetRestoreProcess>();
        }

        public void RestorePackages(string solutionPath)
        {
            _logger.LogInformation("Restoring nuget packages using {0}", "nuget.exe");
            if (string.IsNullOrWhiteSpace(solutionPath))
            {
                throw new StrykerInputException("Solution path is required on .net framework projects. Please provide your solution path using --solution-path ...");
            }
            solutionPath = Path.GetFullPath(solutionPath);
            string solutionDir = Path.GetDirectoryName(solutionPath);

            // Validate nuget.exe is installed and included in path
            var nugetWhereExeResult = _processExecutor.Start(solutionDir, "where.exe", "nuget.exe");
            if (!nugetWhereExeResult.Output.ToLowerInvariant().Contains("nuget.exe"))
            {
                throw new StrykerInputException("Nuget.exe should be installed to restore .net framework nuget packages. Install nuget.exe and make sure it's included in your path.");
            }
            // Get the first nuget.exe path from the where.exe output
            var nugetPath = nugetWhereExeResult.Output.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault().Trim();

            // Locate MSBuild.exe
            var msbuildPath = new MsBuildHelper().GetMsBuildPath(_processExecutor);
            var msBuildVersionOutput = _processExecutor.Start(solutionDir, msbuildPath, "-version /nologo");
            if (msBuildVersionOutput.ExitCode != 0)
            {
                _logger.LogError("Unable to detect msbuild version");
            }
            var msBuildVersion = msBuildVersionOutput.Output.Trim();
            _logger.LogDebug("Auto detected msbuild version {0} at: {1}", msBuildVersion, msbuildPath);

            // Restore packages using nuget.exe
            var nugetRestoreCommand = string.Format("restore \"{0}\" -MsBuildVersion \"{1}\"", solutionPath, msBuildVersion);
            _logger.LogDebug("Restoring packages using command: {0} {1}", nugetPath, nugetRestoreCommand);

            try
            {
                var nugetRestoreResult = _processExecutor.Start(solutionDir, nugetPath, nugetRestoreCommand, timeoutMS: 120000);
                if (nugetRestoreResult.ExitCode != 0)
                {
                    throw new StrykerInputException("Nuget.exe failed to restore packages for your solution. Please review your nuget setup.", nugetRestoreResult.Output);
                }
                _logger.LogDebug("Restored packages using nuget.exe, output: {0}", nugetRestoreResult.Output);
            }
            catch (OperationCanceledException)
            {
                throw new StrykerInputException("Nuget.exe failed to restore packages for your solution. Please review your nuget setup.");
            }
        }
    }
}
