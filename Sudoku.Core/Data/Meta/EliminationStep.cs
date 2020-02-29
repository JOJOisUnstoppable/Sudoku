﻿namespace Sudoku.Data.Meta
{
	/// <summary>
	/// Encapsulates an elimination step.
	/// </summary>
	public sealed class EliminationStep : Step
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <param name="cell">The cell.</param>
		public EliminationStep(int digit, int cell) => (Digit, Cell) = (digit, cell);


		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit { get; }

		/// <summary>
		/// Indicates the cell.
		/// </summary>
		public int Cell { get; }


		/// <inheritdoc/>
		public override void UndoStepTo(UndoableGrid grid) => grid[Cell, Digit] = false;

		/// <inheritdoc/>
		public override void DoStepTo(UndoableGrid grid) => grid[Cell, Digit] = true;
	}
}
