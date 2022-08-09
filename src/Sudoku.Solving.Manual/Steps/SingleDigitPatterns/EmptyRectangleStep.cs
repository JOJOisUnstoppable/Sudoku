﻿namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Empty Rectangle</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
/// <param name="Block">Indicates the block that the empty rectangle structure formed.</param>
/// <param name="ConjugatePair">Indicates the conjugate pair used.</param>
internal sealed record EmptyRectangleStep(
	ConclusionList Conclusions,
	ViewList Views,
	int Digit,
	int Block,
	scoped in Conjugate ConjugatePair
) : SingleDigitPatternStep(Conclusions, Views, Digit), IChainLikeStep, IStepWithRank
{
	/// <inheritdoc/>
	public override decimal Difficulty => 4.6M;

	/// <inheritdoc/>
	public int Rank => 1;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.EmptyRectangle;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.EmptyRectangle;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[FormatItem]
	internal string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit + 1).ToString();
	}

	[FormatItem]
	internal string HouseStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => HouseFormatter.Format(1 << Block);
	}

	[FormatItem]
	internal string ConjStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugatePair.ToString();
	}
}
