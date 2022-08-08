﻿namespace Sudoku.Text.Handlers;

/// <summary>
/// Provides a collection that contains the conclusions.
/// </summary>
public readonly ref partial struct ConclusionCollection
{
	/// <summary>
	/// The internal collection.
	/// </summary>
	private readonly Conclusion[] _collection;


	/// <summary>
	/// Initializes an instance with the specified collection.
	/// </summary>
	/// <param name="collection">The collection.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ConclusionCollection(Conclusion[] collection) : this() => _collection = collection;


	/// <summary>
	/// Indicates all cells used in this conclusions list.
	/// </summary>
	public Cells Cells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Cells)(from conclusion in _collection select conclusion.Cell);
	}

	/// <summary>
	/// Indicates all digits used in this conclusions list, represented as a mask.
	/// </summary>
	public short Digits
	{
		get
		{
			short result = 0;
			foreach (var conclusion in _collection)
			{
				result |= (short)(1 << conclusion.Digit);
			}

			return result;
		}
	}


	/// <inheritdoc/>
	/// <remarks>
	/// This method only compares for two collection's inner reference to their own array of conclusions.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ConclusionCollection other) => ReferenceEquals(_collection, other._collection);

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() => ToString(true, ", ");

	/// <summary>
	/// Converts the current instance to <see cref="string"/> with the specified separator.
	/// </summary>
	/// <param name="shouldSort">Indicates whether the specified collection should be sorted first.</param>
	/// <param name="separator">The separator.</param>
	/// <returns>The string result.</returns>
	public string ToString(bool shouldSort, string separator)
	{
		return _collection switch
		{
			[] => string.Empty,
			[var conclusion] => conclusion.ToString(),
			_ => f(_collection)
		};


		string f(scoped in scoped ReadOnlySpan<Conclusion> collection)
		{
			var conclusions = collection.ToArray();
			scoped var sb = new StringHandler(50);
			if (shouldSort)
			{
				unsafe
				{
					conclusions.Sort(&cmp);
				}

				var selection =
					from conclusion in conclusions
					orderby conclusion.Digit
					group conclusion by conclusion.ConclusionType;
				bool hasOnlyOneType = selection.HasOnlyOneElement();
				foreach (var typeGroup in selection)
				{
					string op = typeGroup.Key == ConclusionType.Assignment ? " = " : " <> ";
					foreach (var digitGroup in
						from conclusion in typeGroup
						group conclusion by conclusion.Digit)
					{
						sb.Append(Cells.Empty + from conclusion in digitGroup select conclusion.Cell);
						sb.Append(op);
						sb.Append(digitGroup.Key + 1);
						sb.Append(separator);
					}

					sb.RemoveFromEnd(separator.Length);
					if (!hasOnlyOneType)
					{
						sb.Append(separator);
					}
				}

				if (!hasOnlyOneType)
				{
					sb.RemoveFromEnd(separator.Length);
				}
			}
			else
			{
				sb.AppendRangeWithSeparator(conclusions, StringHandler.ElementToStringConverter, separator);
			}

			return sb.ToStringAndClear();
		}


		static int cmp(in Conclusion left, in Conclusion right) => left.CompareTo(right);
	}
}