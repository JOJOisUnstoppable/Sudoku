﻿namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Polygons;

/// <summary>
/// Provides with a step that is a <b>Unique Polygon Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Map"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="ExtraDigit">The extra digit.</param>
public sealed record UniquePolygonType2Step(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	in Cells Map,
	short DigitsMask,
	int ExtraDigit
) : UniquePolygonStep(Conclusions, Views, Map, DigitsMask)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.4M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.BdpType2;

	[FormatItem]
	private string ExtraDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (ExtraDigit + 1).ToString();
	}
}
