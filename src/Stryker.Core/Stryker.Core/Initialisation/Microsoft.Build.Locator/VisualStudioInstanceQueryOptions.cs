// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Build.Locator
{
    /// <summary>
    ///     Options to consider when querying for Visual Studio instances.
    /// </summary>
    public class VisualStudioInstanceQueryOptions
    {
        /// <summary>
        ///     Default query options (all instances).
        /// </summary>
        public static VisualStudioInstanceQueryOptions Default => new VisualStudioInstanceQueryOptions
        {
            DiscoveryTypes = DiscoveryType.DeveloperConsole & DiscoveryType.VisualStudioSetup
        };

        /// <summary>
        ///     Discovery types for instances included in the query.
        /// </summary>
        public DiscoveryType DiscoveryTypes { get; set; }
    }
}