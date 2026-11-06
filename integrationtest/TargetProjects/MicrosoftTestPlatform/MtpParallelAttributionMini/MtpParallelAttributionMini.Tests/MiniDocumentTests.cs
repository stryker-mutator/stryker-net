using System;
using System.Collections.Generic;
using Xunit;

namespace MtpParallelAttributionMini.Tests;

public sealed class MiniDocumentTests
{
	[Fact]
	public void Build_IncludesHeaderMetadataFields()
	{
		string tex = Build();

		Assert.Contains("Functional Test Scripts", tex);
		Assert.Contains("12345 REP", tex);
		Assert.Contains(@"\thepage\ of \pageref{LastPage}", tex);
		Assert.Contains("logo.png", tex);
	}

	[Fact]
	public void Build_IncludesTitlePageValues()
	{
		string tex = Build();

		Assert.Contains("A N Other", tex);
		Assert.Contains("Approval By", tex);
		Assert.Contains("19/03/2024", tex);
	}

	[Fact]
	public void Build_EscapesSpecialCharactersInMetadata()
	{
		string tex = Build();

		Assert.Contains(@"Acme \& Partners", tex);
		Assert.DoesNotContain("Acme & Partners", tex);
	}

	[Fact]
	public void Build_AddsContentsEntryForIntroductionAndEachTest()
	{
		List<TestRow> rows = [MakeRow("Test one"), MakeRow("Test two")];

		string tex = MiniReport.Build(SampleMetadata(), rows, "logo.png");

		Assert.Equal(rows.Count + 1, CountOccurrences(tex, @"\addcontentsline"));
		Assert.Contains(@"\addcontentsline{toc}{section}{Introduction}", tex);
		Assert.Contains(@"\addcontentsline{toc}{section}{1 -- Test one}", tex);
		Assert.Contains(@"\tableofcontents", tex);
	}

	[Fact]
	public void Build_OmitsDocumentHistory_WhenNoEntries()
	{
		MiniMetadata metadata = SampleMetadata() with { DocumentHistory = [] };

		string tex = MiniReport.Build(metadata, [MakeRow("Only test")], "logo.png");

		Assert.DoesNotContain("Document History", tex);
	}

	[Fact]
	public void Build_IncludesStep20()
	{
		TestRow row = MakeRow("Long test");
		row.TestStep20 = "Perform the final check";
		row.Result20 = "The final check succeeds";

		string tex = MiniReport.Build(SampleMetadata(), [row], "logo.png");

		Assert.Contains(@"Perform the final check \newline The final check succeeds", tex);
	}

	[Fact]
	public void Build_OmitsBlankNumberedSteps()
	{
		TestRow row = MakeRow("Test with a gap");
		row.TestStep2 = " ";
		row.Result2 = "This result must not appear";
		row.TestStep3 = "Perform the third check";
		row.Result3 = "The third check succeeds";

		string tex = MiniReport.Build(SampleMetadata(), [row], "logo.png");

		Assert.DoesNotContain("This result must not appear", tex);
		Assert.Contains("Perform the third check", tex);
	}

	[Fact]
	public void Build_UsesMultipageStepsTableWithContinuationHeading()
	{
		string tex = Build();

		Assert.Contains(@"\begin{longtable}", tex);
		Assert.Contains(@"\endfirsthead", tex);
		Assert.Contains(@"\endhead", tex);
		Assert.Contains("Continued on next page", tex);
		Assert.Equal(2, CountOccurrences(tex, @"\textbf{Test Method}"));
	}

	[Fact]
	public void Build_UsesRolePermissionsTable_ForPermissionsMatrixLayout()
	{
		TestRow row = MakeRow("Permissions Matrix");
		row.Layout = TestTableLayout.PermissionsMatrix;
		row.TestStep1 = "Administrators";
		row.Result1 = "All permissions";

		string tex = MiniReport.Build(SampleMetadata(), [row], "logo.png");

		Assert.Contains(@"\textbf{Role} & \textbf{Permissions} \\", tex);
		Assert.Contains(@"Administrators & All permissions \\", tex);
		Assert.DoesNotContain(@"\textbf{Test Method}", tex);
		Assert.DoesNotContain("Inspection complies with the requirement", tex);
		Assert.DoesNotContain("Pass / Fail", tex);
	}

	[Fact]
	public void Build_UsesMultipageRolePermissionsTableWithContinuationHeading()
	{
		TestRow row = MakeRow("Permissions Matrix");
		row.Layout = TestTableLayout.PermissionsMatrix;
		row.TestStep1 = "Administrators";
		row.Result1 = "All permissions";

		string tex = MiniReport.Build(SampleMetadata(), [row], "logo.png");

		Assert.Contains(@"\begin{longtable}", tex);
		Assert.Contains(@"\endfirsthead", tex);
		Assert.Contains(@"\endhead", tex);
		Assert.Contains("Continued on next page", tex);
		Assert.Equal(2, CountOccurrences(tex, @"\textbf{Role}"));
	}

	private static string Build() => MiniReport.Build(SampleMetadata(), [MakeRow("Sign in test")], "logo.png");

	private static MiniMetadata SampleMetadata() => new()
	{
		DocumentTitle = "Functional Test Scripts",
		ProjectNumber = "12345",
		SystemDescription = "Widget Print and Verification System",
		DocumentNumber = "12345 REP",
		IssueNumber = "1.0",
		Customer = "Acme & Partners",
		Product = "Widget Print and Verification System",
		CreatedBy = "A N Other",
		CreatedDate = new DateOnly(2024, 3, 19),
		Introduction = "Intro paragraph one.\n\nIntro paragraph two.",
		Approvals = [new Approval { Name = "J Bloggs", JobTitle = "Operations Director" }],
		DocumentHistory = [new DocumentHistoryEntry { Date = new DateOnly(2023, 8, 10), Initials = "ANO", Version = "1.0", Comment = "Initial Version" }]
	};

	private static TestRow MakeRow(string title) => new()
	{
		Title = title,
		Aim = "An aim",
		PreRequisites = "A pre-requisite",
		TestStep1 = "Do the thing",
		Result1 = "The thing happens"
	};

	private static int CountOccurrences(string text, string token)
	{
		int count = 0;
		int index = text.IndexOf(token, StringComparison.Ordinal);
		while (index >= 0)
		{
			count++;
			index = text.IndexOf(token, index + token.Length, StringComparison.Ordinal);
		}

		return count;
	}
}
