﻿namespace Sudoku.Concepts;

/// <summary>
/// Provides extension methods on <see cref="HouseType"/>.
/// </summary>
/// <seealso cref="HouseType"/>
public static class HouseTypeExtensions
{
	/// <inheritdoc cref="CopyHouseInfo(int, int*)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe void CopyHouseInfo(this byte cell, byte* ptr)
	{
		ArgumentNullException.ThrowIfNull(ptr);

		ptr[0] = (byte)BlockTable[cell];
		ptr[1] = (byte)RowTable[cell];
		ptr[2] = (byte)ColumnTable[cell];
	}

	/// <summary>
	/// Gets the row, column and block value and copies to the specified array that represents by a pointer
	/// of 3 elements, where the first element stores the block index, second element stores the row index
	/// and the third element stores the column index.
	/// </summary>
	/// <param name="cell">The cell. The available values must be between 0 and 80.</param>
	/// <param name="ptr">The specified array that represents by a pointer of 3 elements.</param>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="ptr"/> is <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe void CopyHouseInfo(this int cell, int* ptr)
	{
		ArgumentNullException.ThrowIfNull(ptr);

		ptr[0] = BlockTable[cell];
		ptr[1] = RowTable[cell];
		ptr[2] = ColumnTable[cell];
	}

	/// <summary>
	/// <inheritdoc cref="CopyHouseInfo(int, int*)" path="/summary"/>
	/// </summary>
	/// <param name="cell"><inheritdoc cref="CopyHouseInfo(int, int*)" path="/param[@name='cell']"/></param>
	/// <param name="reference">
	/// The specified reference to the first element in a sequence. The sequence type can be an array or a <see cref="Span{T}"/>,
	/// only if the sequence can store at least 3 values.
	/// </param>
	/// <exception cref="ArgumentNullRefException">
	/// Throws when the argument <paramref name="reference"/> references to <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyHouseInfo(this int cell, ref int reference)
	{
		ArgumentNullRefException.ThrowIfNullRef(ref reference);

		AddByteOffset(ref reference, 0) = BlockTable[cell];
		AddByteOffset(ref reference, 1) = RowTable[cell];
		AddByteOffset(ref reference, 2) = ColumnTable[cell];
	}

	/// <summary>
	/// Get the house index (0..27 for block 1-9, row 1-9 and column 1-9)
	/// for the specified cell and the house type.
	/// </summary>
	/// <param name="cell">The cell. The available values must be between 0 and 80.</param>
	/// <param name="houseType">The house type.</param>
	/// <returns>The house index. The return value must be between 0 and 26.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="houseType"/> is not defined.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ToHouseIndex(this byte cell, HouseType houseType)
		=> houseType switch
		{
			HouseType.Block => BlockTable[cell],
			HouseType.Row => RowTable[cell],
			HouseType.Column => ColumnTable[cell],
			_ => throw new ArgumentOutOfRangeException(nameof(houseType))
		};

	/// <inheritdoc cref="ToHouseIndex(byte, HouseType)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ToHouseIndex(this int cell, HouseType houseType)
		=> houseType switch
		{
			HouseType.Block => BlockTable[cell],
			HouseType.Row => RowTable[cell],
			HouseType.Column => ColumnTable[cell],
			_ => throw new ArgumentOutOfRangeException(nameof(houseType))
		};

	/// <summary>
	/// Get the house type for the specified house index.
	/// </summary>
	/// <param name="houseIndex">The house index.</param>
	/// <returns>
	/// The house type. The possible return values are:
	/// <list type="table">
	/// <listheader>
	/// <term>House indices</term>
	/// <description>Return value</description>
	/// </listheader>
	/// <item>
	/// <term><paramref name="houseIndex"/> is <![CDATA[>= 0 and < 9]]></term>
	/// <description><see cref="HouseType.Block"/></description>
	/// </item>
	/// <item>
	/// <term><paramref name="houseIndex"/> is <![CDATA[>= 9 and < 18]]></term>
	/// <description><see cref="HouseType.Row"/></description>
	/// </item>
	/// <item>
	/// <term><paramref name="houseIndex"/> is <![CDATA[>= 18 and < 27]]></term>
	/// <description><see cref="HouseType.Column"/></description>
	/// </item>
	/// </list>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HouseType ToHouseType(this int houseIndex) => (HouseType)(houseIndex / 9);
}
