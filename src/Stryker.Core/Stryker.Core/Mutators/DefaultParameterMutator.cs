﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using Stryker.Abstractions.Options;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators;

public class DefaultParameterMutator : MutatorBase<InvocationExpressionSyntax>
{
    private readonly ICSharpMutantOrchestrator _orchestrator;
    private readonly IStrykerOptions _options;

    public override MutationLevel MutationLevel => MutationLevel.Complete;

    public DefaultParameterMutator(ICSharpMutantOrchestrator orchestrator, IStrykerOptions options)
    {
        _orchestrator = orchestrator;
        _options = options;
    }

    public override IEnumerable<Mutation> ApplyMutations(InvocationExpressionSyntax node, SemanticModel semanticModel)
    {
        var methodSymbol = (IMethodSymbol)semanticModel.GetSymbolInfo(node).Symbol;
        if (methodSymbol is null)
        {
            yield break;
        }
        var parameterSymbols = methodSymbol.Parameters;

        var parametersWithDefaultValues = parameterSymbols.Where(p => p.HasExplicitDefaultValue);

        var overridenDefaultParameters = new List<IParameterSymbol>();
        foreach (var argument in node.ArgumentList.Arguments)
        {
            if (argument.NameColon is not null)
            {
                overridenDefaultParameters.AddRange(parametersWithDefaultValues.Where(p => p.Name == argument.NameColon.Name.Identifier.ValueText));
            }
            else
            {
                overridenDefaultParameters.AddRange(parametersWithDefaultValues.Where(p => p.Ordinal == node.ArgumentList.Arguments.IndexOf(argument)));
            }
        }

        var unoverridenDefaultParameters = parametersWithDefaultValues.Except(overridenDefaultParameters);

        foreach (var parameter in unoverridenDefaultParameters)
        {
            var parameterName = parameter.Name;
            var argumentNameColon = SyntaxFactory.NameColon(parameterName);

            // If we cannot find the parameter declaration, we should not try to mutate.
            if (parameter.DeclaringSyntaxReferences.IsDefaultOrEmpty)
            {
                yield break;
            }

            var parameterSyntaxNode = (ParameterSyntax)parameter.DeclaringSyntaxReferences[0].GetSyntax();

            var parameterDefaultExpressionNode = parameterSyntaxNode.Default!.Value;
            var mutatedDefaultValues = MutateDefaultValueNode(parameterDefaultExpressionNode, semanticModel);

            foreach (var mutatedDefaultValue in mutatedDefaultValues)
            {
                var namedArgument = SyntaxFactory.Argument(nameColon: argumentNameColon, expression: mutatedDefaultValue, refKindKeyword: default);

                var arguments = node.ArgumentList.Arguments.Add(namedArgument);
                var newArgumentList = SyntaxFactory.ArgumentList(arguments);

                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = node.WithArgumentList(newArgumentList),
                    DisplayName = $"Default parameter mutation: {parameterName}",
                    Type = Mutator.DefaultParameter
                };
            }
        }
    }

    private IEnumerable<ExpressionSyntax> MutateDefaultValueNode(SyntaxNode defaultValueNode, SemanticModel semanticModel)
    {
        List<ExpressionSyntax> nodeMutations = new();
        foreach (var mutator in _orchestrator.Mutators.Where(m => m is not DefaultParameterMutator))
        {
            var mutations = mutator.Mutate(defaultValueNode, semanticModel, _options);
            foreach (var mutation in mutations)
            {
                nodeMutations.Add((ExpressionSyntax)mutation.ReplacementNode);
            }
        }

        return nodeMutations;
    }
}
