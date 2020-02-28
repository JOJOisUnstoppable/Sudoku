﻿using System;
using System.Collections.Generic;

namespace Sudoku.Data.Meta
{
	/// <summary>
	/// Provides a read-only sudoku grid.
	/// </summary>
	public interface IReadOnlyGrid : IEnumerable<short>, IEquatable<Grid>, IFormattable
	{
		/// <summary>
		/// Indicates the grid has already solved. If the value is <see langword="true"/>,
		/// the grid is solved; otherwise, <see langword="false"/>.
		/// </summary>
		bool HasSolved { get; }


		/// <summary>
		/// Gets or sets a digit into a cell.
		/// </summary>
		/// <param name="offset">The cell offset you want to get or set.</param>
		/// <value>
		/// The digit you want to set. This value should be between 0 and 8.
		/// In addition, if your input is -1, the candidate mask in this cell
		/// will be re-computed. If your input is none of them above, this indexer
		/// will do nothing.
		/// </value>
		/// <returns>
		/// An <see cref="int"/> value indicating the result.
		/// If the current cell does not have a digit
		/// (i.e. The cell is <see cref="CellStatus.Empty"/>),
		/// The value will be -1.
		/// </returns>
		int this[int offset] { get; }

		/// <summary>
		/// Gets or sets a candidate existence case with a <see cref="bool"/> value.
		/// </summary>
		/// <param name="offset">The cell offset between 0 and 80.</param>
		/// <param name="digit">The digit between 0 and 8.</param>
		/// <value>
		/// The case you want to set. <see langword="true"/> means that this candidate
		/// does not exist in this current sudoku grid; otherwise, <see langword="false"/>.
		/// </value>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		bool this[int offset, int digit] { get; }


		/// <inheritdoc/>
		bool Equals(object? obj);

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>The hash code.</returns>
		int GetHashCode();

		/// <summary>
		/// Serializes this instance to an array, where all digit value will be stored.
		/// </summary>
		/// <returns>
		/// This array. All elements are between 0 to 9, where 0 means the
		/// cell is <see cref="CellStatus.Empty"/> now.
		/// </returns>
		int[] ToArray();

		/// <summary>
		/// Get a mask of the specified cell.
		/// </summary>
		/// <param name="offset">The cell offset you want to get.</param>
		/// <returns>The mask.</returns>
		short GetMask(int offset);

		/// <summary>
		/// Get the candidate mask part of the specified cell.
		/// </summary>
		/// <param name="offset">The cell offset you want to get.</param>
		/// <returns>The candidate mask.</returns>
		short GetCandidates(int offset);

		/// <summary>
		/// Get the candidate mask after reversed all bits mask part
		/// of the specified cell.
		/// </summary>
		/// <param name="offset">The cell offset you want to get.</param>
		/// <returns>The candidate mask.</returns>
		short GetCandidatesReversal(int offset);

		/// <summary>
		/// Returns a string that represents the current object, with the
		/// specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns>The string result.</returns>
		string ToString(string format);

		/// <summary>
		/// Get a cell status of the specified cell.
		/// </summary>
		/// <param name="offset">The cell offset you want to get.</param>
		/// <returns>The cell status.</returns>
		CellStatus GetCellStatus(int offset);

		/// <summary>
		/// Creates a new instance that is a copy of the current instance.
		/// </summary>
		/// <returns>The cloneation.</returns>
		public Grid Clone() => ((Grid)this).Clone();
	}
}
