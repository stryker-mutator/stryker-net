// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.Build.Locator
{
    /// <summary>
    ///     Enum to indicate type of Visual Studio discovery.
    /// </summary>
    [Flags]
    public enum DiscoveryType
    {
        /// <summary>
        ///     Discovery via the current environment. This indicates the caller originated
        ///     from a Visual Studio Developer Command Prompt.
        /// </summary>
        DeveloperConsole = 1,

        /// <summary>
        ///     Discovery via Visual Studio Setup API.
        /// </summary>
        VisualStudioSetup = 2
    }
}