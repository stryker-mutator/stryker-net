// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Source: https://github.com/dotnet/cli/blob/master/test/Microsoft.DotNet.Tools.Tests.Utilities/Extensions/ProcessExtensions.cs 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;

namespace Stryker.Core.Testing
{
    // integration with OS
    [ExcludeFromCodeCoverage]
    internal static class ProcessExtensions
    {
#if NET451
        private static readonly bool _isWindows = true;
#else
        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif

        public static void KillTree(this Process process, TimeSpan timeout)
        {
            if (IsWindows)
            {
                RunProcessAndWaitForExit(
                    "taskkill",
                    $"/T /F /PID {process.Id}",
                    timeout,
                    out _);
            }
            else
            {
                var children = new HashSet<int>();
                GetAllChildIdsUnix(process.Id, children, timeout);
                foreach (var childId in children)
                {
                    KillProcessUnix(childId, timeout);
                }
                KillProcessUnix(process.Id, timeout);
            }
        }

        private static void GetAllChildIdsUnix(int parentId, ISet<int> children, TimeSpan timeout)
        {
            var exitCode = RunProcessAndWaitForExit(
                "pgrep",
                $"-P {parentId}",
                timeout,
                out var stdout);

            if (exitCode != ExitCodes.Success || string.IsNullOrEmpty(stdout))
            {
                return;
            }

            using var reader = new StringReader(stdout);
            while (true)
            {
                var text = reader.ReadLine();
                if (text == null)
                {
                    return;
                }

                if (int.TryParse(text, out var id))
                {
                    children.Add(id);
                    // Recursively get the children
                    GetAllChildIdsUnix(id, children, timeout);
                }
            }
        }

        private static void KillProcessUnix(int processId, TimeSpan timeout) =>
            RunProcessAndWaitForExit(
                "kill",
                $"-TERM {processId}",
                timeout,
                out _);

        private static int RunProcessAndWaitForExit(string fileName, string arguments, TimeSpan timeout, out string stdout)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(startInfo);

            stdout = null;
            if (process.WaitForExit((int)timeout.TotalMilliseconds))
            {
                stdout = process.StandardOutput.ReadToEnd();
            }
            else
            {
                process.Kill();
            }

            return process.ExitCode;
        }
    }
}
