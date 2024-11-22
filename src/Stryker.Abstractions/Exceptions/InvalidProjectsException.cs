using System;

namespace Stryker.Abstractions.Exceptions;

/// <summary>
/// Represents error when no test projects are found in the solution or configured for stryker.
/// </summary>
public class InvalidProjectsException : Exception
{
    public InvalidProjectsException(string message)
        : base(message)
    {
    }

    public static InvalidProjectsException NoTestProjectsFound()
    {
        return new InvalidProjectsException("No test projects found. Please add a test project to your solution or fix your stryker config.");
    }

    public static InvalidProjectsException OnlyTestProjectsFound()
    {
        return new InvalidProjectsException("Only test projects found. Please ensure that your solution contains non-test projects.");
    }
}
