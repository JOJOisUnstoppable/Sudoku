﻿namespace Sudoku.Presentation.Nodes.Grouped;

/// <summary>
/// Defines an oblique line view node.
/// </summary>
public sealed partial class ObliqueLineViewNode : GroupedViewNode
{
	/// <summary>
	/// Initializes an <see cref="ObliqueLineViewNode"/> instance via the specified values.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="firstCell">The first cell.</param>
	/// <param name="lastCell">The last cell.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ObliqueLineViewNode(Identifier identifier, int firstCell, int lastCell) : base(identifier, firstCell, ImmutableArray<int>.Empty)
		=> TailCell = lastCell;


	/// <summary>
	/// Indicates the last cell.
	/// </summary>
	public int TailCell { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is ObliqueLineViewNode comparer
		&& Identifier == comparer.Identifier && HeadCell == comparer.HeadCell && TailCell == comparer.TailCell;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(Identifier), nameof(HeadCell), nameof(TailCell))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(HeadCell), nameof(TailCell))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override ObliqueLineViewNode Clone() => new(Identifier, HeadCell, TailCell);
}
