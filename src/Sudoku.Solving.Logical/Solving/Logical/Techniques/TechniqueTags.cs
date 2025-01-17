﻿namespace Sudoku.Solving.Logical.Techniques;

/// <summary>
/// Provides a series of tags to mark on a technique.
/// </summary>
/// <remarks>
/// For example, a <see cref="Technique.DeathBlossomCellType"/> can be categorized
/// as both <see cref="Als"/> and <see cref="LongChaining"/>.
/// </remarks>
/// <seealso cref="Als"/>
/// <seealso cref="LongChaining"/>
[Flags]
public enum TechniqueTags
{
	/// <summary>
	/// Indicates none of flags that the technique belongs to.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the singles technique.
	/// </summary>
	Singles = 1,

	/// <summary>
	/// Indicates the intersection technique.
	/// </summary>
	Intersections = 1 << 1,

	/// <summary>
	/// Indicates the subset technique. Please note that all ALS techniques shouldn't be with this flag.
	/// </summary>
	Subsets = 1 << 2,

	/// <summary>
	/// Indicates the fish technique.
	/// </summary>
	Fishes = 1 << 3,

	/// <summary>
	/// Indicates the wing technique.
	/// </summary>
	Wings = 1 << 4,

	/// <summary>
	/// Indicates the single digit pattern technique.
	/// </summary>
	SingleDigitPattern = 1 << 5,

	/// <summary>
	/// Indicates the short chain.
	/// </summary>
	ShortChaining = 1 << 6,

	/// <summary>
	/// Indicates the long chain, which includes normal AICs, forcing chains
	/// and other chaining-like techniques, such as Bowman's Bingo.
	/// </summary>
	LongChaining = 1 << 7,

	/// <summary>
	/// Indicates the forcing chains technique, such as Bowman's Bingo, Region Forcing Chains and so on.
	/// </summary>
	ForcingChains = 1 << 8,

	/// <summary>
	/// Indicates the deadly pattern technique.
	/// </summary>
	DeadlyPattern = 1 << 9,

	/// <summary>
	/// Indicates the ALS technique.
	/// </summary>
	Als = 1 << 10,

	/// <summary>
	/// Indicates the MSLS technique.
	/// </summary>
	Msls = 1 << 11,

	/// <summary>
	/// Indicates the exocet technique.
	/// </summary>
	Exocet = 1 << 12,

	/// <summary>
	/// Indicates the technique checked and searched relies on the rank theory.
	/// </summary>
	RankTheory = 1 << 13,

	/// <summary>
	/// Indicates the symmetry technique.
	/// </summary>
	Symmetry = 1 << 14,

	/// <summary>
	/// Indicates the last resort technique.
	/// </summary>
	LastResort = 1 << 15
}
