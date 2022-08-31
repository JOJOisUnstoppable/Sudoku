﻿namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with a <b>Template</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>Template Set</item>
/// <item>Template Delete</item>
/// </list>
/// </summary>
public interface ITemplateStepSearcher : IStepSearcher, IStepSearcherRequiresSolution
{
	/// <summary>
	/// Indicates whether the technique searcher only checks template deletes.
	/// </summary>
	public abstract bool TemplateDeleteOnly { get; set; }
}

[StepSearcher]
internal sealed partial class TemplateStepSearcher : ITemplateStepSearcher
{
	/// <inheritdoc/>
	[StepSearcherProperty]
	public bool TemplateDeleteOnly { get; set; }

	/// <inheritdoc/>
	public Grid Solution { get; set; }


	/// <inheritdoc/>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		// Iterate on each digit.
		var distributedMapsByDigit = Solution.ValuesMap;
		for (int digit = 0; digit < 9; digit++)
		{
			if (!TemplateDeleteOnly)
			{
				// Check template sets.
				if ((distributedMapsByDigit[digit] & CandidatesMap[digit]) is not (var templateSetMap and not []))
				{
					continue;
				}

				var templateSetConclusions = from cell in templateSetMap select new Conclusion(Assignment, cell, digit);
				var candidateOffsets = new CandidateViewNode[templateSetConclusions.Length];
				int z = 0;
				foreach (var (_, candidate) in templateSetConclusions)
				{
					candidateOffsets[z++] = new(DisplayColorKind.Normal, candidate);
				}

				var templateSetStep = new TemplateStep(
					templateSetConclusions,
					ImmutableArray.Create(View.Empty | candidateOffsets),
					false
				);
				if (onlyFindOne)
				{
					return templateSetStep;
				}

				accumulator.Add(templateSetStep);
			}

			// Then check template deletes.
			if (CandidatesMap[digit] - distributedMapsByDigit[digit] is not (var templateDeleteMap and not []))
			{
				continue;
			}

			var templateDeleteStep = new TemplateStep(
				from cell in templateDeleteMap select new Conclusion(Elimination, cell, digit),
				ViewList.Empty,
				true
			);
			if (onlyFindOne)
			{
				return templateDeleteStep;
			}

			accumulator.Add(templateDeleteStep);
		}

		return null;
	}
}