using System.Globalization;
using System.Text;

namespace MtpParallelAttributionMini;

/// <summary>
///     Renders the title page: the document title, a details table (customer, product,
///     author, signature and date) and an "Approval By" sign-off table.
/// </summary>
internal static class TitlePageBuilder
{
	public static void Write(StringBuilder sb, MiniMetadata metadata)
	{
		string title = MiniEscape.Escape(metadata.DocumentTitle);
		string customer = MiniEscape.Escape(metadata.Customer);
		string product = MiniEscape.Escape(metadata.Product);
		string createdBy = MiniEscape.Escape(metadata.CreatedBy);
		string createdDate = DateFormatting.ToBritish(metadata.CreatedDate);

		sb.AppendLine(@"\vspace*{2cm}");
		sb.AppendLine(CultureInfo.InvariantCulture, $@"\begin{{center}}{{\LARGE\bfseries {title}}}\end{{center}}");
		sb.AppendLine(@"\vspace{1.5cm}");

		sb.AppendLine(@"\noindent\begin{tabularx}{\textwidth}{|p{4cm}|X|}");
		sb.AppendLine(@"\hline");
		sb.AppendLine(CultureInfo.InvariantCulture, $@"Customer: & {customer} \\[6pt]");
		sb.AppendLine(@"\hline");
		sb.AppendLine(CultureInfo.InvariantCulture, $@"Product: & {product} \\[6pt]");
		sb.AppendLine(@"\hline");
		sb.AppendLine(CultureInfo.InvariantCulture, $@"Created by: & {createdBy} \\[6pt]");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"\end{tabularx}");

		sb.AppendLine(@"\vspace{0.4cm}");
		sb.AppendLine(@"\noindent\begin{tabularx}{\textwidth}{|p{4cm}|X|p{2cm}|p{3cm}|}");
		sb.AppendLine(@"\hline");
		sb.AppendLine(CultureInfo.InvariantCulture, $@"Signature: & & Date: & {createdDate} \\[1cm]");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"\end{tabularx}");

		WriteApprovalTable(sb, metadata);
	}

	private static void WriteApprovalTable(StringBuilder sb, MiniMetadata metadata)
	{
		sb.AppendLine(@"\vspace{1.5cm}");
		sb.AppendLine(@"\noindent\begin{tabularx}{\textwidth}{|X|X|X|X|}");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"\multicolumn{4}{|c|}{\textbf{Approval By}} \\");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"\textbf{Name} & \textbf{Job Title} & \textbf{Signature} & \textbf{Date} \\");
		sb.AppendLine(@"\hline");

		foreach (Approval approval in metadata.Approvals)
		{
			string name = MiniEscape.Escape(approval.Name);
			string jobTitle = MiniEscape.Escape(approval.JobTitle);
			sb.AppendLine(CultureInfo.InvariantCulture, $@"{name} & {jobTitle} & & \\[0.8cm]");
			sb.AppendLine(@"\hline");
		}

		sb.AppendLine(@"\end{tabularx}");
	}
}
