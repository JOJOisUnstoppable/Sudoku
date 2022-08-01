﻿namespace Sudoku.Solving.Manual.Searchers;

unsafe partial class UniqueRectangleStepSearcher
{
	/// <summary>
	/// Check type 1.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ cornerCell
	/// (abc) ab
	///  ab   ab
	/// ]]></code>
	/// </remarks>
	partial void CheckType1(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell,
		scoped in Cells otherCellsMap, int index)
	{
		// Get the summary mask.
		short mask = grid.GetDigitsUnion(otherCellsMap);
		if (mask != comparer)
		{
			return;
		}

		// Type 1 found. Now check elimination.
		bool d1Exists = grid.Exists(cornerCell, d1) is true;
		bool d2Exists = grid.Exists(cornerCell, d2) is true;
		if (!d1Exists && d2Exists)
		{
			return;
		}

		var conclusions = new List<Conclusion>(2);
		if (d1Exists)
		{
			conclusions.Add(new(ConclusionType.Elimination, cornerCell, d1));
		}
		if (d2Exists)
		{
			conclusions.Add(new(ConclusionType.Elimination, cornerCell, d2));
		}
		if (conclusions.Count == 0)
		{
			return;
		}

		var candidateOffsets = new List<CandidateViewNode>(6);
		foreach (int cell in otherCellsMap)
		{
			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
			}
		}

