﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Constants;

namespace Sudoku.Diagnostics
{
	/// <summary>
	/// Encapsulates a result after <see cref="FileCounter"/>.
	/// </summary>
	/// <param name="ResultLines">The number of lines found.</param>
	/// <param name="CommentLines">The number of comment lines found.</param>
	/// <param name="FilesCount">The number of files found.</param>
	/// <param name="CharactersCount">The number of characters found.</param>
	/// <param name="Bytes">All bytes.</param>
	/// <param name="Elapsed">The elapsed time during searching.</param>
	/// <param name="FileList">
	/// The list of files. This property won't be output. If you want to use this property,
	/// please write this property explicitly.
	/// </param>
	/// <seealso cref="FileCounter"/>
	public sealed record FileCounterResult(
		int ResultLines, int CommentLines, int FilesCount, long CharactersCount, long Bytes,
		TimeSpan Elapsed, IList<string> FileList)
	{
		/// <inheritdoc/>
		public override string ToString() =>
			$"Results:\n" +
			$"* Code lines: {ResultLines} (Comment lines: {CommentLines})\n" +
			$"* Files: {FilesCount}\n" +
			$"* Characters: {CharactersCount}\n" +
			$"* Bytes: {SizeUnitConverter.Convert(Bytes, out var unit):.000} {Tostring(unit)} ({Bytes} Bytes)\n" +
			$"* Time elapsed: {Elapsed:hh\\:mm\\.ss\\.fff}\n" +
			"\n" +
			"About more information, please call each property in this instance.";


		/// <summary>
		/// Gets the specified string notation for a <see cref="SizeUnit"/> instance.
		/// </summary>
		/// <param name="this">The instance.</param>
		/// <returns>The string result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string Tostring(SizeUnit @this) =>
			@this switch
			{
				SizeUnit.Byte => "B",
				SizeUnit.Kilobyte => "KB",
				SizeUnit.Megabyte => "MB",
				SizeUnit.Gigabyte => "GB",
				SizeUnit.Terabyte => "TB",
				_ => throw Throwings.ImpossibleCase
			};
	}
}
