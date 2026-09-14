using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace UnityProject.Tests;

public class CalculatorTests
{
    [UnityTest]
    public IEnumerator AddsNumbers()
    {
        Assert.That(Calculator.Add(1, 2), Is.EqualTo(3));
        yield break;
    }
}
