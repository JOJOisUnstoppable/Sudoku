﻿namespace Sudoku.Concepts;

partial struct Grid
{
	partial struct GridCandidateEnumerator
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
		/// Indicates the current mask.
		/// </summary>
		private short _currentMask;

		/// <summary>
		/// The current index.
		/// </summary>
		private int _currentIndex = -1;


		/// <summary>
		/// Gets the element in the collection at the current position of the enumerator.
		/// </summary>
		public readonly int Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _currentIndex * 9 + TrailingZeroCount(_currentMask);
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
		public bool MoveNext()
		{
			if (_currentMask != 0 && (_currentMask &= (short)~(1 << TrailingZeroCount(_currentMask))) != 0)
			{
				return true;
			}

			while (_currentIndex < 81)
			{
				_currentIndex++;
				if (MaskToStatus(AddByteOffset(ref _start, _currentIndex)) == CellStatus.Empty)
				{
					break;
				}
			}

			if (_currentIndex == 81)
			{
				return false;
			}

			_refCurrent = ref AddByteOffset(ref _start, _currentIndex + 1);
			_currentMask = (short)(_refCurrent & Grid.MaxCandidatesMask);

			return true;
		}

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			_refCurrent = ref _start;
			_currentIndex = -1;
			_currentMask = default;
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly GridCandidateEnumerator GetEnumerator() => this;
	}
}
