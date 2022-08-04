﻿namespace Sudoku.Text;

/// <summary>
/// Provides a grid format option.
/// </summary>
[Flags]
public enum GridFormattingOptions : short
{
	/// <summary>
	/// Indicates the default settings (a single-line string text).
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the output should be with modifiable values.
	/// </summary>
	WithModifiers = 1,

	/// <summary>
	/// <para>
	/// Indicates the output should be with candidates.
	/// If the output is single line, the candidates will indicate
	/// the candidates-have-eliminated before the current grid status;
	/// if the output is multi-line, the candidates will indicate
	/// the real candidate at the current grid status.
	/// </para>
	/// <para>
	/// If the output is single line, the output will append the candidates
	/// value at the tail of the string in '<c>:candidate list</c>'. In addition,
	/// candidates will be represented as 'digit', 'row offset' and
	/// 'column offset' in order.
	/// </para>
	/// </summary>
	WithCandidates = 1 << 1,

	/// <summary>
	/// Indicates the output will treat modifiable values as given ones.
	/// If the output is single line, the output will remove all plus marks '+'.
	/// If the output is multi-line, the output will use '<c><![CDATA[<digit>]]></c>'
	/// instead of '<c>*digit*</c>'.
	/// </summary>
	TreatValueAsGiven = 1 << 2,

	/// <summary>
	/// Indicates whether need to handle all grid outlines while outputting.
	/// Visit the page
	/// <i>
	/// <see href="https://sunnieshine.github.io/Sudoku/types/structs/How-To-Use-Struct-SudokuGrid">
	/// How to use the structure SudokuGrid
	/// </see>
	/// </i>
	/// for more information.
	/// </summary>
	SubtleGridLines = 1 << 3,

	/// <summary>
	/// Indicates whether the output will be compatible with Hodoku library format.
	/// </summary>
	HodokuCompatible = 1 << 4,

	/// <summary>
	/// Indicates the placeholder must be '.' instead of '0'.
	/// If the value is <see langword="true"/>, the placeholder will be '.';
	/// otherwise, '0'.
	/// </summary>
	DotPlaceholder = 1 << 5,

	/// <summary>
	/// Indicates the output should be multi-line.
	/// </summary>
	Multiline = 1 << 6,

	/// <summary>
	/// Indicates the output will be sukaku format.
	/// </summary>
	Sukaku = 1 << 7,

	/// <summary>
	/// Indicates the output will be Excel format.
	/// </summary>
	Excel = 1 << 8,

	/// <summary>
	/// Indicates the open sudoku format.
	/// </summary>
	/// <remarks>
	/// This format will use a triplet to describe a cell. If the cell has already been filled with
	/// a value, it will be output at the first digit of that triplet. For example,
	/// the triplet <c>900</c> means the cell is filled with the digit 9. In addition,
	/// if the cell is empty, the triplet is always <c>001</c>.
	/// </remarks>
	OpenSudoku = 1 << 9,

	/// <summary>
	/// Indicates the shorten susser format.
	/// </summary>
	Shorten = 1 << 10,
}
