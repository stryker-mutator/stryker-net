using System.Collections.Generic;
using System.Reflection;

namespace Stryker.Core.TestRunners.MsTest.Testing.Tests;

internal class MutantController
{
    private readonly IEnumerable<int> _mutants;

    private MutantController(string helperNamespace, IEnumerable<int> mutants)
    {
        MutantControlClassName = $"{helperNamespace}.MutantControl";
        _mutants = mutants;
    }

    public static MutantController Create(string helperNamespace, IEnumerable<int> mutants) => new(helperNamespace, mutants);

    public FieldInfo IsAsyncRunField { get; set; }
    public MethodInfo InitAsyncRunMethod { get; set; }

    public string MutantControlClassName { get; }

    public void SetAsync(bool enabled) => IsAsyncRunField?.SetValue(null, enabled);

    public void InitAsyncRun() => InitAsyncRunMethod?.Invoke(null, [_mutants]);
}