		if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Count) != (6, 2))
		{
			return;
		}

		accumulator.Add(
			new UniqueRectangleType1Step(
				ImmutableArray.CreateRange(conclusions),
				ImmutableArray.Create(
					View.Empty
						| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
						| (arMode ? null : candidateOffsets)
				),
				d1,
				d2,
				(Cells)urCells,
				arMode,
				index
			)
		);
	}

	/// <summary>
	/// Check type 2.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abc) (abc)
	///  ab    ab
	/// ]]></code>
	/// </remarks>
	partial void CheckType2(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in Cells otherCellsMap, int index)
	{
		// Get the summary mask.
		short mask = grid.GetDigitsUnion(otherCellsMap);
		if (mask != comparer)
		{
			return;
		}

		// Gets the extra mask.
		// If the mask is the power of 2, the type 2 will be formed.
		int extraMask = (grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) ^ comparer;
		if (extraMask == 0 || (extraMask & extraMask - 1) != 0)
		{
			return;
		}

		// Type 2 or 5 found. Now check elimination.
		int extraDigit = TrailingZeroCount(extraMask);
		if ((!(Cells.Empty + corner1 + corner2) & CandidatesMap[extraDigit]) is not { Count: not 0 } elimMap)
		{
			return;
		}

		var candidateOffsets = new List<CandidateViewNode>();
		foreach (int cell in urCells)
		{
			if (grid.GetStatus(cell) == CellStatus.Empty)
			{
				foreach (int digit in grid.GetCandidates(cell))
				{
					candidateOffsets.Add(
						new(
							digit == extraDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
							cell * 9 + digit
						)
					);
				}
			}
		}

		if (IsIncomplete(candidateOffsets))
		{
			return;
		}

		bool isType5 = !(Cells.Empty + corner1 + corner2).InOneHouse;
		accumulator.Add(
			new UniqueRectangleType2Step(
				ImmutableArray.Create(Conclusion.ToConclusions(elimMap, extraDigit, ConclusionType.Elimination)),
				ImmutableArray.Create(
					View.Empty
						| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
						| candidateOffsets
				),
				d1,
				d2,
				(arMode, isType5) switch
				{
					(true, true) => Technique.AvoidableRectangleType5,
					(true, false) => Technique.AvoidableRectangleType2,
					(false, true) => Technique.UniqueRectangleType5,
					(false, false) => Technique.UniqueRectangleType2
				},
				(Cells)urCells,
				arMode,
				extraDigit,
				index
			)
		);
	}

	/// <summary>
	/// Check type 3.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">
	/// The map of other cells during the current UR searching.
	/// </param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ corner1, corner2
	/// (ab ) (ab )
	///  abx   aby
	/// ]]></code>
	/// </remarks>
	partial void CheckType3(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in Cells otherCellsMap, int index)
	{
		bool notSatisfiedType3 = false;
		foreach (int cell in otherCellsMap)
		{
			short currentMask = grid.GetCandidates(cell);
			if ((currentMask & comparer) == 0
				|| currentMask == comparer || arMode && grid.GetStatus(cell) != CellStatus.Empty)
			{
				notSatisfiedType3 = true;
				break;
			}
		}
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer || notSatisfiedType3)
		{
			return;
		}

		short mask = grid.GetDigitsUnion(otherCellsMap);
		if ((mask & comparer) != comparer)
		{
			return;
		}

		short otherDigitsMask = (short)(mask ^ comparer);
		foreach (int houseIndex in otherCellsMap.CoveredHouses)
		{
			if (((ValuesMap[d1] | ValuesMap[d2]) & HouseMaps[houseIndex]) is not [])
			{
				return;
			}

			var iterationMap = (HouseMaps[houseIndex] & EmptyCells) - otherCellsMap;
			for (int size = PopCount((uint)otherDigitsMask) - 1, count = iterationMap.Count; size < count; size++)
			{
				foreach (var iteratedCells in iterationMap & size)
				{
					short tempMask = grid.GetDigitsUnion(iteratedCells);
					if ((tempMask & comparer) != 0 || PopCount((uint)tempMask) - 1 != size
						|| (tempMask & otherDigitsMask) != otherDigitsMask)
					{
						continue;
					}

					var conclusions = new List<Conclusion>(16);
					foreach (int digit in tempMask)
					{
						foreach (int cell in (iterationMap - iteratedCells) & CandidatesMap[digit])
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
					if (conclusions.Count == 0)
					{
						continue;
					}

					var cellOffsets = new List<CellViewNode>();
					foreach (int cell in urCells)
					{
						if (grid.GetStatus(cell) != CellStatus.Empty)
						{
							cellOffsets.Add(new(DisplayColorKind.Normal, cell));
						}
					}

					var candidateOffsets = new List<CandidateViewNode>();
					foreach (int cell in urCells)
					{
						if (grid.GetStatus(cell) == CellStatus.Empty)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(
									new(
										(tempMask >> digit & 1) != 0
											? DisplayColorKind.Auxiliary1
											: DisplayColorKind.Normal,
										cell * 9 + digit
									)
								);
							}
						}
					}
					foreach (int cell in iteratedCells)
					{
						foreach (int digit in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
						}
					}

					accumulator.Add(
						new UniqueRectangleType3Step(
							ImmutableArray.CreateRange(conclusions),
							ImmutableArray.Create(
								View.Empty
									| (arMode ? cellOffsets : null)
									| candidateOffsets
									| new HouseViewNode(DisplayColorKind.Normal, houseIndex)
							),
							d1,
							d2,
							(Cells)urCells,
							iteratedCells,
							otherDigitsMask,
							houseIndex,
							arMode,
							index
						)
					);
				}
			}
		}
	}

	/// <summary>
	/// Check type 4.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">
	/// The map of other cells during the current UR searching.
	/// </param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ corner1, corner2
	/// (ab ) ab
	///  abx  aby
	/// ]]></code>
	/// </remarks>
	partial void CheckType4(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in Cells otherCellsMap, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		int* p = stackalloc[] { d1, d2 };
		foreach (int houseIndex in otherCellsMap.CoveredHouses)
		{
			if (houseIndex < 9)
			{
				// Process the case in lines.
				continue;
			}

			for (int digitIndex = 0; digitIndex < 2; digitIndex++)
			{
				int digit = p[digitIndex];
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(digit, otherCellsMap, houseIndex))
				{
					continue;
				}

				// Yes, Type 4 found.
				// Now check elimination.
				int elimDigit = TrailingZeroCount(comparer ^ (1 << digit));
				if ((otherCellsMap & CandidatesMap[elimDigit]) is not { Count: not 0 } elimMap)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(6);
				foreach (int cell in urCells)
				{
					if (grid.GetStatus(cell) != CellStatus.Empty)
					{
						continue;
					}

					if (otherCellsMap.Contains(cell))
					{
						if (d1 != elimDigit && grid.Exists(cell, d1) is true)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + d1));
						}
						if (d2 != elimDigit && grid.Exists(cell, d2) is true)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + d2));
						}
					}
					else
					{
						// Corner1 and corner2.
						foreach (int d in grid.GetCandidates(cell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d));
						}
					}
				}

				var conclusions = Conclusion.ToConclusions(elimMap, elimDigit, ConclusionType.Elimination);
				if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Length) != (6, 2))
				{
					continue;
				}

				int[] offsets = otherCellsMap.ToArray();
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						ImmutableArray.Create(conclusions),
						ImmutableArray.Create(
							View.Empty
								| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
								| candidateOffsets
								| new HouseViewNode(DisplayColorKind.Normal, houseIndex)
						),
						Technique.UniqueRectangleType4,
						d1,
						d2,
						(Cells)urCells,
						arMode,
						new Conjugate[] { new(offsets[0], offsets[1], digit) },
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check type 5.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab ) abc
	///  abc  abc
	/// ]]></code>
	/// </remarks>
	partial void CheckType5(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell,
		scoped in Cells otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		// Get the summary mask.
		short otherCellsMask = grid.GetDigitsUnion(otherCellsMap);

		// Degenerate to type 1.
		short extraMask = (short)(otherCellsMask ^ comparer);
		if ((extraMask & extraMask - 1) != 0)
		{
			return;
		}

		// Type 5 found. Now check elimination.
		int extraDigit = TrailingZeroCount(extraMask);
		var cellsThatContainsExtraDigit = otherCellsMap & CandidatesMap[extraDigit];

		// Degenerate to type 1.
		if (cellsThatContainsExtraDigit.Count == 1)
		{
			return;
		}

		if ((!cellsThatContainsExtraDigit & CandidatesMap[extraDigit]) is not { Count: not 0 } elimMap)
		{
			return;
		}

		var candidateOffsets = new List<CandidateViewNode>(16);
		foreach (int cell in urCells)
		{
			if (grid.GetStatus(cell) != CellStatus.Empty)
			{
				continue;
			}

			foreach (int digit in grid.GetCandidates(cell))
			{
				candidateOffsets.Add(
					new(
						digit == extraDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
						cell * 9 + digit
					)
				);
			}
		}
		if (IsIncomplete(candidateOffsets))
		{
			return;
		}

		accumulator.Add(
			new UniqueRectangleType2Step(
				ImmutableArray.Create(
					Conclusion.ToConclusions(elimMap, extraDigit, ConclusionType.Elimination)
				),
				ImmutableArray.Create(
					View.Empty
						| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
						| candidateOffsets
				),
				d1,
				d2,
				arMode ? Technique.AvoidableRectangleType5 : Technique.UniqueRectangleType5,
				(Cells)urCells,
				arMode,
				extraDigit,
				index
			)
		);
	}

	/// <summary>
	/// Check type 6.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ corner1
	/// (ab )  aby
	///  abx  (ab)
	///        ↑corner2
	/// ]]></code>
	/// </remarks>
	partial void CheckType6(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in Cells otherCellsMap, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		int o1 = otherCellsMap[0], o2 = otherCellsMap[1];
		int r1 = corner1.ToHouseIndex(HouseType.Row);
		int c1 = corner1.ToHouseIndex(HouseType.Column);
		int r2 = corner2.ToHouseIndex(HouseType.Row);
		int c2 = corner2.ToHouseIndex(HouseType.Column);
		int* p = stackalloc[] { d1, d2 };
		var q = stackalloc[] { (r1, r2), (c1, c2) };
		for (int digitIndex = 0; digitIndex < 2; digitIndex++)
		{
			int digit = p[digitIndex];
			for (int housePairIndex = 0; housePairIndex < 2; housePairIndex++)
			{
				var (h1, h2) = q[housePairIndex];
				gather(grid, otherCellsMap, h1 is >= 9 and < 18, digit, h1, h2);
			}
		}


		void gather(scoped in Grid grid, scoped in Cells otherCellsMap, bool isRow, int digit, int house1, int house2)
		{
			if (
				!(
					isRow
						&& IUniqueRectangleStepSearcher.IsConjugatePair(digit, Cells.Empty + corner1 + o1, house1)
						&& IUniqueRectangleStepSearcher.IsConjugatePair(digit, Cells.Empty + corner2 + o2, house2)
						|| !isRow
						&& IUniqueRectangleStepSearcher.IsConjugatePair(digit, Cells.Empty + corner1 + o2, house1)
						&& IUniqueRectangleStepSearcher.IsConjugatePair(digit, Cells.Empty + corner2 + o1, house2)
				)
			)
			{
				return;
			}

			// Check eliminations.
			if ((otherCellsMap & CandidatesMap[digit]) is not { Count: not 0 } elimMap)
			{
				return;
			}

			var candidateOffsets = new List<CandidateViewNode>(6);
			foreach (int cell in urCells)
			{
				if (otherCellsMap.Contains(cell))
				{
					if (d1 != digit && grid.Exists(cell, d1) is true)
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d1));
					}
					if (d2 != digit && grid.Exists(cell, d2) is true)
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d2));
					}
				}
				else
				{
					foreach (int d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(d == digit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cell * 9 + d)
						);
					}
				}
			}

			var conclusions = Conclusion.ToConclusions(elimMap, digit, ConclusionType.Elimination);
			if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Length) != (6, 2))
			{
				return;
			}

			accumulator.Add(
				new UniqueRectangleWithConjugatePairStep(
					ImmutableArray.Create(conclusions),
					ImmutableArray.Create(
						View.Empty
							| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
							| candidateOffsets
							| new HouseViewNode[]
							{
								new(DisplayColorKind.Normal, house1),
								new(DisplayColorKind.Normal, house2)
							}
					),
					Technique.UniqueRectangleType6,
					d1,
					d2,
					(Cells)urCells,
					false,
					new Conjugate[]
					{
						new(corner1, isRow ? o1 : o2, digit),
						new(corner2, isRow ? o2 : o1, digit)
					},
					index
				)
			);
		}
	}

	/// <summary>
	/// Check hidden UR.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab ) abx
	///  aby  abz
	/// ]]></code>
	/// </remarks>
	partial void CheckHidden(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell,
		scoped in Cells otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int abzCell = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
		int r = abzCell.ToHouseIndex(HouseType.Row), c = abzCell.ToHouseIndex(HouseType.Column);
		int* p = stackalloc[] { d1, d2 };
		for (int digitIndex = 0; digitIndex < 2; digitIndex++)
		{
			int digit = p[digitIndex];
			var map1 = Cells.Empty + abzCell + abxCell;
			var map2 = Cells.Empty + abzCell + abyCell;
			if (map1.CoveredLine is not (var m1cl and not InvalidFirstSet)
				|| map2.CoveredLine is not (var m2cl and not InvalidFirstSet))
			{
				// There's no common covered line to display.
				continue;
			}

			if (!IUniqueRectangleStepSearcher.IsConjugatePair(digit, map1, m1cl) || !IUniqueRectangleStepSearcher.IsConjugatePair(digit, map2, m2cl))
			{
				continue;
			}

			// Hidden UR found. Now check eliminations.
			int elimDigit = TrailingZeroCount(comparer ^ (1 << digit));
			if (grid.Exists(abzCell, elimDigit) is not true)
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>();
			foreach (int cell in urCells)
			{
				if (grid.GetStatus(cell) != CellStatus.Empty)
				{
					continue;
				}

				if (otherCellsMap.Contains(cell))
				{
					if ((cell != abzCell || d1 != elimDigit) && grid.Exists(cell, d1) is true)
					{
						candidateOffsets.Add(
							new(
								d1 != elimDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
								cell * 9 + d1
							)
						);
					}
					if ((cell != abzCell || d2 != elimDigit) && grid.Exists(cell, d2) is true)
					{
						candidateOffsets.Add(
							new(
								d2 != elimDigit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
								cell * 9 + d2
							)
						);
					}
				}
				else
				{
					foreach (int d in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d));
					}
				}
			}

			if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
			{
				continue;
			}

			accumulator.Add(
				new HiddenUniqueRectangleStep(
					ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, abzCell, elimDigit)),
					ImmutableArray.Create(
						View.Empty
							| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
							| candidateOffsets
							| new HouseViewNode[] { new(DisplayColorKind.Normal, r), new(DisplayColorKind.Normal, c) }
					),
					d1,
					d2,
					(Cells)urCells,
					arMode,
					new Conjugate[] { new(abzCell, abxCell, digit), new(abzCell, abyCell, digit) },
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR+2D.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )  abx
	///  aby  (ab )  xy  *
	///         ↑ corner2
	/// ]]></code>
	/// </remarks>
	partial void Check2D(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2,
		int corner1, int corner2, scoped in Cells otherCellsMap, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		short o1 = grid.GetCandidates(otherCellsMap[0]), o2 = grid.GetCandidates(otherCellsMap[1]);
		short o = (short)(o1 | o2);
		if (
			(
				TotalNumbersCount: PopCount((uint)o),
				OtherCell1NumbersCount: PopCount((uint)o1),
				OtherCell2NumbersCount: PopCount((uint)o2),
				OtherCell1Intersetion: o1 & comparer,
				OtherCell2Intersetion: o2 & comparer
			) is not (
				TotalNumbersCount: 4,
				OtherCell1NumbersCount: <= 3,
				OtherCell2NumbersCount: <= 3,
				OtherCell1Intersetion: not 0,
				OtherCell2Intersetion: not 0
			) || (o & comparer) != comparer
		)
		{
			return;
		}

		short xyMask = (short)(o ^ comparer);
		int x = TrailingZeroCount(xyMask), y = xyMask.GetNextSet(x);
		var inter = !otherCellsMap - (Cells)urCells;
		foreach (int possibleXyCell in inter)
		{
			if (grid.GetCandidates(possibleXyCell) != xyMask)
			{
				continue;
			}

			// 'xy' found.
			// Now check eliminations.
			var elimMap = inter & PeerMaps[possibleXyCell];
			var conclusions = new List<Conclusion>(10);
			foreach (int cell in elimMap)
			{
				if (grid.Exists(cell, x) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, x));
				}
				if (grid.Exists(cell, y) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, y));
				}
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>(10);
			foreach (int cell in urCells)
			{
				if (otherCellsMap.Contains(cell))
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								(comparer >> digit & 1) == 0 ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
								cell * 9 + digit
							)
						);
					}
				}
				else
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
					}
				}
			}
			foreach (int digit in xyMask)
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, possibleXyCell * 9 + digit));
			}

			if (IsIncomplete(candidateOffsets))
			{
				return;
			}

			accumulator.Add(
				new UniqueRectangle2DOr3XStep(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(
						View.Empty
							| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
							| candidateOffsets
					),
					arMode ? Technique.AvoidableRectangle2D : Technique.UniqueRectangle2D,
					d1,
					d2,
					(Cells)urCells,
					arMode,
					x,
					y,
					possibleXyCell,
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR+2B/1SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (ab )  (ab )
	///  |
	///  | a
	///  |
	///  abx    aby
	/// ]]></code>
	/// </remarks>
	partial void Check2B1SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in Cells otherCellsMap, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		int* corners = stackalloc[] { corner1, corner2 };
		int* digits = stackalloc[] { d1, d2 };
		for (int cellIndex = 0; cellIndex < 2; cellIndex++)
		{
			int cell = corners[cellIndex];
			foreach (int otherCell in otherCellsMap)
			{
				if (!IUniqueRectangleStepSearcher.IsSameHouseCell(cell, otherCell, out int houses))
				{
					continue;
				}

				foreach (int house in houses)
				{
					if (house < 9)
					{
						continue;
					}

					for (int digitIndex = 0; digitIndex < 2; digitIndex++)
					{
						int digit = digits[digitIndex];
						if (!IUniqueRectangleStepSearcher.IsConjugatePair(digit, Cells.Empty + cell + otherCell, house))
						{
							continue;
						}

						int elimCell = (otherCellsMap - otherCell)[0];
						if (grid.Exists(otherCell, digit) is not true)
						{
							continue;
						}

						int elimDigit = TrailingZeroCount(comparer ^ (1 << digit));
						var conclusions = new List<Conclusion>(4);
						if (grid.Exists(elimCell, elimDigit) is true)
						{
							conclusions.Add(new(ConclusionType.Elimination, elimCell, elimDigit));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>(10);
						foreach (int urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								int coveredHouses = (Cells.Empty + urCell + otherCell).CoveredHouses;
								if ((coveredHouses >> house & 1) != 0)
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(
											new(
												d == digit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
												urCell * 9 + d
											)
										);
									}
								}
								else
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(new(DisplayColorKind.Normal, urCell * 9 + d));
									}
								}
							}
							else if (urCell == otherCell || urCell == elimCell)
							{
								if (grid.Exists(urCell, d1) is true)
								{
									if (urCell != elimCell || d1 != elimDigit)
									{
										candidateOffsets.Add(
											new(
												urCell == elimCell
													? DisplayColorKind.Normal
													: d1 == digit
														? DisplayColorKind.Auxiliary1
														: DisplayColorKind.Normal,
												urCell * 9 + d1
											)
										);
									}
								}
								if (grid.Exists(urCell, d2) is true)
								{
									if (urCell != elimCell || d2 != elimDigit)
									{
										candidateOffsets.Add(
											new(
												urCell == elimCell
													? DisplayColorKind.Normal
													: d2 == digit
														? DisplayColorKind.Auxiliary1
														: DisplayColorKind.Normal,
												urCell * 9 + d2
											)
										);
									}
								}
							}
						}

						if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UniqueRectangleWithConjugatePairStep(
								ImmutableArray.CreateRange(conclusions),
								ImmutableArray.Create(
									View.Empty
										| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
										| candidateOffsets
										| new HouseViewNode(DisplayColorKind.Normal, house)
								),
								Technique.UniqueRectangle2B1,
								d1,
								d2,
								(Cells)urCells,
								arMode,
								new Conjugate[] { new(cell, otherCell, digit) },
								index
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR+2D/1SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )   aby
	///  |
	///  | a
	///  |
	///  abx   (ab )
	///          ↑ corner2
	/// ]]></code>
	/// </remarks>
	partial void Check2D1SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2,
		int corner1, int corner2, scoped in Cells otherCellsMap, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		int* corners = stackalloc[] { corner1, corner2 };
		int* digits = stackalloc[] { d1, d2 };
		for (int cellIndex = 0; cellIndex < 2; cellIndex++)
		{
			int cell = corners[cellIndex];
			foreach (int otherCell in otherCellsMap)
			{
				if (!IUniqueRectangleStepSearcher.IsSameHouseCell(cell, otherCell, out int houses))
				{
					continue;
				}

				foreach (int house in houses)
				{
					if (house < 9)
					{
						continue;
					}

					for (int digitIndex = 0; digitIndex < 2; digitIndex++)
					{
						int digit = digits[digitIndex];
						if (!IUniqueRectangleStepSearcher.IsConjugatePair(digit, Cells.Empty + cell + otherCell, house))
						{
							continue;
						}

						int elimCell = (otherCellsMap - otherCell)[0];
						if (grid.Exists(otherCell, digit) is not true)
						{
							continue;
						}

						var conclusions = new List<Conclusion>(4);
						if (grid.Exists(elimCell, digit) is true)
						{
							conclusions.Add(new(ConclusionType.Elimination, elimCell, digit));
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>(10);
						foreach (int urCell in urCells)
						{
							if (urCell == corner1 || urCell == corner2)
							{
								bool flag = false;
								foreach (int r in (Cells.Empty + urCell + otherCell).CoveredHouses)
								{
									if (r == house)
									{
										flag = true;
										break;
									}
								}

								if (flag)
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(
											new(
												d == digit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
												urCell * 9 + d
											)
										);
									}
								}
								else
								{
									foreach (int d in grid.GetCandidates(urCell))
									{
										candidateOffsets.Add(new(DisplayColorKind.Normal, urCell * 9 + d));
									}
								}
							}
							else if (urCell == otherCell || urCell == elimCell)
							{
								if (grid.Exists(urCell, d1) is true && (urCell != elimCell || d1 != digit))
								{
									candidateOffsets.Add(
										new(
											urCell == elimCell
												? DisplayColorKind.Normal
												: d1 == digit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
											urCell * 9 + d1
										)
									);
								}
								if (grid.Exists(urCell, d2) is true && (urCell != elimCell || d2 != digit))
								{
									candidateOffsets.Add(
										new(
											urCell == elimCell
												? DisplayColorKind.Normal
												: d2 == digit ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
											urCell * 9 + d2
										)
									);
								}
							}
						}

						if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
						{
							continue;
						}

						accumulator.Add(
							new UniqueRectangleWithConjugatePairStep(
								ImmutableArray.CreateRange(conclusions),
								ImmutableArray.Create(
									View.Empty
										| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
										| candidateOffsets
										| new HouseViewNode(DisplayColorKind.Normal, house)
								),
								Technique.UniqueRectangle2D1,
								d1,
								d2,
								(Cells)urCells,
								arMode,
								new Conjugate[] { new(cell, otherCell, digit) },
								index
							)
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check UR+3X.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )  abx
	///  aby   abz   xy  *
	/// ]]></code>
	/// Note: <c>z</c> is <c>x</c> or <c>y</c>.
	/// </remarks>
	partial void Check3X(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2,
		int cornerCell, scoped in Cells otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int c1 = otherCellsMap[0], c2 = otherCellsMap[1], c3 = otherCellsMap[2];
		short m1 = grid.GetCandidates(c1), m2 = grid.GetCandidates(c2), m3 = grid.GetCandidates(c3);
		short mask = (short)((short)(m1 | m2) | m3);

		if (
			(
				TotalNumbersCount: PopCount((uint)mask),
				Cell1NumbersCount: PopCount((uint)m1),
				Cell2NumbersCount: PopCount((uint)m2),
				Cell3NumbersCount: PopCount((uint)m3),
				Cell1Intersection: m1 & comparer,
				Cell2Intersection: m2 & comparer,
				Cell3Intersection: m3 & comparer
			) is not (
				TotalNumbersCount: 4,
				Cell1NumbersCount: <= 3,
				Cell2NumbersCount: <= 3,
				Cell3NumbersCount: <= 3,
				Cell1Intersection: not 0,
				Cell2Intersection: not 0,
				Cell3Intersection: not 0
			) || (mask & comparer) != comparer
		)
		{
			return;
		}

		short xyMask = (short)(mask ^ comparer);
		int x = TrailingZeroCount(xyMask), y = xyMask.GetNextSet(x);
		var inter = !otherCellsMap - (Cells)urCells;
		foreach (int possibleXyCell in inter)
		{
			if (grid.GetCandidates(possibleXyCell) != xyMask)
			{
				continue;
			}

			// Possible XY cell found.
			// Now check eliminations.
			var conclusions = new List<Conclusion>(10);
			foreach (int cell in inter & PeerMaps[possibleXyCell])
			{
				if (grid.Exists(cell, x) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, x));
				}
				if (grid.Exists(cell, y) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, y));
				}
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>(10);
			foreach (int cell in urCells)
			{
				if (otherCellsMap.Contains(cell))
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								(comparer >> digit & 1) == 0 ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal,
								cell * 9 + digit
							)
						);
					}
				}
				else
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + digit));
					}
				}
			}
			foreach (int digit in xyMask)
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, possibleXyCell * 9 + digit));
			}
			if (IsIncomplete(candidateOffsets))
			{
				return;
			}

			accumulator.Add(
				new UniqueRectangle2DOr3XStep(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(
						View.Empty
							| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
							| candidateOffsets
					),
					arMode ? Technique.AvoidableRectangle3X : Technique.UniqueRectangle3X,
					d1,
					d2,
					(Cells)urCells,
					arMode,
					x,
					y,
					possibleXyCell,
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR+3X/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )    abx
	///           |
	///           | b
	///       a   |
	///  aby-----abz
	/// ]]></code>
	/// </remarks>
	partial void Check3X2SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2,
		int cornerCell, scoped in Cells otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int abzCell = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		var pairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int pairIndex = 0; pairIndex < 2; pairIndex++)
		{
			var (a, b) = pairs[pairIndex];
			int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
			var map1 = Cells.Empty + abzCell + abxCell;
			var map2 = Cells.Empty + abzCell + abyCell;
			if (!IUniqueRectangleStepSearcher.IsConjugatePair(b, map1, map1.CoveredLine)
				|| !IUniqueRectangleStepSearcher.IsConjugatePair(a, map2, map2.CoveredLine))
			{
				continue;
			}

			var conclusions = new List<Conclusion>(2);
			if (grid.Exists(abxCell, a) is true)
			{
				conclusions.Add(new(ConclusionType.Elimination, abxCell, a));
			}
			if (grid.Exists(abyCell, b) is true)
			{
				conclusions.Add(new(ConclusionType.Elimination, abyCell, b));
			}
			if (conclusions.Count == 0)
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>(6);
			foreach (int digit in grid.GetCandidates(abxCell))
			{
				if ((digit == d1 || digit == d2) && digit != a)
				{
					candidateOffsets.Add(
						new(digit == b ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, abxCell * 9 + digit)
					);
				}
			}
			foreach (int digit in grid.GetCandidates(abyCell))
			{
				if ((digit == d1 || digit == d2) && digit != b)
				{
					candidateOffsets.Add(
						new(digit == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, abyCell * 9 + digit)
					);
				}
			}
			foreach (int digit in grid.GetCandidates(abzCell))
			{
				if (digit == a || digit == b)
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, abzCell * 9 + digit));
				}
			}
			foreach (int digit in comparer)
			{
				candidateOffsets.Add(new(DisplayColorKind.Normal, cornerCell * 9 + digit));
			}
			if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 6)
			{
				continue;
			}

			accumulator.Add(
				new UniqueRectangleWithConjugatePairStep(
					ImmutableArray.CreateRange(conclusions),
					ImmutableArray.Create(
						View.Empty
							| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
							| candidateOffsets
							| new HouseViewNode[]
							{
								new(DisplayColorKind.Normal, map1.CoveredLine),
								new(DisplayColorKind.Auxiliary1, map2.CoveredLine)
							}
					),
					Technique.UniqueRectangle3X2,
					d1,
					d2,
					(Cells)urCells,
					arMode,
					new Conjugate[] { new(abxCell, abzCell, b), new(abyCell, abzCell, a) },
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR+3N/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )-----abx
	///        a   |
	///            | b
	///            |
	///  aby      abz
	/// ]]></code>
	/// </remarks>
	partial void Check3N2SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells,
		bool arMode, short comparer, int d1, int d2, int cornerCell,
		scoped in Cells otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		// Step 1: Get the diagonal cell of 'cornerCell' and determine
		// the existence of strong link.
		int abzCell = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
		var cellPairs = stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) };
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		int* digits = stackalloc[] { d1, d2 };
		for (int cellPairIndex = 0; cellPairIndex < 2; cellPairIndex++)
		{
			var (begin, end) = cellPairs[cellPairIndex];
			var linkMap = Cells.Empty + begin + abzCell;
			for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
			{
				var (a, b) = digitPairs[digitPairIndex];
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(b, linkMap, linkMap.CoveredLine))
				{
					continue;
				}

				// Step 2: Get the link cell that is adjacent to 'cornerCell'
				// and check the strong link.
				var secondLinkMap = Cells.Empty + cornerCell + begin;
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
				{
					continue;
				}

				// Step 3: Check eliminations.
				if (grid.Exists(end, a) is not true)
				{
					continue;
				}

				// Step 4: Check highlight candidates.
				var candidateOffsets = new List<CandidateViewNode>(7);
				foreach (int d in comparer)
				{
					candidateOffsets.Add(
						new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cornerCell * 9 + d)
					);
				}
				for (int digitIndex = 0; digitIndex < 2; digitIndex++)
				{
					int d = digits[digitIndex];
					if (grid.Exists(abzCell, d) is true)
					{
						candidateOffsets.Add(
							new(d == b ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, abzCell * 9 + d)
						);
					}
				}
				foreach (int d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, begin * 9 + d));
					}
				}
				foreach (int d in grid.GetCandidates(end))
				{
					if ((d == d1 || d == d2) && d != a)
					{
						candidateOffsets.Add(new(DisplayColorKind.Normal, end * 9 + d));
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = new Conjugate[] { new(cornerCell, begin, a), new(begin, abzCell, b) };
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, end, a)),
						ImmutableArray.Create(
							View.Empty
								| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
								| candidateOffsets
								| new HouseViewNode[]
								{
									new(DisplayColorKind.Normal, conjugatePairs[0].Line),
									new(DisplayColorKind.Auxiliary1, conjugatePairs[1].Line)
								}
						),
						Technique.UniqueRectangle3N2,
						d1,
						d2,
						(Cells)urCells,
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR+3U/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///  ↓ cornerCell
	/// (ab )-----abx
	///        a
	///
	///        b
	///  aby -----abz
	/// ]]></code>
	/// </remarks>
	partial void Check3U2SL(
		ICollection<UniqueRectangleStep> accumulator,
		scoped in Grid grid, int[] urCells, bool arMode, short comparer, int d1, int d2,
		int cornerCell, scoped in Cells otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int abzCell = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
		var cellPairs = stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) };
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int cellPairIndex = 0; cellPairIndex < 2; cellPairIndex++)
		{
			var (begin, end) = cellPairs[cellPairIndex];
			var linkMap = Cells.Empty + begin + abzCell;
			for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
			{
				var (a, b) = digitPairs[digitPairIndex];
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(b, linkMap, linkMap.CoveredLine))
				{
					continue;
				}

				var secondLinkMap = Cells.Empty + cornerCell + end;
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
				{
					continue;
				}

				if (grid.Exists(begin, a) is not true)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(7);
				foreach (int d in comparer)
				{
					candidateOffsets.Add(
						new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cornerCell * 9 + d)
					);
				}
				foreach (int d in grid.GetCandidates(begin))
				{
					if ((d == d1 || d == d2) && d != a)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, begin * 9 + d));
					}
				}
				foreach (int d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(
							new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, end * 9 + d)
						);
					}
				}
				foreach (int d in grid.GetCandidates(abzCell))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(
							new(d == b ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, abzCell * 9 + d)
						);
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = new Conjugate[] { new(cornerCell, end, a), new(begin, abzCell, b) };
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, begin, a)),
						ImmutableArray.Create(
							View.Empty
								| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
								| candidateOffsets
								| new HouseViewNode[]
								{
									new(DisplayColorKind.Normal, conjugatePairs[0].Line),
									new(DisplayColorKind.Auxiliary1, conjugatePairs[1].Line)
								}
						),
						Technique.UniqueRectangle3U2,
						d1,
						d2,
						(Cells)urCells,
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR+3E/2SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="cornerCell">The corner cell.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ cornerCell
	/// (ab )-----abx
	///        a
	///
	///        a
	///  aby -----abz
	/// ]]></code>
	/// </remarks>
	partial void Check3E2SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell,
		scoped in Cells otherCellsMap, int index)
	{
		if (grid.GetCandidates(cornerCell) != comparer)
		{
			return;
		}

		int abzCell = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, cornerCell);
		var adjacentCellsMap = otherCellsMap - abzCell;
		int abxCell = adjacentCellsMap[0], abyCell = adjacentCellsMap[1];
		var cellPairs = stackalloc[] { (abxCell, abyCell), (abyCell, abxCell) };
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int cellPairIndex = 0; cellPairIndex < 2; cellPairIndex++)
		{
			var (begin, end) = cellPairs[cellPairIndex];
			var linkMap = Cells.Empty + begin + abzCell;
			for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
			{
				var (a, b) = digitPairs[digitPairIndex];
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, linkMap, linkMap.CoveredLine))
				{
					continue;
				}

				var secondLinkMap = Cells.Empty + cornerCell + end;
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, secondLinkMap, secondLinkMap.CoveredLine))
				{
					continue;
				}

				if (grid.Exists(abzCell, b) is not true)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(7);
				foreach (int d in comparer)
				{
					candidateOffsets.Add(
						new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, cornerCell * 9 + d)
					);
				}
				foreach (int d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(
							new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, begin * 9 + d)
						);
					}
				}
				foreach (int d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(
							new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, end * 9 + d)
						);
					}
				}
				foreach (int d in grid.GetCandidates(abzCell))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add(
							new(d == a ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, abzCell * 9 + d)
						);
					}
				}
				if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
				{
					continue;
				}

				var conjugatePairs = new Conjugate[] { new(cornerCell, end, a), new(begin, abzCell, a) };
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, abzCell, b)),
						ImmutableArray.Create(
							View.Empty
								| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
								| candidateOffsets
								| new HouseViewNode[]
								{
									new(DisplayColorKind.Normal, conjugatePairs[0].Line),
									new(DisplayColorKind.Auxiliary1, conjugatePairs[1].Line)
								}
						),
						Technique.UniqueRectangle3E2,
						d1,
						d2,
						(Cells)urCells,
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR+4X/3SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abx)-----(aby)
	///        a    |
	///             | b
	///        a    |
	///  abz ----- abw
	/// ]]></code>
	/// </remarks>
	partial void Check4X3SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells,
		bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in Cells otherCellsMap, int index)
	{
		var link1Map = Cells.Empty + corner1 + corner2;
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
		{
			var (a, b) = digitPairs[digitPairIndex];
			if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, link1Map, link1Map.CoveredLine))
			{
				continue;
			}

			int abwCell = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, corner1);
			int abzCell = (otherCellsMap - abwCell)[0];
			var cellQuadruples = stackalloc[]
			{
				(corner2, corner1, abzCell, abwCell),
				(corner1, corner2, abwCell, abzCell)
			};

			for (int cellQuadrupleIndex = 0; cellQuadrupleIndex < 2; cellQuadrupleIndex++)
			{
				var (head, begin, end, extra) = cellQuadruples[cellQuadrupleIndex];
				var link2Map = Cells.Empty + begin + end;
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(b, link2Map, link2Map.CoveredLine))
				{
					continue;
				}

				var link3Map = Cells.Empty + end + extra;
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, link3Map, link3Map.CoveredLine))
				{
					continue;
				}

				var conclusions = new List<Conclusion>(2);
				if (grid.Exists(head, b) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, head, b));
				}
				if (grid.Exists(extra, b) is true)
				{
					conclusions.Add(new(ConclusionType.Elimination, extra, b));
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>(6);
				foreach (int d in grid.GetCandidates(head))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, head * 9 + d));
					}
				}
				foreach (int d in grid.GetCandidates(extra))
				{
					if ((d == d1 || d == d2) && d != b)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, extra * 9 + d));
					}
				}
				foreach (int d in grid.GetCandidates(begin))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, begin * 9 + d));
					}
				}
				foreach (int d in grid.GetCandidates(end))
				{
					if (d == d1 || d == d2)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, end * 9 + d));
					}
				}
				if (!AllowIncompleteUniqueRectangles && (candidateOffsets.Count, conclusions.Count) != (6, 2))
				{
					continue;
				}

				var conjugatePairs = new Conjugate[]
				{
					new(head, begin, a),
					new(begin, end, b),
					new(end, extra, a)
				};
				accumulator.Add(
					new UniqueRectangleWithConjugatePairStep(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
								| candidateOffsets
								| new HouseViewNode[]
								{
									new(DisplayColorKind.Normal, conjugatePairs[0].Line),
									new(DisplayColorKind.Auxiliary1, conjugatePairs[1].Line),
									new(DisplayColorKind.Normal, conjugatePairs[2].Line)
								}
						),
						Technique.UniqueRectangle4X3,
						d1,
						d2,
						(Cells)urCells,
						arMode,
						conjugatePairs,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR+4C/3SL.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abx)-----(aby)
	///        a    |
	///             | a
	///        b    |
	///  abz ----- abw
	/// ]]></code>
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	///   ↓ corner1, corner2
	/// (abx)-----(aby)
	///   |    a    |
	///   | b       | a
	///   |         |
	///  abz       abw
	/// ]]></code>
	/// </para>
	/// </remarks>
	partial void Check4C3SL(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells,
		bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in Cells otherCellsMap, int index)
	{
		var link1Map = Cells.Empty + corner1 + corner2;
		var innerMaps = stackalloc Cells[2];
		var digitPairs = stackalloc[] { (d1, d2), (d2, d1) };
		for (int digitPairIndex = 0; digitPairIndex < 2; digitPairIndex++)
		{
			var (a, b) = digitPairs[digitPairIndex];
			if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, link1Map, link1Map.CoveredLine))
			{
				continue;
			}

			int end = IUniqueRectangleStepSearcher.GetDiagonalCell(urCells, corner1);
			int extra = (otherCellsMap - end)[0];
			var cellQuadruples = stackalloc[] { (corner2, corner1, extra, end), (corner1, corner2, end, extra) };
			for (int cellQuadrupleIndex = 0; cellQuadrupleIndex < 2; cellQuadrupleIndex++)
			{
				var (abx, aby, abw, abz) = cellQuadruples[cellQuadrupleIndex];
				var link2Map = Cells.Empty + aby + abw;
				if (!IUniqueRectangleStepSearcher.IsConjugatePair(a, link2Map, link2Map.CoveredLine))
				{
					continue;
				}

				var link3Map1 = Cells.Empty + abw + abz;
				var link3Map2 = Cells.Empty + abx + abz;
				innerMaps[0] = link3Map1;
				innerMaps[1] = link3Map2;
				for (int i = 0; i < 2; i++)
				{
					var linkMap = innerMaps[i];
					if (!IUniqueRectangleStepSearcher.IsConjugatePair(b, link3Map1, link3Map1.CoveredLine))
					{
						continue;
					}

					if (grid.Exists(aby, b) is not true)
					{
						continue;
					}

					var candidateOffsets = new List<CandidateViewNode>(7);
					foreach (int d in grid.GetCandidates(abx))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(
								new(
									i == 0
										? d == a
											? DisplayColorKind.Auxiliary1
											: DisplayColorKind.Normal
										: DisplayColorKind.Auxiliary1,
									abx * 9 + d
								)
							);
						}
					}
					foreach (int d in grid.GetCandidates(abz))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(
								new(d == b ? DisplayColorKind.Auxiliary1 : DisplayColorKind.Normal, abz * 9 + d)
							);
						}
					}
					foreach (int d in grid.GetCandidates(aby))
					{
						if ((d == d1 || d == d2) && d != b)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, aby * 9 + d));
						}
					}
					foreach (int d in grid.GetCandidates(abw))
					{
						if (d == d1 || d == d2)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, abw * 9 + d));
						}
					}
					if (!AllowIncompleteUniqueRectangles && candidateOffsets.Count != 7)
					{
						continue;
					}

					int[] offsets = linkMap.ToArray();
					var conjugatePairs = new Conjugate[]
					{
						new(abx, aby, a),
						new(aby, abw, a),
						new(offsets[0], offsets[1], b)
					};
					accumulator.Add(
						new UniqueRectangleWithConjugatePairStep(
							ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, aby, b)),
							ImmutableArray.Create(
								View.Empty
									| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
									| candidateOffsets
									| new HouseViewNode[]
									{
										new(DisplayColorKind.Normal, conjugatePairs[0].Line),
										new(DisplayColorKind.Normal, conjugatePairs[1].Line),
										new(DisplayColorKind.Auxiliary1, conjugatePairs[2].Line)
									}
							),
							Technique.UniqueRectangle4C3,
							d1,
							d2,
							(Cells)urCells,
							arMode,
							conjugatePairs,
							index
						)
					);
				}
			}
		}
	}

	/// <summary>
	/// Check UR-XY-Wing, UR-XYZ-Wing, UR-WXYZ-Wing and AR-XY-Wing, AR-XYZ-Wing and AR-WXYZ-Wing.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="size">The size of the wing to search.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )  abxy  yz  xz
	/// (ab )  abxy  *
	///   ↑ corner2
	/// ]]></code>
	/// Note that the pair of cells <c>abxy</c> should be in the same house.
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	///   ↓ corner1
	/// (ab )  abx   xz
	///  aby  (ab )  *   yz
	///         ↑ corner2
	/// ]]></code>
	/// </para>
	/// </remarks>
	partial void CheckRegularWing(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells,
		bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in Cells otherCellsMap, int size, int index)
	{
		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer)
		{
			return;
		}

		if ((Cells.Empty + corner1 + corner2).AllSetsAreInOneHouse(out int house) && house < 9)
		{
			// Subtype 1.
			int[] offsets = otherCellsMap.ToArray();
			int otherCell1 = offsets[0], otherCell2 = offsets[1];
			short mask1 = grid.GetCandidates(otherCell1);
			short mask2 = grid.GetCandidates(otherCell2);
			short mask = (short)(mask1 | mask2);

			if (PopCount((uint)mask) != 2 + size || (mask & comparer) != comparer
				|| mask1 == comparer || mask2 == comparer)
			{
				return;
			}

			var map = (PeerMaps[otherCell1] | PeerMaps[otherCell2]) & BivalueCells;
			if (map.Count < size)
			{
				return;
			}

			var testMap = !(Cells.Empty + otherCell1 + otherCell2);
			short extraDigitsMask = (short)(mask ^ comparer);
			int[] cells = map.ToArray();
			for (int i1 = 0, length = cells.Length, outerLength = length - size + 1; i1 < outerLength; i1++)
			{
				int c1 = cells[i1];
				short m1 = grid.GetCandidates(c1);
				if ((m1 & ~extraDigitsMask) == 0)
				{
					continue;
				}

				for (int i2 = i1 + 1, lengthMinusSizePlus2 = length - size + 2; i2 < lengthMinusSizePlus2; i2++)
				{
					int c2 = cells[i2];
					short m2 = grid.GetCandidates(c2);
					if ((m2 & ~extraDigitsMask) == 0)
					{
						continue;
					}

					if (size == 2)
					{
						// Check XY-Wing.
						short m = (short)((short)(m1 | m2) ^ extraDigitsMask);
						if ((PopCount((uint)m), PopCount((uint)(m1 & m2))) != (1, 1))
						{
							continue;
						}

						// Now check whether all cells found should see their corresponding
						// cells in UR structure ('otherCells1' or 'otherCells2').
						bool flag = true;
						int* combinationCells = stackalloc[] { c1, c2 };
						for (int cellIndex = 0; cellIndex < 2; cellIndex++)
						{
							int cell = combinationCells[cellIndex];
							int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
							if (!(testMap & CandidatesMap[extraDigit]).Contains(cell))
							{
								flag = false;
								break;
							}
						}
						if (!flag)
						{
							continue;
						}

						// Now check eliminations.
						int elimDigit = TrailingZeroCount(m);
						if ((!(Cells.Empty + c1 + c2) & CandidatesMap[elimDigit]) is not { Count: not 0 } elimMap)
						{
							continue;
						}

						var candidateOffsets = new List<CandidateViewNode>(12);
						foreach (int cell in urCells)
						{
							if (grid.GetStatus(cell) == CellStatus.Empty)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(
										new(
											digit == elimDigit
												? otherCellsMap.Contains(cell)
													? DisplayColorKind.Auxiliary2
													: DisplayColorKind.Normal
												: (extraDigitsMask >> digit & 1) != 0
													? DisplayColorKind.Auxiliary1
													: DisplayColorKind.Normal,
											cell * 9 + digit
										)
									);
								}
							}
						}
						foreach (int digit in grid.GetCandidates(c1))
						{
							candidateOffsets.Add(
								new(
									digit == elimDigit ? DisplayColorKind.Auxiliary2 : DisplayColorKind.Auxiliary1,
									c1 * 9 + digit
								)
							);
						}
						foreach (int digit in grid.GetCandidates(c2))
						{
							candidateOffsets.Add(
								new(
									digit == elimDigit ? DisplayColorKind.Auxiliary2 : DisplayColorKind.Auxiliary1,
									c2 * 9 + digit
								)
							);
						}
						if (IsIncomplete(candidateOffsets))
						{
							return;
						}

						accumulator.Add(
							new UniqueRectangleWithWingStep(
								ImmutableArray.Create(
									Conclusion.ToConclusions(elimMap, elimDigit, ConclusionType.Elimination)
								),
								ImmutableArray.Create(
									View.Empty
										| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
										| candidateOffsets
								),
								arMode
									? Technique.AvoidableRectangleXyWing
									: Technique.UniqueRectangleXyWing,
								d1,
								d2,
								(Cells)urCells,
								arMode,
								Cells.Empty + c1 + c2,
								otherCellsMap,
								extraDigitsMask,
								index
							)
						);
					}
					else // size > 2
					{
						for (
							int i3 = i2 + 1, lengthMinusSizePlus3 = length - size + 3;
							i3 < lengthMinusSizePlus3;
							i3++
						)
						{
							int c3 = cells[i3];
							short m3 = grid.GetCandidates(c3);
							if ((m3 & ~extraDigitsMask) == 0)
							{
								continue;
							}

							if (size == 3)
							{
								// Check XYZ-Wing.
								short m = (short)(((short)(m1 | m2) | m3) ^ extraDigitsMask);
								if ((PopCount((uint)m), PopCount((uint)(m1 & m2 & m3))) != (1, 1))
								{
									continue;
								}

								// Now check whether all cells found should see their corresponding
								// cells in UR structure ('otherCells1' or 'otherCells2').
								bool flag = true;
								int* combinationCells = stackalloc[] { c1, c2, c3 };
								for (int cellIndex = 0; cellIndex < 3; cellIndex++)
								{
									int cell = combinationCells[cellIndex];
									int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
									if (!(testMap & CandidatesMap[extraDigit]).Contains(cell))
									{
										flag = false;
										break;
									}
								}
								if (!flag)
								{
									continue;
								}

								// Now check eliminations.
								int elimDigit = TrailingZeroCount(m);
								var elimMap = !(Cells.Empty + c1 + c2 + c3) & CandidatesMap[elimDigit];
								if (elimMap is [])
								{
									continue;
								}

								var candidateOffsets = new List<CandidateViewNode>();
								foreach (int cell in urCells)
								{
									if (grid.GetStatus(cell) == CellStatus.Empty)
									{
										foreach (int digit in grid.GetCandidates(cell))
										{
											candidateOffsets.Add(
												new(
													(extraDigitsMask >> digit & 1) != 0
														? DisplayColorKind.Auxiliary1
														: DisplayColorKind.Normal,
													cell * 9 + digit
												)
											);
										}
									}
								}
								foreach (int digit in grid.GetCandidates(c1))
								{
									candidateOffsets.Add(
										new(
											digit == elimDigit
												? DisplayColorKind.Auxiliary2
												: DisplayColorKind.Auxiliary1,
											c1 * 9 + digit
										)
									);
								}
								foreach (int digit in grid.GetCandidates(c2))
								{
									candidateOffsets.Add(
										new(
											digit == elimDigit
												? DisplayColorKind.Auxiliary2
												: DisplayColorKind.Auxiliary1,
											c2 * 9 + digit
										)
									);
								}
								foreach (int digit in grid.GetCandidates(c3))
								{
									candidateOffsets.Add(
										new(
											digit == elimDigit
												? DisplayColorKind.Auxiliary2
												: DisplayColorKind.Auxiliary1,
											c3 * 9 + digit
										)
									);
								}
								if (IsIncomplete(candidateOffsets))
								{
									return;
								}

								accumulator.Add(
									new UniqueRectangleWithWingStep(
										ImmutableArray.Create(
											Conclusion.ToConclusions(
												elimMap,
												elimDigit,
												ConclusionType.Elimination
											)
										),
										ImmutableArray.Create(
											View.Empty
												| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
												| candidateOffsets
										),
										arMode
											? Technique.AvoidableRectangleXyzWing
											: Technique.UniqueRectangleXyzWing,
										d1,
										d2,
										(Cells)urCells,
										arMode,
										Cells.Empty + c1 + c2 + c3,
										otherCellsMap,
										extraDigitsMask,
										index
									)
								);
							}
							else // size == 4
							{
								for (int i4 = i3 + 1; i4 < length; i4++)
								{
									int c4 = cells[i4];
									short m4 = grid.GetCandidates(c4);
									if ((m4 & ~extraDigitsMask) == 0)
									{
										continue;
									}

									// Check WXYZ-Wing.
									short m = (short)((short)((short)((short)(m1 | m2) | m3) | m4) ^ extraDigitsMask);
									if ((PopCount((uint)m), PopCount((uint)(m1 & m2 & m3 & m4))) != (1, 1))
									{
										continue;
									}

									// Now check whether all cells found should see their corresponding
									// cells in UR structure ('otherCells1' or 'otherCells2').
									bool flag = true;
									int* combinationCells = stackalloc[] { c1, c2, c3, c4 };
									for (int cellIndex = 0; cellIndex < 4; cellIndex++)
									{
										int cell = combinationCells[cellIndex];
										int extraDigit = TrailingZeroCount(grid.GetCandidates(cell) & ~m);
										if (!(testMap & CandidatesMap[extraDigit]).Contains(cell))
										{
											flag = false;
											break;
										}
									}
									if (!flag)
									{
										continue;
									}

									// Now check eliminations.
									int elimDigit = TrailingZeroCount(m);
									var elimMap = !(Cells.Empty + c1 + c2 + c3 + c4) & CandidatesMap[elimDigit];
									if (elimMap is [])
									{
										continue;
									}

									var candidateOffsets = new List<CandidateViewNode>();
									foreach (int cell in urCells)
									{
										if (grid.GetStatus(cell) == CellStatus.Empty)
										{
											foreach (int digit in grid.GetCandidates(cell))
											{
												candidateOffsets.Add(
													new(
														(extraDigitsMask >> digit & 1) != 0
															? DisplayColorKind.Auxiliary1
															: DisplayColorKind.Normal,
														cell * 9 + digit
													)
												);
											}
										}
									}
									foreach (int digit in grid.GetCandidates(c1))
									{
										candidateOffsets.Add(
											new(
												digit == elimDigit
													? DisplayColorKind.Auxiliary2
													: DisplayColorKind.Auxiliary1,
												c1 * 9 + digit
											)
										);
									}
									foreach (int digit in grid.GetCandidates(c2))
									{
										candidateOffsets.Add(
											new(
												digit == elimDigit
													? DisplayColorKind.Auxiliary2
													: DisplayColorKind.Auxiliary1,
												c2 * 9 + digit
											)
										);
									}
									foreach (int digit in grid.GetCandidates(c3))
									{
										candidateOffsets.Add(
											new(
												digit == elimDigit
													? DisplayColorKind.Auxiliary2
													: DisplayColorKind.Auxiliary1,
												c3 * 9 + digit
											)
										);
									}
									foreach (int digit in grid.GetCandidates(c4))
									{
										candidateOffsets.Add(
											new(
												digit == elimDigit
													? DisplayColorKind.Auxiliary2
													: DisplayColorKind.Auxiliary1,
												c4 * 9 + digit
											)
										);
									}
									if (IsIncomplete(candidateOffsets))
									{
										return;
									}

									accumulator.Add(
										new UniqueRectangleWithWingStep(
											ImmutableArray.Create(
												Conclusion.ToConclusions(
													elimMap,
													elimDigit,
													ConclusionType.Elimination
												)
											),
											ImmutableArray.Create(
												View.Empty
													| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
													| candidateOffsets
											),
											arMode
												? Technique.AvoidableRectangleWxyzWing
												: Technique.UniqueRectangleWxyzWing,
											d1,
											d2,
											(Cells)urCells,
											arMode,
											Cells.Empty + c1 + c2 + c3 + c4,
											otherCellsMap,
											extraDigitsMask,
											index
										)
									);
								}
							}
						}
					}
				}
			}
		}
		else
		{
			// TODO: Finish processing Subtype 2.
		}
	}

	/// <summary>
	/// Check UR+SdC.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="arMode">Indicates whether the current mode is AR mode.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">The map of other cells during the current UR searching.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// The structure:
	/// <code><![CDATA[
	///           |   xyz
	///  ab+ ab+  | abxyz abxyz
	///           |   xyz
	/// ----------+------------
	/// (ab)(ab)  |
	///  ↑ corner1, corner2
	/// ]]></code>
	/// </remarks>
	partial void CheckSueDeCoq(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells,
		bool arMode, short comparer, int d1, int d2, int corner1, int corner2,
		scoped in Cells otherCellsMap, int index)
	{
		bool notSatisfiedType3 = false;
		short mergedMaskInOtherCells = 0;
		foreach (int cell in otherCellsMap)
		{
			short currentMask = grid.GetCandidates(cell);
			mergedMaskInOtherCells |= currentMask;
			if ((currentMask & comparer) == 0
				|| currentMask == comparer || arMode && grid.GetStatus(cell) != CellStatus.Empty)
			{
				notSatisfiedType3 = true;
				break;
			}
		}

		if ((grid.GetCandidates(corner1) | grid.GetCandidates(corner2)) != comparer || notSatisfiedType3
			|| (mergedMaskInOtherCells & comparer) != comparer)
		{
			return;
		}

		// Check whether the corners spanned two blocks. If so, UR+SdC can't be found.
		short blockMaskInOtherCells = otherCellsMap.BlockMask;
		if (!IsPow2(blockMaskInOtherCells))
		{
			return;
		}

		bool* cannibalModeCases = stackalloc[] { false, true };

		short otherDigitsMask = (short)(mergedMaskInOtherCells & ~comparer);
		byte line = (byte)otherCellsMap.CoveredLine;
		byte block = (byte)TrailingZeroCount(otherCellsMap.CoveredHouses & ~(1 << line));
		var (a, _, _, d) = IntersectionMaps[(line, block)];
		var list = new ValueList<Cells>(4);
		for (int caseIndex = 0; caseIndex < 2; caseIndex++)
		{
			bool cannibalMode = cannibalModeCases[caseIndex];
			foreach (byte otherBlock in d)
			{
				var emptyCellsInInterMap = HouseMaps[otherBlock] & HouseMaps[line] & EmptyCells;
				if (emptyCellsInInterMap.Count < 2)
				{
					// The intersection needs at least two empty cells.
					continue;
				}

				Cells b = HouseMaps[otherBlock] - HouseMaps[line], c = a & b;

				list.Clear();
				switch (emptyCellsInInterMap)
				{
					case [_, _]:
					{
						list.Add(emptyCellsInInterMap);

						break;
					}
					case [var i, var j, var k]:
					{
						list.Add(Cells.Empty + i + j);
						list.Add(Cells.Empty + j + k);
						list.Add(Cells.Empty + i + k);
						list.Add(emptyCellsInInterMap);

						break;
					}
				}

				// Iterate on each intersection combination.
				foreach (var currentInterMap in list)
				{
					short selectedInterMask = grid.GetDigitsUnion(currentInterMap);
					if (PopCount((uint)selectedInterMask) <= currentInterMap.Count + 1)
					{
						// The intersection combination is an ALS or a normal subset,
						// which is invalid in SdCs.
						continue;
					}

					var blockMap = (b | c - currentInterMap) & EmptyCells;
					var lineMap = (a & EmptyCells) - otherCellsMap;

					// Iterate on the number of the cells that should be selected in block.
					for (int i = 1; i <= blockMap.Count - 1; i++)
					{
						// Iterate on each combination in block.
						foreach (var selectedCellsInBlock in blockMap & i)
						{
							bool flag = false;
							foreach (int digit in otherDigitsMask)
							{
								foreach (int cell in selectedCellsInBlock)
								{
									if (grid.Exists(cell, digit) is true)
									{
										flag = true;
										break;
									}
								}
							}
							if (flag)
							{
								continue;
							}

							var currentBlockMap = selectedCellsInBlock;
							var elimMapBlock = Cells.Empty;
							var elimMapLine = Cells.Empty;

							// Get the links of the block.
							short blockMask = grid.GetDigitsUnion(selectedCellsInBlock);

							// Get the elimination map in the block.
							foreach (int digit in blockMask)
							{
								elimMapBlock |= CandidatesMap[digit];
							}
							elimMapBlock &= blockMap - currentBlockMap;

							foreach (int digit in otherDigitsMask)
							{
								elimMapLine |= CandidatesMap[digit];
							}
							elimMapLine &= lineMap - currentInterMap;

							checkGeneralizedSdc(
								accumulator, grid, arMode, cannibalMode, d1, d2, urCells,
								line, otherBlock, otherDigitsMask, blockMask, selectedInterMask,
								otherDigitsMask, elimMapLine, elimMapBlock, otherCellsMap, currentBlockMap,
								currentInterMap, i, 0, index
							);
						}
					}
				}
			}
		}


		static void checkGeneralizedSdc(
			ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
			bool arMode, bool cannibalMode, int digit1, int digit2, int[] urCells,
			int line, int block, short lineMask, short blockMask,
			short selectedInterMask, short otherDigitsMask, scoped in Cells elimMapLine,
			scoped in Cells elimMapBlock, scoped in Cells currentLineMap, scoped in Cells currentBlockMap,
			scoped in Cells currentInterMap, int i, int j, int index)
		{
			short maskOnlyInInter = (short)(selectedInterMask & ~(blockMask | lineMask));
			short maskIsolated = (short)(
				cannibalMode ? (lineMask & blockMask & selectedInterMask) : maskOnlyInInter
			);
			if (
				!cannibalMode && (
					(blockMask & lineMask) != 0
					|| maskIsolated != 0 && !IsPow2(maskIsolated)
				) || cannibalMode && !IsPow2(maskIsolated)
			)
			{
				return;
			}

			var elimMapIsolated = Cells.Empty;
			int digitIsolated = TrailingZeroCount(maskIsolated);
			if (digitIsolated != InvalidFirstSet)
			{
				elimMapIsolated = (
					cannibalMode
						? currentBlockMap | currentLineMap
						: currentInterMap
				) % CandidatesMap[digitIsolated] & EmptyCells;
			}

			if (currentInterMap.Count + i + j + 1 == PopCount((uint)blockMask) + PopCount((uint)lineMask) + PopCount((uint)maskOnlyInInter)
				&& (elimMapBlock | elimMapLine | elimMapIsolated) is not [])
			{
				// Check eliminations.
				var conclusions = new List<Conclusion>(10);
				foreach (int cell in elimMapBlock)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						if ((blockMask >> digit & 1) != 0)
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
				}
				foreach (int cell in elimMapLine)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						if ((lineMask >> digit & 1) != 0)
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
				}
				foreach (int cell in elimMapIsolated)
				{
					conclusions.Add(new(ConclusionType.Elimination, cell, digitIsolated));
				}
				if (conclusions.Count == 0)
				{
					return;
				}

				// Record highlight candidates and cells.
				var candidateOffsets = new List<CandidateViewNode>();
				foreach (int cell in urCells)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								(otherDigitsMask >> digit & 1) != 0
									? DisplayColorKind.Auxiliary1
									: DisplayColorKind.Normal,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (int cell in currentBlockMap)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								!cannibalMode && digit == digitIsolated
									? DisplayColorKind.Auxiliary3
									: DisplayColorKind.Auxiliary2,
								cell * 9 + digit
							)
						);
					}
				}
				foreach (int cell in currentInterMap)
				{
					foreach (int digit in grid.GetCandidates(cell))
					{
						candidateOffsets.Add(
							new(
								digitIsolated == digit
									? DisplayColorKind.Auxiliary3
									: (otherDigitsMask >> digit & 1) != 0
										? DisplayColorKind.Auxiliary1
										: DisplayColorKind.Auxiliary2,
								cell * 9 + digit
							)
						);
					}
				}

				accumulator.Add(
					new UniqueRectangleWithSueDeCoqStep(
						ImmutableArray.CreateRange(conclusions),
						ImmutableArray.Create(
							View.Empty
								| (arMode ? IUniqueRectangleStepSearcher.GetHighlightCells(urCells) : null)
								| candidateOffsets
								| new HouseViewNode[]
								{
									new(DisplayColorKind.Normal, block),
									new(DisplayColorKind.Auxiliary2, line)
								}
						),
						digit1,
						digit2,
						(Cells)urCells,
						arMode,
						block,
						line,
						blockMask,
						lineMask,
						selectedInterMask,
						cannibalMode,
						maskIsolated,
						currentBlockMap,
						currentLineMap,
						currentInterMap,
						index
					)
				);
			}
		}
	}

	/// <summary>
	/// Check UR+Unknown covering.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The comparer.</param>
	/// <param name="d1">The digit 1.</param>
	/// <param name="d2">The digit 2.</param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structures:</para>
	/// <para>
	/// Subtype 1:
	/// <code><![CDATA[
	///      ↓urCellInSameBlock
	/// ab  abc      abc  ←anotherCell
	///
	///     abcx-----abcy ←resultCell
	///           c
	///      ↑targetCell
	/// ]]></code>
	/// Where the digit <c>a</c> and <c>b</c> in the bottom-left cell <c>abcx</c> can be removed.
	/// </para>
	/// <para>
	/// Subtype 2:
	/// <code><![CDATA[
	/// abcx   | ab  abc
	///  |     |
	///  | c   |
	///  |     |
	/// abcy   |     abc
	/// ]]></code>
	/// </para>
	/// </remarks>
	partial void CheckUnknownCoveringUnique(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid, int[] urCells,
		short comparer, int d1, int d2, int index)
	{
		checkType1(grid);
#if IMPLEMENTED
		checkType2(grid);
#endif

		void checkType1(scoped in Grid grid)
		{
			var cells = (Cells)urCells;

			// Check all cells are empty.
			bool containsValueCells = false;
			foreach (int cell in cells)
			{
				if (grid.GetStatus(cell) != CellStatus.Empty)
				{
					containsValueCells = true;
					break;
				}
			}
			if (containsValueCells)
			{
				return;
			}

			// Iterate on each cell.
			foreach (int targetCell in cells)
			{
				int block = targetCell.ToHouseIndex(HouseType.Block);
				var bivalueCellsToCheck = (PeerMaps[targetCell] & HouseMaps[block] & BivalueCells) - cells;
				if (bivalueCellsToCheck is [])
				{
					continue;
				}

				// Check all bi-value cells.
				foreach (int bivalueCellToCheck in bivalueCellsToCheck)
				{
					if ((Cells.Empty + bivalueCellToCheck + targetCell).CoveredLine != InvalidFirstSet)
					{
						// 'targetCell' and 'bivalueCellToCheck' can't lie on a same line.
						continue;
					}

					if (grid.GetCandidates(bivalueCellToCheck) != comparer)
					{
						// 'bivalueCell' must contain both 'd1' and 'd2'.
						continue;
					}

					int urCellInSameBlock = ((HouseMaps[block] & cells) - targetCell)[0];
					int coveredLine = (Cells.Empty + bivalueCellToCheck + urCellInSameBlock).CoveredLine;
					if (coveredLine == InvalidFirstSet)
					{
						// The bi-value cell 'bivalueCellToCheck' should be lie on a same house
						// as 'urCellInSameBlock'.
						continue;
					}

					int anotherCell = (cells - urCellInSameBlock & HouseMaps[coveredLine])[0];
					foreach (int extraDigit in grid.GetCandidates(targetCell) & ~comparer)
					{
						short abcMask = (short)(comparer | (short)(1 << extraDigit));

						if (grid.GetCandidates(anotherCell) != abcMask)
						{
							continue;
						}

						// Check the conjugate pair of the extra digit.
						int resultCell = (cells - urCellInSameBlock - anotherCell - targetCell)[0];
						var map = Cells.Empty + targetCell + resultCell;
						int line = map.CoveredLine;
						if (!IUniqueRectangleStepSearcher.IsConjugatePair(extraDigit, map, line))
						{
							continue;
						}

						if (grid.GetCandidates(urCellInSameBlock) != abcMask)
						{
							goto SubType2;
						}

						// Here, is the basic sub-type having passed the checking.
						// Gather conclusions.
						var conclusions = new List<Conclusion>();
						foreach (int digit in grid.GetCandidates(targetCell))
						{
							if (digit == d1 || digit == d2)
							{
								conclusions.Add(new(ConclusionType.Elimination, targetCell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							goto SubType2;
						}

						// Gather views.
						var candidateOffsets = new List<CandidateViewNode>
						{
							new(DisplayColorKind.Auxiliary1, targetCell * 9 + extraDigit)
						};
						if (grid.Exists(resultCell, d1) is true)
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, resultCell * 9 + d1));
						}
						if (grid.Exists(resultCell, d2) is true)
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, resultCell * 9 + d2));
						}
						if (grid.Exists(resultCell, extraDigit) is true)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, resultCell * 9 + extraDigit));
						}

						foreach (int digit in grid.GetCandidates(urCellInSameBlock) & abcMask)
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, urCellInSameBlock * 9 + digit));
						}
						foreach (int digit in grid.GetCandidates(anotherCell))
						{
							candidateOffsets.Add(new(DisplayColorKind.Normal, anotherCell * 9 + digit));
						}
						short _xOr_yMask = grid.GetCandidates(bivalueCellToCheck);
						foreach (int digit in _xOr_yMask)
						{
							candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, bivalueCellToCheck * 9 + digit));
						}

						// Add into the list.
						byte extraDigitId = (byte)(char)(extraDigit + '1');
						short extraDigitMask = (short)(1 << extraDigit);
						accumulator.Add(
							new UniqueRectangleWithUnknownCoveringStep(
								ImmutableArray.CreateRange(conclusions),
								ImmutableArray.Create(
									View.Empty
										| new CellViewNode(DisplayColorKind.Normal, targetCell)
										| candidateOffsets
										| new HouseViewNode[]
										{
											new(DisplayColorKind.Normal, block),
											new(DisplayColorKind.Auxiliary1, line)
										},
									View.Empty
										| new CandidateViewNode[]
										{
											new(DisplayColorKind.Auxiliary1, resultCell * 9 + extraDigit),
											new(DisplayColorKind.Auxiliary1, targetCell * 9 + extraDigit)
										}
										| new UnknownViewNode[]
										{
											new(DisplayColorKind.Normal, bivalueCellToCheck, (byte)'y', _xOr_yMask),
											new(DisplayColorKind.Normal, targetCell, (byte)'x', _xOr_yMask),
											new(DisplayColorKind.Normal, urCellInSameBlock, extraDigitId, extraDigitMask),
											new(DisplayColorKind.Normal, anotherCell, (byte)'x', _xOr_yMask),
											new(DisplayColorKind.Normal, resultCell, extraDigitId, extraDigitMask)
										}
								),
								d1,
								d2,
								(Cells)urCells,
								targetCell,
								extraDigit,
								index
							)
						);

					SubType2:
						// Sub-type 2.
						// The extra digit should form a conjugate pair in that line.
						var anotherMap = Cells.Empty + urCellInSameBlock + anotherCell;
						int anotherLine = anotherMap.CoveredLine;
						if (!IUniqueRectangleStepSearcher.IsConjugatePair(extraDigit, anotherMap, anotherLine))
						{
							continue;
						}

						// Gather conclusions.
						var conclusionsAnotherSubType = new List<Conclusion>();
						foreach (int digit in grid.GetCandidates(targetCell))
						{
							if (digit == d1 || digit == d2)
							{
								conclusionsAnotherSubType.Add(new(ConclusionType.Elimination, targetCell, digit));
							}
						}
						if (conclusionsAnotherSubType.Count == 0)
						{
							continue;
						}

						// Gather views.
						var candidateOffsetsAnotherSubtype = new List<CandidateViewNode>
						{
							new(DisplayColorKind.Auxiliary1, targetCell * 9 + extraDigit)
						};
						if (grid.Exists(resultCell, d1) is true)
						{
							candidateOffsetsAnotherSubtype.Add(new(DisplayColorKind.Normal, resultCell * 9 + d1));
						}
						if (grid.Exists(resultCell, d2) is true)
						{
							candidateOffsetsAnotherSubtype.Add(new(DisplayColorKind.Normal, resultCell * 9 + d2));
						}
						if (grid.Exists(resultCell, extraDigit) is true)
						{
							candidateOffsetsAnotherSubtype.Add(
								new(DisplayColorKind.Auxiliary1, resultCell * 9 + extraDigit)
							);
						}

						var candidateOffsetsAnotherSubtypeLighter = new List<CandidateViewNode>
						{
							new(DisplayColorKind.Auxiliary1, resultCell * 9 + extraDigit),
							new(DisplayColorKind.Auxiliary1, targetCell * 9 + extraDigit)
						};
						foreach (int digit in grid.GetCandidates(urCellInSameBlock) & abcMask)
						{
							if (digit == extraDigit)
							{
								candidateOffsetsAnotherSubtype.Add(
									new(DisplayColorKind.Auxiliary1, urCellInSameBlock * 9 + digit)
								);
								candidateOffsetsAnotherSubtypeLighter.Add(
									new(DisplayColorKind.Auxiliary1, urCellInSameBlock * 9 + digit)
								);
							}
							else
							{
								candidateOffsetsAnotherSubtype.Add(
									new(DisplayColorKind.Normal, urCellInSameBlock * 9 + digit)
								);
							}
						}
						foreach (int digit in grid.GetCandidates(anotherCell))
						{
							if (digit == extraDigit)
							{
								candidateOffsetsAnotherSubtype.Add(
									new(DisplayColorKind.Auxiliary1, anotherCell * 9 + digit)
								);
								candidateOffsetsAnotherSubtypeLighter.Add(
									new(DisplayColorKind.Auxiliary1, anotherCell * 9 + digit)
								);
							}
							else
							{
								candidateOffsetsAnotherSubtype.Add(
									new(DisplayColorKind.Normal, anotherCell * 9 + digit)
								);
							}
						}
						short _xOr_yMask2 = grid.GetCandidates(bivalueCellToCheck);
						foreach (int digit in _xOr_yMask2)
						{
							candidateOffsetsAnotherSubtype.Add(
								new(DisplayColorKind.Auxiliary2, bivalueCellToCheck * 9 + digit)
							);
						}

						// Add into the list.
						byte extraDigitId2 = (byte)(char)(extraDigit + '1');
						short extraDigitMask2 = (short)(1 << extraDigit);
						accumulator.Add(
							new UniqueRectangleWithUnknownCoveringStep(
								ImmutableArray.CreateRange(conclusionsAnotherSubType),
								ImmutableArray.Create(
									View.Empty
										| new CellViewNode(DisplayColorKind.Normal, targetCell)
										| candidateOffsetsAnotherSubtype
										| new HouseViewNode[]
										{
											new(DisplayColorKind.Normal, block),
											new(DisplayColorKind.Auxiliary1, line),
											new(DisplayColorKind.Auxiliary1, anotherLine)
										},
									View.Empty
										| candidateOffsetsAnotherSubtypeLighter
										| new UnknownViewNode[]
										{
											new(DisplayColorKind.Normal, bivalueCellToCheck, (byte)'y', _xOr_yMask2),
											new(DisplayColorKind.Normal, targetCell, (byte)'x', _xOr_yMask2),
											new(DisplayColorKind.Normal, urCellInSameBlock, extraDigitId2, extraDigitMask2),
											new(DisplayColorKind.Normal, anotherCell, (byte)'x', _xOr_yMask2),
											new(DisplayColorKind.Normal, resultCell, extraDigitId2, extraDigitMask2)
										}
								),
								d1,
								d2,
								(Cells)urCells,
								targetCell,
								extraDigit,
								index
							)
						);
					}
				}
			}
		}

