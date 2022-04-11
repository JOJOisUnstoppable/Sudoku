﻿namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>3-demensional Sue de Coq</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>3-demensional Sue de Coq</item>
/// </list>
/// </summary>
[StepSearcher]
public sealed unsafe partial class SueDeCoq3DemensionStepSearcher : ISueDeCoq3DemensionStepSearcher
{
	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
	{
		using ValueList<Cells> rbList = new(3), cbList = new(3);
		foreach (int pivot in EmptyMap)
		{
			int r = pivot.ToHouseIndex(HouseType.Row);
			int c = pivot.ToHouseIndex(HouseType.Column);
			int b = pivot.ToHouseIndex(HouseType.Block);
			var rbMap = HouseMaps[r] & HouseMaps[b];
			var cbMap = HouseMaps[c] & HouseMaps[b];
			var rbEmptyMap = rbMap & EmptyMap;
			var cbEmptyMap = cbMap & EmptyMap;
			if (rbEmptyMap.Count < 2 || cbEmptyMap.Count < 2)
			{
				// The intersection needs at least two cells.
				continue;
			}

			reinitializeList(&rbList, &rbEmptyMap);
			reinitializeList(&cbList, &cbEmptyMap);

			foreach (var rbCurrentMap in rbList)
			{
				short rbSelectedInterMask = grid.GetDigitsUnion(rbCurrentMap);
				if (PopCount((uint)rbSelectedInterMask) <= rbCurrentMap.Count + 1)
				{
					continue;
				}

				foreach (var cbCurrentMap in cbList)
				{
					short cbSelectedInterMask = grid.GetDigitsUnion(cbCurrentMap);
					if (PopCount((uint)cbSelectedInterMask) <= cbCurrentMap.Count + 1)
					{
						continue;
					}

					if ((cbCurrentMap & rbCurrentMap).Count != 1)
					{
						continue;
					}

					// Get all maps to use later.
					var blockMap = HouseMaps[b] - rbCurrentMap - cbCurrentMap & EmptyMap;
					var rowMap = HouseMaps[r] - HouseMaps[b] & EmptyMap;
					var columnMap = HouseMaps[c] - HouseMaps[b] & EmptyMap;

					// Iterate on the number of the cells that should be selected in block.
					for (int i = 1, count = blockMap.Count; i < count; i++)
					{
						foreach (var selectedBlockCells in blockMap & i)
						{
							short blockMask = grid.GetDigitsUnion(selectedBlockCells);
							var elimMapBlock = Cells.Empty;

							// Get the elimination map in the block.
							foreach (int digit in blockMask)
							{
								elimMapBlock |= CandMaps[digit];
							}
							elimMapBlock &= blockMap - selectedBlockCells;

							for (int j = 1, limit = MathExtensions.Min(9 - i - selectedBlockCells.Count, rowMap.Count, columnMap.Count); j < limit; j++)
							{
								foreach (var selectedRowCells in rowMap & j)
								{
									short rowMask = grid.GetDigitsUnion(selectedRowCells);
									var elimMapRow = Cells.Empty;

									foreach (int digit in rowMask)
									{
										elimMapRow |= CandMaps[digit];
									}
									elimMapRow &= HouseMaps[r] - rbCurrentMap - selectedRowCells;

									for (int k = 1; k <= MathExtensions.Min(9 - i - j - selectedBlockCells.Count - selectedRowCells.Count, rowMap.Count, columnMap.Count); k++)
									{
										foreach (var selectedColumnCells in columnMap & k)
										{
											short columnMask = grid.GetDigitsUnion(selectedColumnCells);
											var elimMapColumn = Cells.Empty;

											foreach (int digit in columnMask)
											{
												elimMapColumn |= CandMaps[digit];
											}
											elimMapColumn &= HouseMaps[c] - cbCurrentMap - selectedColumnCells;

											if ((blockMask & rowMask) != 0
												&& (rowMask & columnMask) != 0
												&& (columnMask & blockMask) != 0)
											{
												continue;
											}

											var fullMap = rbCurrentMap | cbCurrentMap | selectedRowCells | selectedColumnCells | selectedBlockCells;
											var otherMap_row = fullMap - (selectedRowCells | rbCurrentMap);
											var otherMap_column = fullMap - (selectedColumnCells | cbCurrentMap);
											short mask = grid.GetDigitsUnion(otherMap_row);
											if ((mask & rowMask) != 0)
											{
												// At least one digit spanned two houses.
												continue;
											}

											mask = grid.GetDigitsUnion(otherMap_column);
											if ((mask & columnMask) != 0)
											{
												continue;
											}

											mask = (short)((short)(blockMask | rowMask) | columnMask);
											short rbMaskOnlyInInter = (short)(rbSelectedInterMask & ~mask);
											short cbMaskOnlyInInter = (short)(cbSelectedInterMask & ~mask);

											int bCount = PopCount((uint)blockMask);
											int rCount = PopCount((uint)rowMask);
											int cCount = PopCount((uint)columnMask);
											int rbCount = PopCount((uint)rbMaskOnlyInInter);
											int cbCount = PopCount((uint)cbMaskOnlyInInter);
											if (cbCurrentMap.Count + rbCurrentMap.Count + i + j + k - 1 == bCount + rCount + cCount + rbCount + cbCount
												&& (elimMapRow | elimMapColumn | elimMapBlock) is not [])
											{
												// Check eliminations.
												var conclusions = new List<Conclusion>();
												foreach (int digit in blockMask)
												{
													foreach (int cell in elimMapBlock & CandMaps[digit])
													{
														conclusions.Add(new(ConclusionType.Elimination, cell, digit));
													}
												}
												foreach (int digit in rowMask)
												{
													foreach (int cell in elimMapRow & CandMaps[digit])
													{
														conclusions.Add(new(ConclusionType.Elimination, cell, digit));
													}
												}
												foreach (int digit in columnMask)
												{
													foreach (int cell in elimMapColumn & CandMaps[digit])
													{
														conclusions.Add(new(ConclusionType.Elimination, cell, digit));
													}
												}
												if (conclusions.Count == 0)
												{
													continue;
												}

												var candidateOffsets = new List<CandidateViewNode>();
												foreach (int digit in rowMask)
												{
													foreach (int cell in (selectedRowCells | rbCurrentMap) & CandMaps[digit])
													{
														candidateOffsets.Add(new(0, cell * 9 + digit));
													}
												}
												foreach (int digit in columnMask)
												{
													foreach (int cell in (selectedColumnCells | cbCurrentMap) & CandMaps[digit])
													{
														candidateOffsets.Add(new(1, cell * 9 + digit));
													}
												}
												foreach (int digit in blockMask)
												{
													foreach (int cell in (selectedBlockCells | rbCurrentMap | cbCurrentMap) & CandMaps[digit])
													{
														candidateOffsets.Add(new(2, cell * 9 + digit));
													}
												}

												var step = new SueDeCoq3DimensionStep(
													ImmutableArray.CreateRange(conclusions),
													ImmutableArray.Create(
														View.Empty
															+ candidateOffsets
															+ new HouseViewNode[]
															{
																new(0, r),
																new(2, c),
																new(3, b)
															}
													),
													rowMask,
													columnMask,
													blockMask,
													selectedRowCells | rbCurrentMap,
													selectedColumnCells | cbCurrentMap,
													selectedBlockCells | rbCurrentMap | cbCurrentMap
												);
												if (onlyFindOne)
												{
													return step;
												}

												accumulator.Add(step);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		return null;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void reinitializeList(ValueList<Cells>* list, Cells* emptyMap)
		{
			list->Clear();
			switch (*emptyMap)
			{
				case [_, _]:
				{
					list->Add(*emptyMap);

					break;
				}
				case [var i, var j, var k]:
				{
					list->Add(Cells.Empty + (i + j));
					list->Add(Cells.Empty + (i + k));
					list->Add(Cells.Empty + (j + k));

					break;
				}
			}
		}
	}
}