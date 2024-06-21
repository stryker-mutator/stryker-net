using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest;

/// <summary>
/// An extension to the [Ignore] attribute. Instead of using test lists / test categories to conditionally
/// skip tests, allow a [TestClass] or [TestMethod] to specify a method to run. If the method returns
/// `true` the test method will be skipped. The "ignore criteria" method must be `static`, return a single
/// `bool` value, and not accept any parameters. By default, it is named "IgnoreIf".
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class IgnoreIfAttribute : Attribute
{
    public string IgnoreCriteriaMethodName { get; }

    public IgnoreIfAttribute(string ignoreCriteriaMethodName = "IgnoreIf")
    {
        IgnoreCriteriaMethodName = ignoreCriteriaMethodName;
    }

    internal bool ShouldIgnore(ITestMethod testMethod)
    {
        try
        {
            // Search for the method specified by name in this class or any parent classes.
            var searchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Static;
            var method = testMethod.MethodInfo.DeclaringType.GetMethod(IgnoreCriteriaMethodName, searchFlags);
            return (bool)method.Invoke(null, null);
        }
        catch (Exception e)
        {
            var message = $"Conditional ignore method {IgnoreCriteriaMethodName} not found. Ensure the method is in the same class as the test method, marked as `static`, returns a `bool`, and doesn't accept any parameters.";
            throw new ArgumentException(message, e);
        }
    }
}

/// <summary>
/// An extension to the [TestMethod] attribute. It walks the method and class hierarchy looking
/// for an [IgnoreIf] attribute. If one or more are found, they are each evaluated, if the attribute
/// returns `true`, evaluation is short-circuited, and the test method is skipped.
/// </summary>
public class TestMethodWithIgnoreIfSupportAttribute : TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var ignoreAttributes = FindAttributes(testMethod);

        // Evaluate each attribute, and skip if one returns `true`
        foreach (var ignoreAttribute in ignoreAttributes)
        {
            if (ignoreAttribute.ShouldIgnore(testMethod))
            {
                var message = $"Test not executed. Conditional ignore method '{ignoreAttribute.IgnoreCriteriaMethodName}' evaluated to 'true'.";
                return new[]
                {
                    new TestResult
                    {
                        Outcome = UnitTestOutcome.Inconclusive,
                        TestFailureException = new AssertInconclusiveException(message)
                    }
                };
            }
        }
        return base.Execute(testMethod);
    }

    private IEnumerable<IgnoreIfAttribute> FindAttributes(ITestMethod testMethod)
    {
        // Look for an [IgnoreIf] on the method, including any virtuals this method overrides
        var ignoreAttributes = new List<IgnoreIfAttribute>();
        ignoreAttributes.AddRange(testMethod.GetAttributes<IgnoreIfAttribute>(inherit: true));

        // Walk the class hierarchy looking for an [IgnoreIf] attribute
        var type = testMethod.MethodInfo.DeclaringType;
        while (type != null)
        {
            ignoreAttributes.AddRange(type.GetCustomAttributes<IgnoreIfAttribute>(inherit: true));
            type = type.DeclaringType;
        }
        return ignoreAttributes;
    }
}

/// <summary>
/// An extension of the [TestClass] attribute. If applied to a class, any [TestMethod] attributes
/// are automatically upgraded to [TestMethodWithIgnoreIfSupport].
/// </summary>
public class TestClassWithIgnoreIfSupportAttribute : TestClassAttribute
{
    public override TestMethodAttribute GetTestMethodAttribute(TestMethodAttribute testMethodAttribute)
    {
        if (testMethodAttribute is TestMethodWithIgnoreIfSupportAttribute)
        {
            return testMethodAttribute;
        }
        return new TestMethodWithIgnoreIfSupportAttribute();
    }
}

public static class Is
{
    public static bool Windows() => Environment.OSVersion.Platform == PlatformID.Win32NT;
    public static bool Unix() => Environment.OSVersion.Platform == PlatformID.Unix;
}
