using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
    public class CsharpMutantOrchestratorTests : MutantOrchestratorTestsBase
    {
        [Fact]
        public void ShouldNotMutateEmptyInterfaces()
        {
            var source = @"using System;
using System.Collections.Generic;
using System.Text;
namespace StrykerNet.UnitTest.Mutants.TestResources
{
    interface TestClass
    {
        int A { get; set; }
        int B { get; set; }
        void MethodA();
    }
}";

            var expected = @"using System;
using System.Collections.Generic;
using System.Text;
namespace StrykerNet.UnitTest.Mutants.TestResources
{
    interface TestClass
    {
        int A { get; set; }
        int B { get; set; }
        void MethodA();
    }
}";
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateDefaultImplementationInterfaces()
        {
            var source = @"using System;
using System.Collections.Generic;
using System.Text;
namespace StrykerNet.UnitTest.Mutants.TestResources
{
    interface TestClass
    {
        int A { get; set; }
        int B { get; set; }
        void MethodA() {
            var three = 1 + 2;
        }
    }
}";

            var expected = @"using System;
using System.Collections.Generic;
using System.Text;
namespace StrykerNet.UnitTest.Mutants.TestResources
{
    interface TestClass
    {
        int A { get; set; }
        int B { get; set; }
        void MethodA() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
            var three = (StrykerNamespace.MutantControl.IsActive(1)?1 - 2:1 + 2);
        }}
    }
}";
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateBlockStatements()
        {
            var options = new StrykerOptions
            {
                MutationLevel = MutationLevel.Complete,
                OptimizationMode = OptimizationModes.CoverageBasedTest,
            };
            _target = new CsharpMutantOrchestrator(new MutantPlacer(_injector), options: options);

            string source = @"private void Move()
			{
				;
			}";
            string expected = @"private void Move()
    {if(StrykerNamespace.MutantControl.IsActive(0)){}else		    {
    			    ;
    		    }}";

            ShouldMutateSourceInClassToExpected(source, expected);

            source = @"private int Move()
			{
				;
			}";
            expected = @"private int Move()
    {if(StrykerNamespace.MutantControl.IsActive(1)){}else		    {
    			    ;
    		    }}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldAddReturnDefaultToOperator()
        {
            string source = @"public static string operator+ (TestClass value, TestClass other)
{ while(true) return value;
}";
            string expected = @"public static string operator+ (TestClass value, TestClass other)
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{ while((StrykerNamespace.MutantControl.IsActive(2)?!(true):(StrykerNamespace.MutantControl.IsActive(1)?false:true))) return value;
}return default(string);}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldAddReturnDefaultToArrowExpressionOperator()
        {
            string source =
                @"public static int operator+ (TestClass value, TestClass other) => Sub(out var x, """")?1:2;";
            string expected =
                @"public static int operator+ (TestClass value, TestClass other) {if(StrykerNamespace.MutantControl.IsActive(0)){return!(Sub(out var x, """"))?1:2;}else{return Sub(out var x, (StrykerNamespace.MutantControl.IsActive(1)?""Stryker was here!"":""""))?1:2;}}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotAddReturnDefaultToDestructor()
        {
            var source = @"~TestClass(){;}";
            var expected = @"~TestClass(){if(StrykerNamespace.MutantControl.IsActive(0)){}else{;}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldProperlyMutatePrefixUnitaryExpressionStatement()
        {
            const string Source = @"void Method(int x) {++x;}";
            const string Expected = @"void Method(int x) {if(StrykerNamespace.MutantControl.IsActive(0)){}else{if(StrykerNamespace.MutantControl.IsActive(1)){;}else{if(StrykerNamespace.MutantControl.IsActive(2)){--x;}else{++x;}}}}}";

            ShouldMutateSourceInClassToExpected(Source, Expected);
        }

        [Fact]
        public void ShouldMutateExpressionBodiedLocalFunction()
        {
            string source = @"void TestMethod(){
int SomeMethod()  => (true && SomeOtherMethod(out var x)) ? x : 5;
}";
            string expected = @"void TestMethod(){if(StrykerNamespace.MutantControl.IsActive(0)){}else{
int SomeMethod()  {if(StrykerNamespace.MutantControl.IsActive(1)){return!((true && SomeOtherMethod(out var x)) )? x : 5;}else{if(StrykerNamespace.MutantControl.IsActive(2)){return(true || SomeOtherMethod(out var x)) ? x : 5;}else{return ((StrykerNamespace.MutantControl.IsActive(3)?false:true )&& SomeOtherMethod(out var x)) ? x : 5;}}};}}
}";

            ShouldMutateSourceInClassToExpected(source, expected);
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
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{if(StrykerNamespace.MutantControl.IsActive(19)){
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

	if (i is not int x)
	{
		return x + 1;
	}
}
else{if(StrykerNamespace.MutantControl.IsActive(16)){
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
	if ((StrykerNamespace.MutantControl.IsActive(1)?i + 8 != 8:(StrykerNamespace.MutantControl.IsActive(2)?i - 8 :i + 8 )== 8))
{if(StrykerNamespace.MutantControl.IsActive(3)){}else	{
		i = (StrykerNamespace.MutantControl.IsActive(4)?i - 1:i + 1);
		if ((StrykerNamespace.MutantControl.IsActive(5)?i + 8 != 9:(StrykerNamespace.MutantControl.IsActive(6)?i - 8 :i + 8 )== 9))
{if(StrykerNamespace.MutantControl.IsActive(7)){}else		{
			i = (StrykerNamespace.MutantControl.IsActive(8)?i - 1:i + 1);
		}};
	}
}	else
{if(StrykerNamespace.MutantControl.IsActive(9)){}else	{
		i = (StrykerNamespace.MutantControl.IsActive(10)?i - 3:i + 3);
		if ((StrykerNamespace.MutantControl.IsActive(11)?i != i + i - 8:i == (StrykerNamespace.MutantControl.IsActive(12)?i + i + 8:(StrykerNamespace.MutantControl.IsActive(13)?i - i :i + i )- 8)))
{if(StrykerNamespace.MutantControl.IsActive(14)){}else		{
			i = (StrykerNamespace.MutantControl.IsActive(15)?i - 1:i + 1);
		}};
	}
}
	if (!Out(out var test))
{if(StrykerNamespace.MutantControl.IsActive(17)){}else	{
		return (StrykerNamespace.MutantControl.IsActive(18)?i - 1:i + 1);
	}
}
	if (i is int x)
{if(StrykerNamespace.MutantControl.IsActive(20)){}else	{
		return (StrykerNamespace.MutantControl.IsActive(21)?x - 1:x + 1);
	}
}}
}}}
private bool Out(out string test)
{{test= default(string);}if(StrykerNamespace.MutantControl.IsActive(22)){}else{
	return (StrykerNamespace.MutantControl.IsActive(23)?false:true);
}return default(bool);}";

            ShouldMutateSourceInClassToExpected(source, expected);
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
                if (StrykerNamespace.MutantControl.IsActive(3))
                { }
                else
                {
                    _collection = value;
                }
            }
        }
    }";

            ShouldMutateSourceInClassToExpected(source, expected);
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
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{if(StrykerNamespace.MutantControl.IsActive(1)){
	int i = 0;
	var result = !(Out(out var test) )? test : """";
}
else{
	int i = 0;
	var result = Out(out var test) ? test : (StrykerNamespace.MutantControl.IsActive(2)?""Stryker was here!"":"""");
}
    }
}
private bool Out(out string test)
{
    { test = default(string); }
    if (StrykerNamespace.MutantControl.IsActive(3))
    { }
    else
    {
        return (StrykerNamespace.MutantControl.IsActive(4) ? false : true);
    }
    return default(bool);
}
";

            ShouldMutateSourceInClassToExpected(source, expected);
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
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	int i = 0;
	var result = (StrykerNamespace.MutantControl.IsActive(1)?!(Out(i, (x) => { int.TryParse(""3"", out int y); return x == y;} ) ):Out(i, (x) => {if(StrykerNamespace.MutantControl.IsActive(2)){}else{ int.TryParse((StrykerNamespace.MutantControl.IsActive(3)?"""":""3""), out int y); return (StrykerNamespace.MutantControl.IsActive(4)?x != y:x == y);} return default;}) )? i.ToString() : (StrykerNamespace.MutantControl.IsActive(5)?""Stryker was here!"":"""");
}
    }
    private bool Out(int test, Func<int, bool> lambda)
    {
        if (StrykerNamespace.MutantControl.IsActive(6))
        { }
        else
        {
            return (StrykerNamespace.MutantControl.IsActive(7) ? false : true);
        }
        return default(bool);
    }";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateWhenDeclarationInInnerScopeInExpressionForm()
        {
            var source = @"void TestMethod()
{
	int i = 0;
	var result = Out(i, (x) => int.TryParse(""3"", out int y) ? true : false);
}
";
            var expected = @"void TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	int i = 0;
	var result = Out(i, (x) => {if(StrykerNamespace.MutantControl.IsActive(1)){return!(int.TryParse(""3"", out int y) )? true : false;}else{return int.TryParse((StrykerNamespace.MutantControl.IsActive(2)?"""":""3""), out int y) ? (StrykerNamespace.MutantControl.IsActive(3)?false:true ): (StrykerNamespace.MutantControl.IsActive(4)?true:false);}});
}}
";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateLambdaAndAddDefaultReturn()
        {
            var source = @"void TestMethod()
{
	int i = 0;
	var result = Out(i, (x) => {if (x>2) return false; i++;});
}
";
            var expected = @"void TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	int i = 0;
	var result = Out(i, (x) => {if(StrykerNamespace.MutantControl.IsActive(1)){}else{if ((StrykerNamespace.MutantControl.IsActive(4)?!(x>2):(StrykerNamespace.MutantControl.IsActive(3)?x>=2:(StrykerNamespace.MutantControl.IsActive(2)?x<2:x>2)))) return (StrykerNamespace.MutantControl.IsActive(5)?true:false); if(StrykerNamespace.MutantControl.IsActive(6)){;}else{if(StrykerNamespace.MutantControl.IsActive(7)){i--;}else{i++;}}}return default;});}}
";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateInsideStringDeclarationInsideLocalFunction()
        {
            var source = @"void TestMethod()
		{
			string SomeLocalFunction()
			{
				var test3 = 2 + 5;
				return $""test{1 + test3}"";
			};
		}";
            var expected = @"void TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else		{
			string SomeLocalFunction()
{if(StrykerNamespace.MutantControl.IsActive(1)){}else			{
				var test3 = (StrykerNamespace.MutantControl.IsActive(2)?2 - 5:2 + 5);
				return (StrykerNamespace.MutantControl.IsActive(3)?$"""":$""test{(StrykerNamespace.MutantControl.IsActive(4) ? 1 - test3 : 1 + test3)}"");

            }return default(string);};
}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateConditionalExpressionProperly()
        {
            var source = @"void TestMethod()
		{
			string SomeLocalFunction()
			{
				return string.Empty?.All(x => !string.IsEmpty(x));
			};
		}";
            var expected = @"void TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else		{
			string SomeLocalFunction()
{if(StrykerNamespace.MutantControl.IsActive(1)){}else			{
				return (StrykerNamespace.MutantControl.IsActive(3)?string.Empty?.Any(x => !string.IsEmpty(x)):(StrykerNamespace.MutantControl.IsActive(2)?""Stryker was here!"":string.Empty)?.All(x => (StrykerNamespace.MutantControl.IsActive(4)?string.IsEmpty(x):!string.IsEmpty(x))));            }return default(string);};
}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }
               

        [Fact]
        public void ShouldMutateConditionalExpressionOnArrayDeclaration()
        {
            var source =
                @"public static IEnumerable<int> Foo() => new int[] { }.ToArray()!.Any(x => x==1)?.OrderBy(e => e).ToList();";
            var expected =
                @"public static IEnumerable<int> Foo() => (StrykerNamespace.MutantControl.IsActive(2)?new int[] { }.ToArray()!.Any(x => x==1)?.OrderByDescending(e => e).ToList():(StrykerNamespace.MutantControl.IsActive(0)?new int[] { }.ToArray()!.All(x => x==1):new int[] { }.ToArray()!.Any(x => (StrykerNamespace.MutantControl.IsActive(1)?x!=1:x==1)))?.OrderBy(e => e).ToList());";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateSuppressNullableWarningExpressionOnArrayDeclaration()
        {
            var source =
                @"public static void Foo(){
var employeePerson = group.First().Entitlement!.Employee.Person;}";
            var expected =
                @"public static void Foo(){if(StrykerNamespace.MutantControl.IsActive(0)){}else{
var employeePerson = (StrykerNamespace.MutantControl.IsActive(1)?group.FirstOrDefault():group.First()).Entitlement!.Employee.Person;}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }


        [Fact]
        public void ShouldMutateChainedInvocation()
        {
            var source =
                @"public string ExampleBugMethod()
{
    string someString = """";
    return someString.Replace(""ab"", ""cd"")
        .Replace(""12"", ""34"")
        .PadLeft(12)
        .Replace(""12"", ""34"");
}";
            var expected =
                @"public string ExampleBugMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
    string someString = (StrykerNamespace.MutantControl.IsActive(1)?""Stryker was here!"":"""");
    return someString.Replace((StrykerNamespace.MutantControl.IsActive(2)?"""":""ab""), (StrykerNamespace.MutantControl.IsActive(3)?"""":""cd""))
        .Replace((StrykerNamespace.MutantControl.IsActive(4)?"""":""12""), (StrykerNamespace.MutantControl.IsActive(5)?"""":""34""))
        .PadLeft(12)
        .Replace((StrykerNamespace.MutantControl.IsActive(6)?"""":""12""), (StrykerNamespace.MutantControl.IsActive(7)?"""":""34""));
}return default(string);}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateArrayInitializer()
        {
            string source = @"public int[] Foo(){
int[] test = { 1 };
}";
            string expected =
                @"public int[] Foo(){if(StrykerNamespace.MutantControl.IsActive(0)){}else{if(StrykerNamespace.MutantControl.IsActive(1)){
int[] test = {};
}else{
int[] test = { 1 };
}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateArrayDeclarationAsReturnValue()
        {
            var source = @"public int[] Foo() => new int[] { 1 };";
            var expected =
                @"public int[] Foo() => (StrykerNamespace.MutantControl.IsActive(0)?new int[] {}:new int[] { 1 });";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateListCreation()
        {
            var source = @"public int[] Foo() => new List<int> { 1 };";
            var expected =
                @"public int[] Foo() => (StrykerNamespace.MutantControl.IsActive(0)?new List<int> {}:new List<int> { 1 });";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateImplicitArrayCreationProperties()
        {
            string source = @"public int[] Foo() => new [] { 1 };";
            string expected = @"public int[] Foo() => new [] { 1 };";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateImplicitArrayCreation()
        {
            string source = "public static readonly int[] Foo =  { 1 };";
            string expected = "public static readonly int[] Foo =  { 1 };";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateProperties()
        {
            string source = @"private string text => ""Some"" + ""Text"";";
            string expected =
                @"private string text => (StrykerNamespace.MutantControl.IsActive(0) ? """" : ""Some"") + (StrykerNamespace.MutantControl.IsActive(1) ? """" : ""Text"");";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateTupleDeclaration()
        {
            string source = @"public void TestMethod() {
var (one, two) = (1 + 1, """");
}";
            string expected = @"public void TestMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
var (one, two) = ((StrykerNamespace.MutantControl.IsActive(1)?1 - 1:1 + 1), (StrykerNamespace.MutantControl.IsActive(2)?""Stryker was here!"":""""));
}
    }";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateConst()
        {
            string source = @"private const int x = 1 + 2;";
            string expected = @"private const int x = 1 + 2;";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateStackalloc()
        {
            string source = @"Span<ushort> kindaUnrelated = stackalloc ushort[] { 0 };";
            string expected =
                @"Span<ushort> kindaUnrelated = (StrykerNamespace.MutantControl.IsActive(0)?stackalloc ushort[] {}:stackalloc ushort[] { 0 });";

            ShouldMutateSourceInClassToExpected(source, expected);
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

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateAttributes()
        {
            string source = @"[Obsolete(""thismustnotbemutated"")]
public void SomeMethod() {}";
            string expected = @"[Obsolete(""thismustnotbemutated"")]
public void SomeMethod() {}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateForWithIfStatementAndConditionalStatement()
        {
            string source = @"public void SomeMethod() {
for (var i = 0; i < 10; i++)
{ }
}";
            string expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
if(StrykerNamespace.MutantControl.IsActive(3)){for (var i = 0; i < 10; i--)
{ }
}else{for (var i = 0; (StrykerNamespace.MutantControl.IsActive(2)?i <= 10:(StrykerNamespace.MutantControl.IsActive(1)?i > 10:i < 10)); i++)
{ }
}}}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateForWithoutConditionWithIfStatementAndConditionalStatement()
        {
            var source = @"public void SomeMethod() {
for (var i = 0; ; i++)
{ }
}";
            var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
if(StrykerNamespace.MutantControl.IsActive(1)){for (var i = 0; ; i--)
{ }
}else{for (var i = 0; ; i++)
{ }
}}}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateInitializersForWithoutConditionWithIfStatementAndConditionalStatement()
        {
            var source = @"public void SomeMethod() {
for (var i = Method(true); ; i++)
{ }
}";
            var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
if(StrykerNamespace.MutantControl.IsActive(2)){for (var i = Method(true); ; i--)
{ }
}else{for (var i = Method((StrykerNamespace.MutantControl.IsActive(1)?false:true)); ; i++)
{ }
}}}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateComplexExpressionBodiedMethod()
        {
            string source = @"public int SomeMethod()  => (true && SomeOtherMethod(out var x)) ? x : 5;";
            string expected =
                @"public int SomeMethod()  {if(StrykerNamespace.MutantControl.IsActive(0)){return!((true && SomeOtherMethod(out var x)) )? x : 5;}else{if(StrykerNamespace.MutantControl.IsActive(1)){return(true || SomeOtherMethod(out var x)) ? x : 5;}else{return ((StrykerNamespace.MutantControl.IsActive(2)?false:true )&& SomeOtherMethod(out var x)) ? x : 5;}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateExpressionBodiedStaticConstructor()
        {
            string source = @"static Test() => (true && SomeOtherMethod(out var x)) ? x : 5;";
            string expected =
                @"static Test() {using(new StrykerNamespace.MutantContext()){if(StrykerNamespace.MutantControl.IsActive(0)){!((true && SomeOtherMethod(out var x)) )? x : 5;}else{if(StrykerNamespace.MutantControl.IsActive(1)){(true || SomeOtherMethod(out var x)) ? x : 5;}else{((StrykerNamespace.MutantControl.IsActive(2)?false:true )&& SomeOtherMethod(out var x)) ? x : 5;}}}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateInlineArrowFunction()
        {
            string source = @"public void SomeMethod() {
int Add(int x, int y) => x + y;
}";
            string expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
int Add(int x, int y) => (StrykerNamespace.MutantControl.IsActive(1)?x - y:x + y);
}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateLambdaSecondParameter()
        {
            string source = @"public void SomeMethod() {
Action act = () => Console.WriteLine(1 + 1, 1 + 1);
}";
            string expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
Action act = () => Console.WriteLine((StrykerNamespace.MutantControl.IsActive(1)?1 - 1:1 + 1), (StrykerNamespace.MutantControl.IsActive(2)?1 - 1:1 + 1));
}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateLinqMethods()
        {
            string source = @"public int TestLinq(int count){
	var list = Enumerable.Range(1, count);
	return list.Last();
}";
            string expected = @"public int TestLinq(int count){if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var list = Enumerable.Range(1, count);
	return (StrykerNamespace.MutantControl.IsActive(1)?list.First():list.Last());
}return default(int);}";

            ShouldMutateSourceInClassToExpected(source, expected);
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
{if(StrykerNamespace.MutantControl.IsActive(0)){}else		{
			var array = new []{1, 2};

			var alt1 = (StrykerNamespace.MutantControl.IsActive(1)?array.Sum(x => x % 2 == 0):array.Count(x => (StrykerNamespace.MutantControl.IsActive(2)?x % 2 != 0:(StrykerNamespace.MutantControl.IsActive(3)?x * 2 :x % 2 )== 0)));
			var alt2 = (StrykerNamespace.MutantControl.IsActive(4)?array.Max():array.Min());
		}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateReturnStatements()
        {
            string source = @"private bool Move()
		{
			return true;
		}";
            string expected = @"private bool Move()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else		{
			return (StrykerNamespace.MutantControl.IsActive(1)?false:true);
		}return default(bool);}";

            ShouldMutateSourceInClassToExpected(source, expected);
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
{if(StrykerNamespace.MutantControl.IsActive(0)){}else		{
			while ((StrykerNamespace.MutantControl.IsActive(1)?!(this.Move()):this.Move()))
{if(StrykerNamespace.MutantControl.IsActive(2)){}else			{
				int x = (StrykerNamespace.MutantControl.IsActive(3)?2 - 3:2 + 3);
			}
}		}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateAssignmentStatementsWithIfStatement()
        {
            string source = @"public void SomeMethod() {
	var x = 0;
	x *= x + 2;
}";
            string expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
if(StrykerNamespace.MutantControl.IsActive(1)){	x /=x + 2;
}else{	x *= (StrykerNamespace.MutantControl.IsActive(2)?x - 2:x + 2);
}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateRecursiveCoalescingAssignmentStatements()
        {
            string source = @"public void SomeMethod() {
    List<int> a = null;
    List<int> b = null;
    a ??= b ??= new List<int>();
}";
            string expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
    List<int> a = null;
    List<int> b = null;
if(StrykerNamespace.MutantControl.IsActive(1)){    a = b ??= new List<int>();
}else{if(StrykerNamespace.MutantControl.IsActive(2)){    a ??= b = new List<int>();
}else{    a ??= b ??= new List<int>();
}}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateRecursiveNullCoalescingStatements()
        {
            string source = @"public void SomeMethod() {
    List<int> a = null;
    List<int> b = null;
    List<int> c = null;
    var d = a ?? b ?? c;
}";
            string expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
    List<int> a = null;
    List<int> b = null;
    List<int> c = null;
    var d = (StrykerNamespace.MutantControl.IsActive(3)?a :(StrykerNamespace.MutantControl.IsActive(2)?b ?? c:(StrykerNamespace.MutantControl.IsActive(1)?b ?? c ?? a :a ?? (StrykerNamespace.MutantControl.IsActive(6)?b :(StrykerNamespace.MutantControl.IsActive(5)?c:(StrykerNamespace.MutantControl.IsActive(4)?c ?? b :b ?? c))))));
}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateIncrementStatementWithIfStatement()
        {
            string source = @"public void SomeMethod() {
	var x = 0;
	x++;
}";
            string expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
if(StrykerNamespace.MutantControl.IsActive(1)){;}else{if(StrykerNamespace.MutantControl.IsActive(2)){	x--;
}else{	x++;
}}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateIfDisabledByComment()
        {
            string source = @"public void SomeMethod() {
	var x = 0;
// Stryker disable all
	x++;
	x/=2;
}";
            string expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
// Stryker disable all
	x++;
	x/=2;
}}";

            ShouldMutateSourceInClassToExpected(source, expected);
            source = @"public void SomeMethod() {
	var x = 0;
	{
	// Stryker disable all
	  x++;
	}
	x/=2;
}";
            expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(4)){}else{
	var x = 0;
{if(StrykerNamespace.MutantControl.IsActive(5)){}else	{
	// Stryker disable all
	  x++;
	}
}if(StrykerNamespace.MutantControl.IsActive(8)){	x*=2;
}else{	x/=2;
}}}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateIfDisabledByCommentOnStaticFields()
        {
            var source = @"
    // Stryker disable all
    static string x = ""test"";";

            var expected = @"
    // Stryker disable all
    static string x = StrykerNamespace.MutantContext.TrackValue(()=>""test"");";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateIfDisabledByCommentAtMethodLevel()
        {
            string source = @"
// Stryker disable all : testing
public void SomeMethod()
 {
	var x = 0;
	x++;
	x/=2;
}";
            string expected = @"// Stryker disable all : testing
public void SomeMethod()
 {
	var x = 0;
	x++;
	x/=2;
}";

            ShouldMutateSourceInClassToExpected(source, expected);
            source = @"public void SomeMethod()
{
	var x = 0;
	{
	// Stryker disable once all
	  x++;
	}
	x/=2;
}";
            expected = @"public void SomeMethod()
{if(StrykerNamespace.MutantControl.IsActive(4)){}else{
	var x = 0;
{if(StrykerNamespace.MutantControl.IsActive(5)){}else	{
	// Stryker disable once all
	  x++;
	}
}if(StrykerNamespace.MutantControl.IsActive(8)){	x*=2;
}else{	x/=2;
}}}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateOneLineIfDisabledByComment()
        {
            string source = @"public void SomeMethod() {
	var x = 0;
// Stryker disable once all
	x++;
	x/=2;
}";
            string expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
// Stryker disable once all
	x++;
if(StrykerNamespace.MutantControl.IsActive(3)){	x*=2;
}else{	x/=2;
}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateOneLineIfDisabledAndEnabledByComment()
        {
            string source = @"public void SomeMethod() {
	var x = 0;
// Stryker disable all
	x++;
// Stryker restore all
	x/=2;
}";
            string expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
// Stryker disable all
	x++;
if(StrykerNamespace.MutantControl.IsActive(3)){// Stryker restore all
	x*=2;
}else{// Stryker restore all
	x/=2;
}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateOneLineIfDisabledOnce()
        {
            string source = @"public void SomeMethod() {
	var x = 0;
// Stryker disable once all
	x++;
	x/=2;
}";
            string expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
// Stryker disable Update , Statement : comment
	x++;
if(StrykerNamespace.MutantControl.IsActive(3)){// Stryker restore all
	x*=2;
}else{// Stryker restore all
	x/=2;
}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateOneLineIfSpecificMutatorDisabled()
        {
            string source = @"public void SomeMethod() {
	var x = 0;
// Stryker disable Update , Statement : comment
	x++;
// Stryker restore all
	x/=2;
}";
            string expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
// Stryker disable Update , Statement : comment
	x++;
if(StrykerNamespace.MutantControl.IsActive(3)){// Stryker restore all
	x*=2;
}else{// Stryker restore all
	x/=2;
}}}";

            ShouldMutateSourceInClassToExpected(source, expected);

            _target.Mutants.Count.ShouldBe(4);
            _target.Mutants.ElementAt(0).ResultStatus.ShouldBe(MutantStatus.Pending);
            _target.Mutants.ElementAt(1).ResultStatus.ShouldBe(MutantStatus.Ignored);
            _target.Mutants.ElementAt(1).ResultStatusReason.ShouldBe("comment");
            _target.Mutants.ElementAt(2).ResultStatus.ShouldBe(MutantStatus.Ignored);
            _target.Mutants.ElementAt(2).ResultStatusReason.ShouldBe("comment");
            _target.Mutants.ElementAt(3).ResultStatus.ShouldBe(MutantStatus.Pending);
        }

        [Fact]
        public void ShouldNotMutateASubExpressionIfDisabledByComment()
        {
            string source = @"public void SomeMethod() {
	var x = 0;
	x/=
// Stryker disable once all
x +2;
}";
            string expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
if(StrykerNamespace.MutantControl.IsActive(1)){	x*=// Stryker disable once all
x +2;
}else{	x/=
// Stryker disable once all
x +2;
}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
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
{if(StrykerNamespace.MutantControl.IsActive(0)){}else		{
			object value = null;
			var flag1 = (StrykerNamespace.MutantControl.IsActive(1)?true:false);
			var flag2 = (StrykerNamespace.MutantControl.IsActive(2)?true:false);
			if ((StrykerNamespace.MutantControl.IsActive(4)?!(value != null && !flag1 && !flag2):(StrykerNamespace.MutantControl.IsActive(3)?value != null && !flag1 || !flag2:(StrykerNamespace.MutantControl.IsActive(5)?value != null || !flag1 :(StrykerNamespace.MutantControl.IsActive(6)?value == null :value != null )&& (StrykerNamespace.MutantControl.IsActive(7)?flag1 :!flag1 ))&& (StrykerNamespace.MutantControl.IsActive(8)?flag2:!flag2))))
			{
			}
		}}";

            ShouldMutateSourceInClassToExpected(source, expected);
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
            string expected = @"private static bool willMutateToFalse = StrykerNamespace.MutantContext.TrackValue(()=>(StrykerNamespace.MutantControl.IsActive(0)?false:true));

		private static bool NoWorries => (StrykerNamespace.MutantControl.IsActive(1)?true:false);
		private static bool NoWorriesGetter
		{
			get {if(StrykerNamespace.MutantControl.IsActive(2)){}else{ return (StrykerNamespace.MutantControl.IsActive(3)?true:false); }
return default(bool);}		}

static Mutator_Flag_MutatedStatics()
{using(new StrykerNamespace.MutantContext()){if(StrykerNamespace.MutantControl.IsActive(4)){}else		{
			int x = 0;
			var y = (StrykerNamespace.MutantControl.IsActive(5)?x--:x++);
		}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateDefaultValues()
        {
            string source = @"public void SomeMethod(bool option = true) {}";
            string expected = @"public void SomeMethod(bool option = true) {}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldInitializeOutVars()
        {
            string source = @"public void SomeMethod(out int x, out string text) { x = 1; text = ""hello"";}";
            string expected = @"public void SomeMethod(out int x, out string text) {{x= default(int);text= default(string);}if(StrykerNamespace.MutantControl.IsActive(0)){}else{ x = 1; text = (StrykerNamespace.MutantControl.IsActive(1)?"""":""hello"");
        }}";

            ShouldMutateSourceInClassToExpected(source, expected);
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
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
if(StrykerNamespace.MutantControl.IsActive(1)){;}else{	await SendRequest(url, HttpMethod.Get, (request) =>
{if(StrykerNamespace.MutantControl.IsActive(2)){}else	{
if(StrykerNamespace.MutantControl.IsActive(3)){;}else{		request.Headers.Add((StrykerNamespace.MutantControl.IsActive(4)?"""":""Accept""), (StrykerNamespace.MutantControl.IsActive(5)?"""":""application / json; version = 1""));
}if(StrykerNamespace.MutantControl.IsActive(6)){;}else{		request.Headers.TryAddWithoutValidation((StrykerNamespace.MutantControl.IsActive(7)?"""":""Date""), date);
}
}}, ensureSuccessStatusCode: (StrykerNamespace.MutantControl.IsActive(8) ? true : false));
}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void MutationsShouldHaveLinespan()
        {
            string source = @"void TestMethod()
{
	var test3 = 2 + 5;
}";
            _target.Mutate(CSharpSyntaxTree.ParseText(source), null);

            var mutants = _target.GetLatestMutantBatch().ToList();
            mutants.Count.ShouldBe(2);
            var blockLinespan = mutants.First().Mutation.OriginalNode.GetLocation().GetLineSpan();
            blockLinespan.StartLinePosition.Line.ShouldBe(1);
            blockLinespan.EndLinePosition.Line.ShouldBe(3);
            mutants.Last().Mutation.OriginalNode.GetLocation().GetLineSpan().StartLinePosition.Line.ShouldBe(2);
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
            _target.Mutate(CSharpSyntaxTree.ParseText(source), null);

            var mutants = _target.GetLatestMutantBatch().ToList();
            mutants.Count.ShouldBe(7);

            var blockLinespan = mutants.First().Mutation.OriginalNode.GetLocation().GetLineSpan();
            blockLinespan.StartLinePosition.Line.ShouldBe(9);
            blockLinespan.EndLinePosition.Line.ShouldBe(11);
            foreach (var mutant in mutants.Skip(1))
            {
                mutant.Mutation.OriginalNode.GetLocation().GetLineSpan().StartLinePosition.Line.ShouldBe(10);
            }
        }

        [Fact]
        public void ShouldAddReturnDefaultToMethods()
        {
            string source = @"bool TestMethod()
{
	while(true) return false;
}";
            string expected = @"bool TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	while((StrykerNamespace.MutantControl.IsActive(2)?!(true):(StrykerNamespace.MutantControl.IsActive(1)?false:true))) return (StrykerNamespace.MutantControl.IsActive(3)?true:false);
}return default(bool);}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldAddReturnDefaultToConversion()
        {
            string source = @"public static explicit operator string(TestClass value)
{ while(true) return value;
}";
            string expected = @"public static explicit operator string(TestClass value)
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{ while((StrykerNamespace.MutantControl.IsActive(2)?!(true):(StrykerNamespace.MutantControl.IsActive(1)?false:true))) return value;
}return default(string);}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotMutateConstDeclaration()
        {
            var source = @"void Test(){
const string text = ""a""+""b"";}";
            var expected = @"void Test(){if(StrykerNamespace.MutantControl.IsActive(0)){}else{
const string text = ""a""+""b"";}}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldAddReturnDefaultToAsyncMethods()
        {
            string source = @"public async Task<bool> TestMethod()
{   while(true) return false;
}";
            string expected = @"public async Task<bool> TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{  while((StrykerNamespace.MutantControl.IsActive(2)?!(true):(StrykerNamespace.MutantControl.IsActive(1)?false:true))) return (StrykerNamespace.MutantControl.IsActive(3)?true:false);
}return default(bool);}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldAddReturnDefaultToIndexerGetter()
        {
            string source = @"public string this[string key]
{
    get
    {
        return key;
    }
}";
            string expected = @"public string this[string key]
{
    get
{if(StrykerNamespace.MutantControl.IsActive(0)){}else        {
        return key;
    }
return default(string);}
}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotAddReturnDefaultToEnumerationMethods()
        {
            string source = @"public static IEnumerable<object> Extracting<T>(this IEnumerable<T> enumerable, string propertyName)
	  {
		foreach (var o in enumerable)
		{
			yield return value;
		}
	  }
public static IEnumerable<object> Extracting<T>(this IEnumerable<T> enumerable)
	  {
		yield break;
	  }";
            string expected = @"public static IEnumerable<object> Extracting<T>(this IEnumerable<T> enumerable, string propertyName)
{if(StrykerNamespace.MutantControl.IsActive(0)){}else	  {
		foreach (var o in enumerable)
{if(StrykerNamespace.MutantControl.IsActive(1)){}else		{
if(StrykerNamespace.MutantControl.IsActive(2)){;}else{			yield return value;
}		}
}	  }
}public static IEnumerable<object> Extracting<T>(this IEnumerable<T> enumerable)
{if(StrykerNamespace.MutantControl.IsActive(3)){}else	  {
if(StrykerNamespace.MutantControl.IsActive(4)){;}else{		yield break;
}	  }}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldAddReturnDefaultToAsyncWithFullNamespaceMethods()
        {
            string source = @"public async System.Threading.Tasks.Task<bool> TestMethod()
{  while(true) return false;
}";
            string expected = @"public async System.Threading.Tasks.Task<bool> TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{  while((StrykerNamespace.MutantControl.IsActive(2)?!(true):(StrykerNamespace.MutantControl.IsActive(1)?false:true))) return (StrykerNamespace.MutantControl.IsActive(3)?true:false);
}return default(bool);}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotAddReturnDefaultToAsyncTaskMethods()
        {
            string source = @"public async Task TestMethod()
{
	;
}";
            string expected = @"public async Task TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	;
}}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotAddReturnDefaultToMethodsWithReturnTypeVoid()
        {
            string source = @"void TestMethod()
{
	;
}";
            string expected = @"void TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	;
}}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Theory]
        [InlineData("{return 0;}")]
        [InlineData("{return 1; throw Exception();}")]
        [InlineData("{return 1;yield return 0;}")]
        [InlineData("{return 1;yield break;}")]
        [InlineData("{;}")]
        public void ShouldNotAddReturnOnSomeBlocks(string code)
        {
            string source = @$"
int TestMethod()
// Stryker disable all
{code}";
            ShouldMutateSourceInClassToExpected(source, source);
        }

        [Fact]
        public void ShouldSkipStringsInSwitchExpression()
        {
            string source = @"string TestMethod()
{
	return input switch
	{
		""test"" => ""test""
	};
}";
            string expected = @"string TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	return input switch
	{
		""test"" => (StrykerNamespace.MutantControl.IsActive(1)?"""":""test""
)	};
    }return default(string);}
";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateStaticConstructor()
        {
            var source = @"
static string Value { get; }
static TestClass() => Value = ""Hello, World!"";";

            var expected = @"static string Value { get; }
static TestClass() {using(new StrykerNamespace.MutantContext()){Value = (StrykerNamespace.MutantControl.IsActive(0)?"""":""Hello, World!"");
        }}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateStaticArrowConstructor()
        {
            var source = @"
static string Value { get; }
static TestClass() {Value = ""Hello, World!"";}";

            var expected = @"static string Value { get; }
static TestClass() {using(new StrykerNamespace.MutantContext()){if(StrykerNamespace.MutantControl.IsActive(0)){}else{Value = (StrykerNamespace.MutantControl.IsActive(1)?"""":""Hello, World!"");
        }
    }
}
";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateStaticPropertiesInArrowForm()
        {
            var source = @"class Test {
static string Value => """";
static TestClass(){}}";

            var expected = @"class Test {
static string Value => (StrykerNamespace.MutantControl.IsActive(0)?""Stryker was here!"":"""");
static TestClass(){using(new StrykerNamespace.MutantContext()){}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutatePropertiesInArrowFormEvenWithComplexConstruction()
        {
            var source = @"class Test {
string Value {get => Out(out var x)? ""empty"": """";}
static TestClass(){}}";

            var expected = @"class Test {
string Value {get {if(StrykerNamespace.MutantControl.IsActive(0)){return!(Out(out var x))? ""empty"": """";}else{return Out(out var x)? (StrykerNamespace.MutantControl.IsActive(1)?"""":""empty""): (StrykerNamespace.MutantControl.IsActive(2)?""Stryker was here!"":"""");}}}
static TestClass(){using(new StrykerNamespace.MutantContext()){}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateStaticProperties()
        {
            var source = @"
static string Value
{
	get { return ""TestDescription"";}
	set { value = ""TestDescription"";}
}
static TestClass(){}";

            var expected = @"static string Value
{get {if(StrykerNamespace.MutantControl.IsActive(0)){}else{ return (StrykerNamespace.MutantControl.IsActive(1)?"""":""TestDescription"");
        }
return default(string);
        }
        set {if(StrykerNamespace.MutantControl.IsActive(2)){}else{ value = (StrykerNamespace.MutantControl.IsActive(3)?"""":""TestDescription"");
        }
}}
static TestClass() { using (new StrykerNamespace.MutantContext()) { } }}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

       [Fact]
        public void ShouldMarkStaticMutationStaticInPropertiesInitializer()
        {
            var source = @"class Test {
static string Value {get;} = """";}";

            var expected = @"class Test {
static string Value {get;} = StrykerNamespace.MutantContext.TrackValue(()=>(StrykerNamespace.MutantControl.IsActive(0)?""Stryker was here!"":""""));}}}";
            ShouldMutateSourceInClassToExpected(source, expected);
            _target.Mutants.Count.ShouldBe(1);
            _target.Mutants.First().IsStaticValue.ShouldBeTrue();
        }

        [Fact]
        public void ShouldMutateStaticPropertiesInArrowFormEvenWithComplexConstruction()
        {
            var source = @"class Test {
static string Value => Out(out var x)? ""empty"": """";}";

            var expected = @"class Test {
static string Value {get {if(StrykerNamespace.MutantControl.IsActive(0)){return!(Out(out var x))? ""empty"": """";}else{return Out(out var x)? (StrykerNamespace.MutantControl.IsActive(1)?"""":""empty""): (StrykerNamespace.MutantControl.IsActive(2)?""Stryker was here!"":"""");}}}}";

            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutatePropertiesAsArrowExpression()
        {
            var source = @"class Test {
string Value => Generator(out var x) ? """" :""test"";
}";

            var expected = @"class Test {
string Value {get{if(StrykerNamespace.MutantControl.IsActive(0)){return!(Generator(out var x) )? """" :""test"";}else{return Generator(out var x) ? (StrykerNamespace.MutantControl.IsActive(1)?""Stryker was here!"":"""" ):(StrykerNamespace.MutantControl.IsActive(2)?"""":""test"");}}}
}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        public void ShouldControlLocalDeclarationMutationAtTheBlockLevel()
        {
            var source = @"public static string FormatPrettyByte(Int64 value)
		{
			string[] SizeSuffixes = { ""bytes"", ""KB"", ""MB"", ""GB"" };

			int mag = (int)Math.Log(value, 1024);
			decimal adjustedSize = (decimal)value / (1L << (mag * 10));

			return $""{adjustedSize:n1} {SizeSuffixes[mag]}"";
		}
";

            var expected = @"public static string FormatPrettyByte(Int64 value)
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{if(StrykerNamespace.MutantControl.IsActive(1))		{
			string[] SizeSuffixes = {};

			int mag = (int)Math.Log(value, 1024);
			decimal adjustedSize = (decimal)value / (1L << (mag * 10));

			return $""{ adjustedSize:n1} { SizeSuffixes[mag] }"";

        }
else		{
			string[] SizeSuffixes = { (StrykerNamespace.MutantControl.IsActive(2) ? """" : ""bytes""), (StrykerNamespace.MutantControl.IsActive(3) ? """" : ""KB""), (StrykerNamespace.MutantControl.IsActive(4) ? """" : ""MB""), (StrykerNamespace.MutantControl.IsActive(5) ? """" : ""GB"") };

        int mag = (int)(StrykerNamespace.MutantControl.IsActive(7) ? Math.Pow(value, 1024) : (StrykerNamespace.MutantControl.IsActive(6) ? Math.Exp(value, 1024) : Math.Log(value, 1024)));
        decimal adjustedSize = (StrykerNamespace.MutantControl.IsActive(8) ? (decimal)value * (1L << (mag * 10)) : (decimal)value / ((StrykerNamespace.MutantControl.IsActive(9) ? 1L >> (mag * 10) : 1L << ((StrykerNamespace.MutantControl.IsActive(10) ? mag / 10 : mag * 10)))));

			return (StrykerNamespace.MutantControl.IsActive(11)?$"""":$""{ adjustedSize:n1} {SizeSuffixes[mag]}"");
		}
}
return default(string);}";
            ShouldMutateSourceInClassToExpected(source, expected);
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
            ShouldMutateSourceInClassToExpected(source, expected);
        }

        [Fact]
        // test for issue #1386
        public void ShouldHandleLocalFunctions()
        {
            var source = @"
        public string DoStuff(char myChar, int myInt)
        {
            if (TryGet(myChar, out int i))
            {}
            string makeString(char c, int i) => new string(c, i);
            return getString;
        }
        private bool TryGet(char myChar, out int i)
        {
            i = 0;
            return true;
        }";

            var expected = @"public string DoStuff(char myChar, int myInt)
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{if(StrykerNamespace.MutantControl.IsActive(1))        {
            if (!(TryGet(myChar, out int i)))
            {}
            string makeString(char c, int i) => new string(c, i);
            return getString;
        }
else        {
            if (TryGet(myChar, out int i))
            {}
            string makeString(char c, int i) => new string(c, i);
            return getString;
        }
}return default(string);}        private bool TryGet(char myChar, out int i)
{{i= default(int);}if(StrykerNamespace.MutantControl.IsActive(2)){}else        {
            i = 0;
            return (StrykerNamespace.MutantControl.IsActive(3)?false:true);
        }return default(bool);}}}}";
            ShouldMutateSourceInClassToExpected(source, expected);
        }
    }
}
