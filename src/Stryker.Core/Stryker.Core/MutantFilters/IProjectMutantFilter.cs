using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.MutantFilters
{
    public interface IProjectMutantFilter
    {
        void FilterMutants();
    }
}
