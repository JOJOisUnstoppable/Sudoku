﻿namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Almost Locked Sets</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
internal abstract record AlmostLockedSetsStep(ConclusionList Conclusions, ViewList Views) :
	Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public override string? Format => base.Format;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.AlmostLockedSetsChainingLike;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.Als;

	/// <inheritdoc/>
	public sealed override Stableness Stableness => base.Stableness;
}
