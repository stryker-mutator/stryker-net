using Stryker.Core.Reporters.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.DashboardCompare
{
    /// <summary>
    /// Singleton class intended to hold the baseline report. Can be initiated by calling BaselineReport.Instance;
    /// </summary>
    public sealed class BaselineReport
    {
        /// <summary>
        /// Constructor is private, making sure it is impossible to create a new instance of BaselineReport when the _instance property is set.
        /// </summary>
        private BaselineReport()
        {

        }

        /// <summary>
        /// _lockObj is used as the lock object in the getter of Instance
        /// </summary>
        private static readonly object _lockObj = new object();

        /// <summary>
        /// The actual instance of BaselineReport
        /// </summary>
        private static BaselineReport _instance = null;

        /// <summary>
        /// Thread safe getter for _instance of this class.
        /// If _instance is null, _instance will be set to a fresh instance of this class.
        /// Setting properties has to be done manually.
        /// </summary>
        public static BaselineReport Instance
        {
            get
            {
                lock (_lockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new BaselineReport();
                    }
                    return _instance;
                }
            }
        }
        /// <summary>
        /// Value that holds the actual report
        /// </summary>
        private JsonReport _report;

        /// <summary>
        /// Property for the report. The setter makes sure the value can only be set once.
        /// </summary>
        public JsonReport Report
        {
            get
            {
                return _report;
            }
            set
            {
                if (_report != null) throw new InvalidOperationException("Value of Report is already set");

                _report = value;
            }
        }
    }
}
