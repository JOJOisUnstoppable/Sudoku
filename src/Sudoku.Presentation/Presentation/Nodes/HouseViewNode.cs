﻿namespace Sudoku.Presentation.Nodes;

/// <summary>
/// Defines a view node that highlights for a house.
/// </summary>
public sealed partial class HouseViewNode : BasicViewNode
{
	/// <summary>
	/// Initializes a <see cref="HouseViewNode"/> instance via the identifier and the highlight house.
	/// </summary>
	/// <param name="identifier">The identifier.</param>
	/// <param name="houseIndex">The house index.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HouseViewNode(Identifier identifier, int houseIndex) : base(identifier) => House = houseIndex;


	/// <summary>
	/// Indicates the house highlighted.
	/// </summary>
	public int House { get; }


	[DeconstructionMethod]
	public partial void Deconstruct(out Identifier identifier, out int house);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] ViewNode? other)
		=> other is HouseViewNode comparer && Identifier == comparer.Identifier && House == comparer.House;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, nameof(TypeIdentifier), nameof(Identifier), nameof(House))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(Identifier), nameof(House))]
	public override partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override HouseViewNode Clone() => new(Identifier, House);
}
