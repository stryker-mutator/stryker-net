using System.Globalization;
using System.Text;

namespace MtpParallelAttributionMini;

/// <summary>
///     Renders the document-history table (date, initials, version, comment).
/// </summary>
internal static class MiniTableBuilder
{
	public static void Write(StringBuilder sb, MiniMetadata metadata)
	{
		sb.AppendLine(@"\vspace*{1cm}");
		sb.AppendLine(@"\noindent\begin{tabularx}{\textwidth}{|p{2.5cm}|p{2cm}|p{2cm}|X|}");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"\multicolumn{4}{|c|}{\textbf{Document History}} \\");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"\textbf{Date} & \textbf{Initials} & \textbf{Version} & \textbf{Comment} \\");
		sb.AppendLine(@"\hline");

		foreach (DocumentHistoryEntry entry in metadata.DocumentHistory)
		{
			string date = DateFormatting.ToBritish(entry.Date);
			string initials = MiniEscape.Escape(entry.Initials);
			string version = MiniEscape.Escape(entry.Version);
			string comment = MiniEscape.Escape(entry.Comment);
			sb.AppendLine(CultureInfo.InvariantCulture, $@"{date} & {initials} & {version} & {comment} \\[4pt]");
			sb.AppendLine(@"\hline");
		}

		sb.AppendLine(@"\end{tabularx}");
	}
}
