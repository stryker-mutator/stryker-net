using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Stryker.Abstractions;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.CLI.MutationServer;

internal static class MutationServerMutantIdentity
{
    public static string GetFileName(string basePath, IReadOnlyFileLeaf file)
        => Path.GetRelativePath(basePath, file.FullPath).Replace('\\', '/');

    public static string GetId(string basePath, IReadOnlyFileLeaf file, IReadOnlyMutant mutant)
    {
        var location = GetLocation(mutant);
        var fingerprint = string.Join(
            '\n',
            GetFileName(basePath, file),
            $"{location.Start.Line}:{location.Start.Column}-{location.End.Line}:{location.End.Column}",
            mutant.Mutation.DisplayName,
            mutant.Mutation.ReplacementNode?.ToString(),
            mutant.Mutation.Description);

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(fingerprint)));
    }

    public static MutationServerLocation GetLocation(IReadOnlyMutant mutant)
    {
        var location = mutant.Mutation.OriginalNode.GetLocation().GetMappedLineSpan();
        return new MutationServerLocation
        {
            Start = new MutationServerPosition
            {
                Line = location.StartLinePosition.Line + 1,
                Column = location.StartLinePosition.Character + 1
            },
            End = new MutationServerPosition
            {
                Line = location.EndLinePosition.Line + 1,
                Column = location.EndLinePosition.Character + 1
            }
        };
    }
}
