using Xunit;

namespace MtpParallelAttributionMini.Tests;

public sealed class MiniEscapeTests
{
	[Theory]
	[InlineData("&", @"\&")]
	[InlineData("%", @"\%")]
	[InlineData("$", @"\$")]
	[InlineData("#", @"\#")]
	[InlineData("_", @"\_")]
	[InlineData("{", @"\{")]
	[InlineData("}", @"\}")]
	[InlineData("~", @"\textasciitilde{}")]
	[InlineData("^", @"\textasciicircum{}")]
	[InlineData(@"\", @"\textbackslash{}")]
	public void Escape_SpecialCharacter_IsEscaped(string input, string expected)
	{
		Assert.Equal(expected, MiniEscape.Escape(input));
	}

	[Fact]
	public void Escape_PlainText_IsUnchanged()
	{
		Assert.Equal("Acme and Partners", MiniEscape.Escape("Acme and Partners"));
	}

	[Fact]
	public void Escape_MixedText_EscapesOnlySpecialCharacters()
	{
		Assert.Equal(@"A\&P 50\% off", MiniEscape.Escape("A&P 50% off"));
	}
}
