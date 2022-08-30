﻿namespace Sudoku.Drawing;

/// <summary>
/// Defines a grid image generator that parses a sudoku grid and converts it to an image
/// as the result representation.
/// </summary>
public interface IGridImageGenerator
{
	/// <summary>
	/// The square root of 2.
	/// </summary>
	protected const float SqrtOf2 = 1.4142135F;

	/// <summary>
	/// The rotate angle (45 degrees). This field is used for rotate the chains if some of them are overlapped.
	/// </summary>
	protected const float RotateAngle = MathF.PI / 4;

	/// <summary>
	/// The text offset that corrects the pixel of the text output.
	/// </summary>
	protected const float TextOffset = 6F;


	/// <summary>
	/// Indicates the default string format.
	/// </summary>
	protected static readonly StringFormat DefaultStringFormat = new()
	{
		Alignment = StringAlignment.Center,
		LineAlignment = StringAlignment.Center
	};


	/// <summary>
	/// Indicates the drawing width.
	/// </summary>
	public abstract float Width { get; }

	/// <summary>
	/// Indicates the drawing height.
	/// </summary>
	public abstract float Height { get; }

	/// <summary>
	/// Indicates the focused cells.
	/// </summary>
	public abstract CellMap FocusedCells { get; set; }

	/// <summary>
	/// Indicates the view.
	/// </summary>
	public abstract View? View { get; set; }

	/// <summary>
	/// Indicates all conclusions.
	/// </summary>
	public abstract IEnumerable<Conclusion>? Conclusions { get; set; }


	/// <summary>
	/// To draw the image manually.
	/// </summary>
	/// <returns>The result image.</returns>
	/// <remarks>
	/// The method may be called manually, because we can't control whether the value is modified.
	/// </remarks>
	public abstract Image DrawManually();


	/// <summary>
	/// Get the font via the specified name, size and the scale.
	/// </summary>
	/// <param name="fontName">The font name that decides the font to use and presentation.</param>
	/// <param name="size">The size that decides the default font size.</param>
	/// <param name="scale">The scale that decides the result font size.</param>
	/// <param name="style">The style that decides the font style of the text in the picture.</param>
	/// <returns>The font.</returns>
	/// <exception cref="ArgumentNullException">
	/// Throws when <paramref name="fontName"/> is <see langword="null"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static sealed Font GetFont(string? fontName, float size, decimal scale, FontStyle style) =>
		new(fontName ?? throw new ArgumentNullException(nameof(size)), size * (float)scale, style);
}
