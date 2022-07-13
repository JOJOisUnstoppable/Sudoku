﻿namespace System.Drawing;

/// <summary>
/// Provides extension methods on <see cref="Point"/>.
/// </summary>
/// <seealso cref="Point"/>
internal static class PointExtensions
{
	/// <summary>
	/// Get a new <see cref="PointF"/> instance created by the original one, with the specified offset
	/// added into the properties <see cref="PointF.X"/> and <see cref="PointF.Y"/>.
	/// </summary>
	/// <param name="this">The point.</param>
	/// <param name="offset">The offset.</param>
	/// <returns>The result point.</returns>
	/// <seealso cref="PointF.X"/>
	/// <seealso cref="PointF.Y"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point WithOffset(this scoped in Point @this, int offset) => new(@this.X + offset, @this.Y + offset);

	/// <summary>
	/// Get a new <see cref="PointF"/> instance created by the original one, with the specified offset
	/// added into the properties <see cref="PointF.X"/> and <see cref="PointF.Y"/>.
	/// </summary>
	/// <param name="this">The point.</param>
	/// <param name="xOffset">The X offset.</param>
	/// <param name="yOffset">The Y offset.</param>
	/// <returns>The result point.</returns>
	/// <seealso cref="PointF.X"/>
	/// <seealso cref="PointF.Y"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point WithOffset(this scoped in Point @this, int xOffset, int yOffset)
		=> new(@this.X + xOffset, @this.Y + yOffset);
}
