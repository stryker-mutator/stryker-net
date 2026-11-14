using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MtpParallelAttributionMini;

/// <summary>
///     Renders a single test: the header table (source / title / aim / pre-requisites),
///     the multipage steps table, a comments box and the signature table. Also registers a contents entry.
/// </summary>
internal static class MiniPageBuilder
{
	public static void Write(StringBuilder sb, TestRow row, int number)
	{
		string title = MiniEscape.Escape(row.Title);
		string aim = MiniEscape.Escape(row.Aim);
		string prereqs = MiniEscape.Escape(row.PreRequisites);

		sb.AppendLine(@"\phantomsection");
		sb.AppendLine(CultureInfo.InvariantCulture, $@"\addcontentsline{{toc}}{{section}}{{{number} -- {title}}}");

		WriteHeaderTable(sb, number, title, aim, prereqs);

		if (row.Layout is TestTableLayout.PermissionsMatrix)
			WritePermissionsMatrixTable(sb, row);
		else
			WriteStepsTable(sb, row);

		WriteCommentsBox(sb);
		WriteSignatureTable(sb);
		sb.AppendLine();
	}

	private static void WriteHeaderTable(StringBuilder sb, int number, string title, string aim, string prereqs)
	{
		sb.AppendLine(@"\noindent\begin{tabularx}{\textwidth}{|X|X|}");
		sb.AppendLine(@"\hline");
		sb.AppendLine(CultureInfo.InvariantCulture, $@"\textbf{{Test Source:}} \newline URS & Test Number / Test Title: \newline {{\Large\color{{blue}} {number} -- {title}}} \\[10pt]");
		sb.AppendLine(@"\hline");
		sb.AppendLine(CultureInfo.InvariantCulture, $@"\multicolumn{{2}}{{|>{{\hsize=2\hsize\linewidth=\hsize}}X|}}{{\textbf{{Aim / Objective Of Test:}} \newline {aim}}} \\[4pt]");
		sb.AppendLine(@"\hline");
		sb.AppendLine(CultureInfo.InvariantCulture, $@"\multicolumn{{2}}{{|>{{\hsize=2\hsize\linewidth=\hsize}}X|}}{{\textbf{{Pre-Requisites:}} \newline {prereqs}}} \\[4pt]");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"\end{tabularx}");
	}

	private static void WriteStepsTable(StringBuilder sb, TestRow row) =>
		WriteLongtable(
			sb,
			row,
			@"|p{0.38\textwidth}|p{0.38\textwidth}|p{2.2cm}|",
			@"\textbf{Test Method} & \textbf{Expected Result} & \textbf{Pass / Fail} \newline {\footnotesize(cross out)} \\",
			columnCount: 3,
			(escapedStep, escapedResult) =>
				$@"{escapedStep} \newline {escapedResult} & Inspection complies with the requirement (visual or analysis) & Pass / Fail \\");

	private static void WritePermissionsMatrixTable(StringBuilder sb, TestRow row) =>
		WriteLongtable(sb, row, @"|p{0.35\textwidth}|p{0.55\textwidth}|", @"\textbf{Role} & \textbf{Permissions} \\", columnCount: 2, (escapedStep, escapedResult) => $@"{escapedStep} & {escapedResult} \\");

	// Shared longtable scaffold (repeated header for page breaks, "Continued on next page" footer) for both
	// the Default steps table and the PermissionsMatrix table, which differ only in column layout, header text,
	// and per-row formatting.
	private static void WriteLongtable(StringBuilder sb, TestRow row, string columnSpec, string headerRow, int columnCount, Func<string, string, string> formatRow)
	{
		sb.AppendLine(@"\vspace{1em}");
		sb.AppendLine(CultureInfo.InvariantCulture, $@"\begin{{longtable}}{{{columnSpec}}}");
		sb.AppendLine(@"\hline");
		sb.AppendLine(headerRow);
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"\endfirsthead");
		sb.AppendLine(@"\hline");
		sb.AppendLine(headerRow);
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"\endhead");
		sb.AppendLine(@"\hline");
		sb.AppendLine(CultureInfo.InvariantCulture, $@"\multicolumn{{{columnCount}}}{{r}}{{\footnotesize Continued on next page}} \\");
		sb.AppendLine(@"\endfoot");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"\endlastfoot");

		foreach ((string step, string result) in GetSteps(row))
		{
			string escapedStep = MiniEscape.Escape(step);
			string escapedResult = MiniEscape.Escape(result);
			sb.AppendLine(formatRow(escapedStep, escapedResult));
			sb.AppendLine(@"\hline");
		}

		sb.AppendLine(@"\end{longtable}");
	}

	private static void WriteCommentsBox(StringBuilder sb)
	{
		sb.AppendLine(@"\vspace{0.5em}");
		sb.AppendLine(@"\noindent\begin{tabularx}{\textwidth}{|X|}");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"Comments: \\[5cm]");
		sb.AppendLine(@"{\footnotesize\textit{Cross through if no comments are made. Initial and date all comments made.}} \\");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"\end{tabularx}");
	}

	private static void WriteSignatureTable(StringBuilder sb)
	{
		sb.AppendLine(@"\vspace{0.5em}");
		sb.AppendLine(@"\noindent\begin{tabularx}{\textwidth}{|X|X|X|}");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"Test Performed By (Print):- & Signed & Date \\[2cm]");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"Test Witnessed By (Print) & Signed & Date \\[2cm]");
		sb.AppendLine(@"\hline");
		sb.AppendLine(@"\end{tabularx}");
	}

	private static IEnumerable<(string Step, string Result)> GetSteps(TestRow row) =>
		new (string Step, string Result)[]
		{
			(row.TestStep1, row.Result1),
			(row.TestStep2, row.Result2),
			(row.TestStep3, row.Result3),
			(row.TestStep4, row.Result4),
			(row.TestStep5, row.Result5),
			(row.TestStep6, row.Result6),
			(row.TestStep7, row.Result7),
			(row.TestStep8, row.Result8),
			(row.TestStep9, row.Result9),
			(row.TestStep10, row.Result10),
			(row.TestStep11, row.Result11),
			(row.TestStep12, row.Result12),
			(row.TestStep13, row.Result13),
			(row.TestStep14, row.Result14),
			(row.TestStep15, row.Result15),
			(row.TestStep16, row.Result16),
			(row.TestStep17, row.Result17),
			(row.TestStep18, row.Result18),
			(row.TestStep19, row.Result19),
			(row.TestStep20, row.Result20)
		}.Where(s => !string.IsNullOrWhiteSpace(s.Step));
}
