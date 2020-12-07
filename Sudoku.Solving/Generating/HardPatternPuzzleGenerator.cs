﻿using System;
using System.Extensions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sudoku.Data;
using Sudoku.Globalization;
using Sudoku.Models;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual;
using static System.Algorithms;

namespace Sudoku.Solving.Generating
{
	/// <summary>
	/// Provides an extended puzzle generator.
	/// </summary>
	public class HardPatternPuzzleGenerator : DiggingPuzzleGenerator
	{
		/// <summary>
		/// The block factor.
		/// </summary>
		private static readonly int[] BlockFactor = { 0, 6, 54, 60, 3, 27, 33, 57, 30 };

		/// <summary>
		/// Indicates the swapping factor.
		/// </summary>
		private static readonly int[,] SwappingFactor = { { 0, 1, 2 }, { 0, 2, 1 }, { 1, 0, 2 }, { 1, 2, 0 }, { 2, 0, 1 }, { 2, 1, 0 } };

		/// <summary>
		/// The backdoor searcher.
		/// </summary>
		private static readonly BackdoorSearcher BackdoorSearcher = new();


		/// <inheritdoc/>
		public override SudokuGrid Generate() => Generate(-1, null);

		/// <summary>
		/// To generate a sudoku grid with a backdoor filter depth.
		/// </summary>
		/// <param name="backdoorFilterDepth">
		/// The backdoor filter depth. When the value is -1, the generator won't check
		/// any backdoors.
		/// </param>
		/// <param name="progress">The progress.</param>
		/// <param name="difficultyLevel">The difficulty level.</param>
		/// <param name="countryCode">The country code.</param>
		/// <returns>The grid.</returns>
		public SudokuGrid Generate(
			int backdoorFilterDepth, IProgress<IProgressResult>? progress,
			DifficultyLevel difficultyLevel = DifficultyLevel.Unknown,
			CountryCode countryCode = CountryCode.Default)
		{
			PuzzleGeneratingProgressResult
				defaultValue = default,
				pr = new(0, countryCode == CountryCode.Default ? CountryCode.EnUs : countryCode);
			ref var progressResult = ref progress is null ? ref defaultValue : ref pr;
			progress?.Report(defaultValue);

			var puzzle = new StringBuilder() { Length = 81 };
			var solution = new StringBuilder() { Length = 81 };
			var emptyGridStr = new StringBuilder(SudokuGrid.EmptyString);
			static string valueOf(StringBuilder solution) => solution.ToString();

			while (true)
			{
				emptyGridStr.CopyTo(puzzle);
				emptyGridStr.CopyTo(solution);
				GenerateAnswerGrid(puzzle, solution);

				int[] holeCells = new int[81];
				CreatePattern(holeCells);
				for (int trial = 0; trial < 1000; trial++)
				{
					for (int cell = 0; cell < 81; cell++)
					{
						int p = holeCells[cell];
						char temp = solution[p];
						solution[p] = '0';

						if (!FastSolver.CheckValidity(valueOf(solution)))
						{
							// Reset the value.
							solution[p] = temp;
						}
					}

					if (progress is not null)
					{
						progressResult.GeneratingTrial++;
						progress.Report(progressResult);
					}

					if (FastSolver.CheckValidity(valueOf(solution)))
					{
						var grid = SudokuGrid.Parse(valueOf(solution));
						if ((
							backdoorFilterDepth != -1
							&& BackdoorSearcher.SearchForBackdoors(grid, backdoorFilterDepth).None()
							|| backdoorFilterDepth == -1)
							&& (
							difficultyLevel != DifficultyLevel.Unknown
							&& grid.GetDifficultyLevel() == difficultyLevel
							|| difficultyLevel == DifficultyLevel.Unknown))
						{
							return grid;
						}
					}

					RecreatePattern(holeCells);
				}
			}
		}

		/// <summary>
		/// To generate a sudoku grid with a backdoor filter depth asynchronizedly.
		/// </summary>
		/// <param name="backdoorFilterDepth">
		/// The backdoor filter depth. When the value is -1, the generator won't check
		/// any backdoors.
		/// </param>
		/// <param name="progress">The progress.</param>
		/// <param name="difficultyLevel">The difficulty level.</param>
		/// <param name="countryCode">The country code.</param>
		/// <returns>The task.</returns>
		public async Task<SudokuGrid> GenerateAsync(
			int backdoorFilterDepth, IProgress<IProgressResult>? progress,
			DifficultyLevel difficultyLevel = DifficultyLevel.Unknown,
			CountryCode countryCode = CountryCode.Default) =>
			await Task.Run(() => Generate(backdoorFilterDepth, progress, difficultyLevel, countryCode));

		/// <inheritdoc/>
		protected sealed override void CreatePattern(int[] pattern)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static double rnd() => IPuzzleGenerator.Rng.NextDouble();

			int a = 54, b = 0;
			for (int i = 0; i < 9; i++)
			{
				int n = (int)(rnd() * 6);
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 3; k++)
					{
						pattern[(k == SwappingFactor[n, j] ? ref a : ref b)++] = BlockFactor[i] + j * 9 + k;
					}
				}
			}

			for (int i = 23; i >= 0; i--)
			{
				Swap(ref pattern[i], ref pattern[(int)((i + 1) * rnd())]);
			}
			for (int i = 47; i >= 24; i--)
			{
				Swap(ref pattern[i], ref pattern[24 + (int)((i - 23) * rnd())]);
			}
			for (int i = 53; i >= 48; i--)
			{
				Swap(ref pattern[i], ref pattern[48 + (int)((i - 47) * rnd())]);
			}
			for (int i = 80; i >= 54; i--)
			{
				Swap(ref pattern[i], ref pattern[54 + (int)(27 * rnd())]);
			}
		}


		/// <summary>
		/// To re-create the pattern.
		/// </summary>
		/// <param name="pattern">The pattern array.</param>
		private static void RecreatePattern(int[] pattern)
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static double rnd() => IPuzzleGenerator.Rng.NextDouble();

			for (int i = 23; i >= 0; i--)
			{
				Swap(ref pattern[i], ref pattern[(int)((i + 1) * rnd())]);
			}
			for (int i = 47; i >= 24; i--)
			{
				Swap(ref pattern[i], ref pattern[24 + (int)((i - 23) * rnd())]);
			}
			for (int i = 53; i >= 48; i--)
			{
				Swap(ref pattern[i], ref pattern[48 + (int)((i - 47) * rnd())]);
			}
			for (int i = 80; i >= 54; i--)
			{
				Swap(ref pattern[i], ref pattern[54 + (int)(27 * rnd())]);
			}
		}
	}
}
