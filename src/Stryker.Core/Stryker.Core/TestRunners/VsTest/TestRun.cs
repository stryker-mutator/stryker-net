// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="TestRun.cs" company="NFluent">
//   Copyright 2022 Cyrille DUPUYDAUBY
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Stryker.Core.TestRunners.VsTest;

internal class TestRun
{
    private readonly VsTestDescription _testDescription;
    private readonly IList<TestResult> _results;

    public TestRun(VsTestDescription testDescription)
    {
        _testDescription = testDescription;
        _results = new List<TestResult>(testDescription.NbSubCases);
    }

    public bool AddResult(TestResult result)
    {
        _results.Add(result);
        return _results.Count >= _testDescription.NbSubCases;
    }

    public bool IsComplete() => _results.Count >= _testDescription.NbSubCases;

    public TestResult Result()
    {
        var result = _results.Aggregate((TestResult)null, (acc, next) =>
        {
            if (acc == null)
            {
                return next;
            }
            if (next.Outcome == TestOutcome.Failed || acc.Outcome == TestOutcome.None)
            {
                acc.Outcome = next.Outcome;
            }
            if (acc.StartTime > next.StartTime)
            {
                acc.StartTime = next.StartTime;
            }
            if (acc.EndTime < next.EndTime)
            {
                acc.EndTime = next.EndTime;
            }

            acc.Duration += next.Duration;
            foreach (var message in next.Messages)
            {
                acc.Messages.Add(message);
            }

            return acc;
        });
        return result;
    }
}
