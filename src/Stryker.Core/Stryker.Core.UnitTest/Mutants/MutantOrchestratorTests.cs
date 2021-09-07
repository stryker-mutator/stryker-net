using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
    public class MutantOrchestratorTests : MutantOrchestratorTestsBase
    {
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
{if(StrykerNamespace.MutantControl.IsActive(11)){
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

    if (Out(out var test))
    {
        return i + 1;
    }

    if (i is int x)
    {
        return x + 1;
    }
}
else{
    int i = 0;
    if ((StrykerNamespace.MutantControl.IsActive(0)?i + 8 != 8:(StrykerNamespace.MutantControl.IsActive(1)?i - 8 :i + 8 )== 8))
    {
        i = (StrykerNamespace.MutantControl.IsActive(2)?i - 1:i + 1);
        if ((StrykerNamespace.MutantControl.IsActive(3)?i + 8 != 9:(StrykerNamespace.MutantControl.IsActive(4)?i - 8 :i + 8 )== 9))
        {
            i = (StrykerNamespace.MutantControl.IsActive(5)?i - 1:i + 1);
        };
    }
    else
    {
        i = (StrykerNamespace.MutantControl.IsActive(6)?i - 3:i + 3);
        if ((StrykerNamespace.MutantControl.IsActive(7)?i != i + i - 8:i == (StrykerNamespace.MutantControl.IsActive(8)?i + i + 8:(StrykerNamespace.MutantControl.IsActive(9)?i - i :i + i )- 8)))
        {
            i = (StrykerNamespace.MutantControl.IsActive(10)?i - 1:i + 1);
        };
    }

    if (!Out(out var test))
    {
        return (StrykerNamespace.MutantControl.IsActive(12)?i - 1:i + 1);
    }

    if (i is int x)
    {
        return (StrykerNamespace.MutantControl.IsActive(13)?x - 1:x + 1);
    }
}
}
private bool Out(out string test)
{
    {test= default(string);}    return (StrykerNamespace.MutantControl.IsActive(14)?false:true);
}}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotLeakMutationsToNextMethodOrProperty()
        {
            string source = @"public static class ExampleExtension
    {
        private static string[] tabs = { ""tab1"", ""tab2""};

        private List<string> _collection;

        public List<string> Collection
        {
            get => _collection;

            set
            {
                _collection = value;
            }
        }
    }";
            string expected = @"public static class ExampleExtension
    {
        private static string[] tabs = { (StrykerNamespace.MutantControl.IsActive(1)?"""":""tab1""), (StrykerNamespace.MutantControl.IsActive(2)?"""":""tab2"")};

        private List<string> _collection;

        public List<string> Collection
        {
            get => _collection;

            set
            {
                _collection = value;
            }
        }
    }";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateWhenDeclaration()
        {
            string source = @"void TestMethod()
{
    int i = 0;
    var result = Out(out var test) ? test : """";
}
private bool Out(out string test)
{
       return true;
}";
            string expected = @"void TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){
    int i = 0;
    var result = !(Out(out var test) )? test : """";
}
else{
    int i = 0;
    var result = Out(out var test) ? test : (StrykerNamespace.MutantControl.IsActive(1)?""Stryker was here!"":"""");
}
}private bool Out(out string test)
{
{test= default(string);}     return (StrykerNamespace.MutantControl.IsActive(2)?false:true);
}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateWhenDeclarationInInnerScope()
        {
            string source = @"void TestMethod()
{
    int i = 0;
    var result = Out(i, (x) => { int.TryParse(""3"", out int y); return x == y;} ) ? i.ToString() : """";
}
private bool Out(int test, Func<int, bool>lambda )
{
    return true;
}
";
            string expected = @"void TestMethod()
{
    int i = 0;
    var result = (StrykerNamespace.MutantControl.IsActive(0)?!(Out(i, (x) => { int.TryParse(""3"", out int y); return x == y;} ) ):Out(i, (x) => { int.TryParse((StrykerNamespace.MutantControl.IsActive(1)?"""":""3""), out int y); return (StrykerNamespace.MutantControl.IsActive(2)?x != y:x == y);} ) )? i.ToString() : (StrykerNamespace.MutantControl.IsActive(3)?""Stryker was here!"":"""");
        }
        private bool Out(int test, Func<int, bool>lambda )
        {
            return (StrykerNamespace.MutantControl.IsActive(4)?false:true);
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
                var test3 = (StrykerNamespace.MutantControl.IsActive(0)?2 - 5:2 + 5);
                return (StrykerNamespace.MutantControl.IsActive(1)?$"""":$""test{(StrykerNamespace.MutantControl.IsActive(2)?1 - test3:1 + test3)}"");
        };
    }";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldInitializeOutParametersInLocalFunction()
        {
            string source = @"void TestMethod()
        {
            void SomeLocalFunction(out string x)
            {
                var test3 = 2 + 5;
                x = $""test{1 + test3}"";
            };
        }";
            string expected = @"void TestMethod()
        {
             void SomeLocalFunction(out string x)
            {
{x= default(string);}                var test3 = (StrykerNamespace.MutantControl.IsActive(0)?2 - 5:2 + 5);
                x = (StrykerNamespace.MutantControl.IsActive(1)?$"""":$""test{(StrykerNamespace.MutantControl.IsActive(2)?1 - test3:1 + test3)}"");
            };    }";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateConditionalExpressionProperly()
        {
            string source = @"void TestMethod()
        {
            string SomeLocalFunction()
            {
                return string.Empty?.All(x => !string.IsEmpty(x));
            };
        }";
            string expected = @"void TestMethod()
        {
            string SomeLocalFunction()
            {
                return (StrykerNamespace.MutantControl.IsActive(0)?string.Empty?.Any(x => !string.IsEmpty(x)):(StrykerNamespace.MutantControl.IsActive(1)?""Stryker was here!"":string.Empty)?.All(x => (StrykerNamespace.MutantControl.IsActive(2)?string.IsEmpty(x):!string.IsEmpty(x))));
        };
    }";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldAddDefaultReturnToLocalFunction()
        {
            string source = @"void TestMethod()
        {
            string SomeLocalFunction()
            {
                while(true);
            };
        }";
            string expected = @"void TestMethod()
        {
            string SomeLocalFunction()
            {
                while((StrykerNamespace.MutantControl.IsActive(1)?!(true):(StrykerNamespace.MutantControl.IsActive(0)?false:true)));
                return default(string);
        };}";


            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateConditionalExpressionOnArrayDeclaration()
        {
            string source = @"public static IEnumerable<int> Foo() => new int[] { }.ToArray().Any(x => x==1)?.OrderBy(e => e).ToList();";
            string expected = @"public static IEnumerable<int> Foo() => (StrykerNamespace.MutantControl.IsActive(1)?new int[] { }.ToArray().Any(x => x==1)?.OrderByDescending(e => e).ToList():(StrykerNamespace.MutantControl.IsActive(0)?new int[] { }.ToArray().All(x => x==1)?.OrderBy(e => e).ToList():new int[] { }.ToArray().Any(x => (StrykerNamespace.MutantControl.IsActive(2)?x!=1:x==1))?.OrderBy(e => e).ToList()));";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateArrayInitializer()
        {
            string source = @"public int[] Foo(){
int[] test = { 1 };
}";
            string expected = @"public int[] Foo(){
if(StrykerNamespace.MutantControl.IsActive(0)){int[] test = {};
}else{int[] test = { 1 };
}return default(int[] );}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateArrayDeclarationAsReturnValue()
        {
            var source = @"public int[] Foo() => new int[] { 1 };";
            var expected = @"public int[] Foo() => (StrykerNamespace.MutantControl.IsActive(0)?new int[] {}:new int[] { 1 });";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateListCreation()
        {
            var source = @"public int[] Foo() => new List<int> { 1 };";
            var expected = @"public int[] Foo() => (StrykerNamespace.MutantControl.IsActive(0)?new List<int> {}:new List<int> { 1 });";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateImplicitArrayCreationProperties()
        {
            string source = @"public int[] Foo() => new [] { 1 };";
            string expected = @"public int[] Foo() => new [] { 1 };";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateImplicitArrayCreation()
        {
            string source = "public static readonly int[] Foo =  { 1 };";
            string expected = "public static readonly int[] Foo =  { 1 };";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateProperties()
        {
            string source = @"private string text => ""Some"" + ""Text"";";
            string expected = @"private string text => (StrykerNamespace.MutantControl.IsActive(0) ? """" : ""Some"") + (StrykerNamespace.MutantControl.IsActive(1) ? """" : ""Text"");";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateTupleDeclaration()
        {
            string source = @"public void TestMethod() {
var (one, two) = (1 + 1, """");
}";
            string expected = @"public void TestMethod() {
var (one, two) = ((StrykerNamespace.MutantControl.IsActive(0)?1 - 1:1 + 1), (StrykerNamespace.MutantControl.IsActive(1)?""Stryker was here!"":""""));
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
        public void ShouldMutateStackalloc()
        {
            string source = @"Span<ushort> kindaUnrelated = stackalloc ushort[] { 0 };";
            string expected = @"Span<ushort> kindaUnrelated = (StrykerNamespace.MutantControl.IsActive(0)?stackalloc ushort[] {}:stackalloc ushort[] { 0 });";

            ShouldMutateSourceToExpected(source, expected);
        }

        /// <summary>
        /// Verifies that <code>EnumMemberDeclarationSyntax</code> nodes are not mutated.
        /// Mutating would introduce code like <code>StrykerXGJbRBlHxqRdD9O.MutantControl.IsActive(0) ? One + 1 : One - 1</code>
        /// Since enum members need to be constants, this mutated code would not compile and print a warning.
        /// </summary>
        [Fact]
        public void ShouldNotMutateEnum()
        {
            string source = @"private enum Numbers { One = 1, Two = One + 1 }";
            string expected = @"private enum Numbers { One = 1, Two = One + 1 }";

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
if(StrykerNamespace.MutantControl.IsActive(0)){for (var i = 0; i < 10; i--)
{ }
}else{for (var i = 0; (StrykerNamespace.MutantControl.IsActive(2)?i <= 10:(StrykerNamespace.MutantControl.IsActive(1)?i > 10:i < 10)); i++)
{ }
}}";
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateForWithoutConditionWithIfStatementAndConditionalStatement()
        {
            var source = @"public void SomeMethod() {
for (var i = 0; ; i++)
{ }
}";
            var expected = @"public void SomeMethod() {
if(StrykerNamespace.MutantControl.IsActive(0)){for (var i = 0; ; i--)
{ }
}else{for (var i = 0; ; i++)
{ }
}}";
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateInitializersForWithoutConditionWithIfStatementAndConditionalStatement()
        {
            var source = @"public void SomeMethod() {
for (var i = Method(true); ; i++)
{ }
}";
            var expected = @"public void SomeMethod() {
if(StrykerNamespace.MutantControl.IsActive(0)){for (var i = Method(true); ; i--)
{ }
}else{for (var i = Method((StrykerNamespace.MutantControl.IsActive(1)?false:true)); ; i++)
{ }
}}";
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateComplexExpressionBodiedMethod()
        {
            string source = @"public int SomeMethod()  => (true && SomeOtherMethod(out var x)) ? x : 5;";
            string expected = @"public int SomeMethod()  {if(StrykerNamespace.MutantControl.IsActive(0)){return!((true && SomeOtherMethod(out var x)) )? x : 5;}else{if(StrykerNamespace.MutantControl.IsActive(1)){return(true || SomeOtherMethod(out var x)) ? x : 5;}else{return ((StrykerNamespace.MutantControl.IsActive(2)?false:true )&& SomeOtherMethod(out var x)) ? x : 5;}}}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateExpressionBodiedStaticConstructor()
        {
            string source = @"static Test()  => (true && SomeOtherMethod(out var x)) ? x : 5;";
            string expected = @"static Test() {using(new StrykerNamespace.MutantContext()){if(StrykerNamespace.MutantControl.IsActive(0)){!((true && SomeOtherMethod(out var x)) )? x : 5;}else{if(StrykerNamespace.MutantControl.IsActive(1)){(true || SomeOtherMethod(out var x)) ? x : 5;}else{((StrykerNamespace.MutantControl.IsActive(2)?false:true )&& SomeOtherMethod(out var x)) ? x : 5;}}}}}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateInlineArrowFunction()
        {
            string source = @"public void SomeMethod() {
int Add(int x, int y) => x + y;
}";
            string expected = @"public void SomeMethod() {
int Add(int x, int y) => (StrykerNamespace.MutantControl.IsActive(0)?x - y:x + y);
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
Action act = () => Console.WriteLine((StrykerNamespace.MutantControl.IsActive(0)?1 - 1:1 + 1), (StrykerNamespace.MutantControl.IsActive(1)?1 - 1:1 + 1));
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
    return (StrykerNamespace.MutantControl.IsActive(0)?list.First():list.Last());
}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateComplexLinqMethods()
        {
            string source = @"private void Linq()
        {
            var array = new []{1, 2};

            var alt1 = array.Count(x => x % 2 == 0);
            var alt2 = array.Min();
        }";
            string expected = @"private void Linq()
        {
            var array = new []{1, 2};

            var alt1 = (StrykerNamespace.MutantControl.IsActive(0)?array.Sum(x => x % 2 == 0):array.Count(x => (StrykerNamespace.MutantControl.IsActive(1)?x % 2 != 0:(StrykerNamespace.MutantControl.IsActive(2)?x * 2 :x % 2 )== 0)));
            var alt2 = (StrykerNamespace.MutantControl.IsActive(3)?array.Max():array.Min());
        }";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateReturnStatements()
        {
            string source = @"private bool Move()
        {
            return true;
        }";
            string expected = @"private bool Move()
        {
            return (StrykerNamespace.MutantControl.IsActive(0)?false:true);
        }";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateWhileLoop()
        {
            string source = @"private void DummyLoop()
        {
            while (this.Move())
            {
                int x = 2 + 3;
            }
        }";
            string expected = @"private void DummyLoop()
        {
            while ((StrykerNamespace.MutantControl.IsActive(0)?!(this.Move()):this.Move()))
            {
                int x = (StrykerNamespace.MutantControl.IsActive(1)?2 - 3:2 + 3);
            }
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
    if (StrykerNamespace.MutantControl.IsActive(0))
    {
        x /= x + 2;
    }
    else
    {
        x *= (StrykerNamespace.MutantControl.IsActive(1) ? x - 2 : x + 2);
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
    if (StrykerNamespace.MutantControl.IsActive(0))
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
        public void ShouldMutateChainedMutations()
        {
            string source = @"public void Simple()
        {
            object value = null;
            var flag1 = false;
            var flag2 = false;
            if (value != null && !flag1 && !flag2)
            {
            }
        }";
            string expected = @"public void Simple()
        {
            object value = null;
            var flag1 = (StrykerNamespace.MutantControl.IsActive(0)?true:false);
            var flag2 = (StrykerNamespace.MutantControl.IsActive(1)?true:false);
            if ((StrykerNamespace.MutantControl.IsActive(3)?!(value != null && !flag1 && !flag2):(StrykerNamespace.MutantControl.IsActive(2)?value != null && !flag1 || !flag2:(StrykerNamespace.MutantControl.IsActive(4)?value != null || !flag1 :(StrykerNamespace.MutantControl.IsActive(5)?value == null :value != null )&& (StrykerNamespace.MutantControl.IsActive(6)?flag1 :!flag1 ))&& (StrykerNamespace.MutantControl.IsActive(7)?flag2:!flag2))))
            {
            }
        }";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateStatics()
        {
            string source = @"private static bool willMutateToFalse = true;

        private static bool NoWorries => false;
        private static bool NoWorriesGetter
        {
            get { return false; }
        }

static Mutator_Flag_MutatedStatics()
        {
            int x = 0;
            var y = x++;
        }";
            string expected = @"private static bool willMutateToFalse = (StrykerNamespace.MutantControl.IsActive(0)?false:true);

        private static bool NoWorries => (StrykerNamespace.MutantControl.IsActive(1)?true:false);
        private static bool NoWorriesGetter
        {
            get {return (StrykerNamespace.MutantControl.IsActive(2)?true:false); }
        }

static Mutator_Flag_MutatedStatics()
{using(new StrykerNamespace.MutantContext())        {
            int x = 0;
            var y = (StrykerNamespace.MutantControl.IsActive(3)?x--:x++);
        }}";

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
        public void ShouldInitializeOutVars()
        {
            string source = @"public void SomeMethod(out int x, out string text) { x = 1; text = ""hello"";}";
            string expected = @"public void SomeMethod(out int x, out string text) { {x= default(int);text= default(string);}x = 1; text = (StrykerNamespace.MutantControl.IsActive(0)?"""":""hello"");)}";

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
        request.Headers.Add((StrykerNamespace.MutantControl.IsActive(0)?"""":""Accept""), (StrykerNamespace.MutantControl.IsActive(1)?"""":""application / json; version = 1""));
        request.Headers.TryAddWithoutValidation((StrykerNamespace.MutantControl.IsActive(2)?"""":""Date""), date);
}, ensureSuccessStatusCode: (StrykerNamespace.MutantControl.IsActive(3)?true:false));
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
            _target.Mutate(CSharpSyntaxTree.ParseText(source).GetRoot());

            var mutants = _target.GetLatestMutantBatch().ToList();
            mutants.ShouldHaveSingleItem().Mutation.OriginalNode.GetLocation().GetLineSpan().StartLinePosition.Line.ShouldBe(2);
        }

        [Fact]
        public void MutationsShouldHaveLinespan2()
        {
            string source = @"using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp
{
    public static class ExampleExtension
    {
        public static bool InvokeIfNotNull(this Action a)
        {
            if (a == null) { return false; } else { a.Invoke(); return true; }
        }
    }
}";
            _target.Mutate(CSharpSyntaxTree.ParseText(source).GetRoot());

            var mutants = _target.GetLatestMutantBatch().ToList();
            mutants.Count.ShouldBe(3);
            foreach (var mutant in mutants)
            {
                mutant.Mutation.OriginalNode.GetLocation().GetLineSpan().StartLinePosition.Line.ShouldBe(10);
            }
        }

        [Fact]
        // test for issue #1386
        public void ShouldNotLeakMutationsAcrossDefinitions()
        {
            var source = @"class Test {
int GetId(string input) => int.TryParse(input, out var result) ? result : 0;
string Value => Generator(out var x) ? """" :""test"";
}";

            var expected = @"class Test {
int GetId(string input) {if(StrykerNamespace.MutantControl.IsActive(0)){return!(int.TryParse(input, out var result) )? result : 0;}else{return int.TryParse(input, out var result) ? result : 0;}}
string Value {get{if(StrykerNamespace.MutantControl.IsActive(1)){return!(Generator(out var x) )? """" :""test"";}else{return Generator(out var x) ? (StrykerNamespace.MutantControl.IsActive(2)?""Stryker was here!"":"""" ):(StrykerNamespace.MutantControl.IsActive(3)?"""":""test"");}}}}}}";
            ShouldMutateSourceToExpected(source, expected);
        }
    }
}
