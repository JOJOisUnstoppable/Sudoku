﻿namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Subset</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="House"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
public sealed record class HiddenSubsetStep(
	ConclusionList Conclusions, ViewList Views, int House, in Cells Cells, short DigitsMask) :
	SubsetStep(Conclusions, Views, House, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		Size switch
		{
			2 => 3.4M,
			3 => 4.0M,
			4 => 5.4M,
			_ => throw new NotSupportedException("The specified size is not supported.")
		};

	/// <inheritdoc/>
	public override Technique TechniqueCode =>
		Size switch
		{
			2 => Technique.HiddenPair,
			3 => Technique.HiddenTriple,
			4 => Technique.HiddenQuadruple,
			_ => throw new NotSupportedException("The specified size is not supported.")
		};

	[FormatItem]
	internal string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	[FormatItem]
	internal string HouseStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new HouseCollection(House).ToString();
	}
}
