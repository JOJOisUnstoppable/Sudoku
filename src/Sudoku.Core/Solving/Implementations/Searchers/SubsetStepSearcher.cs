﻿namespace Sudoku.Solving.Implementations.Searchers;

[StepSearcher]
internal sealed unsafe partial class SubsetStepSearcher : ISubsetStepSearcher
{
	/// <inheritdoc/>
	/// <remarks>
	/// I hide this member on purpose because 4 is the maximum size of subsets found in practice.
	/// </remarks>
	int ISubsetStepSearcher.MaxSize { get; set; } = 4;


	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		for (int size = 2; size <= ((ISubsetStepSearcher)this).MaxSize; size++)
		{
			// Naked subsets.
			for (int house = 0; house < 27; house++)
			{
				if ((HousesMap[house] & EmptyCells) is not { Count: >= 2 } currentEmptyMap)
				{
					continue;
				}

				// Iterate on each combination.
				foreach (var cells in currentEmptyMap & size)
				{
					short mask = grid.GetDigitsUnion(cells);
					if (PopCount((uint)mask) != size)
					{
						continue;
					}

					// Naked subset found. Now check eliminations.
					short flagMask = 0;
					var conclusions = new List<Conclusion>(18);
					foreach (int digit in mask)
					{
						var map = cells % CandidatesMap[digit];
						flagMask |= (short)(map.InOneHouse ? 0 : 1 << digit);

						foreach (int cell in map)
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>(16);
					foreach (int cell in cells)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}

					bool? isLocked = flagMask == mask ? true : flagMask != 0 ? false : null;
					var step = new NakedSubsetStep(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, house)
						),
						house,
						cells,
						mask,
						isLocked
					);

					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}

			// Hidden subsets.
			for (int house = 0; house < 27; house++)
			{
				var traversingMap = HousesMap[house] - EmptyCells;
				if (traversingMap.Count >= 8)
				{
					// No available digit (Or hidden single).
					continue;
				}

				short mask = Grid.MaxCandidatesMask;
				foreach (int cell in traversingMap)
				{
					mask &= (short)~(1 << grid[cell]);
				}
				foreach (int[] digits in mask.GetAllSets().GetSubsets(size))
				{
					short tempMask = mask;
					short digitsMask = 0;
					var map = CellMap.Empty;
					foreach (int digit in digits)
					{
						tempMask &= (short)~(1 << digit);
						digitsMask |= (short)(1 << digit);
						map |= HousesMap[house] & CandidatesMap[digit];
					}
					if (map.Count != size)
					{
						continue;
					}

					// Gather eliminations.
					var conclusions = new List<Conclusion>();
					foreach (int digit in tempMask)
					{
						foreach (int cell in map & CandidatesMap[digit])
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					// Gather highlight candidates.
					var candidateOffsets = new List<CandidateViewNode>();
					foreach (int digit in digits)
					{
						foreach (int cell in map & CandidatesMap[digit])
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
						}
					}

					var step = new HiddenSubsetStep(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, house)
						),
						house,
						map,
						digitsMask
					);

					if (onlyFindOne)
					{
						return step;
					}

					accumulator.Add(step);
				}
			}
		}

		return null;
	}
}