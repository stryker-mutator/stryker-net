// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Taken from https://github.com/Microsoft/msbuild/blob/6851538897f5d7b08024a6d8435bc44be5869e53/src/Shared/VisualStudioLocationHelper.cs

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
//using Microsoft.VisualStudio.Setup.Configuration;

namespace Microsoft.Build.Locator
{
    /// <summary>
    ///     Helper class to wrap the Microsoft.VisualStudio.Setup.Configuration.Interop API to query
    ///     Visual Studio setup for instances installed on the machine.
    ///     Code derived from sample: https://code.msdn.microsoft.com/Visual-Studio-Setup-0cedd331
    /// </summary>
    internal class VisualStudioLocationHelper
    {
        private const int REGDB_E_CLASSNOTREG = unchecked((int)0x80040154);

        /// <summary>
        ///     Query the Visual Studio setup API to get instances of Visual Studio installed
        ///     on the machine. Will not include anything before Visual Studio "15".
        /// </summary>
        /// <returns>Enumerable list of Visual Studio instances</returns>
        internal static IList<VisualStudioInstance> GetInstances()
        {
            var validInstances = new List<VisualStudioInstance>();

            //try
            //{
            //    // This code is not obvious. See the sample (link above) for reference.
            //    var query = (ISetupConfiguration2)GetQuery();
            //    var e = query.EnumAllInstances();

            //    int fetched;
            //    var instances = new ISetupInstance[1];
            //    do
            //    {
            //        // Call e.Next to query for the next instance (single item or nothing returned).
            //        e.Next(1, instances, out fetched);
            //        if (fetched <= 0) continue;

            //        var instance = instances[0];
            //        var state = ((ISetupInstance2)instance).GetState();

            //        if (!Version.TryParse(instance.GetInstallationVersion(), out Version version))
            //            continue;

            //        // If the install was complete and a valid version, consider it.
            //        if (state == InstanceState.Complete ||
            //            (state.HasFlag(InstanceState.Registered) && state.HasFlag(InstanceState.NoRebootRequired)))
            //            validInstances.Add(new VisualStudioInstance(
            //                instance.GetDisplayName(),
            //                instance.GetInstallationPath(),
            //                version,
            //                DiscoveryType.VisualStudioSetup));
            //        ;
            //    } while (fetched > 0);
            //}
            //catch (COMException)
            //{
            //}
            //catch (DllNotFoundException)
            //{
            //    // This is OK, VS "15" or greater likely not installed.
            //}
            return validInstances;
        }

        //private static ISetupConfiguration GetQuery()
        //{
        //    try
        //    {
        //        // Try to CoCreate the class object.
        //        return new SetupConfiguration();
        //    }

        //    catch (COMException ex) when (ex.ErrorCode == REGDB_E_CLASSNOTREG)
        //    {
        //        // Try to get the class object using app-local call.
        //        ISetupConfiguration query;
        //        var result = GetSetupConfiguration(out query, IntPtr.Zero);

        //        if (result < 0)
        //            throw new COMException($"Failed to get {nameof(query)}", result);

        //        return query;
        //    }
        //}

        //[DllImport("Microsoft.VisualStudio.Setup.Configuration.Native.dll", ExactSpelling = true, PreserveSig = true)]
        //private static extern int GetSetupConfiguration(
        //    [MarshalAs(UnmanagedType.Interface)] [Out] out ISetupConfiguration configuration,
        //    IntPtr reserved);
    }
}