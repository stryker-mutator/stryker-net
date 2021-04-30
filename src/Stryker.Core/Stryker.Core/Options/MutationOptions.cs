using System.Collections.Generic;

namespace Stryker.Core.Options
{
    public class MutationOptions
    {
        public MutationOptions(IDictionary<string, bool> linq)
        {
            Linq = linq;
        }

        public IDictionary<string, bool> Linq { get; private set; }
    }
}
