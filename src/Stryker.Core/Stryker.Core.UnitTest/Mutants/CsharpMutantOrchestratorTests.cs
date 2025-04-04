using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Configuration;
using Stryker.Core.Mutants;

namespace Stryker.Core.UnitTest.Mutants;

[TestClass]
public class CsharpMutantOrchestratorTests : MutantOrchestratorTestsBase
{
    [TestMethod]
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

    [TestMethod]
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



    [TestMethod]
    public void ShouldMutatePatterns()
    {
        
        var source = @"public void Test()
    {
           Console.WriteLine(new[] { 1, 2, 3, 4 } is [>= 0, .., 2 or 4]);
    }
    ";
        var expected = @"public void Test()
{    if(StrykerNamespace.MutantControl.IsActive(0))    {}else{
           if(StrykerNamespace.MutantControl.IsActive(1)){;}else{Console.WriteLine((StrykerNamespace.MutantControl.IsActive(2)?new[] { 1, 2, 3, 4 } is not [>= 0, .., 2 or 4]:(StrykerNamespace.MutantControl.IsActive(5)?new[] { 1, 2, 3, 4 } is [>= 0, .., 2 and 4]:(StrykerNamespace.MutantControl.IsActive(4)?new[] { 1, 2, 3, 4 } is [< 0, .., 2 or 4]:(StrykerNamespace.MutantControl.IsActive(3)?new[] { 1, 2, 3, 4 } is [> 0, .., 2 or 4]:new[] { 1, 2, 3, 4 } is [>= 0, .., 2 or 4])))));}
    }
}    
    ";
       ShouldMutateSourceInClassToExpected(source, expected);
        
    }

