﻿namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Oddagon Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Loop"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="ExtraCells">Indicates the extra cells used.</param>
/// <param name="ExtraDigitsMask">Indicates the mask that contains all extra digits used.</param>
internal sealed record BivalueOddagonType3Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Loop,
	int Digit1,
	int Digit2,
	scoped in CellMap ExtraCells,
	short ExtraDigitsMask
) : BivalueOddagonStep(Conclusions, Views, Loop, Digit1, Digit2), IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => base.Difficulty;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.Size, (ExtraCells.Count >> 1) * .1M) };

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BivalueOddagonType3;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[FormatItem]
	internal string LoopStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Loop.ToString();
	}

	[FormatItem]
	internal string Digit1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit1 + 1).ToString();
	}

	[FormatItem]
	internal string Digit2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit2 + 1).ToString();
	}

	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);
	}

	[FormatItem]
	internal string ExtraCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ExtraCells.ToString();
	}
}