#if false
		void checkType2(scoped in Grid grid)
		{
			// TODO: Check type 2.
		}
#endif
	}

	/// <summary>
	/// Check UR+Guardian.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The index.</param>
	partial void CheckExternalType2(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, int d1, int d2, int index)
	{
		var cells = (Cells)urCells;

		if (!IUniqueRectangleStepSearcher.CheckPreconditionsOnIncomplete(grid, urCells, d1, d2))
		{
			return;
		}

		// Iterate on two houses used.
		foreach (int[] houseCombination in cells.Houses.GetAllSets().GetSubsets(2))
		{
			var houseCells = HouseMaps[houseCombination[0]] | HouseMaps[houseCombination[1]];
			if ((houseCells & cells) != cells)
			{
				// The houses must contain all 4 UR cells.
				continue;
			}

			var guardian1 = houseCells - cells & CandidatesMap[d1];
			var guardian2 = houseCells - cells & CandidatesMap[d2];
			if (!(guardian1 is [] ^ guardian2 is []))
			{
				// Only one digit can contain guardians.
				continue;
			}

			int guardianDigit = -1;
			Cells? targetElimMap = null, targetGuardianMap = null;
			if (guardian1 is not [] && (!guardian1 & CandidatesMap[d1]) is { Count: not 0 } a)
			{
				targetElimMap = a;
				guardianDigit = d1;
				targetGuardianMap = guardian1;
			}
			else if (guardian2 is not [] && (!guardian2 & CandidatesMap[d2]) is { Count: not 0 } b)
			{
				targetElimMap = b;
				guardianDigit = d2;
				targetGuardianMap = guardian2;
			}

			if (targetElimMap is not { } elimMap || targetGuardianMap is not { } guardianMap
				|| guardianDigit == -1)
			{
				continue;
			}

			var candidateOffsets = new List<CandidateViewNode>(16);
			foreach (int cell in urCells)
			{
				if (grid.Exists(cell, d1) is true)
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d1));
				}
				if (grid.Exists(cell, d2) is true)
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d2));
				}
			}
			foreach (int cell in guardianMap)
			{
				candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + guardianDigit));
			}

			accumulator.Add(
				new UniqueRectangleWithGuardianStep(
					ImmutableArray.Create(
						Conclusion.ToConclusions(elimMap, guardianDigit, ConclusionType.Elimination)
					),
					ImmutableArray.Create(
						View.Empty
							| candidateOffsets
							| new HouseViewNode[]
							{
								new(DisplayColorKind.Normal, houseCombination[0]),
								new(DisplayColorKind.Normal, houseCombination[1])
							}
					),
					d1,
					d2,
					(Cells)urCells,
					guardianMap,
					guardianDigit,
					IsIncomplete(candidateOffsets),
					index
				)
			);
		}
	}

	/// <summary>
	/// Check UR+Guardian, with the external subset.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="comparer">The mask comparer.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="index">The index.</param>
	partial void CheckExternalType3(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, short comparer, int d1, int d2, int index)
	{
		if (!IUniqueRectangleStepSearcher.CheckPreconditionsOnIncomplete(grid, urCells, d1, d2))
		{
			return;
		}

		var cells = (Cells)urCells;

		// Iterate on two houses used.
		foreach (int[] houseCombination in cells.Houses.GetAllSets().GetSubsets(2))
		{
			var guardianMap = HouseMaps[houseCombination[0]] | HouseMaps[houseCombination[1]];
			if ((guardianMap & cells) != cells)
			{
				// The houses must contain all 4 UR cells.
				continue;
			}

			var guardianCells = guardianMap - cells & EmptyCells;
			foreach (var guardianCellPair in guardianCells & 2)
			{
				int c1 = guardianCellPair[0], c2 = guardianCellPair[1];
				if (!IUniqueRectangleStepSearcher.IsSameHouseCell(c1, c2, out int houses))
				{
					// Those two cells must lie in a same house.
					continue;
				}

				short mask = (short)(grid.GetCandidates(c1) | grid.GetCandidates(c2));
				if ((mask & comparer) != comparer)
				{
					// The two cells must contain both two digits.
					continue;
				}

				if ((grid.GetCandidates(c1) & comparer) == 0 || (grid.GetCandidates(c2) & comparer) == 0)
				{
					// Both two cells chosen must contain at least one of two UR digits.
					continue;
				}

				if ((guardianCells & (CandidatesMap[d1] | CandidatesMap[d2])) != guardianCellPair)
				{
					// The current map must be equal to the whole guardian full map.
					continue;
				}

				foreach (int house in houses)
				{
					var houseCells = HouseMaps[house] - cells - guardianCellPair & EmptyCells;
					for (int size = 2; size <= houseCells.Count; size++)
					{
						foreach (var otherCells in houseCells & size - 1)
						{
							short subsetDigitsMask = (short)(grid.GetDigitsUnion(otherCells) | comparer);
							if (PopCount((uint)subsetDigitsMask) != size)
							{
								// The subset cannot formed.
								continue;
							}

							// UR Guardian External Subsets found. Now check eliminations.
							var elimMap = (houseCells | guardianCellPair) - otherCells;
							var conclusions = new List<Conclusion>();
							foreach (int cell in elimMap)
							{
								short elimDigitsMask = guardianCellPair.Contains(cell)
									? (short)(subsetDigitsMask & ~comparer)
									: subsetDigitsMask;

								foreach (int digit in elimDigitsMask)
								{
									if (CandidatesMap[digit].Contains(cell))
									{
										conclusions.Add(new(ConclusionType.Elimination, cell, digit));
									}
								}
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							var candidateOffsets = new List<CandidateViewNode>();
							foreach (int cell in urCells)
							{
								if (grid.Exists(cell, d1) is true)
								{
									candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d1));
								}
								if (grid.Exists(cell, d2) is true)
								{
									candidateOffsets.Add(new(DisplayColorKind.Normal, cell * 9 + d2));
								}
							}
							foreach (int cell in guardianCellPair)
							{
								if (grid.Exists(cell, d1) is true)
								{
									candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, cell * 9 + d1));
								}
								if (grid.Exists(cell, d2) is true)
								{
									candidateOffsets.Add(new(DisplayColorKind.Auxiliary2, cell * 9 + d2));
								}
							}
							foreach (int cell in otherCells)
							{
								foreach (int digit in grid.GetCandidates(cell))
								{
									candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + digit));
								}
							}

							accumulator.Add(
								new UniqueRectangleWithGuardianSubsetStep(
									conclusions.ToImmutableArray(),
									ImmutableArray.Create(
										View.Empty
											| candidateOffsets
											| new HouseViewNode[]
											{
												new(DisplayColorKind.Normal, house),
												new(DisplayColorKind.Auxiliary2, houseCombination[0]),
												new(DisplayColorKind.Auxiliary2, houseCombination[1])
											}
									),
									d1,
									d2,
									cells,
									guardianCellPair,
									otherCells,
									subsetDigitsMask,
									IsIncomplete(candidateOffsets),
									index
								)
							);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Check AR+Hidden single.
	/// </summary>
	/// <param name="accumulator">The technique accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <param name="urCells">All UR cells.</param>
	/// <param name="d1">The digit 1 used in UR.</param>
	/// <param name="d2">The digit 2 used in UR.</param>
	/// <param name="corner1">The corner cell 1.</param>
	/// <param name="corner2">The corner cell 2.</param>
	/// <param name="otherCellsMap">
	/// The map of other cells during the current UR searching.
	/// </param>
	/// <param name="index">The index.</param>
	/// <remarks>
	/// <para>The structure:</para>
	/// <para>
	/// <code><![CDATA[
	/// ↓corner1
	/// a   | aby  -  -
	/// abx | a    -  b
	///     | -    -  -
	///       ↑corner2(cell 'a')
	/// ]]></code>
	/// There's only one cell can be filled with the digit 'b' besides the cell 'aby'.
	/// </para>
	/// </remarks>
	partial void CheckHiddenSingleAvoidable(
		ICollection<UniqueRectangleStep> accumulator, scoped in Grid grid,
		int[] urCells, int d1, int d2, int corner1, int corner2, scoped in Cells otherCellsMap, int index)
	{
		if (grid.GetStatus(corner1) != CellStatus.Modifiable
			|| grid.GetStatus(corner2) != CellStatus.Modifiable
			|| grid[corner1] != grid[corner2] || grid[corner1] != d1 && grid[corner1] != d2)
		{
			return;
		}

		// Get the base digit ('a') and the other digit ('b').
		// Here 'b' is the digit that we should check the possible hidden single.
		int baseDigit = grid[corner1], otherDigit = baseDigit == d1 ? d2 : d1;
		var cellsThatTwoOtherCellsBothCanSee = !otherCellsMap & CandidatesMap[otherDigit];

		// Iterate on two cases (because we holds two other cells,
		// and both those two cells may contain possible elimination).
		for (int i = 0; i < 2; i++)
		{
			var (baseCell, anotherCell) = i == 0
				? (otherCellsMap[0], otherCellsMap[1])
				: (otherCellsMap[1], otherCellsMap[0]);

			// Iterate on each house type.
			foreach (var houseType in HouseTypes)
			{
				int houseIndex = baseCell.ToHouseIndex(houseType);

				// If the house doesn't overlap with the specified house, just skip it.
				if ((cellsThatTwoOtherCellsBothCanSee & HouseMaps[houseIndex]) is [])
				{
					continue;
				}

				var otherCells = HouseMaps[houseIndex] & CandidatesMap[otherDigit] & PeerMaps[anotherCell];
				int sameHouses = (otherCells + anotherCell).CoveredHouses;
				foreach (int sameHouse in sameHouses)
				{
					// Check whether all possible positions of the digit 'b' in this house only
					// lies in the given cells above ('cellsThatTwoOtherCellsBothCanSee').
					if ((HouseMaps[sameHouse] - anotherCell & CandidatesMap[otherDigit]) != otherCells)
					{
						continue;
					}

					// Possible hidden single found.
					// If the elimination doesn't exist, just skip it.
					if (grid.Exists(baseCell, otherDigit) is not true)
					{
						continue;
					}

					var cellOffsets = new List<CellViewNode>();
					foreach (int cell in urCells)
					{
						cellOffsets.Add(new(DisplayColorKind.Normal, cell));
					}

					var candidateOffsets = new List<CandidateViewNode>
					{
						new(DisplayColorKind.Normal, anotherCell * 9 + otherDigit)
					};
					foreach (int cell in otherCells)
					{
						candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, cell * 9 + otherDigit));
					}

					accumulator.Add(
						new AvoidableRectangleWithHiddenSingleStep(
							ImmutableArray.Create(new Conclusion(ConclusionType.Elimination, baseCell, otherDigit)),
							ImmutableArray.Create(
								View.Empty
									| cellOffsets
									| candidateOffsets
									| new HouseViewNode(DisplayColorKind.Normal, sameHouse)
							),
							d1,
							d2,
							(Cells)urCells,
							baseCell,
							anotherCell,
							sameHouse,
							index
						)
					);
				}
			}
		}
	}
}