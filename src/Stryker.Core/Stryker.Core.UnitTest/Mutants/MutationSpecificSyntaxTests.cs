using System.Linq;
using Shouldly;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
    public class MutationSpecificSyntaxTests: MutantOrchestratorTestsBase
    {
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
string Value {get {if(StrykerNamespace.MutantControl.IsActive(0)){return!(Out(out var x))? ""empty"": """";}else{return Out(out var x)? (StrykerNamespace.MutantControl.IsActive(1)?"""":""empty""): (StrykerNamespace.MutantControl.IsActive(2)?""Stryker was here!"":"""");}}}
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
static string Value {get;} = (StrykerNamespace.MutantControl.IsActive(0)?""Stryker was here!"":"""");}";
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
        public void ShouldAddReturnDefaultToMethods()
        {
            string source = @"bool TestMethod()
{
    ;
}";
            string expected = @"bool TestMethod()
{
    ;
    return default(bool);
}";
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldAddReturnDefaultToConversion()
        {
            string source = @"public static explicit operator string(TestClass value)
{;
}";
            string expected = @"public static explicit operator string(TestClass value)
{;
return default(string);
}";
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldAddReturnDefaultToOperator()
        {
            string source = @"public static string operator+ (TestClass value, TestClass other)
{;
}";
            string expected = @"public static string operator+ (TestClass value, TestClass other)
{;
return default(string);
}";
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldAddReturnDefaultToArrowExpressionOperator()
        {
            string source = @"public static int operator+ (TestClass value, TestClass other) => Sub(out var x, """")?1:2;";
            string expected = @"public static int operator+ (TestClass value, TestClass other) {if(StrykerNamespace.MutantControl.IsActive(0)){return!(Sub(out var x, """"))?1:2;}else{return Sub(out var x, (StrykerNamespace.MutantControl.IsActive(1)?""Stryker was here!"":""""))?1:2;}}";
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotAddReturnDefaultToDestructor()
        {
            string source = @"~TestClass(){;}";

            ShouldMutateSourceToExpected(source, source);
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
        public void ShouldAddReturnDefaultToAsyncMethods()
        {
            string source = @"public async Task<bool> TestMethod()
{
    ;
}";
            string expected = @"public async Task<bool> TestMethod()
{
    ;
    return default(bool);
}";
            ShouldMutateSourceToExpected(source, expected);
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
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldAddReturnDefaultToAsyncWithFullNamespaceMethods()
        {
            string source = @"public async System.Threading.Tasks.Task<bool> TestMethod()
{
    ;
}";
            string expected = @"public async System.Threading.Tasks.Task<bool> TestMethod()
{
    ;
    return default(bool);
}";
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotAddReturnDefaultToAsyncTaskMethods()
        {
            string source = @"public async Task TestMethod()
{
    ;
}";
            string expected = @"public async Task TestMethod()
{
    ;
}";
            ShouldMutateSourceToExpected(source, expected);
        }

        [Fact]
        public void ShouldNotAddReturnDefaultToMethodsWithReturnTypeVoid()
        {
            string source = @"void TestMethod()
{
    ;
}";
            string expected = @"void TestMethod()
{
    ;
}";
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