    [TestMethod]
    public void ShouldMutateBlockStatements()
    {
        var options = new StrykerOptions
        {
            MutationLevel = MutationLevel.Complete,
            OptimizationMode = OptimizationModes.CoverageBasedTest,
        };
        Target = new CsharpMutantOrchestrator(new MutantPlacer(Injector), options: options);

        var source = @"private void Move()
			{
				;
			}";
        var expected = @"private void Move()
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

    [TestMethod]
    public void ShouldAddReturnDefaultToOperator()
    {
        var source = @"public static string operator+ (TestClass value, TestClass other)
{ while(true) return value;
}";
        var expected = @"public static string operator+ (TestClass value, TestClass other)
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{ while((StrykerNamespace.MutantControl.IsActive(2)?!(true):(StrykerNamespace.MutantControl.IsActive(1)?false:true))) return value;
}return default(string);}";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldAddReturnDefaultToArrowExpressionOperator()
    {
        var source =
            @"public static int operator+ (TestClass value, TestClass other) => Sub(out var x, """")?1:2;";
        var expected =
            @"public static int operator+ (TestClass value, TestClass other) {if(StrykerNamespace.MutantControl.IsActive(1)){return(false?1:2);}else{if(StrykerNamespace.MutantControl.IsActive(0)){return(true?1:2);}else{return Sub(out var x, (StrykerNamespace.MutantControl.IsActive(2)?""Stryker was here!"":""""))?1:2;}}}";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldNotAddReturnDefaultToDestructor()
    {
        var source = @"~TestClass(){;}";
        var expected = @"~TestClass(){if(StrykerNamespace.MutantControl.IsActive(0)){}else{;}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldProperlyMutatePrefixUnitaryExpressionStatement()
    {
        const string Source = @"void Method(int x) {++x;}";
        const string Expected = @"void Method(int x) {if(StrykerNamespace.MutantControl.IsActive(0)){}else{if(StrykerNamespace.MutantControl.IsActive(1)){;}else{if(StrykerNamespace.MutantControl.IsActive(2)){--x;}else{++x;}}}}}";

        ShouldMutateSourceInClassToExpected(Source, Expected);
    }

    [TestMethod]
    public void ShouldMutateExpressionBodiedLocalFunction()
    {
        var source = @"void TestMethod(){
int SomeMethod()  => (true && SomeOtherMethod(out var x)) ? x : 5;
}";
        var expected = @"void TestMethod(){if(StrykerNamespace.MutantControl.IsActive(0)){}else{
int SomeMethod()  {if(StrykerNamespace.MutantControl.IsActive(2)){return(false?x :5);}else{if(StrykerNamespace.MutantControl.IsActive(1)){return(true?x :5);}else{if(StrykerNamespace.MutantControl.IsActive(3)){return(true || SomeOtherMethod(out var x)) ? x : 5;}else{return((StrykerNamespace.MutantControl.IsActive(4)?false:true )&& SomeOtherMethod(out var x)) ? x : 5;}}}};
}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void IfStatementsShouldBeNested()
    {
        var source = @"void TestMethod()
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
        var expected = @"void TestMethod()
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

    [TestMethod]
    public void ShouldNotLeakMutationsToNextMethodOrProperty()
    {
        var source = @"public static class ExampleExtension
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
        var expected = @"public static class ExampleExtension
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

    [TestMethod]
    public void ShouldNotMutateWhenDeclaration()
    {
        var source = @"void TestMethod()
{
	int i = 0;
	var result = Out(out var test) ? test : """";
}
private bool Out(out string test)
{
	   return true;
}";
        var expected = @"void TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{if(StrykerNamespace.MutantControl.IsActive(2)){
	int i = 0;
	var result = (false?test :"""");
}
else{if(StrykerNamespace.MutantControl.IsActive(1)){
	int i = 0;
	var result = (true?test :"""");
}
else{
	int i = 0;
	var result = Out(out var test) ? test : (StrykerNamespace.MutantControl.IsActive(3)?""Stryker was here!"":"""");
}
}}}private bool Out(out string test)
{{test= default(string);}if(StrykerNamespace.MutantControl.IsActive(4)){}else{
	   return (StrykerNamespace.MutantControl.IsActive(5)?false:true);
}return default(bool);}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateWhenDeclarationInInnerScope()
    {
        var source = @"void TestMethod()
{
	int i = 0;
	var result = Out(i, (x) => { int.TryParse(""3"", out int y); return x == y;} ) ? i.ToString() : """";
}
private bool Out(int test, Func<int, bool>lambda )
{
	return true;
}
";
        var expected = @"void TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	int i = 0;
	var result = (StrykerNamespace.MutantControl.IsActive(2)?(false?i.ToString() :""""):(StrykerNamespace.MutantControl.IsActive(1)?(true?i.ToString() :""""):Out(i, (x) => {if(StrykerNamespace.MutantControl.IsActive(3)){}else{ int.TryParse((StrykerNamespace.MutantControl.IsActive(4)?"""":""3""), out int y); return (StrykerNamespace.MutantControl.IsActive(5)?x != y:x == y);} return default;}) ? i.ToString() : (StrykerNamespace.MutantControl.IsActive(6)?""Stryker was here!"":"""")));
}
}private bool Out(int test, Func<int, bool>lambda )
{if(StrykerNamespace.MutantControl.IsActive(7)){}else{
	return (StrykerNamespace.MutantControl.IsActive(8)?false:true);
}
return default(bool);}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
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
	var result = Out(i, (x) => (StrykerNamespace.MutantControl.IsActive(2)?(false?true :false):(StrykerNamespace.MutantControl.IsActive(1)?(true?true :false):int.TryParse((StrykerNamespace.MutantControl.IsActive(3)?"""":""3""), out int y) ? (StrykerNamespace.MutantControl.IsActive(4)?false:true ): (StrykerNamespace.MutantControl.IsActive(5)?true:false))));
}
}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
    public void ShouldMutateConditionalMemberAccessProperly()
    {
        var source = @"void TestMethod() {
                var labelNode = myAttribute?.ArgumentList.Arguments.First()?.Expression;
                return test?.Other()?.Count;
		}";
        var expected = @"void TestMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
            var labelNode = (StrykerNamespace.MutantControl.IsActive(1)?myAttribute?.ArgumentList.Arguments.FirstOrDefault()?.Expression:myAttribute?.ArgumentList.Arguments.First()?.Expression);
                return (StrykerNamespace.MutantControl.IsActive(2)? test?.Other()?.Sum:test?.Other()?.Count);
		}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }


    [TestMethod]
    public void ShouldMutateConditionalExpressionOnArrayDeclaration()
    {
        var source =
            @"public static IEnumerable<int> Foo() => new int[] { }.ToArray()!.Any(x => x==1)?.OrderBy(e => e).ToList();";
        var expected =
            @"public static IEnumerable<int> Foo() => (StrykerNamespace.MutantControl.IsActive(2)?new int[] { }.ToArray()!.Any(x => x==1)?.OrderByDescending(e => e).ToList():(StrykerNamespace.MutantControl.IsActive(0)?new int[] { }.ToArray()!.All(x => x==1):new int[] { }.ToArray()!.Any(x => (StrykerNamespace.MutantControl.IsActive(1)?x!=1:x==1)))?.OrderBy(e => e).ToList());";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
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


    [TestMethod]
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
            """
            public string ExampleBugMethod()
            {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
                string someString = (StrykerNamespace.MutantControl.IsActive(1)?"Stryker was here!":"");
                return (StrykerNamespace.MutantControl.IsActive(2) ? someString.Replace("ab", "cd").Replace("12", "34").PadRight(12).Replace("12", "34") :
                someString.Replace(
                    (StrykerNamespace.MutantControl.IsActive(3)?"":"ab"), (StrykerNamespace.MutantControl.IsActive(4)?"":"cd")
                ).Replace(
                    (StrykerNamespace.MutantControl.IsActive(5)?"":"12"), (StrykerNamespace.MutantControl.IsActive(6)?"":"34")
                ).PadLeft(12).Replace(
                    (StrykerNamespace.MutantControl.IsActive(7)?"":"12"), (StrykerNamespace.MutantControl.IsActive(8)?"":"34")
                ));
            }return default(string);}}
            """;

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateArrayInitializer()
    {
        var source = @"public int[] Foo(){
int[] test = { 1 };
}";
        var expected =
            @"public int[] Foo(){if(StrykerNamespace.MutantControl.IsActive(0)){}else{if(StrykerNamespace.MutantControl.IsActive(1)){
int[] test = {};
}else{
int[] test = { 1 };
}}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateArrayDeclarationAsReturnValue()
    {
        var source = @"public int[] Foo() => new int[] { 1 };";
        var expected =
            @"public int[] Foo() => (StrykerNamespace.MutantControl.IsActive(0)?new int[] {}:new int[] { 1 });";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateListCreation()
    {
        var source = @"public int[] Foo() => new List<int> { 1 };";
        var expected =
            @"public int[] Foo() => (StrykerNamespace.MutantControl.IsActive(0)?new List<int> {}:new List<int> { 1 });";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldNotMutateImplicitArrayCreationProperties()
    {
        var source = @"public int[] Foo() => new [] { 1 };";
        var expected = @"public int[] Foo() => new [] { 1 };";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldNotMutateImplicitArrayCreation()
    {
        var source = "public static readonly int[] Foo =  { 1 };";
        var expected = "public static readonly int[] Foo =  { 1 };";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateProperties()
    {
        var source = @"private string text => ""Some"" + ""Text"";";
        var expected =
            @"private string text => (StrykerNamespace.MutantControl.IsActive(0) ? """" : ""Some"") + (StrykerNamespace.MutantControl.IsActive(1) ? """" : ""Text"");";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateTupleDeclaration()
    {
        var source = @"public void TestMethod() {
var (one, two) = (1 + 1, """");
}";
        var expected = @"public void TestMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
var (one, two) = ((StrykerNamespace.MutantControl.IsActive(1)?1 - 1:1 + 1), (StrykerNamespace.MutantControl.IsActive(2)?""Stryker was here!"":""""));
}
    }";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldNotMutateConst()
    {
        var source = @"private const int x = 1 + 2;";
        var expected = @"private const int x = 1 + 2;";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateStackalloc()
    {
        var source = @"Span<ushort> kindaUnrelated = stackalloc ushort[] { 0 };";
        var expected =
            @"Span<ushort> kindaUnrelated = (StrykerNamespace.MutantControl.IsActive(0)?stackalloc ushort[] {}:stackalloc ushort[] { 0 });";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    /// <summary>
    /// Verifies that <code>EnumMemberDeclarationSyntax</code> nodes are not mutated.
    /// Mutating would introduce code like <code>StrykerXGJbRBlHxqRdD9O.MutantControl.IsActive(0) ? One + 1 : One - 1</code>
    /// Since enum members need to be constants, this mutated code would not compile and print a warning.
    /// </summary>
    [TestMethod]
    public void ShouldNotMutateEnum()
    {
        var source = @"private enum Numbers { One = 1, Two = One + 1 }";
        var expected = @"private enum Numbers { One = 1, Two = One + 1 }";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldNotMutateAttributes()
    {
        var source = @"[Obsolete(""thismustnotbemutated"")]
public void SomeMethod() {}";
        var expected = @"[Obsolete(""thismustnotbemutated"")]
public void SomeMethod() {}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateForWithIfStatementAndConditionalStatement()
    {
        var source = @"public void SomeMethod() {
for (var i = 0; i < 10; i++)
{ }
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
if(StrykerNamespace.MutantControl.IsActive(3)){for (var i = 0; i < 10; i--)
{ }
}else{for (var i = 0; (StrykerNamespace.MutantControl.IsActive(2)?i <= 10:(StrykerNamespace.MutantControl.IsActive(1)?i > 10:i < 10)); i++)
{ }
}}}";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
    public void ShouldMutateComplexExpressionBodiedMethod()
    {
        var source = @"public int SomeMethod()  => (true && SomeOtherMethod(out var x)) ? x : 5;";
        var expected =
            @"public int SomeMethod()  {if(StrykerNamespace.MutantControl.IsActive(1)){return(false?x :5);}else{if(StrykerNamespace.MutantControl.IsActive(0)){return(true?x :5);}else{if(StrykerNamespace.MutantControl.IsActive(2)){return(true || SomeOtherMethod(out var x)) ? x : 5;}else{return ((StrykerNamespace.MutantControl.IsActive(3)?false:true )&& SomeOtherMethod(out var x)) ? x : 5;}}}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateExpressionBodiedStaticConstructor()
    {
        var source = @"static Test()  => (true && SomeOtherMethod(out var x)) ? x : 5;";
        var expected =
            @"static Test()  {using(new StrykerNamespace.MutantContext()){if(StrykerNamespace.MutantControl.IsActive(1)){(false?x :5);}else{if(StrykerNamespace.MutantControl.IsActive(0)){(true?x :5);}else{if(StrykerNamespace.MutantControl.IsActive(2)){(true || SomeOtherMethod(out var x)) ? x : 5;}else{((StrykerNamespace.MutantControl.IsActive(3)?false:true )&& SomeOtherMethod(out var x)) ? x : 5;}}}}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateInlineArrowFunction()
    {
        var source = @"public void SomeMethod() {
int Add(int x, int y) => x + y;
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
int Add(int x, int y) => (StrykerNamespace.MutantControl.IsActive(1)?x - y:x + y);
}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateLambdaSecondParameter()
    {
        var source = @"public void SomeMethod() {
Action act = () => Console.WriteLine(1 + 1, 1 + 1);
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
Action act = () => Console.WriteLine((StrykerNamespace.MutantControl.IsActive(1)?1 - 1:1 + 1), (StrykerNamespace.MutantControl.IsActive(2)?1 - 1:1 + 1));
}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateLinqMethods()
    {
        var source = @"public int TestLinq(int count){
	var list = Enumerable.Range(1, count);
	return list.Last();
}";
        var expected = @"public int TestLinq(int count){if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var list = Enumerable.Range(1, count);
	return (StrykerNamespace.MutantControl.IsActive(1)?list.First():list.Last());
}return default(int);}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateComplexLinqMethods()
    {
        var source = @"private void Linq()
		{
			var array = new []{1, 2};

			var alt1 = array.Count(x => x % 2 == 0);
			var alt2 = array.Min();
		}";
        var expected = @"private void Linq()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else		{
			var array = new []{1, 2};

			var alt1 = (StrykerNamespace.MutantControl.IsActive(1)?array.Sum(x => x % 2 == 0):array.Count(x => (StrykerNamespace.MutantControl.IsActive(2)?x % 2 != 0:(StrykerNamespace.MutantControl.IsActive(3)?x * 2 :x % 2 )== 0)));
			var alt2 = (StrykerNamespace.MutantControl.IsActive(4)?array.Max():array.Min());
		}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateReturnStatements()
    {
        var source = @"private bool Move()
		{
			return true;
		}";
        var expected = @"private bool Move()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else		{
			return (StrykerNamespace.MutantControl.IsActive(1)?false:true);
		}return default(bool);}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateWhileLoop()
    {
        var source = @"private void DummyLoop()
		{
			while (this.Move())
			{
				int x = 2 + 3;
			}
		}";
        var expected = @"private void DummyLoop()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else		{
			while ((StrykerNamespace.MutantControl.IsActive(1)?!(this.Move()):this.Move()))
{if(StrykerNamespace.MutantControl.IsActive(2)){}else			{
				int x = (StrykerNamespace.MutantControl.IsActive(3)?2 - 3:2 + 3);
			}
}		}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateAssignmentStatementsWithIfStatement()
    {
        var source = @"public void SomeMethod() {
	var x = 0;
	x *= x + 2;
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
if(StrykerNamespace.MutantControl.IsActive(1)){	x /=x + 2;
}else{	x *= (StrykerNamespace.MutantControl.IsActive(2)?x - 2:x + 2);
}}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateRecursiveCoalescingAssignmentStatements()
    {
        var source = @"public void SomeMethod() {
    List<int> a = null;
    List<int> b = null;
    a ??= b ??= new List<int>();
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
    List<int> a = null;
    List<int> b = null;
if(StrykerNamespace.MutantControl.IsActive(1)){    a = b ??= new List<int>();
}else{if(StrykerNamespace.MutantControl.IsActive(2)){    a ??= b = new List<int>();
}else{    a ??= b ??= new List<int>();
}}}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateRecursiveNullCoalescingStatements()
    {
        var source = @"public void SomeMethod() {
    List<int> a = null;
    List<int> b = null;
    List<int> c = null;
    var d = a ?? b ?? c;
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
    List<int> a = null;
    List<int> b = null;
    List<int> c = null;
    var d = (StrykerNamespace.MutantControl.IsActive(3)?a :(StrykerNamespace.MutantControl.IsActive(2)?b ?? c:(StrykerNamespace.MutantControl.IsActive(1)?b ?? c ?? a :a ?? (StrykerNamespace.MutantControl.IsActive(6)?b :(StrykerNamespace.MutantControl.IsActive(5)?c:(StrykerNamespace.MutantControl.IsActive(4)?c ?? b :b ?? c))))));
}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateIncrementStatementWithIfStatement()
    {
        var source = @"public void SomeMethod() {
	var x = 0;
	x++;
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
if(StrykerNamespace.MutantControl.IsActive(1)){;}else{if(StrykerNamespace.MutantControl.IsActive(2)){	x--;
}else{	x++;
}}}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    public void ShouldNotMutateTopLevelStatementsIfDisabledByComment()
    {
        var source = @"
	var x = 0;
x++;
// Stryker disable all
	x++;
	x/=2;
";
        var expected = @"	var x = 0;
if(StrykerNamespace.MutantControl.IsActive(0)){;}else{if(StrykerNamespace.MutantControl.IsActive(1)){x--;
}else{x++;
}}// Stryker disable all
	x++;
	x/=2;
";

        ShouldMutateSourceToExpected(source, expected);
            
    }

    [TestMethod]
    public void ShouldMutateChainedMutations()
    {
        var source = @"public void Simple()
		{
			object value = null;
			var flag1 = false;
			var flag2 = false;
			if (value != null && !flag1 && !flag2)
			{
			}
		}";
        var expected = @"public void Simple()
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

    [TestMethod]
    public void ShouldMutateStatics()
    {
        var source = @"private static bool willMutateToFalse = true;

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
        var expected = @"private static bool willMutateToFalse = StrykerNamespace.MutantContext.TrackValue(()=>(StrykerNamespace.MutantControl.IsActive(0)?false:true));

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

    [TestMethod]
    public void ShouldNotMutateDefaultValues()
    {
        var source = @"public void SomeMethod(bool option = true) {}";
        var expected = @"public void SomeMethod(bool option = true) {}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldInitializeOutVars()
    {
        var source = @"public void SomeMethod(out int x, out string text) { x = 1; text = ""hello"";}";
        var expected = @"public void SomeMethod(out int x, out string text) {{x= default(int);text= default(string);}if(StrykerNamespace.MutantControl.IsActive(0)){}else{ x = 1; text = (StrykerNamespace.MutantControl.IsActive(1)?"""":""hello"");
        }}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateInsideLambda()
    {
        var source = @"private async Task GoodLuck()
{
	await SendRequest(url, HttpMethod.Get, (request) =>
	{
		request.Headers.Add(""Accept"", ""application / json; version = 1"");
		request.Headers.TryAddWithoutValidation(""Date"", date);
}, ensureSuccessStatusCode: false);
}";
        var expected = @"private async Task GoodLuck()
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

    [TestMethod]
    public void ShouldMutateDelegate()
    {
        var source = @"private void LocalFun()
{
	var test = delegate(string name)
{
Console.WriteLine($""Hello {name}"");
};
}";
        var expected = @"private void LocalFun()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var test = delegate(string name)
{if(StrykerNamespace.MutantControl.IsActive(1)){}else{
if(StrykerNamespace.MutantControl.IsActive(2)){;}else{Console.WriteLine((StrykerNamespace.MutantControl.IsActive(3)?$"""":$""Hello {name}""));
}}};
}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateParameterLessDelegate()
    {
        var source = @"private void LocalFun()
{
	var test = delegate
{
Console.WriteLine(""Hello"");
};
}";
        var expected = @"private void LocalFun()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var test = delegate
{if(StrykerNamespace.MutantControl.IsActive(1)){}else{
if(StrykerNamespace.MutantControl.IsActive(2)){;}else{Console.WriteLine((StrykerNamespace.MutantControl.IsActive(3)?"""":""Hello""));
}}};
}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void MutationsShouldHaveLinespan()
    {
        var source = @"void TestMethod()
{
	var test3 = 2 + 5;
}";
        Target.Mutate(CSharpSyntaxTree.ParseText(source), null);

        var mutants = Target.GetLatestMutantBatch().ToList();
        mutants.Count.ShouldBe(2);
        var blockLinespan = mutants.First().Mutation.OriginalNode.GetLocation().GetLineSpan();
        blockLinespan.StartLinePosition.Line.ShouldBe(1);
        blockLinespan.EndLinePosition.Line.ShouldBe(3);
        mutants.Last().Mutation.OriginalNode.GetLocation().GetLineSpan().StartLinePosition.Line.ShouldBe(2);
    }

    [TestMethod]
    public void MutationsShouldHaveLinespan2()
    {
        var source = @"using System;
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
        Target.Mutate(CSharpSyntaxTree.ParseText(source), null);

        var mutants = Target.GetLatestMutantBatch().ToList();
        mutants.Count.ShouldBe(7);

        var blockLinespan = mutants.First().Mutation.OriginalNode.GetLocation().GetLineSpan();
        blockLinespan.StartLinePosition.Line.ShouldBe(9);
        blockLinespan.EndLinePosition.Line.ShouldBe(11);
        foreach (var mutant in mutants.Skip(1))
        {
            mutant.Mutation.OriginalNode.GetLocation().GetLineSpan().StartLinePosition.Line.ShouldBe(10);
        }
    }

    [TestMethod]
    public void ShouldAddReturnDefaultToMethods()
    {
        var source = @"bool TestMethod()
{
	while(true) return false;
}";
        var expected = @"bool TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	while((StrykerNamespace.MutantControl.IsActive(2)?!(true):(StrykerNamespace.MutantControl.IsActive(1)?false:true))) return (StrykerNamespace.MutantControl.IsActive(3)?true:false);
}return default(bool);}";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldAddReturnDefaultToConversion()
    {
        var source = @"public static explicit operator string(TestClass value)
{ while(true) return value;
}";
        var expected = @"public static explicit operator string(TestClass value)
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{ while((StrykerNamespace.MutantControl.IsActive(2)?!(true):(StrykerNamespace.MutantControl.IsActive(1)?false:true))) return value;
}return default(string);}";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldNotMutateConstDeclaration()
    {
        var source = @"void Test(){
const string text = ""a""+""b"";}";
        var expected = @"void Test(){if(StrykerNamespace.MutantControl.IsActive(0)){}else{
const string text = ""a""+""b"";}}";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldAddReturnDefaultToAsyncMethods()
    {
        var source = @"public async Task<bool> TestMethod()
{   while(true) return false;
}";
        var expected = @"public async Task<bool> TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{  while((StrykerNamespace.MutantControl.IsActive(2)?!(true):(StrykerNamespace.MutantControl.IsActive(1)?false:true))) return (StrykerNamespace.MutantControl.IsActive(3)?true:false);
}return default(bool);}";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldAddReturnDefaultToIndexerGetter()
    {
        var source = @"public string this[string key]
{
    get
    {
        return key;
    }
}";
        var expected = @"public string this[string key]
{
    get
{if(StrykerNamespace.MutantControl.IsActive(0)){}else        {
        return key;
    }
return default(string);}
}";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldNotAddReturnDefaultToEnumerationMethods()
    {
        var source = @"public static IEnumerable<object> Extracting<T>(this IEnumerable<T> enumerable, string propertyName)
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
        var expected = @"public static IEnumerable<object> Extracting<T>(this IEnumerable<T> enumerable, string propertyName)
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

    [TestMethod]
    public void ShouldAddReturnDefaultToAsyncWithFullNamespaceMethods()
    {
        var source = @"public async System.Threading.Tasks.Task<bool> TestMethod()
{  while(true) return false;
}";
        var expected = @"public async System.Threading.Tasks.Task<bool> TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{  while((StrykerNamespace.MutantControl.IsActive(2)?!(true):(StrykerNamespace.MutantControl.IsActive(1)?false:true))) return (StrykerNamespace.MutantControl.IsActive(3)?true:false);
}return default(bool);}";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldNotAddReturnDefaultToAsyncTaskMethods()
    {
        var source = @"public async Task TestMethod()
{
	;
}";
        var expected = @"public async Task TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}
else{
	;
}
}";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldNotAddReturnDefaultToMethodsWithReturnTypeVoid()
    {
        var source = @"void TestMethod()
{
	;
}";
        var expected = @"void TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else
{
	;
}
}";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    [DataRow("{return 0;}")]
    [DataRow("{return 1; throw Exception();}")]
    [DataRow("{return 1;yield return 0;}")]
    [DataRow("{return 1;yield break;}")]
    [DataRow("{;}")]
    public void ShouldNotAddReturnOnSomeBlocks(string code)
    {
        var source = @$"
int TestMethod()
// Stryker disable all
{code}";
        ShouldMutateSourceInClassToExpected(source, source);
    }

    [TestMethod]
    public void ShouldMutatetringsInSwitchExpression()
    {
        var source = @"string TestMethod()
{
	return input switch
	{
		""test"" => ""test""
	};
}";
        var expected = @"string TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	return (StrykerNamespace.MutantControl.IsActive(1)?input switch
	{
""""=> ""test""
	}:input switch
	{
		""test"" => (StrykerNamespace.MutantControl.IsActive(2)?"""":""test""
)	});
}return default(string);}
";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
    public void ShouldMutatePropertiesInArrowFormEvenWithComplexConstruction()
    {
        var source = @"class Test {
string Value {get => Out(out var x)? ""empty"": """";}
static TestClass(){}}";

        var expected = @"class Test {
string Value {get {if(StrykerNamespace.MutantControl.IsActive(1)){return(false?""empty"":"""");}else{if(StrykerNamespace.MutantControl.IsActive(0)){return(true?""empty"":"""");}else{return Out(out var x)? (StrykerNamespace.MutantControl.IsActive(2)?"""":""empty""): (StrykerNamespace.MutantControl.IsActive(3)?""Stryker was here!"":"""");}}}}
static TestClass(){using(new StrykerNamespace.MutantContext()){}}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateConditionalExpression()
    {
        var source = @"void TestMethod()
{
var a = 1;
var x = a == 1 ? 5 : 7;
}";
        var expected = @"void TestMethod()
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{
var a = 1;
var x = (StrykerNamespace.MutantControl.IsActive(2)?(false?5 :7):(StrykerNamespace.MutantControl.IsActive(1)?(true?5 :7):(StrykerNamespace.MutantControl.IsActive(3)?a != 1 :a == 1 )? 5 : 7));
}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
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

    [TestMethod]
    public void ShouldMarkStaticMutationStaticInPropertiesInitializer()
    {
        var source = @"class Test {
static string Value {get;} = """";}";

        var expected = @"class Test {
static string Value {get;} = StrykerNamespace.MutantContext.TrackValue(()=>(StrykerNamespace.MutantControl.IsActive(0)?""Stryker was here!"":""""));}}}";
        ShouldMutateSourceInClassToExpected(source, expected);
        Target.Mutants.Count.ShouldBe(1);
        Target.Mutants.First().IsStaticValue.ShouldBeTrue();
    }

    [TestMethod]
    public void ShouldMutateStaticPropertiesInArrowFormEvenWithComplexConstruction()
    {
        var source = @"class Test {
static string Value => Out(out var x)? ""empty"": """";}";

        var expected = @"class Test {
static string Value {get{if(StrykerNamespace.MutantControl.IsActive(1)){return(false?""empty"":"""");}else{if(StrykerNamespace.MutantControl.IsActive(0)){return(true?""empty"":"""");}else{return Out(out var x)? (StrykerNamespace.MutantControl.IsActive(2)?"""":""empty""): (StrykerNamespace.MutantControl.IsActive(3)?""Stryker was here!"":"""");}}}}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutatePropertiesAsArrowExpression()
    {
        var source = @"class Test {
string Value => Generator(out var x) ? """" :""test"";
}";

        var expected = @"class Test {
string Value {get{if(StrykerNamespace.MutantControl.IsActive(1)){return(false?"""" :""test"");}else{if(StrykerNamespace.MutantControl.IsActive(0)){return(true?"""" :""test"");}else{return Generator(out var x) ? (StrykerNamespace.MutantControl.IsActive(2)?""Stryker was here!"":"""" ):(StrykerNamespace.MutantControl.IsActive(3)?"""":""test"");}}}}}";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
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

    [TestMethod]
    // test for issue #1386
    public void ShouldNotLeakMutationsAcrossDefinitions()
    {
        var source = @"class Test {
int GetId(string input) => int.TryParse(input, out var result) ? result : 0;
string Value => Generator(out var x) ? """" :""test"";
}";

        var expected = @"class Test {
int GetId(string input) {if(StrykerNamespace.MutantControl.IsActive(1)){return(false?result :0);}else{if(StrykerNamespace.MutantControl.IsActive(0)){return(true?result :0);}else{return int.TryParse(input, out var result) ? result : 0;}}}string Value {get{if(StrykerNamespace.MutantControl.IsActive(3)){return(false?"""" :""test"");}else{if(StrykerNamespace.MutantControl.IsActive(2)){return(true?"""" :""test"");}else{return Generator(out var x) ? (StrykerNamespace.MutantControl.IsActive(4)?""Stryker was here!"":"""" ):(StrykerNamespace.MutantControl.IsActive(5)?"""":""test"");}}}}}";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
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


    [TestMethod]
    public void ShouldNotMutateMethodsWithStringNameMethodsOnCustomClass()
    {
        var source = """
                     class Test {
                         void Substring() {}
                         string Trim() {
                             return "test";
                         }
                     }
                     static string Value() {
                         var testClass = new Test();
                         testClass.Substring();
                         return testText.Trim();
                     }
                     """;

        // 0 = block removal
        // 1 = string mutation (not method mutation)
        // 2 = block removal
        // 3 = statement mutator
        var expected = """
                       class Test {
                           void Substring() {}
                           string Trim() {
                               if (StrykerNamespace.MutantControl.IsActive(0)) {}
                               else {
                                   return (StrykerNamespace.MutantControl.IsActive(1) ? "" : "test");
                               }
                               return default(string);
                           }
                       }
                       static string Value() {
                           if(StrykerNamespace.MutantControl.IsActive(2)) {}
                           else {
                               var testClass = new Test();
                               if (StrykerNamespace.MutantControl.IsActive(3)) {;}
                               else { testClass.Substring(); }
                               return testText.Trim();
                           }
                           return default(string);
                       }
                       """;

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateTrimMethodOnStringIdentifier()
    {
        var source = "static string Value(string text) => text.Trim();";

        var expected = """
                       static string Value(string text) =>
                       (StrykerNamespace.MutantControl.IsActive(0) ? "" : text.Trim());
                       """;

        ShouldMutateSourceInClassToExpected(source, expected);
    }


    [TestMethod]
    public void ShouldMutateSubStringMethod()
    {
        var source = """static string Value => "test ".Substring(2);""";

        var expected =
            """
            static string Value => (
                StrykerNamespace.MutantControl.IsActive(0)
                    ? ""
                    : (StrykerNamespace.MutantControl.IsActive(1) ? "" : "test ")
                .Substring(2)
            );
            """;

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateChainedStringMethods()
    {
        var source = """static char Value => "test ".ToUpper().Trim().PadLeft(2).Substring(2).ElementAt(2);""";

        var expected =
            """
            static char Value =>
            (StrykerNamespace.MutantControl.IsActive(0)?'\0':
            (StrykerNamespace.MutantControl.IsActive(2)?"test ".ToUpper().Trim().PadRight(2).Substring(2).ElementAt(2):
            (StrykerNamespace.MutantControl.IsActive(4)?"test ".ToLower().Trim().PadLeft(2).Substring(2).ElementAt(2):
            (StrykerNamespace.MutantControl.IsActive(1)?"".ElementAt(2):
            (StrykerNamespace.MutantControl.IsActive(3)?"".PadLeft(2).Substring(2).ElementAt(2):
            (StrykerNamespace.MutantControl.IsActive(5)?"":"test ").ToUpper().Trim().PadLeft(2).Substring(2).ElementAt(2))))));
            """;

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldIncrementMutantCountUniquely()
    {
        var strykerOptions = new StrykerOptions
        {
            MutantIdProvider = new BasicIdProvider()
        };

        var firstOrchestrator =
            new CsharpMutantOrchestrator(new MutantPlacer(Injector), options: strykerOptions);
        var secondOrchestrator =
            new CsharpMutantOrchestrator(new MutantPlacer(Injector), options: strykerOptions);
        var node = SyntaxFactory.ParseExpression("1 == 1") as BinaryExpressionSyntax;

        var firstMutant = firstOrchestrator
            .GenerateMutationsForNode(node, null, new MutationContext(secondOrchestrator))
            .Single();

        var secondMutant = secondOrchestrator
            .GenerateMutationsForNode(node, null, new MutationContext(secondOrchestrator))
            .Single();

        secondMutant.Id.ShouldBe(firstMutant.Id + 1);
    }

    [TestMethod]
    public void ShouldMutateCollectionExpressionSpanProperty()
    {
        var source = "static ReadOnlySpan<int> Value => [1, 2, 3];";

        var expected =
            "static ReadOnlySpan<int> Value => (StrykerNamespace.MutantControl.IsActive(0)?(ReadOnlySpan<int>)[]:[1,2,3]);";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateCollectionExpressionLocalsInMethod()
    {
        var source = """
                     public void M() {
                         int[] abc = { 5, 5 };
                         int[] bcd = [1, .. abc, 3];
                     }
                     """;

        var expected =
            """
            public void M() {
                if (StrykerNamespace.MutantControl.IsActive(0)) { }
                else{
                    if(StrykerNamespace.MutantControl.IsActive(1)){
                        int[] abc = {};
                        int[] bcd = [1, .. abc, 3];
                    } else {
                        int[] abc = { 5, 5 };
                        int[] bcd = (StrykerNamespace.MutantControl.IsActive(2)?(int[])[]:[1, .. abc, 3]);
                    }
                }
            }
            """;
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateImplicitCollectionExpressionInMethod()
    {
        var source = """
                     public void M() {
                         int[] abc = { 5, 5 };
                         var bcd = (int[])[1, .. abc, 3];
                     }
                     """;

        var expected =
            """
            public void M() {
              if (StrykerNamespace.MutantControl.IsActive(0)) {
              } else {
                if (StrykerNamespace.MutantControl.IsActive(1)) {
                  int[] abc = {};
                  var bcd = (int[])[1, ..abc, 3];
                } else {
                  int[] abc = {5, 5};
                  var bcd = (int[])(
                      StrykerNamespace.MutantControl.IsActive(2) ? (int[])[] : [ 1, ..abc, 3 ]);
                }
              }
            }
            """;
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateUsedCollectionExpression()
    {
        var source = """
                     public void M() {
                         // Stryker disable String : Not mutation under test
                         Span<string> weekDays = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
                     }
                     """;

        var expected =
            """
            public void M() {
              if (StrykerNamespace.MutantControl.IsActive(0)) {
              } else {
                // Stryker disable String : Not mutation under test
                Span<string> weekDays = (StrykerNamespace.MutantControl.IsActive(1) ? (Span<string>)[] : ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"]);
              }
            }
            """;
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateCollectionExpressionInManyForms()
    {
        var source = """
                     // Initialize private field:
                     // Stryker disable String : Not mutation under test
                     private static readonly ImmutableArray<string> _months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                     // property with expression body:
                     public IEnumerable<int> MaxDays =>
                         [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
                     public int Sum(IEnumerable<int> values) =>
                         values.Sum();
                     public void Example()
                     {
                         // As a parameter:
                         int sum = Sum([1, 2, 3, 4, 5]);
                     }
                     """;

        var expected =
            """
            // Initialize private field:
            // Stryker disable String : Not mutation under test
            private static readonly ImmutableArray<string> _months =StrykerNamespace.MutantContext.TrackValue(() =>(StrykerNamespace.MutantControl.IsActive(0) ? (ImmutableArray<string>)[] : ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]));

            // property with expression body:
            public IEnumerable<int> MaxDays =>
                (StrykerNamespace.MutantControl.IsActive(13)
                     ? (IEnumerable<int>)[]
                     : [ 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 ]);

            public int Sum(IEnumerable<int> values) =>
                (StrykerNamespace.MutantControl.IsActive(14) ? values.Max() : values.Sum());

            public void Example() {
              if (StrykerNamespace.MutantControl.IsActive(15)) {
              } else {
                // As a parameter:
                int sum = Sum((StrykerNamespace.MutantControl.IsActive(16)?(IEnumerable<int>)[]:[1, 2, 3, 4, 5]));
              }
            }
            """;
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateNonConstatCollectionExpression()
    {
        var source = """
                     public void M() {
                         // Stryker disable String : Not mutation under test
                         string hydrogen = "H";
                         string helium = "He";
                         string lithium = "Li";
                         string beryllium = "Be";
                         string boron = "B";
                         string carbon = "C";
                         string nitrogen = "N";
                         string oxygen = "O";
                         string fluorine = "F";
                         string neon = "Ne";
                         string[] elements = [hydrogen, helium, lithium, beryllium, boron, carbon, nitrogen, oxygen, fluorine, neon];
                     }
                     """;

        var expected =
            """
            public void M() {
              // Stryker disable String : Not mutation under test
              if (StrykerNamespace.MutantControl.IsActive(0)) {
              } else {
                string hydrogen = "H";
                string helium = "He";
                string lithium = "Li";
                string beryllium = "Be";
                string boron = "B";
                string carbon = "C";
                string nitrogen = "N";
                string oxygen = "O";
                string fluorine = "F";
                string neon = "Ne";
                string[] elements = (StrykerNamespace.MutantControl.IsActive(11) ? (string[])[] : [
                  hydrogen, helium, lithium, beryllium, boron, carbon, nitrogen, oxygen,
                  fluorine, neon
                ]);
              }
            }
            """;
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateSpreadCollectionExpression()
    {
        var source = """
                     public void M() {
                        // Stryker disable String : Not mutation under test
                        string[] vowels = ["a", "e", "i", "o", "u"];
                        string[] consonants = ["b", "c", "d", "f", "g", "h", "j", "k", "l", "m",
                                               "n", "p", "q", "r", "s", "t", "v", "w", "x", "z"];
                        string[] alphabet = [.. vowels, .. consonants, "y"];
                     }
                     """;

        var expected =
            """
            public void M() {
              // Stryker disable String : Not mutation under test
              if (StrykerNamespace.MutantControl.IsActive(0)) {
              } else {
                string[] vowels = (StrykerNamespace.MutantControl.IsActive(1) ? (string[])[] : ["a", "e", "i", "o", "u"]);
                string[] consonants = (StrykerNamespace.MutantControl.IsActive(7) ? (string[])[] : [
                                        "b", "c", "d", "f", "g", "h", "j", "k", "l", "m",
                                        "n", "p", "q", "r", "s", "t", "v", "w", "x", "z"
                                       ]);
                string[] alphabet =
                    (StrykerNamespace.MutantControl.IsActive(28)
                         ? (string[])[]
                         : [..vowels, ..consonants, "y"]);
              }
            }
            """;
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateImplicitCollectionExpressionParameter()
    {
        var source = """
                     public void M() {
                         Iter([1]);
                     }
                     public IEnumerable<T> Iter<T>(IList<T> list) { }
                     """;

        var expected =
            """
            public void M() {
              if (StrykerNamespace.MutantControl.IsActive(0)) {
              } else {
                if (StrykerNamespace.MutantControl.IsActive(1)) {
                  ;
                } else {
                  Iter((StrykerNamespace.MutantControl.IsActive(2)?(IList<int>)[]:[1]));
                }
              }
            }
            public IEnumerable<T> Iter<T>(IList<T> list) { }
            """;
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateNestedImplicitCollectionExpression()
    {
        var source = "static int[][] Value => [[1, 2], [3]];";

        var expected =
            "static int[][] Value => (StrykerNamespace.MutantControl.IsActive(0)?(int[][])[]:[(StrykerNamespace.MutantControl.IsActive(1)?(int[])[]:[1, 2]), (StrykerNamespace.MutantControl.IsActive(2)?(int[])[]:[3])]);";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldMutateNestedExplicitCollectionExpression()
    {
        var source = "static int[][] Value => [[1, 2], new int[] { 3 }];";

        var expected =
            "static int[][] Value => (StrykerNamespace.MutantControl.IsActive(0)?(int[][])[]:[(StrykerNamespace.MutantControl.IsActive(1)?(int[])[]:[1, 2]), (StrykerNamespace.MutantControl.IsActive(2)?new int[] {}:new int[] { 3 })]);";
        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldProtectDirectives()
    {
        var source = @"public void SomeMethod() {
    var x = 0;
#if !DEBUG
    x++;
#endif
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
    var x = 0;
#if !DEBUG
if(StrykerNamespace.MutantControl.IsActive(1)){;}else{if(StrykerNamespace.MutantControl.IsActive(2)){
    x--;
}else{
    x++;
}}
#endif
}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }


}
