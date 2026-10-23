using System.Globalization;
using System.Text;

namespace MtpParallelAttributionMini;

/// <summary>
///     Writes the LaTeX preamble: document class, packages, page geometry and the running
///     header (logo, document title, system description, doc/issue numbers and page count)
///     repeated on every page via <c>fancyhdr</c>.
/// </summary>
internal static class Preamble
{
	public static void Write(StringBuilder sb, MiniMetadata metadata, string logoFileName)
	{
		sb.AppendLine(@"\documentclass[a4paper]{article}");
		sb.AppendLine(@"\usepackage[hmargin=2cm,top=3.4cm,bottom=2cm,headheight=2.4cm,headsep=0.4cm]{geometry}");
		sb.AppendLine(@"\usepackage{tabularx,longtable,array,xcolor,parskip,graphicx,multirow,fancyhdr,lastpage}");
		sb.AppendLine(@"\usepackage[hidelinks]{hyperref}");
		WriteHeader(sb, metadata, logoFileName);
		sb.AppendLine();
	}

	private static void WriteHeader(StringBuilder sb, MiniMetadata metadata, string logoFileName)
	{
		string documentTitle = MiniEscape.Escape(metadata.DocumentTitle);
		string system = MiniEscape.Escape(metadata.SystemDescription);
		string projectNumber = MiniEscape.Escape(metadata.ProjectNumber);
		string documentNumber = MiniEscape.Escape(metadata.DocumentNumber);
		string issueNumber = MiniEscape.Escape(metadata.IssueNumber);

		sb.AppendLine(@"\pagestyle{fancy}");
		sb.AppendLine(@"\fancyhf{}");
		sb.AppendLine(@"\renewcommand{\headrulewidth}{0pt}");
		sb.AppendLine(@"\setlength{\extrarowheight}{2pt}");
		sb.AppendLine(@"\fancyhead[C]{%");
		sb.AppendLine(@"\small\begin{tabularx}{\textwidth}{|c|>{\centering\arraybackslash}X|l|l|}");
		sb.AppendLine(@"\hline");
		sb.AppendLine(CultureInfo.InvariantCulture, $@"\multirow{{2}}{{*}}{{\includegraphics[height=0.8cm]{{{logoFileName}}}}} & \textbf{{{documentTitle}}} & Doc No & {documentNumber} \\");
		sb.AppendLine(@"\cline{2-4}");
		sb.AppendLine(CultureInfo.InvariantCulture, $@" & {system} & Issue No. & {issueNumber} \\");
		sb.AppendLine(@"\hline");
		sb.AppendLine(CultureInfo.InvariantCulture, $@"Project No: & {projectNumber} & Page & \thepage\ of \pageref{{LastPage}} \\");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"\end{tabularx}}");
	}
}
