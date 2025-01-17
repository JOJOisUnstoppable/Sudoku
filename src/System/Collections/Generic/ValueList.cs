﻿#pragma warning disable IDE0032, IDE0044

namespace System.Collections.Generic;

/// <summary>
/// Defines a value-type sequence list.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <remarks>
/// We recommend you use this type like:
/// <code><![CDATA[
/// static int[] Example()
/// {
///     using scoped var list = new ValueList<int>(10);
///     list.Add(3);
///     list.Add(6);
///     return list.ToArray();
/// }
/// ]]></code>
/// </remarks>
public unsafe ref partial struct ValueList<T> where T : notnull
{
	/// <summary>
	/// Indicates the length of the list.
	/// </summary>
	private byte _capacity;

	/// <summary>
	/// Indicates the current length.
	/// </summary>
	private byte _length = 0;

	/// <summary>
	/// Indicates the pointer that points to the first element.
	/// </summary>
	private T* _startPtr;


	/// <summary>
	/// Initializes a list that stores the specified number of elements.
	/// </summary>
	/// <param name="capacity">The initial capacity.</param>
	/// <remarks>
	/// If you call this constructor to initialize an instance, please append keyword <see langword="using"/>
	/// to implicitly call the dispose method.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueList(byte capacity)
	{
		_startPtr = (T*)NativeMemory.Alloc((nuint)sizeof(T) * capacity);
		_capacity = capacity;
	}


	/// <summary>
	/// Indicates the length of the list.
	/// </summary>
	public readonly byte Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _length;
	}

	/// <summary>
	/// Indicates the length of the list. The property is same as <see cref="Count"/>, but the property is used
	/// by slicing and list patterns.
	/// </summary>
	/// <seealso cref="Count"/>
	public readonly int Length
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _length;
	}


	/// <summary>
	/// Gets the element from the current list, or sets the element to the current list,
	/// with the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The reference to the element at the specified index.</returns>
	public readonly ref T this[byte index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref _startPtr[index];
	}

	/// <inheritdoc cref="this[byte]"/>
	public readonly ref T this[Index index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref _startPtr[index.GetOffset(_length)];
	}


	/// <summary>
	/// Adds a new element into the collection if the element does not exist in the collection.
	/// </summary>
	/// <param name="element">The element.</param>
	/// <param name="predicate">The predicate checking whether two elements are same.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddIfNotContain(T element, delegate*<T, T, bool> predicate)
	{
		if (!Contains(element, predicate))
		{
			Add(element);
		}
	}

	/// <summary>
	/// Adds the element to the current list.
	/// </summary>
	/// <param name="element">The element.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(T element)
	{
		Argument.ThrowIfInvalid(_length < _capacity, "Cannot add because the collection is full.");

		_startPtr[_length++] = element;
	}

	/// <summary>
	/// Adds a list of elements into the collection.
	/// </summary>
	/// <param name="elements">A list of elements.</param>
	public void AddRange(scoped in ValueList<T> elements)
	{
		foreach (var element in elements)
		{
			Add(element);
		}
	}

	/// <summary>
	/// Removes the last element from the collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove() => _length--;

	/// <summary>
	/// Removes all elements in this collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear() => _length = 0;

	/// <summary>
	/// To dispose the current list.
	/// </summary>
	/// <remarks><i>
	/// This method should be called when the constructor <see cref="ValueList{T}(byte)"/> is called.
	/// </i></remarks>
	/// <seealso cref="ValueList{T}(byte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose()
	{
		NativeMemory.Free(_startPtr);
		_startPtr = null;
	}

	/// <summary>
	/// Determines whether the specified element is in the current collection
	/// using the specified equality comparing method to define whether two instances are considered equal.
	/// </summary>
	/// <param name="instance">The instance to be determined.</param>
	/// <param name="predicate">A method that defines whether two instances are considered equal.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	public bool Contains(T instance, delegate*<T, T, bool> predicate)
	{
		foreach (var element in this)
		{
			if (predicate(element, instance))
			{
				return true;
			}
		}

		return false;
	}

	[GeneratedOverriddingMember(GeneratedToStringBehavior.CallOverloadWithNull)]
	public override readonly partial string ToString();

	/// <summary>
	/// Returns a string that represents the current object with the custom format string.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>The string that represents the current object.</returns>
	/// <exception cref="FormatException">Throws when the specified format is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format)
	{
		return format switch
		{
			null or "L" or "l" => $"ValueList<{typeof(T).Name}> {{ Count = {_length}, Capacity = {_capacity} }}",
			"C" or "c" => toContentString(this),
			"S" or "s" => $"ValueList<{typeof(T).Name}> {{ Size = {sizeof(T) * _length} }}",
			_ => throw new FormatException("The specified format doesn't support.")
		};


		static string toContentString(scoped in ValueList<T> @this)
		{
			const string separator = ", ";
			scoped var sb = new StringHandler();
			foreach (var element in @this)
			{
				sb.Append(element.ToString()!);
				sb.Append(separator);
			}

			sb.RemoveFromEnd(separator.Length);
			return $"[{sb.ToStringAndClear()}]";
		}
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Enumerator GetEnumerator() => new(this);

	/// <summary>
	/// Converts the current instance into an array of type <typeparamref name="T"/>.
	/// </summary>
	/// <returns>The array of elements of type <typeparamref name="T"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly T[] ToArray()
	{
		var result = new T[_length];
		fixed (T* pResult = result)
		{
			CopyBlock(pResult, _startPtr, (uint)(sizeof(T) * _length));
		}

		return result;
	}

	/// <summary>
	/// Converts the current instance into an immutable array of type <typeparamref name="T"/>.
	/// </summary>
	/// <returns>The array of elements of type <typeparamref name="T"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ImmutableArray<T> ToImmutableArray() => ImmutableArray.Create(ToArray());


	/// <summary>
	/// Defines the enumerator of this type.
	/// </summary>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// Initializes the <see cref="Enumerator"/> type via the current instance.
		/// </summary>
		/// <param name="ptr">The pointer that points to the list.</param>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(scoped in ValueList<T> ptr)
		{
			fixed (ValueList<T>* p = &ptr)
			{
				_ptr = p;
			}
		}
	}
}
