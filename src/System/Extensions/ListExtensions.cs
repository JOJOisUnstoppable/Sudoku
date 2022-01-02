﻿namespace System.Collections.Generic;

/// <summary>
/// Provides extension methods on <see cref="IList{T}"/>.
/// </summary>
/// <seealso cref="IList{T}"/>
public static class ListExtensions
{
	/// <summary>
	/// Remove duplicate items in the specified list.
	/// </summary>
	/// <typeparam name="TEquatable">The type of each elements.</typeparam>
	/// <param name="this">The list.</param>
	/// <returns>Returns the reference of the argument <paramref name="this"/>.</returns>
	public static List<TEquatable> RemoveDuplicateItems<TEquatable>(this List<TEquatable> @this)
	where TEquatable : IEquatable<TEquatable>
	{
		var set = new Set<TEquatable>(@this);
		@this.Clear();
		@this.AddRange(set);

		return @this;
	}

	/// <summary>
	/// Remove duplicate items in the specified list.
	/// </summary>
	/// <typeparam name="TEquatable">The type of each elements.</typeparam>
	/// <param name="this">The list.</param>
	/// <returns>Returns the reference of the argument <paramref name="this"/>.</returns>
	public static IList<TEquatable> RemoveDuplicateItems<TEquatable>(this IList<TEquatable> @this)
	where TEquatable : IEquatable<TEquatable>
	{
		var set = new Set<TEquatable>(@this);
		@this.Clear();
		@this.AddRange(set);

		return @this;
	}

	/// <summary>
	/// Remove the last element of the specified list, which is equivalent to code:
	/// <code><![CDATA[list.RemoveAt(list.Count - 1);]]></code>
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The list.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveLastElement<T>(this IList<T?> @this) => @this.RemoveAt(@this.Count - 1);
}
