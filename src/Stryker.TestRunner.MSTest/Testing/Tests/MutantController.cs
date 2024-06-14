using System.Reflection;
using Stryker.Shared.Tests;

namespace Stryker.TestRunner.MSTest.Testing.Tests;
internal class MutantController
{
    public const string AnyId = "*";

    private readonly Dictionary<string, int> _mutantTestedBy;

    private MutantController(string helperNamespace, IDictionary<int, ITestIdentifiers> mutantTestMap)
    {
        MutantControlClassName = $"{helperNamespace}.MutantControl";
        _mutantTestedBy = ToTestedByMap(mutantTestMap);
    }

    public static MutantController Create(string helperNamespace, IDictionary<int, ITestIdentifiers> mutantTestMap) =>
        new(helperNamespace, mutantTestMap);

    public FieldInfo? ActiveMutantField { get; set; }
    public string MutantControlClassName { get; }
    public int ActiveMutation { get; private set; }

    public void SetActiveMutation(string id)
    {
        ActiveMutation = GetActiveMutantForTest(id);
        ActiveMutantField?.SetValue(null, ActiveMutation);
    }

    private int GetActiveMutantForTest(string id)
    {
        if (_mutantTestedBy.TryGetValue(id, out var test))
        {
            return test;
        }

        return _mutantTestedBy.TryGetValue(AnyId, out var value) ? value : -1;
    }

    private static Dictionary<string, int> ToTestedByMap(IDictionary<int, ITestIdentifiers> mutantTestMap) =>
        mutantTestMap
        .SelectMany(entry => entry.Value.GetIdentifiers().Select(id => (entry.Key, id)))
        .ToDictionary(key => key.id.ToString(), value => value.Key);
}
