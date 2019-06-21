using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
    public class MutantOrchestratorTests
    {
        private Collection<IMutator> _mutators { get; set; }
        private string _currentDirectory { get; set; }
        private MutantOrchestrator _target { get; set; }

        public MutantOrchestratorTests()
        {
            _target = new MutantOrchestrator();
            _currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        [Fact]
        public void IfStatementsShouldBeNested()
        {
            string source = @"void TestMethod()
{
    int i = 0;
    if (i + 8 == 8)
    {
        i = i + 1;
        if (i + 8 == 9)
        {
            i = i + 1;
        };
    }
    else
    {
        i = i + 3;
        if (i == i + i - 8)
        {
            i = i + 1;
        };
    }

    if (!Out(out var test))
    {
        return i + 1;
    }

    if (i is int x)
    {
        return x + 1;
    }
}

private bool Out(out string test)
{
    return true;
}";
            string expected = @"void TestMethod()
{
    int i = 0;
    if ((StrykerNamespace.ActiveMutationHelper.ActiveMutation==1?i + 8 != 8:(StrykerNamespace.ActiveMutationHelper.ActiveMutation==0?i - 8 :i + 8 )== 8))
    {
        i = (StrykerNamespace.ActiveMutationHelper.ActiveMutation==7?i - 1:i + 1);
        if ((StrykerNamespace.ActiveMutationHelper.ActiveMutation==9?i + 8 != 9:(StrykerNamespace.ActiveMutationHelper.ActiveMutation==8?i - 8 :i + 8 )== 9))
        {
            i = (StrykerNamespace.ActiveMutationHelper.ActiveMutation==10?i - 1:i + 1);
        };
    }
    else
    {
        i = (StrykerNamespace.ActiveMutationHelper.ActiveMutation==2?i - 3:i + 3);
        if ((StrykerNamespace.ActiveMutationHelper.ActiveMutation==5?i != i + i - 8:i == (StrykerNamespace.ActiveMutationHelper.ActiveMutation==4?i + i + 8:(StrykerNamespace.ActiveMutationHelper.ActiveMutation==3?i - i :i + i )- 8)))
        {
            i = (StrykerNamespace.ActiveMutationHelper.ActiveMutation==6?i - 1:i + 1);
        };
    }

    if (!Out(out var test))
    {
        return (StrykerNamespace.ActiveMutationHelper.ActiveMutation==11?i - 1:i + 1);
    }

    if (i is int x)
    {
        return (StrykerNamespace.ActiveMutationHelper.ActiveMutation==12?x - 1:x + 1);
    }
}

private bool Out(out string test)
{
    return (StrykerNamespace.ActiveMutationHelper.ActiveMutation==13?false:true);
}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateInsideStringDeclarationInsideLocalFunction()
        {
            string source = @"void TestMethod()
        {
            string SomeLocalFunction()
            {
                var test3 = 2 + 5;
                return $""test{1 + test3}"";
            };
        }";
            string expected = @"void TestMethod()
        {
            string SomeLocalFunction()
            {
                var test3 = (StrykerNamespace.ActiveMutationHelper.ActiveMutation==0?2 -5:2 + 5);
                return (StrykerNamespace.ActiveMutationHelper.ActiveMutation==2?$"""":$""test{ (StrykerNamespace.ActiveMutationHelper.ActiveMutation == 1 ? 1 - test3 : 1 + test3)}"");
            };
        }";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateProperties()
        {
            string source = @"private string text = ""Some"" + ""Text"";";
            string expected = @"private string text = (StrykerNamespace.ActiveMutationHelper.ActiveMutation == 0 ? """" : ""Some"") + (StrykerNamespace.ActiveMutationHelper.ActiveMutation == 1 ? """" : ""Text"");";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateTupleDeclaration()
        {
            string source = @"public void TestMethod() {
var (one, two) = (1 + 1, """");
}";
            string expected = @"public void TestMethod() {
var (one, two) = ((StrykerNamespace.ActiveMutationHelper.ActiveMutation==0?1 - 1:1 + 1), (StrykerNamespace.ActiveMutationHelper.ActiveMutation==1?""Stryker was here!"":""""));
}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateConst()
        {
            string source = @"private const int x = 1 + 2;";
            string expected = @"private const int x = 1 + 2;";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateAttributes()
        {
            string source = @"[Obsolete(""thismustnotbemutated"")]
public void SomeMethod() {}";
            string expected = @"[Obsolete(""thismustnotbemutated"")]
public void SomeMethod() {}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateForWithIfStatementAndConditionalStatement()
        {
            string source = @"public void SomeMethod() {
for (var i = 0; i < 10; i++)
{ }
}";
            string expected = @"public void SomeMethod() {
if(StrykerNamespace.ActiveMutationHelper.ActiveMutation==0){for (var i = 0; i < 10; i--)
{ }
}else{for (var i = 0; (StrykerNamespace.ActiveMutationHelper.ActiveMutation==2?i <= 10:(StrykerNamespace.ActiveMutationHelper.ActiveMutation==1?i > 10:i < 10)); i++)
{ }
}}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateForWithoutConditionWithIfStatementAndConditionalStatement()
        {
            string source = @"public void SomeMethod() {
for (var i = 0; ; i++)
{ }
}";
            string expected = @"public void SomeMethod() {
if(StrykerNamespace.ActiveMutationHelper.ActiveMutation==0){for (var i = 0; ; i--)
{ }
}else{for (var i = 0; ; i++)
{ }
}}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateInlineArrowFunction()
        {
            string source = @"public void SomeMethod() {
int Add(int x, int y) => x + y;
}";
            string expected = @"public void SomeMethod() {
int Add(int x, int y) => (StrykerNamespace.ActiveMutationHelper.ActiveMutation==0?x - y:x + y);
}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateLambdaSecondParameter()
        {
            string source = @"public void SomeMethod() {
Action act = () => Console.WriteLine(1 + 1, 1 + 1);
}";
            string expected = @"public void SomeMethod() {
Action act = () => Console.WriteLine((StrykerNamespace.ActiveMutationHelper.ActiveMutation==0?1 - 1:1 + 1), (StrykerNamespace.ActiveMutationHelper.ActiveMutation==1?1 - 1:1 + 1));
}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateLinqMethods()
        {
            string source = @"public int TestLinq(int count){ 
    var list = Enumerable.Range(1, count);
    return list.Last();
}";
            string expected = @"public int TestLinq(int count){ 
    var list = Enumerable.Range(1, count);
    return (StrykerNamespace.ActiveMutationHelper.ActiveMutation==0?list.First():list.Last());
}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateAssignmentStatementsWithIfStatement()
        {
            string source = @"public void SomeMethod() {
    var x = 0;
    x *= x + 2;
}";
            string expected = @"public void SomeMethod() {
    var x = 0;
    if (StrykerNamespace.ActiveMutationHelper.ActiveMutation == 0)
    {
        x /= x + 2;
    }
    else
    {
        x *= (StrykerNamespace.ActiveMutationHelper.ActiveMutation == 1 ? x - 2 : x + 2);
    }
}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateIncrementStatementWithIfStatement()
        {
            string source = @"public void SomeMethod() {
    var x = 0;
    x++;
}";
            string expected = @"public void SomeMethod() {
    var x = 0;
    if (StrykerNamespace.ActiveMutationHelper.ActiveMutation == 0)
    {
        x--;
    }
    else
    {
        x++;
    }  
}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateDefaultValues()
        {
            string source = @"public void SomeMethod(bool option = true) {}";
            string expected = @"public void SomeMethod(bool option = true) {}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateInsideLambda()
        {
            string source = @"private async Task GoodLuck()
{
    await SendRequest(url, HttpMethod.Get, (request) =>
    {
        request.Headers.Add(""Accept"", ""application / json; version = 1"");
        request.Headers.TryAddWithoutValidation(""Date"", date);
}, ensureSuccessStatusCode: false);
}";
            string expected = @"private async Task GoodLuck()
{
    await SendRequest(url, HttpMethod.Get, (request) =>
    {
        request.Headers.Add((StrykerNamespace.ActiveMutationHelper.ActiveMutation==0?"""":""Accept""), (StrykerNamespace.ActiveMutationHelper.ActiveMutation==1?"""":""application / json; version = 1""));
        request.Headers.TryAddWithoutValidation((StrykerNamespace.ActiveMutationHelper.ActiveMutation==2?"""":""Date""), date);
}, ensureSuccessStatusCode: (StrykerNamespace.ActiveMutationHelper.ActiveMutation==3?true:false));
}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void MutationsShouldHaveLinespan()
        {
            string source = @"void TestMethod()
{
    var test3 = 2 + 5;
}";
            string expected = @"void TestMethod()
{
    var test3 = (StrykerNamespace.ActiveMutationHelper.ActiveMutation==0?2 - 5:2 + 5);
}";
            expected = expected.Replace("StrykerNamespace", MutantPlacer.HelperNamespace);
            var actualNode = _target.Mutate(CSharpSyntaxTree.ParseText(source).GetRoot());
            var expectedNode = CSharpSyntaxTree.ParseText(expected).GetRoot();
            actualNode.ShouldBeSemantically(expectedNode);
            actualNode.ShouldNotContainErrors();

            var mutants = _target.GetLatestMutantBatch().ToList();
            mutants.ShouldHaveSingleItem().Mutation.OriginalNode.GetLocation().GetLineSpan().StartLinePosition.Line.ShouldBe(2);
        }

        private void ShouldMutateSourceToExpected(string original, string expected)
        {
            original = @"using System;
using System.Collections.Generic;
            using System.Text;

namespace StrykerNet.UnitTest.Mutants.TestResources
    {
        class TestClass
        {" + original + "}}";

            expected = @"using System;
using System.Collections.Generic;
            using System.Text;

namespace StrykerNet.UnitTest.Mutants.TestResources
    {
        class TestClass
        {" + expected + "}}";
            expected = expected.Replace("StrykerNamespace", MutantPlacer.HelperNamespace);
            var actualNode = _target.Mutate(CSharpSyntaxTree.ParseText(original).GetRoot());
            var expectedNode = CSharpSyntaxTree.ParseText(expected).GetRoot();
            actualNode.ShouldBeSemantically(expectedNode);
            actualNode.ShouldNotContainErrors();
        }
    }
}
