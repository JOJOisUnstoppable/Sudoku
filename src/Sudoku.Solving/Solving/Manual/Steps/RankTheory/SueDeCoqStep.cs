﻿namespace Sudoku.Solving.Manual.Steps.RankTheory;

/// <summary>
/// Provides with a step that is a <b>Sue de Coq</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Block">The block.</param>
/// <param name="Line">The line.</param>
/// <param name="BlockMask">The block mask.</param>
/// <param name="LineMask">The line mask.</param>
/// <param name="IntersectionMask">The intersection mask.</param>
/// <param name="IsCannibalistic">Indicates whether the SdC is cannibalistic.</param>
/// <param name="IsolatedDigitsMask">The isolated digits mask.</param>
/// <param name="BlockCells">The map of block cells.</param>
/// <param name="LineCells">The map of line cells.</param>
/// <param name="IntersectionCells">The map of intersection cells.</param>
public sealed record SueDeCoqStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	int Block,
	int Line,
	short BlockMask,
	short LineMask,
	short IntersectionMask,
	bool IsCannibalistic,
	short IsolatedDigitsMask,
	in Cells BlockCells,
	in Cells LineCells,
	in Cells IntersectionCells
) : RankTheoryStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		5.0M
		+ (IsolatedDigitsMask != 0 ? .1M : 0) // The extra difficulty for isolated digit existence.
		+ (IsCannibalistic ? .2M : 0); // The extra difficulty for cannibalism.

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.RankTheory | TechniqueTags.Als;

	/// <inheritdoc/>
	public override Technique TechniqueCode => IsCannibalistic ? Technique.CannibalizedSdc : Technique.Sdc;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.Sdc;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Sometimes;

	[FormatItem]
	private string IntersectionCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => IntersectionCells.ToString();
	}

	[FormatItem]
	private string IntersectionDigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(IntersectionMask).ToString(null);
	}

	[FormatItem]
	private string BlockCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => BlockCells.ToString();
	}

	[FormatItem]
	private string BlockDigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(BlockMask).ToString(null);
	}

	[FormatItem]
	private string LineCellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => LineCells.ToString();
	}

	[FormatItem]
	private string LineDigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(LineMask).ToString(null);
	}
}