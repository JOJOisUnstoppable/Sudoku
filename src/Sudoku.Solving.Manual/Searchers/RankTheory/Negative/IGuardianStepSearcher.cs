﻿namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Guardian</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Guardian</item>
/// </list>
/// </summary>
public interface IGuardianStepSearcher : INegativeRankStepSearcher, ICellLinkingLoopStepSearcher
{
}

[StepSearcher]
internal sealed unsafe partial class GuardianStepSearcher : IGuardianStepSearcher
{
	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		// Check POM eliminations first.
		scoped var eliminationMaps = (stackalloc Cells[9]);
		eliminationMaps.Fill(Cells.Empty);
		var pomSteps = new List<IStep>();
		new PatternOverlayStepSearcher().GetAll(pomSteps, grid, onlyFindOne: false);
		foreach (var step in pomSteps.Cast<PatternOverlayStep>())
		{
			scoped ref var currentMap = ref eliminationMaps[step.Digit];
			foreach (var conclusion in step.Conclusions)
			{
				currentMap.Add(conclusion.Cell);
			}
		}

		var resultAccumulator = new List<GuardianStep>();
		for (int digit = 0; digit < 9; digit++)
		{
			if (eliminationMaps[digit] is not (var baseElimMap and not []))
			{
				// Using global view, we cannot find any eliminations for this digit.
				// Just skip to the next loop.
				continue;
			}

			static bool predicate(in Cells loop) => loop.Count is var l && (l & 1) == 0 && l >= 6;
			if (ICellLinkingLoopStepSearcher.GatherLoops_Guardian(digit, &predicate) is not { } linkingCellsList)
			{
				continue;
			}

			foreach (var (loop, guardians, housesMask) in linkingCellsList)
			{
				if ((!guardians & CandidatesMap[digit]) is not (var elimMap and not []))
				{
					continue;
				}

				var candidateOffsets = new List<CandidateViewNode>();
				foreach (int c in loop)
				{
					candidateOffsets.Add(new(DisplayColorKind.Normal, c * 9 + digit));
				}
				foreach (int c in guardians)
				{
					candidateOffsets.Add(new(DisplayColorKind.Auxiliary1, c * 9 + digit));
				}

				var step = new GuardianStep(
					ImmutableArray.Create(from c in elimMap select new Conclusion(Elimination, c, digit)),
					ImmutableArray.Create(
						View.Empty
							| candidateOffsets
							| ICellLinkingLoopStepSearcher.GetLinks(loop, housesMask)
					),
					digit,
					loop,
					guardians
				);
				if (onlyFindOne)
				{
					return step;
				}

				resultAccumulator.Add(step);
			}
		}

		if (resultAccumulator.Count == 0)
		{
			return null;
		}

		accumulator.AddRange(
			from info in IDistinctableStep<GuardianStep>.Distinct(resultAccumulator)
			orderby info.Loop.Count, info.Guardians.Count
			select info
		);

		return null;
	}
}
