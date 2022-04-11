﻿namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
public abstract record class WingStep(ConclusionList Conclusions, ViewList Views) :
	Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Wing;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.Wings;

	/// <inheritdoc/>
	public sealed override Stableness Stableness => base.Stableness;
}
