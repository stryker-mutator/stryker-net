using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Stryker.Core.Mutants
{
    class MutantComparer : IEqualityComparer<Mutant>
    {
        public bool Equals(Mutant x, Mutant y)
        {
            if (ReferenceEquals(x, y))
                return true;


            if (x is null || y is null)
                return false;

            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] Mutant obj)
        {
            if (obj is null)
                return 0;

            return obj.Id.GetHashCode();
        }
    }
}
