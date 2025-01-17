﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Pattern Overlay</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
internal sealed record PatternOverlayStep(ConclusionList Conclusions) :
	LastResortStep(Conclusions, ImmutableArray.Create(View.Empty))
{
	/// <summary>
	/// Indicates the digit.
	/// </summary>
	public int Digit => Conclusions[0].Digit;

	/// <inheritdoc/>
	public override decimal Difficulty => 8.5M;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.LastResort;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.PatternOverlay;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.PatternOverlay;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.SingleDigitPattern;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	[ResourceTextFormatter]
	internal string DigitStr() => (Digit + 1).ToString();
}
