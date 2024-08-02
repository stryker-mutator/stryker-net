using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stryker.Configuration.Mutants;
using Stryker.Configuration.TestRunners;

namespace Stryker.Configuration;

/// <summary>
/// This interface should only contain readonly properties to ensure that others than the mutation test process cannot modify mutants.
/// </summary>
public interface IReadOnlyMutant
{
    int Id { get; }
    Mutation Mutation { get; }
    MutantStatus ResultStatus { get; }
    string ResultStatusReason { get; }
    ITestGuids CoveringTests { get; }
    ITestGuids KillingTests { get; }
    ITestGuids AssessingTests { get; }
    bool CountForStats { get; }
    bool IsStaticValue { get; }
}
