using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.DashboardCompare
{
    public interface IBranchProvider
    {
        string GetCurrentBranchCanonicalName();
    }
}
