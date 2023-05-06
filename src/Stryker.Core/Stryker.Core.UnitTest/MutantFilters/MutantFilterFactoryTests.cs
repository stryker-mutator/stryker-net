using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.DiffProviders;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters;

public class MutantFilterFactoryTests : TestBase
{
    [Fact]
    public void MutantFilterFactory_Creates_of_type_BroadcastFilter()
    {
        var options = new StrykerOptions()
        {
            Since = true
        };

        var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);
        var branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);
        var baselineProvider = new Mock<IBaselineProvider>(MockBehavior.Loose);

        var result = MutantFilterFactory.Create(options, null, diffProviderMock.Object, baselineProvider.Object, branchProviderMock.Object);

        result.ShouldBeOfType<BroadcastMutantFilter>();
    }

    [Fact]
    public void Create_Throws_ArgumentNullException_When_Stryker_Options_Is_Null()
    {
        var result = Should.Throw<ArgumentNullException>(() => MutantFilterFactory.Create(null, null));
    }

    [Fact]
    public void MutantFilterFactory_Creates_Standard_Mutant_Filters()
    {
        // Arrange
        var options = new StrykerOptions()
        {
            Since = false
        };
        var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);
        var branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);
        var baselineProvider = new Mock<IBaselineProvider>(MockBehavior.Loose);

        // Act
        var result = MutantFilterFactory.Create(options, null, diffProviderMock.Object, baselineProvider.Object, branchProviderMock.Object);

        // Assert
        var resultAsBroadcastFilter = result as BroadcastMutantFilter;

        resultAsBroadcastFilter.MutantFilters.Count().ShouldBe(5);
    }

    [Fact]
    public void MutantFilterFactory_Creates_DiffMutantFilter_When_Since_Enabled()
    {
        // Arrange
        var options = new StrykerOptions()
        {
            Since = true
        };

        var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);
        var branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);
        var baselineProvider = new Mock<IBaselineProvider>(MockBehavior.Loose);

        // Act
        var result = MutantFilterFactory.Create(options, null, diffProviderMock.Object, baselineProvider.Object, branchProviderMock.Object);

        // Assert
        var resultAsBroadcastFilter = result as BroadcastMutantFilter;

        resultAsBroadcastFilter.MutantFilters.Count().ShouldBe(6);

        resultAsBroadcastFilter.MutantFilters.Where(x => x.GetType() == typeof(SinceMutantFilter)).Count().ShouldBe(1);
    }

    [Fact]
    public void MutantFilterFactory_Creates_ExcludeLinqExpressionFilter_When_ExcludedLinqExpressions_IsNotEmpty()
    {
        // Arrange
        var options = new StrykerOptions()
        {
            ExcludedLinqExpressions = new List<LinqExpression>() { LinqExpression.Any }
        };

        var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);
        var branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);
        var baselineProvider = new Mock<IBaselineProvider>(MockBehavior.Loose);

        // Act
        var result = MutantFilterFactory.Create(options, null, diffProviderMock.Object, baselineProvider.Object, branchProviderMock.Object);

        // Assert
        var resultAsBroadcastFilter = result.ShouldBeOfType<BroadcastMutantFilter>();

        resultAsBroadcastFilter.MutantFilters.Count().ShouldBe(6);

        resultAsBroadcastFilter.MutantFilters.Where(x => x.GetType() == typeof(ExcludeLinqExpressionFilter)).Count().ShouldBe(1);
    }

    [Fact]
    public void MutantFilterFactory_Creates_DashboardMutantFilter_And_DiffMutantFilter_WithBaseline_Enabled() {
        var options = new StrykerOptions()
        {
            WithBaseline = true,
            ProjectVersion = "foo"
        };

        var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);
        var gitInfoProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);
        var baselineProviderMock = new Mock<IBaselineProvider>(MockBehavior.Loose);

        var result = MutantFilterFactory.Create(options, null, diffProviderMock.Object, baselineProviderMock.Object, gitInfoProviderMock.Object);

        var resultAsBroadcastFilter = result as BroadcastMutantFilter;

        resultAsBroadcastFilter.MutantFilters.Count().ShouldBe(7);
        resultAsBroadcastFilter.MutantFilters.Where(x => x.GetType() == typeof(BaselineMutantFilter)).Count().ShouldBe(1);
        resultAsBroadcastFilter.MutantFilters.Where(x => x.GetType() == typeof(SinceMutantFilter)).Count().ShouldBe(1);
    }

    [Fact]
    public void MutantFilterFactory_Creates_BlockMutantFilter_Last()
    {
        // Arrange
        var options = new StrykerOptions()
        {
            // These options are added here to make sure this test covers all branches in the source method.
            WithBaseline = true,
            ExcludedLinqExpressions = new List<LinqExpression>
            {
                LinqExpression.Distinct
            },
        };
        var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Strict);
        var gitInfoProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Strict);
        var baselineProviderMock = new Mock<IBaselineProvider>(MockBehavior.Strict);
        var branch = "branch";
        gitInfoProviderMock.Setup(m => m.GetCurrentBranchName()).Returns(branch);
        baselineProviderMock.Setup(m => m.Load($"baseline/{branch}")).ReturnsAsync(new JsonReport());
        diffProviderMock.Setup(m => m.ScanDiff()).Returns(new DiffResult());
        diffProviderMock.Setup(m => m.Tests).Returns(new TestSet());

        // Act
        var result = MutantFilterFactory.Create(options, null, diffProviderMock.Object, baselineProviderMock.Object, gitInfoProviderMock.Object);
        var broadcastFilterResult = result as BroadcastMutantFilter;

        // Assert
        broadcastFilterResult.MutantFilters.Last().ShouldBeOfType<IgnoreBlockMutantFilter>();
    }
}
