using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Options;
using Stryker.Core.ProjectComponents.SourceProjects;

namespace Stryker.Core.Initialisation;

/// <summary>
/// Creates the language-specific <see cref="ProjectComponentsBuilder"/>. Registered as a keyed service per <see cref="Stryker.Abstractions.Language"/>.
/// </summary>
public delegate ProjectComponentsBuilder ProjectComponentsBuilderFactory(
    SourceProjectInfo projectInfo,
    IStrykerOptions options,
    string[] foldersToExclude,
    ILogger logger,
    IFileSystem fileSystem);
