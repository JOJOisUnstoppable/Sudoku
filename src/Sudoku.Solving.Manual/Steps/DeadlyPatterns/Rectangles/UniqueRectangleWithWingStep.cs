﻿namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Wing</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TechniqueCode2"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="IsAvoidable"><inheritdoc/></param>
/// <param name="Pivots">Indicates the pivots used.</param>
/// <param name="Petals">Indicates the petals used.</param>
/// <param name="ExtraDigitsMask">Indicates the mask that contains all extra digits.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
internal sealed record UniqueRectangleWithWingStep(
	ConclusionList Conclusions,
	ViewList Views,
	Technique TechniqueCode2,
	int Digit1,
	int Digit2,
	scoped in Cells Cells,
	bool IsAvoidable,
	scoped in Cells Pivots,
	scoped in Cells Petals,
	short ExtraDigitsMask,
	int AbsoluteOffset
) :
	UniqueRectangleStep(
		Conclusions,
		Views,
		TechniqueCode2,
		Digit1,
		Digit2,
		Cells,
		IsAvoidable,
		AbsoluteOffset
	),
	IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[]
		{
			("Avoidable rectangle", IsAvoidable ? .1M : 0),
			(
				"Wing",
				TechniqueCode switch
				{
					Technique.UniqueRectangleXyWing or Technique.AvoidableRectangleXyWing => .2M,
					Technique.UniqueRectangleXyzWing or Technique.AvoidableRectangleXyzWing => .3M,
					Technique.UniqueRectangleWxyzWing or Technique.AvoidableRectangleWxyzWing => .5M,
					_ => throw new NotSupportedException("The specified technique code is not supported.")
				}
			)
		};

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	[FormatItem]
	internal string PivotsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Pivots.ToString();
	}

	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => DigitMaskFormatter.Format(ExtraDigitsMask, FormattingMode.Normal);
	}
}
