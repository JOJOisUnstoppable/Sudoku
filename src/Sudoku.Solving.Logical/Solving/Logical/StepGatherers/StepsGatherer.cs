﻿namespace Sudoku.Solving.Logical.StepGatherers;

/// <summary>
/// Defines a steps gatherer.
/// </summary>
public sealed class StepsGatherer : IStepGatherableSearcher, IStepGatherableSearcherOptions
{
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	public bool OnlyShowSameLevelTechniquesInFindAllSteps { get; set; } = true;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is 1000.
	/// </remarks>
	public int MaxStepsGathered { get; set; } = 1000;


	/// <inheritdoc/>
	public IEnumerable<IStep> Search(scoped in Grid puzzle, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
	{
		if (puzzle.IsSolved || !puzzle.ExactlyValidate(out _, out var sukaku))
		{
			return Array.Empty<IStep>();
		}

		const SearcherDisplayingLevel defaultLevelValue = (SearcherDisplayingLevel)255;

		var totalSearchersCount = StepSearcherPool.Collection.Count;

		InitializeMaps(puzzle);

		var (i, bag, currentSearcherIndex) = (defaultLevelValue, new List<IStep>(), 0);
		foreach (var searcher in StepSearcherPool.Collection)
		{
			switch (searcher)
			{
				case { Options.EnabledArea: var enabledArea } when !enabledArea.Flags(SearcherEnabledArea.Gathering):
				case { IsNotSupportedForSukaku: true } when sukaku.Value:
				{
					goto ReportProgress;
				}
				case { Options.DisplayingLevel: var currentLevel }:
				{
					// If a searcher contains the upper level, it will be skipped.
					if (OnlyShowSameLevelTechniquesInFindAllSteps && i != defaultLevelValue && i != currentLevel)
					{
						goto ReportProgress;
					}

					cancellationToken.ThrowIfCancellationRequested();

					// Searching.
					var accumulator = new List<IStep>();
					scoped var context = new LogicalAnalysisContext(accumulator, puzzle, false);
					searcher.GetAll(ref context);

					switch (accumulator.Count)
					{
						case 0:
						{
							goto ReportProgress;
						}
						case var count:
						{
							if (OnlyShowSameLevelTechniquesInFindAllSteps)
							{
								i = currentLevel;
							}

							bag.AddRange(count > MaxStepsGathered ? accumulator.Slice(0, MaxStepsGathered) : accumulator);

							break;
						}
					}

					break;
				}
			}

		// Report the progress if worth.
		ReportProgress:
			progress?.Report(++currentSearcherIndex / totalSearchersCount);
		}

		// Return the result.
		return bag;
	}
}
