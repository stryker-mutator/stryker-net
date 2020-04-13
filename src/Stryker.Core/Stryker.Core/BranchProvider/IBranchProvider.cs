using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.BranchProvider
{
    public interface IBranchProvider
    {
        string GetCurrentBranchCanonicalName();
    }
}
