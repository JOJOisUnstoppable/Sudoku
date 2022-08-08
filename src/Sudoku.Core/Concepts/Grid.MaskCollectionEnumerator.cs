﻿namespace Sudoku.Concepts;

partial struct Grid
{
	/// <summary>
	/// Defines the default enumerator that iterates the <see cref="Grid"/>
	/// through the masks in the current <see cref="Grid"/> instance.
	/// </summary>
	public ref partial struct MaskCollectionEnumerator
	{
		/// <summary>
		/// The pointer to the start value.
		/// </summary>
		private readonly ref short _start;

		/// <summary>
		/// The current pointer.
		/// </summary>
		private ref short _refCurrent;

		/// <summary>
		/// The current index.
		/// </summary>
		private int _currentIndex = -1;


		/// <summary>
		/// Initializes an instance with the specified pointer to an array to iterate.
		/// </summary>
		/// <param name="arr">The pointer to an array.</param>
		/// <remarks>
		/// Note here we should point at the one-unit-length memory before the array start.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal MaskCollectionEnumerator(in short arr)
		{
			_refCurrent = ref Unsafe.SubtractByteOffset(ref Unsafe.AsRef(arr), 1);
			_start = ref _refCurrent;
		}


		/// <summary>
		/// Gets the element in the collection at the current position of the enumerator.
		/// </summary>
		public readonly ref short Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _refCurrent;
		}


		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// <list type="table">
		/// <listheader>
		/// <term>Return value</term>
		/// <description>Meaning</description>
		/// </listheader>
		/// <item>
		/// <term><see langword="true"/></term>
		/// <description>If the enumerator was successfully advanced to the next element.</description>
		/// </item>
		/// <item>
		/// <term><see langword="false"/></term>
		/// <description>If the enumerator has passed the end of the collection.</description>
		/// </item>
		/// </list>
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (++_currentIndex >= 81)
			{
				return false;
			}
			else
			{
				_refCurrent = ref Unsafe.AddByteOffset(ref _refCurrent, 1);
				return true;
			}
		}

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element
		/// in the collection.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			_refCurrent = ref _start;
			_currentIndex = -1;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly MaskCollectionEnumerator GetEnumerator() => this;
	}
}