using System;

namespace Stryker.Abstractions.Exceptions;

/// <summary>
/// Represents errors which are related to roslyn compilation errors that we cannot fix,
/// but the user might also might not be able to fix
/// </summary>
public class CompilationException(string message) : Exception(message);
