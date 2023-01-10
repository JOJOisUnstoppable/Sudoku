﻿namespace SudokuStudio.Markup;

/// <summary>
/// Defines a markup extension that can parse a <see cref="string"/> value, converting it into a valid <see cref="Grid"/>
/// or throwing exceptions when the code is invalid.
/// </summary>
[ContentProperty(Name = nameof(GridCode))]
[MarkupExtensionReturnType(ReturnType = typeof(Grid))]
public sealed class GridExtension : MarkupExtension
{
	/// <summary>
	/// Indicates whether the conversion ignores casing.
	/// </summary>
	public bool IgnoreCasing { get; set; } = false;

	/// <summary>
	/// Indicates the grid code.
	/// </summary>
	public string GridCode { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the exact format string.
	/// </summary>
	public string ExactFormatString { get; set; } = string.Empty;


	/// <inheritdoc/>
	protected override object ProvideValue()
	{
		if (GridCode.Equals(nameof(Grid.Empty), IgnoreCasing ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
		{
			return Grid.Empty;
		}

		if (GridCode.Equals(nameof(Grid.Undefined), IgnoreCasing ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
		{
			return Grid.Undefined;
		}

		var targetGrid = Grid.Parse(GridCode);
		return !string.IsNullOrEmpty(ExactFormatString) && targetGrid.ToString(ExactFormatString) != GridCode
			? throw new FormatException("The specified grid code cannot be converted into a valid grid, using specified format.")
			: targetGrid;
	}
}