﻿namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Empty Rectangle Intersection Pair</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Empty Rectangle Intersection Pair</item>
/// </list>
/// </summary>
public interface IEmptyRectangleIntersectionPairStepSearcher : IAlmostLockedSetsStepSearcher
{
}

[StepSearcher]
internal sealed unsafe partial class EmptyRectangleIntersectionPairStepSearcher : IEmptyRectangleIntersectionPairStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		for (int i = 0, length = BivalueCells.Count, iterationLength = length - 1; i < iterationLength; i++)
		{
			int c1 = BivalueCells[i];

			short mask = grid.GetCandidates(c1);
			int d1 = TrailingZeroCount(mask), d2 = mask.GetNextSet(d1);
			for (int j = i + 1; j < length; j++)
			{
				int c2 = BivalueCells[j];

				// Check the candidates that cell holds is totally same with 'c1'.
				if (grid.GetCandidates(c2) != mask)
				{
					continue;
				}

				// Check the two cells are not in same house index.
				if ((CellMap.Empty + c1 + c2).InOneHouse)
				{
					continue;
				}

				int block1 = c1.ToHouseIndex(HouseType.Block);
				int block2 = c2.ToHouseIndex(HouseType.Block);
				if (block1 % 3 == block2 % 3 || block1 / 3 == block2 / 3)
				{
					continue;
				}

				// Check the block that two cells both see.
				var interMap = +(CellMap.Empty + c1 + c2);
				var unionMap = (PeersMap[c1] | PeersMap[c2]) + c1 + c2;
				foreach (int interCell in interMap)
				{
					int block = interCell.ToHouseIndex(HouseType.Block);
					var houseMap = HousesMap[block];
					var checkingMap = houseMap - unionMap & houseMap;
					if (checkingMap & CandidatesMap[d1] || checkingMap & CandidatesMap[d2])
					{
						continue;
					}

					// Check whether two digits are both in the same empty rectangle.
					int b1 = c1.ToHouseIndex(HouseType.Block);
					int b2 = c2.ToHouseIndex(HouseType.Block);
					var erMap = (unionMap & HousesMap[b1] - interMap) | (unionMap & HousesMap[b2] - interMap);
					var erCellsMap = houseMap & erMap;
					short m = grid.GetDigitsUnion(erCellsMap);
					if ((m & mask) != mask)
					{
						continue;
					}

					// Check eliminations.
					var conclusions = new List<Conclusion>();
					int z = (interMap & houseMap)[0];
					var c1Map = HousesMap[(CellMap.Empty + z + c1).CoveredLine];
					var c2Map = HousesMap[(CellMap.Empty + z + c2).CoveredLine];
					foreach (int elimCell in (c1Map | c2Map) - c1 - c2 - erMap)
					{
						if (CandidatesMap[d1].Contains(elimCell))
						{
							conclusions.Add(new(Elimination, elimCell, d1));
						}
						if (CandidatesMap[d2].Contains(elimCell))
						{
							conclusions.Add(new(Elimination, elimCell, d2));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (int digit in grid.GetCandidates(c1))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, c1 * 9 + digit));
					}
					foreach (int digit in grid.GetCandidates(c2))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, c2 * 9 + digit));
					}
					foreach (int cell in erCellsMap)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							if (digit != d1 && digit != d2)
							{
								continue;
							}

							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}

					var step = new EmptyRectangleIntersectionPairStep(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, block)
						),
						c1,
						c2,
						block,
						d1,
						d2
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