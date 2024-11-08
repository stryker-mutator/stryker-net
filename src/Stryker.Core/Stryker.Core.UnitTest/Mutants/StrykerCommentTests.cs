using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Mutants;

namespace Stryker.Core.UnitTest.Mutants;
internal class StrykerCommentTests : MutantOrchestratorTestsBase
{
        [TestMethod]
    public void ShouldNotMutateIfDisabledByComment()
    {
        var source = @"public void SomeMethod() {
	var x = 0;
// Stryker disable all
	x++;
	x/=2;
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
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

    [TestMethod]
    public void ShouldNotMutateIfDisabledByMultilineComment()
    {
        var source = @"public void SomeMethod() {
	var x = 0;
    if (condition && other) /* Stryker disable once all */ {  
	  x++;
      x*=2;
	}
	x/=2;
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
    if ((StrykerNamespace.MutantControl.IsActive(2)?!(condition && other):(StrykerNamespace.MutantControl.IsActive(1)?condition || other:condition && other))) /* Stryker disable once all */ {  
	  x++;
      x*=2;
	}
if(StrykerNamespace.MutantControl.IsActive(7)){	x*=2;
}else{	x/=2;
}}}";
        ;

        ShouldMutateSourceInClassToExpected(source, expected);

        source = @"public void SomeMethod() {
	var x = 0;
	{ /* Stryker disable all */
	  x++;
	}
	x/=2;
}";
        expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(8)){}else{
	var x = 0;
{if(StrykerNamespace.MutantControl.IsActive(9)){}else{ /* Stryker disable all */
	  x++;
	}
}if(StrykerNamespace.MutantControl.IsActive(12)){	x*=2;
}else{	x/=2;
}}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    [DataRow("/* Stryker disable once all*/ x++;", "/* Stryker disable once all*/ x++;")]
    [DataRow("if (cond) /* Stryker disable once all*/ x++;", "if ((StrykerNamespace.MutantControl.IsActive(1)?!(cond):cond)) /* Stryker disable once all*/ x++;")]
    [DataRow("if (cond);/* Stryker disable once all*/ else x++;", "if ((StrykerNamespace.MutantControl.IsActive(1)?!(cond):cond));/* Stryker disable once all*/ else x++;")]
    [DataRow(@"if (cond) // Stryker disable once all
x++;",
        @"if ((StrykerNamespace.MutantControl.IsActive(1)?!(cond):cond)) // Stryker disable once all
x++;")]
    [DataRow("if (/* Stryker disable once all*/cond) x++;", "if (/* Stryker disable once all*/cond) if(StrykerNamespace.MutantControl.IsActive(2)){;}else{if(StrykerNamespace.MutantControl.IsActive(3)){x--;}else{x++;}}")]
    
    public void ShouldNotMutateDependingOnWhereMultilineCommentIs(string source, string expected)
    {
        // must call reset as MsTest reuse test instance on/datarow
        var sourceTemplate = $@"public void SomeMethod() {{
	var x = 0;
	{source}
}}";
        var expectedTemplate = $@"public void SomeMethod() {{if(StrykerNamespace.MutantControl.IsActive(0)){{}}else{{
	var x = 0;
{expected}
 }}}}";

        ShouldMutateSourceInClassToExpected(sourceTemplate, expectedTemplate);
    }

    [TestMethod]
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

    [TestMethod]
    public void ShouldMutateIfInvalidComment()
    {
        var source = @"public int SomeMethod()// Stryker disabel all
{x++;}";
        var expected = @"public int SomeMethod()// Stryker disabel all
{if(StrykerNamespace.MutantControl.IsActive(0)){}else{if(StrykerNamespace.MutantControl.IsActive(1)){;}else{if(StrykerNamespace.MutantControl.IsActive(2)){x--;}else{x++;}}}}";

        ShouldMutateSourceInClassToExpected(source, expected);

        source = @"public int SomeMethod()// Stryker disable maths
{x++;}";
        expected = @"public int SomeMethod()// Stryker disable maths
{if(StrykerNamespace.MutantControl.IsActive(3)){}else{if(StrykerNamespace.MutantControl.IsActive(5)){x--;}else{x++;}}}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldOnlyUseFirstComment()
    {
        // enabling Stryker comment should have no impact
        var source = @"public void SomeMethod() {
    /* Stryker disable once all */
    // Stryker restore all
        x++;
    }";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
    /* Stryker disable once all */
    // Stryker restore all
        x++;
    }}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldNotMutateIfDisabledByCommentAtMethodLevel()
    {
        var source = @"
// Stryker disable all : testing
public void SomeMethod()
 {
	var x = 0;
	x++;
	x/=2;
}";
        var expected = @"// Stryker disable all : testing
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

    [TestMethod]
    public void ShouldNotMutateOneLineIfDisabledByComment()
    {
        var source = @"public void SomeMethod() {
	var x = 0;
// Stryker disable once all
	x++;
	x/=2;
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
// Stryker disable once all
	x++;
if(StrykerNamespace.MutantControl.IsActive(3)){	x*=2;
}else{	x/=2;
}}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

    [TestMethod]
    public void ShouldNotMutateOneLineIfDisabledAndEnabledByComment()
    {
        var source = @"public void SomeMethod() {
	var x = 0;
// Stryker disable all
	x++;
// Stryker restore all
	x/=2;
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
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

    [TestMethod]
    public void ShouldNotMutateOneLineIfDisabledOnce()
    {
        var source = @"public void SomeMethod() {
	var x = 0;
// Stryker disable once all
	x++;
	x/=2;
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
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

    [TestMethod]
    public void ShouldNotMutateOneLineIfSpecificMutatorDisabled()
    {
        var source = @"public void SomeMethod() {
	var x = 0;
// Stryker disable Update , Statement : comment
	x++;
// Stryker restore all
	x/=2;
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
// Stryker disable Update , Statement : comment
	x++;
if(StrykerNamespace.MutantControl.IsActive(3)){// Stryker restore all
	x*=2;
}else{// Stryker restore all
	x/=2;
}}}";

        ShouldMutateSourceInClassToExpected(source, expected);

        Target.Mutants.Count.ShouldBe(4);
        Target.Mutants.ElementAt(0).ResultStatus.ShouldBe(MutantStatus.Pending);
        Target.Mutants.ElementAt(1).ResultStatus.ShouldBe(MutantStatus.Ignored);
        Target.Mutants.ElementAt(1).ResultStatusReason.ShouldBe("comment");
        Target.Mutants.ElementAt(2).ResultStatus.ShouldBe(MutantStatus.Ignored);
        Target.Mutants.ElementAt(2).ResultStatusReason.ShouldBe("comment");
        Target.Mutants.ElementAt(3).ResultStatus.ShouldBe(MutantStatus.Pending);
    }

    [TestMethod]
    public void ShouldNotMutateASubExpressionIfDisabledByComment()
    {
        var source = @"public void SomeMethod() {
	var x = 0;
	x/=
// Stryker disable once all
x +2;
}";
        var expected = @"public void SomeMethod() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
	var x = 0;
if(StrykerNamespace.MutantControl.IsActive(1)){	x*=// Stryker disable once all
x +2;
}else{	x/=
// Stryker disable once all
x +2;
}}}";

        ShouldMutateSourceInClassToExpected(source, expected);
    }

}
