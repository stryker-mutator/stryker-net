using System.Collections.Generic;

namespace MtpParallelAttributionMini;

/// <summary>
///     Document-level information that does not appear in the test CSV but is needed to
///     build the title page, running header, document history and introduction.
/// </summary>
public sealed record MiniMetadata
{
	public string DocumentTitle { get; init; } = "Functional Test Scripts";
	public string ProjectNumber { get; init; } = string.Empty;
	public string SystemDescription { get; init; } = string.Empty;
	public string DocumentNumber { get; init; } = string.Empty;
	public string IssueNumber { get; init; } = string.Empty;
	public string Customer { get; init; } = string.Empty;
	public string Product { get; init; } = string.Empty;
	public string CreatedBy { get; init; } = string.Empty;
	public DateOnly CreatedDate { get; init; }
	public string Introduction { get; init; } = string.Empty;
	public IReadOnlyList<Approval> Approvals { get; init; } = [];
	public IReadOnlyList<DocumentHistoryEntry> DocumentHistory { get; init; } = [];
}

public sealed record Approval
{
	public string Name { get; init; } = string.Empty;
	public string JobTitle { get; init; } = string.Empty;
}

public sealed record DocumentHistoryEntry
{
	public DateOnly Date { get; init; }
	public string Initials { get; init; } = string.Empty;
	public string Version { get; init; } = string.Empty;
	public string Comment { get; init; } = string.Empty;
}
