using Stryker.Core.ProjectComponents;
using System.Collections.Generic;

namespace Stryker.Core.Initialisation
{
    public class FolderCompositeCache
    {
        private FolderCompositeCache()
        {

        }

        private static FolderCompositeCache _instance;
        private static object _lockObj = new object();

        public static FolderCompositeCache Instance
        {
            get
            {
                lock (_lockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new FolderCompositeCache();
                    }

                    return _instance;
                }
            }
        }

        public Dictionary<string, FolderComposite> Cache { get; set; }
    }
}
