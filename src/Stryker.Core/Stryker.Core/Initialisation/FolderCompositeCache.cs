using System.Collections.Generic;

namespace Stryker.Core.Initialisation;

public class FolderCompositeCache<T>
{
    private FolderCompositeCache()
    {

    }

    private static FolderCompositeCache<T> _instance;
    private static readonly object _lockObj = new object();

    public static FolderCompositeCache<T> Instance
    {
        get
        {
            lock (_lockObj)
            {
                if (_instance == null)
                {
                    _instance = new FolderCompositeCache<T>();
                }

                return _instance;
            }
        }
    }

    public Dictionary<string, T> Cache { get; set; }
}
