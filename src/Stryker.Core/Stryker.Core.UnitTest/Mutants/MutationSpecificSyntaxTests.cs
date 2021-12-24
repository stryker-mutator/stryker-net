using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
    public class MutationSpecificSyntaxTests: MutantOrchestratorTestsBase
    {
        [Fact]
        public void ShouldMutateInterfaces()
        {
            var actual = @"using System;
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
            var actualNode = _target.Mutate(CSharpSyntaxTree.ParseText(actual).GetRoot());
            actual = actualNode.ToFullString();
            actual = actual.Replace(CodeInjection.HelperNamespace, "StrykerNamespace");
            actualNode = CSharpSyntaxTree.ParseText(actual).GetRoot();
            var expectedNode = CSharpSyntaxTree.ParseText(expected).GetRoot();
            actualNode.ShouldBeSemantically(expectedNode);
            actualNode.ShouldNotContainErrors();
        }

        [Fact]
        public void ShouldMutateBlockStatements()
        {
            var options = new StrykerOptions
            {
                MutationLevel = MutationLevel.Complete,
                OptimizationMode = OptimizationModes.CoverageBasedTest,
            };
            _target = new CsharpMutantOrchestrator(options: options);

            string source = @"private void Move()
			{
				;
			}";
            string expected = @"private void Move()
    {if(StrykerNamespace.MutantControl.IsActive(0)){}else		    {
    			    ;
    		    }}";

            ShouldMutateSourceToExpected(source, expected);

            source = @"private int Move()
			{
				;
			}";
            expected = @"private int Move()
    {if(StrykerNamespace.MutantControl.IsActive(1)){}else		    {
    			    ;
    		    } return default(int);}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Theory]
        [InlineData("=> Value = \"Hello, World!\";")]
        [InlineData("{Value = \"Hello, World!\";}")]
        public void ShouldMutateStaticConstructor(string source)
        {
            source = @"class Test {
static string Value { get; }
static TestClass() " + source + "}";

            var expected = @"class Test {
static string Value { get; }
static TestClass() {using(new StrykerNamespace.MutantContext()){Value = (StrykerNamespace.MutantControl.IsActive(0)?"""":""Hello, World!"");}}}";

            ShouldMutateSourceToExpected(source, expected);
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

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutatePropertiesInArrowFormEvenWithComplexConstruction()
        {
            var source = @"class Test {
string Value {get => Out(out var x)? ""empty"": """";}
static TestClass(){}}";

            var expected = @"class Test {
string Value {get {if(StrykerNamespace.MutantControl.IsActive(0)){return!(Out(out var x))? ""empty"": """";}else{return Out(out var x)? (StrykerNamespace.MutantControl.IsActive(1)?"""":""empty""): (StrykerNamespace.MutantControl.IsActive(2)?""Stryker was here!"":"""");}return default(string);}}
static TestClass(){using(new StrykerNamespace.MutantContext()){}}}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateStaticProperties()
        {
            var source = @"class Test {
static string Value
{
    get { return ""TestDescription"";}
    set { value = ""TestDescription"";}
}
static TestClass(){}}";

            var expected = @"class Test {
static string Value
{
    get { return (StrykerNamespace.MutantControl.IsActive(0)?"""":""TestDescription"");}
    set { value = (StrykerNamespace.MutantControl.IsActive(1)?"""":""TestDescription"");}
}
static TestClass(){using(new StrykerNamespace.MutantContext()){}}}";
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMarkStaticMutationStaticInPropertiesInitializer()
        {
            var source = @"class Test {
static string Value {get;} = """";}";

            var expected = @"class Test {
static string Value {get;} = StrykerNamespace.MutantContext.TrackValue(()=>(StrykerNamespace.MutantControl.IsActive(0)?""Stryker was here!"":""""));}}}";
            ShouldMutateSourceToExpected(source, expected);
            _target.Mutants.Count.ShouldBe(1);
            _target.Mutants.First().IsStaticValue.ShouldBeTrue();
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
            ShouldMutateSourceToExpected(source, expected);
        }


        [Fact]
        public void ShouldControlLocalDeclarationMutationAtTheBlockLevel()
        {
            var source = @"public static string FormatPrettyByte(Int64 value)
		{
			string[] SizeSuffixes = { ""bytes"", ""KB"", ""MB"", ""GB"", ""TB"", ""PB"", ""EB"", ""ZB"", ""YB"" };

			int mag = (int)Math.Log(value, 1024);
			decimal adjustedSize = (decimal)value / (1L << (mag * 10));

			return $""{adjustedSize:n1} {SizeSuffixes[mag]}"";
		}
";

            var expected = @"public static string FormatPrettyByte(Int64 value)
{if(StrykerNamespace.MutantControl.IsActive(0))		{
			string[] SizeSuffixes = {};

			int mag = (int)Math.Log(value, 1024);
			decimal adjustedSize = (decimal)value / (1L << (mag * 10));

			return $""{adjustedSize:n1} {SizeSuffixes[mag]}"";
		}
else		{
			string[] SizeSuffixes = { (StrykerNamespace.MutantControl.IsActive(1)?"""":""bytes""), (StrykerNamespace.MutantControl.IsActive(2)?"""":""KB""), (StrykerNamespace.MutantControl.IsActive(3)?"""":""MB""), (StrykerNamespace.MutantControl.IsActive(4)?"""":""GB""), (StrykerNamespace.MutantControl.IsActive(5)?"""":""TB""), (StrykerNamespace.MutantControl.IsActive(6)?"""":""PB""), (StrykerNamespace.MutantControl.IsActive(7)?"""":""EB""), (StrykerNamespace.MutantControl.IsActive(8)?"""":""ZB""), (StrykerNamespace.MutantControl.IsActive(9)?"""":""YB"" )};

			int mag = (int)Math.Log(value, 1024);
			decimal adjustedSize = (StrykerNamespace.MutantControl.IsActive(10)?(decimal)value * (1L << (mag * 10)):(decimal)value / ((StrykerNamespace.MutantControl.IsActive(11)?1L >> (mag * 10):1L << ((StrykerNamespace.MutantControl.IsActive(12)?mag / 10:mag * 10)))));

			return (StrykerNamespace.MutantControl.IsActive(13)?$"""":$""{adjustedSize:n1} {SizeSuffixes[mag]}"");
		}
return default(string);}}";
            ShouldMutateSourceToExpected(source, expected);

        }

        [Fact]
        public void ShouldNotMutateConstDeclaration()
        {
            var source = @"void Test(){
const string text = ""a""+""b""+""c"";}";
            var expected = @"void Test(){
const string text = ""a""+""b""+""c"";}";
            ShouldMutateSourceToExpected(source, expected);
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
{
    return input switch
    {
        ""test"" => (StrykerNamespace.MutantControl.IsActive(0)?"""":""test""
)    };
}";
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateComplexExpressionBodiedLocalFunction()
        {
            string source = @"void TestMethod(){
int SomeMethod()  => (true && SomeOtherMethod(out var x)) ? x : 5;
}";
            string expected = @"void TestMethod(){
int SomeMethod()  {if(StrykerNamespace.MutantControl.IsActive(0)){return!((true && SomeOtherMethod(out var x)) )? x : 5;}else{if(StrykerNamespace.MutantControl.IsActive(1)){return(true || SomeOtherMethod(out var x)) ? x : 5;}else{return ((StrykerNamespace.MutantControl.IsActive(2)?false:true )&& SomeOtherMethod(out var x)) ? x : 5;}}};
}";

            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldMutateExpressionBodiedLocalFunction()
        {
            string source = @"void TestMethod(){
void SomeMethod()  => (true && SomeOtherMethod(out var x)) ? x : 5;
}";
            string expected = @"void TestMethod(){
void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){!((true && SomeOtherMethod(out var x)) )? x : 5;}else{if(StrykerNamespace.MutantControl.IsActive(1)){(true || SomeOtherMethod(out var x)) ? x : 5;}else{((StrykerNamespace.MutantControl.IsActive(2)?false:true )&& SomeOtherMethod(out var x)) ? x : 5;}}};
}";

            ShouldMutateSourceToExpected(source, expected);
        }
    }
}
