﻿namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Universal;

/// <summary>
/// Provides with a step that is a <b>Bivalue Universal Grave Type 3</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TrueCandidates">Indicates the true candidates.</param>
/// <param name="DigitsMask">Indicates the digits used.</param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="IsNaked">Indicates whether the specified subset is naked subset.</param>
public sealed record BivalueUniversalGraveType3Step(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	IReadOnlyList<int> TrueCandidates,
	short DigitsMask,
	Cells Cells,
	bool IsNaked
) : BivalueUniversalGraveStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty =>
		base.Difficulty
			+ Size * .1M // Subset extra difficulty.
			+ (IsNaked ? 0 : .1M); // Hidden subset extra difficulty.

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BugType3;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Seldom;

	/// <summary>
	/// Indicates the size of the subset.
	/// </summary>
	private int Size => PopCount((uint)DigitsMask);

	[FormatItem]
	private string TrueCandidatesStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Candidates(TrueCandidates).ToString();
	}

	[FormatItem]
	private string SubsetTypeStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => IsNaked ? TextResources.Current.NakedKeyword : TextResources.Current.HiddenKeyword;
	}

	[FormatItem]
	private string SizeStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => SubsetNames[Size].ToLower(null);
	}

	[FormatItem]
	private string SizeStrZhCn
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TextResources.Current[$"SubsetNamesSize{Size}"];
	}

	[FormatItem]
	private string ExtraDigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	[FormatItem]
	private string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.ToString();
	}
}