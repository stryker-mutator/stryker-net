namespace Stryker.Abstractions.Exceptions;

/// <summary>
/// Represents error when no test projects are found in the solution or configured for stryker.
/// </summary>
public class NoTestProjectsException()
    : Exception("No test projects found. Please add a test project to your solution or fix your stryker config.");
