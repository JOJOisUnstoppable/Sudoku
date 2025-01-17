﻿namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with a <b>Single</b> step searcher. The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Full House (If the property <see cref="EnableFullHouse"/> is <see langword="true"/>)</item>
/// <item>Last Digit (If the property <see cref="EnableLastDigit"/> is <see langword="true"/>)</item>
/// <item>Hidden Single</item>
/// <item>Naked Single</item>
/// </list>
/// </summary>
public interface ISingleStepSearcher : IStepSearcher
{
	/// <summary>
	/// Indicates whether the solver enables the technique full house.
	/// </summary>
	bool EnableFullHouse { get; set; }

	/// <summary>
	/// Indicates whether the solver enables the technique last digit.
	/// </summary>
	bool EnableLastDigit { get; set; }

	/// <summary>
	/// Indicates whether the solver checks for hidden single in block firstly.
	/// </summary>
	bool HiddenSinglesInBlockFirst { get; set; }

	/// <summary>
	/// Indicates whether the solver uses ittoryu mode to solve a puzzle.
	/// </summary>
	/// <remarks>
	/// For more information about what is an ittoryu puzzle, please visit
	/// <see href="https://sunnieshine.github.io/Sudoku/terms/ittouryu-puzzle">this link</see>.
	/// </remarks>
	bool UseIttoryuMode { get; set; }
}
