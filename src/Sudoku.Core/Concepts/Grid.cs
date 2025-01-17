﻿#define TARGET_64BIT
namespace Sudoku.Concepts;

/// <summary>
/// Represents a sudoku grid that uses the mask list to construct the data structure.
/// </summary>
[IsLargeStruct]
[JsonConverter(typeof(Converter))]
[DebuggerDisplay($$"""{{{nameof(ToString)}}("#")}""")]
[GeneratedOverloadingOperator(GeneratedOperator.EqualityOperators)]
public unsafe partial struct Grid :
	IEnumerable<int>,
	IEqualityOperators<Grid, Grid, bool>,
	IFormattable,
	IMinMaxValue<Grid>,
	IParsable<Grid>,
	IReadOnlyCollection<int>,
	IReadOnlyList<int>,
	ISelectClauseProvider<short>,
	ISelectClauseProvider<int>,
	ISimpleFormattable,
	ISimpleParsable<Grid>
{
	/// <summary>
	/// Indicates the default mask of a cell (an empty cell, with all 9 candidates left).
	/// </summary>
	public const short DefaultMask = EmptyMask | MaxCandidatesMask;

	/// <summary>
	/// Indicates the maximum candidate mask that used.
	/// </summary>
	public const short MaxCandidatesMask = (1 << 9) - 1;

	/// <summary>
	/// Indicates the empty mask, modifiable mask and given mask.
	/// </summary>
	public const short EmptyMask = (int)CellStatus.Empty << 9;

	/// <summary>
	/// Indicates the modifiable mask.
	/// </summary>
	public const short ModifiableMask = (int)CellStatus.Modifiable << 9;

	/// <summary>
	/// Indicates the given mask.
	/// </summary>
	public const short GivenMask = (int)CellStatus.Given << 9;


	/// <summary>
	/// Indicates the empty grid string.
	/// </summary>
	public static readonly string EmptyString = new('0', 81);

	/// <summary>
	/// Indicates the event triggered when the value is changed.
	/// </summary>
	[DisallowFunctionPointerInvocation]
	public static readonly delegate*<ref Grid, int, short, short, int, void> ValueChanged;

	/// <summary>
	/// Indicates the event triggered when should re-compute candidates.
	/// </summary>
	[DisallowFunctionPointerInvocation]
	public static readonly delegate*<ref Grid, void> RefreshingCandidates;

	/// <summary>
	/// The empty grid that is valid during implementation or running the program (all values are <see cref="DefaultMask"/>, i.e. empty cells).
	/// </summary>
	/// <remarks>
	/// This field is initialized by the static constructor of this structure.
	/// </remarks>
	/// <seealso cref="DefaultMask"/>
	public static readonly Grid Empty;

	/// <summary>
	/// Indicates the default grid that all values are initialized 0.
	/// </summary>
	public static readonly Grid Undefined;

	/// <summary>
	/// Indicates the minimum possible grid value that the current type can reach.
	/// </summary>
	/// <remarks>
	/// This value is found out via backtracking algorithm. For more information about backtracking
	/// algorithm, please visit documentation comments in project <c>Sudoku.Solving.Algorithms</c>.
	/// </remarks>
	public static readonly Grid MinValue;

	/// <summary>
	/// Indicates the maximum possible grid value that the current type can reach.
	/// </summary>
	/// <remarks>
	/// This value is found out via backtracking algorithm. For more information about backtracking
	/// algorithm, please visit documentation comments in project <c>Sudoku.Solving.Algorithms</c>.
	/// </remarks>
	public static readonly Grid MaxValue;


	/// <summary>
	/// Indicates the inner array that stores the masks of the sudoku grid, which stores the in-time sudoku grid inner information.
	/// </summary>
	/// <remarks>
	/// The field uses the mask table of length 81 to indicate the status and all possible candidates
	/// holding for each cell. Each mask uses a <see cref="short"/> value, but only uses 11 of 16 bits.
	/// <code>
	/// 16       8       0
	///  |-------|-------|
	///  |   |--|--------|
	/// 16  12  9        0
	/// </code>
	/// Here the first-nine bits indicate whether the digit 1-9 is possible candidate in the current cell respectively,
	/// and the higher 3 bits indicate the cell status. The possible cell status are:
	/// <list type="table">
	/// <listheader>
	/// <term>Status name</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>Empty cell (i.e. <see cref="CellStatus.Empty"/>)</term>
	/// <description>The cell is currently empty, and wait for being filled.</description>
	/// </item>
	/// <item>
	/// <term>Modifiable cell (i.e. <see cref="CellStatus.Modifiable"/>)</term>
	/// <description>The cell is filled by a digit, but the digit isn't the given by the initial grid.</description>
	/// </item>
	/// <item>
	/// <term>Given cell (i.e. <see cref="CellStatus.Given"/>)</term>
	/// <description>The cell is filled by a digit, which is given by the initial grid and can't be modified.</description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="CellStatus"/>
	internal fixed short _values[81];


	/// <summary>
	/// Creates a <see cref="Grid"/> instance via the pointer of the first element of the cell digit,
	/// and the creating option.
	/// </summary>
	/// <param name="firstElement">
	/// <para>The reference of the first element.</para>
	/// <para>
	/// <include file='../../global-doc-comments.xml' path='g/csharp7/feature[@name="ref-returns"]/target[@name="in-parameter"]'/>
	/// </para>
	/// </param>
	/// <param name="creatingOption">The creating option.</param>
	/// <remarks>
	/// <include file='../../global-doc-comments.xml' path='g/csharp7/feature[@name="ref-returns"]/target[@name="method"]'/>
	/// </remarks>
	/// <exception cref="ArgumentNullRefException">
	/// Throws when the argument <paramref name="firstElement"/> is <see langword="null"/> reference.
	/// </exception>
	private Grid(scoped in int firstElement, GridCreatingOption creatingOption = GridCreatingOption.None)
	{
		ArgumentNullRefException.ThrowIfNullRef(ref AsRef(firstElement));

		// Firstly we should initialize the inner values.
		this = Empty;

		// Then traverse the array (span, pointer or etc.), to get refresh the values.
		var minusOneEnabled = creatingOption == GridCreatingOption.MinusOne;
		for (var i = 0; i < 81; i++)
		{
			var value = AddByteOffset(ref AsRef(firstElement), (nuint)(i * sizeof(int)));
			if ((minusOneEnabled ? value - 1 : value) is var realValue and not -1)
			{
				// Calls the indexer to trigger the event (Clear the candidates in peer cells).
				this[i] = realValue;

				// Set the status to 'CellStatus.Given'.
				SetStatus(i, CellStatus.Given);
			}
		}
	}


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static Grid()
	{
		// Initializes the empty grid.
		Empty = default;
		scoped ref var firstElement = ref Empty.GetMaskRef(0);
		for (var i = 0; i < 81; i++)
		{
			AddByteOffset(ref firstElement, (nuint)(i * sizeof(short))) = DefaultMask;
		}

		// Initializes events.
		ValueChanged = &onValueChanged;
		RefreshingCandidates = &onRefreshingCandidates;

		// Initializes special fields.
		Undefined = default; // This field must be initialized after parsing the following two special fields.
		MinValue = (Grid)"123456789456789123789123456214365897365897214897214365531642978642978531978531642";
		MaxValue = (Grid)"987654321654321987321987654896745213745213896213896745579468132468132579132579468";


		static void onRefreshingCandidates(ref Grid @this)
		{
			for (var i = 0; i < 81; i++)
			{
				if (@this.GetStatus(i) == CellStatus.Empty)
				{
					// Remove all appeared digits.
					var mask = MaxCandidatesMask;
					foreach (var cell in PeersMap[i])
					{
						if (@this[cell] is var digit and not -1)
						{
							mask &= (short)~(1 << digit);
						}
					}

					@this._values[i] = (short)(EmptyMask | mask);
				}
			}
		}

		static void onValueChanged(ref Grid @this, int cell, short oldMask, short newMask, int setValue)
		{
			if (setValue != -1)
			{
				foreach (var peerCell in PeersMap[cell])
				{
					if (@this.GetStatus(peerCell) == CellStatus.Empty)
					{
						// You can't do this because of being invoked recursively.
						//@this[peerCell, setValue] = false;

						@this._values[peerCell] &= (short)~(1 << setValue);
					}
				}
			}
		}
	}


	/// <summary>
	/// Indicates the grid has already solved. If the value is <see langword="true"/>,
	/// the grid is solved; otherwise, <see langword="false"/>.
	/// </summary>
	public readonly bool IsSolved
	{
		get
		{
			for (var i = 0; i < 81; i++)
			{
				if (GetStatus(i) == CellStatus.Empty)
				{
					return false;
				}
			}

			for (var i = 0; i < 81; i++)
			{
				switch (GetStatus(i))
				{
					case CellStatus.Given or CellStatus.Modifiable:
					{
						var curDigit = this[i];
						foreach (var cell in PeersMap[i])
						{
							if (curDigit == this[cell])
							{
								return false;
							}
						}

						break;
					}
					case CellStatus.Empty:
					{
						continue;
					}
					default:
					{
						return false;
					}
				}
			}

			return true;
		}
	}

	/// <summary>
	/// Indicates whether the grid is <see cref="Undefined"/>, which means the grid
	/// holds totally same value with <see cref="Undefined"/>.
	/// </summary>
	/// <seealso cref="Undefined"/>
	public readonly bool IsUndefined
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this == Undefined;
	}

	/// <summary>
	/// Indicates whether the grid is <see cref="Empty"/>, which means the grid
	/// holds totally same value with <see cref="Empty"/>.
	/// </summary>
	/// <seealso cref="Empty"/>
	public readonly bool IsEmpty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this == Empty;
	}

	/// <summary>
	/// Indicates the number of total candidates.
	/// </summary>
	public readonly int CandidatesCount
	{
		get
		{
			var count = 0;
			for (var i = 0; i < 81; i++)
			{
				if (GetStatus(i) == CellStatus.Empty)
				{
					count += PopCount((uint)GetCandidates(i));
				}
			}

			return count;
		}
	}

	/// <summary>
	/// Indicates the total number of given cells.
	/// </summary>
	public readonly int GivensCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => GivenCells.Count;
	}

	/// <summary>
	/// Indicates the total number of modifiable cells.
	/// </summary>
	public readonly int ModifiablesCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ModifiableCells.Count;
	}

	/// <summary>
	/// Indicates the total number of empty cells.
	/// </summary>
	public readonly int EmptiesCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => EmptyCells.Count;
	}

	/// <summary>
	/// <para>Indicates which houses are null houses.</para>
	/// <para>A <b>Null House</b> is a house whose hold cells are all empty cells.</para>
	/// <para>
	/// The property returns an <see cref="int"/> value as a mask that contains all possible house indices.
	/// For example, if the row 5, column 5 and block 5 (1-9) are null houses, the property will return
	/// the result <see cref="int"/> value, <c>000010000_000010000_000010000</c> as binary.
	/// </para>
	/// </summary>
	public readonly int NullHouses
	{
		get
		{
			var result = 0;
			for (var (houseIndex, valueCells) = (0, ~EmptyCells); houseIndex < 27; houseIndex++)
			{
				if (valueCells / houseIndex == 0)
				{
					result |= 1 << houseIndex;
				}
			}

			return result;
		}
	}

	/// <summary>
	/// Gets the cell template that only contains the given cells.
	/// </summary>
	public readonly CellMap GivenCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => g.GetStatus(cell) == CellStatus.Given;
		}
	}

	/// <summary>
	/// Gets the cell template that only contains the modifiable cells.
	/// </summary>
	public readonly CellMap ModifiableCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => g.GetStatus(cell) == CellStatus.Modifiable;
		}
	}

	/// <summary>
	/// Indicates the cells that corresponding position in this grid is empty.
	/// </summary>
	public readonly CellMap EmptyCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => g.GetStatus(cell) == CellStatus.Empty;
		}
	}

	/// <summary>
	/// Indicates the cells that corresponding position in this grid contain two candidates.
	/// </summary>
	public readonly CellMap BivalueCells
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetCells(&p);


			static bool p(in Grid g, int cell) => PopCount((uint)g.GetCandidates(cell)) == 2;
		}
	}

	/// <summary>
	/// Indicates the map of possible positions of the existence of the candidate value for each digit.
	/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </summary>
	public readonly CellMap[] CandidatesMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetMap(&p);


			static bool p(in Grid g, int cell, int digit) => g.Exists(cell, digit) is true;
		}
	}

	/// <summary>
	/// <para>
	/// Indicates the map of possible positions of the existence of each digit. The return value will
	/// be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </para>
	/// <para>
	/// Different with <see cref="CandidatesMap"/>, this property contains all givens, modifiables and
	/// empty cells only if it contains the digit in the mask.
	/// </para>
	/// </summary>
	/// <seealso cref="CandidatesMap"/>
	public readonly CellMap[] DigitsMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetMap(&p);


			static bool p(in Grid g, int cell, int digit) => (g.GetCandidates(cell) >> digit & 1) != 0;
		}
	}

	/// <summary>
	/// <para>
	/// Indicates the map of possible positions of the existence of that value of each digit.
	/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </para>
	/// <para>
	/// Different with <see cref="CandidatesMap"/>, the value only contains the given or modifiable
	/// cells whose mask contain the set bit of that digit.
	/// </para>
	/// </summary>
	/// <seealso cref="CandidatesMap"/>
	public readonly CellMap[] ValuesMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetMap(&p);


			static bool p(in Grid g, int cell, int digit) => g[cell] == digit;
		}
	}

	/// <summary>
	/// Indicates all possible conjugate pairs appeared in this grid.
	/// </summary>
	public readonly ImmutableArray<Conjugate> ConjugatePairs
	{
		get
		{
			var candidatesMap = CandidatesMap; // Cache the map.

			var conjugatePairs = new List<Conjugate>();
			for (var digit = 0; digit < 9; digit++)
			{
				scoped ref readonly var cellsMap = ref candidatesMap[digit];
				for (var houseIndex = 0; houseIndex < 27; houseIndex++)
				{
					if ((HousesMap[houseIndex] & cellsMap) is { Count: 2 } temp)
					{
						conjugatePairs.Add(new(temp, digit));
					}
				}
			}

			return ImmutableArray.CreateRange(conjugatePairs);
		}
	}

	/// <summary>
	/// Gets the grid where all modifiable cells are empty cells (i.e. the initial one).
	/// </summary>
	public readonly Grid ResetGrid => Preserve(GivenCells);

	/// <inheritdoc/>
	readonly int IReadOnlyCollection<int>.Count => 81;

	/// <inheritdoc/>
	static Grid IMinMaxValue<Grid>.MinValue => MinValue;

	/// <inheritdoc/>
	static Grid IMinMaxValue<Grid>.MaxValue => MaxValue;


	/// <summary>
	/// Gets or sets the digit that has been filled in the specified cell.
	/// </summary>
	/// <param name="cell">The cell you want to get or set a value.</param>
	/// <value>
	/// <para>
	/// The value you want to set. The value should be between 0 and 8.
	/// If assigning -1, the grid will execute an implicit behavior that candidates in <b>all</b> empty cells will be re-computed.
	/// </para>
	/// <para>
	/// The values set into the grid will be regarded as the modifiable values.
	/// If the cell contains a digit, it will be covered when it is a modifiable value.
	/// If the cell is a given cell, the setter will do nothing.
	/// </para>
	/// </value>
	/// <returns>
	/// The value that the cell filled with. The possible values are:
	/// <list type="table">
	/// <item>
	/// <term>-1</term>
	/// <description>The status of the specified cell is <see cref="CellStatus.Empty"/>.</description>
	/// </item>
	/// <item>
	/// <term><![CDATA[>= 0 and < 9]]></term>
	/// <description>The actual value that the cell filled with. 0 is for the digit 1, 1 is for the digit 2, etc..</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified cell keeps a wrong cell status value. For example, <see cref="CellStatus.Undefined"/>.
	/// </exception>
	public int this[int cell]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get => GetStatus(cell) switch
		{
			CellStatus.Empty => -1,
			CellStatus.Modifiable or CellStatus.Given => TrailingZeroCount(_values[cell]),
			_ => throw new InvalidOperationException("The grid cannot keep invalid cell status value.")
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			switch (value)
			{
				case -1 when GetStatus(cell) == CellStatus.Modifiable:
				{
					// If 'value' is -1, we should reset the grid.
					// Note that reset candidates may not trigger the event.
					_values[cell] = DefaultMask;

					RefreshingCandidates(ref this);

					break;
				}
				case >= 0 and < 9:
				{
					scoped ref var result = ref _values[cell];
					var copied = result;

					// Set cell status to 'CellStatus.Modifiable'.
					result = (short)(ModifiableMask | 1 << value);

					// To trigger the event, which is used for eliminate all same candidates in peer cells.
					ValueChanged(ref this, cell, copied, result, value);

					break;
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets a candidate existence case with a <see cref="bool"/> value.
	/// </summary>
	/// <param name="cell">The cell offset between 0 and 80.</param>
	/// <param name="digit">The digit between 0 and 8.</param>
	/// <value>
	/// The case you want to set. <see langword="false"/> means that this candidate
	/// doesn't exist in this current sudoku grid; otherwise, <see langword="true"/>.
	/// </value>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	public bool this[int cell, int digit]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get => (_values[cell] >> digit & 1) != 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (cell is >= 0 and < 81 && digit is >= 0 and < 9)
			{
				var copied = _values[cell];
				if (value)
				{
					_values[cell] |= (short)(1 << digit);
				}
				else
				{
					_values[cell] &= (short)~(1 << digit);
				}

				// To trigger the event.
				ValueChanged(ref this, cell, copied, _values[cell], -1);
			}
		}
	}


	[GeneratedOverriddingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override readonly partial bool Equals(object? obj);

	/// <summary>
	/// Determine whether the specified <see cref="Grid"/> instance hold the same values as the current instance.
	/// </summary>
	/// <param name="other">The instance to compare.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(scoped in Grid other)
		=> Helper.SequenceEqual(ref AsByteRef(ref AsRef(_values[0])), ref AsByteRef(ref AsRef(other._values[0])), sizeof(short) * 81);

	/// <summary>
	/// Determine whether the digit in the target cell may be duplicated with a certain cell in the peers of the current cell,
	/// if the digit is filled into the cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public readonly bool DuplicateWith(int cell, int digit)
	{
		foreach (var tempCell in PeersMap[cell])
		{
			if (this[tempCell] == digit)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Indicates whether the current grid contains the specified candidate offset.
	/// </summary>
	/// <param name="candidate">The candidate offset.</param>
	/// <returns><inheritdoc cref="Exists(int, int)" path="/returns"/></returns>
	/// <remarks><inheritdoc cref="Exists(int, int)" path="/remarks"/></remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(int candidate) => Exists(candidate / 9, candidate % 9);

	/// <summary>
	/// Indicates whether the current grid contains the digit in the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>
	/// The method will return a <see cref="bool"/>? value
	/// (containing three possible cases: <see langword="true"/>, <see langword="false"/> and <see langword="null"/>).
	/// All values corresponding to the cases are below:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Case description on this value</description>
	/// </listheader>
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>
	/// The cell is an empty cell <b>and</b> contains the specified digit.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>
	/// The cell is an empty cell <b>but doesn't</b> contain the specified digit.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The cell is <b>not</b> an empty cell.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// <para>
	/// Note that the method will return a <see cref="bool"/>?, so you should use the code
	/// '<c>grid.Exists(cell, digit) is true</c>' or '<c>grid.Exists(cell, digit) == true</c>'
	/// to decide whether a condition is true.
	/// </para>
	/// <para>
	/// In addition, because the type is <see cref="bool"/>? rather than <see cref="bool"/>,
	/// the result case will be more precisely than the indexer <see cref="this[int, int]"/>,
	/// which is the main difference between this method and that indexer.
	/// </para>
	/// </remarks>
	/// <seealso cref="this[int, int]"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool? Exists(int cell, int digit) => GetStatus(cell) == CellStatus.Empty ? this[cell, digit] : null;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
		=> this switch { { IsUndefined: true } => 0, { IsEmpty: true } => 1, _ => ToString("#").GetHashCode() };

	/// <summary>
	/// Serializes this instance to an array, where all digit value will be stored.
	/// </summary>
	/// <returns>
	/// This array. All elements are between 0 and 9, where 0 means the cell is <see cref="CellStatus.Empty"/> now.
	/// </returns>
	public readonly int[] ToArray()
	{
		var result = new int[81];
		for (var i = 0; i < 81; i++)
		{
			// 'this[i]' is always between -1 and 8 (-1 is empty, and 0 to 8 is 1 to 9 for human representation).
			result[i] = this[i] + 1;
		}

		return result;
	}

	/// <summary>
	/// Get a mask at the specified cell.
	/// </summary>
	/// <param name="offset">The cell offset you want to get.</param>
	/// <returns>The mask.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly short GetMask(int offset) => _values[offset];

	/// <summary>
	/// Get the candidate mask part of the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset you want to get.</param>
	/// <returns>
	/// <para>
	/// The candidate mask. The return value is a 9-bit <see cref="short"/>
	/// value, where each bit will be:
	/// <list type="table">
	/// <item>
	/// <term><c>0</c></term>
	/// <description>The cell <b>doesn't contain</b> the possibility of the digit.</description>
	/// </item>
	/// <item>
	/// <term><c>1</c></term>
	/// <description>The cell <b>contains</b> the possibility of the digit.</description>
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// For example, if the result mask is 266 (i.e. <c>0b<b>1</b>00_00<b>1</b>_0<b>1</b>0</c> in binary),
	/// the value will indicate the cell contains the digit 2, 4 and 9.
	/// </para>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly short GetCandidates(int cell) => (short)(_values[cell] & MaxCandidatesMask);

	/// <inheritdoc cref="GetDigitsUnion(in CellMap)"/>
	public readonly short GetDigitsUnion(int[] cells)
	{
		var result = (short)0;
		for (var i = 0; i < cells.Length; i++)
		{
			result |= _values[cells[i]];
		}

		return (short)(result & MaxCandidatesMask);
	}

	/// <summary>
	/// Creates a mask of type <see cref="short"/> that represents the usages of digits 1 to 9,
	/// ranged in a specified list of cells in the current sudoku grid.
	/// </summary>
	/// <param name="cells">The list of cells to gather the usages on all digits.</param>
	/// <returns>A mask of type <see cref="short"/> that represents the usages of digits 1 to 9.</returns>
	public readonly short GetDigitsUnion(scoped in CellMap cells)
	{
		var result = (short)0;
		foreach (var cell in cells)
		{
			result |= _values[cell];
		}

		return (short)(result & MaxCandidatesMask);
	}

	/// <summary>
	/// <inheritdoc cref="GetDigitsUnion(in CellMap)" path="/summary"/>
	/// </summary>
	/// <param name="cells"><inheritdoc cref="GetDigitsUnion(in CellMap)" path="/param[@name='cells']"/></param>
	/// <param name="withValueCells">
	/// Indicates whether the value cells (given or modifiable ones) will be included to be gathered.
	/// If <see langword="true"/>, all value cells (no matter what kind of cell) will be summed up.
	/// </param>
	/// <returns><inheritdoc cref="GetDigitsUnion(in CellMap)" path="/returns"/></returns>
	public readonly short GetDigitsUnion(scoped in CellMap cells, bool withValueCells)
	{
		var result = (short)0;
		foreach (var cell in cells)
		{
			if (!withValueCells && GetStatus(cell) != CellStatus.Empty || withValueCells)
			{
				result |= _values[cell];
			}
		}

		return (short)(result & MaxCandidatesMask);
	}

	/// <summary>
	/// Creates a mask of type <see cref="short"/> that represents the usages of digits 1 to 9,
	/// ranged in a specified list of cells in the current sudoku grid,
	/// to determine which digits are not used.
	/// </summary>
	/// <param name="cells">The list of cells to gather the usages on all digits.</param>
	/// <returns>A mask of type <see cref="short"/> that represents the usages of digits 1 to 9.</returns>
	public readonly short GetDigitsIntersection(scoped in CellMap cells)
	{
		var result = MaxCandidatesMask;
		foreach (var cell in cells)
		{
			result &= (short)~_values[cell];
		}

		return result;
	}

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='custom-fixed']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ref readonly short GetPinnableReference() => ref _values[0];

	[GeneratedOverriddingMember(GeneratedToStringBehavior.CallOverloadWithNull)]
	public override readonly partial string ToString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format)
		=> this switch
		{
			{ IsEmpty: true } => $"<{nameof(Empty)}>",
			{ IsUndefined: true } => $"<{nameof(Undefined)}>",
			_ => GridFormatterFactory.GetBuiltInFormatter(format)?.ToString(this) ?? throw new FormatException("The specified format is invalid.")
		};

	/// <summary>
	/// Gets <see cref="string"/> representation of the current grid, using pre-defined grid formatters.
	/// </summary>
	/// <param name="gridFormatter">
	/// The grid formatter instance to format the current grid.
	/// </param>
	/// <returns>The <see cref="string"/> result.</returns>
	/// <remarks>
	/// <para>
	/// The target and supported types are stored in namespace <see cref="Text.Formatting"/>.
	/// If you don't remember the full format strings, you can try this method instead by passing
	/// actual <see cref="IGridFormatter"/> instances.
	/// </para>
	/// <para>
	/// For example, by using Susser formatter <see cref="SusserFormat"/> instances:
	/// <code><![CDATA[
	/// // Suppose the variable is of type 'Grid'.
	/// var grid = ...;
	/// 
	/// // Creates a Susser-based formatter, with placeholder text as '0',
	/// // missing candidates output and modifiable distinction.
	/// var formatter = SusserFormat.Default with
	/// {
	///     Placeholder = '0',
	///     WithCandidates = true,
	///     WithModifiables = true
	/// };
	/// 
	/// // Using this method to get the target string representation.
	/// string targetStr = grid.ToString(formatter);
	/// 
	/// // Output the result.
	/// Console.WriteLine(targetStr);
	/// ]]></code>
	/// </para>
	/// <para>
	/// In some cases we suggest you use this method instead of calling <see cref="ToString(string?)"/>
	/// and <see cref="ToString(string?, IFormatProvider?)"/> because you may not remember all possible string formats.
	/// </para>
	/// <para>
	/// In addition, the method <see cref="ToString(string?, IFormatProvider?)"/> is also compatible with this method.
	/// If you forget to call this one, you can also use that method to get the same target result by passing first argument
	/// named <c>format</c> with <see langword="null"/> value:
	/// <code><![CDATA[
	/// string targetStr = grid.ToString(null, formatter);
	/// ]]></code>
	/// </para>
	/// </remarks>
	/// <seealso cref="Text.Formatting"/>
	/// <seealso cref="IGridFormatter"/>
	/// <seealso cref="SusserFormat"/>
	/// <seealso cref="ToString(string?)"/>
	/// <seealso cref="ToString(string?, IFormatProvider?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(IGridFormatter gridFormatter) => gridFormatter.ToString(this);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string? format, IFormatProvider? formatProvider)
		=> (format, formatProvider) switch
		{
			(null, null) => ToString(SusserFormat.Default),
			(not null, _) => ToString(format),
			(_, IGridFormatter formatter) => formatter.ToString(this),
			(_, ICustomFormatter formatter) => formatter.Format(format, this, formatProvider),
			(_, CultureInfo { Name: "zh-CN" }) => ToString(SusserFormat.Full),
			(_, CultureInfo { Name: ['e', 'n', '-', >= 'A' and <= 'Z', >= 'A' and <= 'Z'] }) => ToString(MultipleLineFormat.Default),
			_ => ToString(SusserFormat.Default)
		};

	/// <summary>
	/// Get the cell status at the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The cell status.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly CellStatus GetStatus(int cell) => MaskToStatus(_values[cell]);

	/// <summary>
	/// Gets the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly GridCandidateEnumerator GetEnumerator() => EnumerateCandidates();

	/// <summary>
	/// Try to enumerate all possible candidates in the current grid.
	/// </summary>
	/// <returns>
	/// An enumerator that allows us using <see langword="foreach"/> statement
	/// to iterate all possible candidates in the current grid.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly GridCandidateEnumerator EnumerateCandidates() => new(ref AsRef(_values[0]));

	/// <summary>
	/// Try to enumerate the mask table of the current grid.
	/// </summary>
	/// <returns>
	/// An enumerator that allows us using <see langword="foreach"/> statement
	/// to iterate all masks in the current grid. The mask list must contain 81 masks.
	/// </returns>
	/// <remarks>
	/// <para>
	/// Please note that the iterator will iterate all masks by reference, which means
	/// you can apply <see langword="ref"/> and <see langword="ref readonly"/> modifier
	/// onto the iteration variable:
	/// <code>
	/// foreach (ref readonly short mask in grid)
	/// {
	///     // Do something.
	/// }
	/// </code>
	/// </para>
	/// <para>
	/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="scoped-ref"]/target[@name="foreach-variables"]'/>
	/// </para>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly GridMaskEnumerator EnumerateMasks() => new(ref AsRef(_values[0]));

	/// <summary>
	/// Reset the sudoku grid, to set all modifiable values to empty ones.
	/// </summary>
	public void Reset()
	{
		for (var i = 0; i < 81; i++)
		{
			if (GetStatus(i) == CellStatus.Modifiable)
			{
				this[i] = -1; // Reset the cell, and then re-compute all candidates.
			}
		}
	}

	/// <summary>
	/// To fix the current grid (all modifiable values will be changed to given ones).
	/// </summary>
	public void Fix()
	{
		for (var i = 0; i < 81; i++)
		{
			if (GetStatus(i) == CellStatus.Modifiable)
			{
				SetStatus(i, CellStatus.Given);
			}
		}
	}

	/// <summary>
	/// To unfix the current grid (all given values will be changed to modifiable ones).
	/// </summary>
	public void Unfix()
	{
		for (var i = 0; i < 81; i++)
		{
			if (GetStatus(i) == CellStatus.Given)
			{
				SetStatus(i, CellStatus.Modifiable);
			}
		}
	}

	/// <summary>
	/// Set the specified cell to the specified status.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="status">The status.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetStatus(int cell, CellStatus status)
	{
		scoped ref var mask = ref _values[cell];
		var copied = mask;
		mask = (short)((int)status << 9 | mask & MaxCandidatesMask);

		ValueChanged(ref this, cell, copied, mask, -1);
	}

	/// <summary>
	/// Set the specified cell to the specified mask.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="mask">The mask to set.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMask(int cell, short mask)
	{
		scoped ref var m = ref _values[cell];
		var copied = m;
		m = mask;

		ValueChanged(ref this, cell, copied, m, -1);
	}

	/// <summary>
	/// Gets the mask at the specified position. This method returns the reference of the mask.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The reference to the mask that you want to get.</returns>
	/// <remarks>
	/// This method returns the reference, which means you can use this method as an l-value.
	/// For example, if you want to use bitwise-or operator to update the value, you can use:
	/// <code><![CDATA[
	/// // Update the mask.
	/// short mask = ...;
	/// grid.GetMaskRefAt(0) |= mask;
	/// ]]></code>
	/// The expression <c>grid.GetMaskRefAt(0) |= mask</c> is equivalent to
	/// <c>grid.GetMaskRefAt(0) = grid.GetMaskRefAt(0) | mask</c>, and it can be replaced
	/// with the expression <c>grid._values[0] = grid._values[0] | mask</c>,
	/// meaning we update the mask at the first place (i.e. <c>r1c1</c>).
	/// </remarks>
	/// <seealso cref="GetPinnableReference"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref short GetMaskRef(int index) => ref _values[index];

	/// <summary>
	/// Gets a sudoku grid, replacing all digits with modifiable
	/// if it doesn't appear in the specified <paramref name="pattern"/> from the solution of the current grid.
	/// </summary>
	/// <param name="pattern">The pattern.</param>
	/// <returns>The result grid.</returns>
	internal readonly Grid Unfix(scoped in CellMap pattern)
	{
		Argument.ThrowIfFalse(IsSolved, "The current grid must be solved.");

		var result = this;
		foreach (var cell in ~pattern)
		{
			if (result.GetStatus(cell) == CellStatus.Given)
			{
				result.SetStatus(cell, CellStatus.Modifiable);
			}
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator IEnumerable.GetEnumerator() => ToArray().GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator<int> IEnumerable<int>.GetEnumerator() => ((IEnumerable<int>)ToArray()).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator<short> IEnumerable<short>.GetEnumerator()
	{
		var maskArray = new short[81];
		CopyBlock(ref AsByteRef(ref AsRef(_values[0])), ref AsByteRef(ref maskArray[0]), sizeof(short) * 81);

		return ((IEnumerable<short>)maskArray).GetEnumerator();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerable<TResult> ISelectClauseProvider<int>.Select<TResult>(Func<int, TResult> selector) => this.Select(selector);

	/// <inheritdoc/>
	readonly IEnumerable<TResult> ISelectClauseProvider<short>.Select<TResult>(Func<short, TResult> selector)
	{
		var result = new TResult[81];
		var i = 0;
		foreach (var mask in EnumerateMasks())
		{
			result[i++] = selector(mask);
		}

		return result;
	}

	/// <summary>
	/// Called by properties <see cref="CandidatesMap"/>, <see cref="DigitsMap"/> and <see cref="ValuesMap"/>.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The map of digits.</returns>
	/// <seealso cref="CandidatesMap"/>
	/// <seealso cref="DigitsMap"/>
	/// <seealso cref="ValuesMap"/>
	private readonly CellMap[] GetMap(delegate*<in Grid, int, int, bool> predicate)
	{
		var result = new CellMap[9];
		for (var digit = 0; digit < 9; digit++)
		{
			scoped ref var map = ref result[digit];
			for (var cell = 0; cell < 81; cell++)
			{
				if (predicate(this, cell, digit))
				{
					map.Add(cell);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Called by properties <see cref="EmptyCells"/> and <see cref="BivalueCells"/>.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>The cells.</returns>
	/// <seealso cref="EmptyCells"/>
	/// <seealso cref="BivalueCells"/>
	private readonly CellMap GetCells(delegate*<in Grid, int, bool> predicate)
	{
		var result = CellMap.Empty;
		for (var cell = 0; cell < 81; cell++)
		{
			if (predicate(this, cell))
			{
				result.Add(cell);
			}
		}

		return result;
	}

	/// <summary>
	/// Gets a sudoku grid, removing all value digits not appearing in the specified <paramref name="pattern"/>.
	/// </summary>
	/// <param name="pattern">The pattern.</param>
	/// <returns>The result grid.</returns>
	private readonly Grid Preserve(scoped in CellMap pattern)
	{
		var result = this;
		foreach (var cell in ~pattern)
		{
			result[cell] = -1;
		}

		return result;
	}


	/// <summary>
	/// Creates a <see cref="Grid"/> instance using grid values.
	/// </summary>
	/// <param name="gridValues">The array of grid values.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Create(int[] gridValues, GridCreatingOption creatingOption = GridCreatingOption.None)
		=> new(gridValues[0], creatingOption);

	/// <summary>
	/// Creates a <see cref="Grid"/> instance with the specified mask array.
	/// </summary>
	/// <param name="masks">The masks.</param>
	/// <exception cref="ArgumentException">Throws when <see cref="Array.Length"/> is not 81.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Create(short[] masks)
	{
		Argument.ThrowIfNotEqual(masks.Length, 81, nameof(masks));

		var result = Empty;
		CopyBlock(ref AsByteRef(ref result._values[0]), ref AsByteRef(ref masks[0]), sizeof(short) * 81);

		return result;
	}

	/// <summary>
	/// Creates a <see cref="Grid"/> instance via the array of cell digits
	/// of type <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <param name="gridValues">The list of cell digits.</param>
	/// <param name="creatingOption">The grid creating option.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Create(scoped ReadOnlySpan<int> gridValues, GridCreatingOption creatingOption = GridCreatingOption.None)
		=> new(gridValues[0], creatingOption);

	/// <inheritdoc/>
	/// <remarks>
	/// We suggest you use <see cref="op_Explicit(string)"/> to achieve same goal if the passing argument is a constant.
	/// For example:
	/// <code><![CDATA[
	/// var grid1 = (Grid)"123456789456789123789123456214365897365897214897214365531642978642978531978531642";
	/// var grid2 = (Grid)"987654321654321987321987654896745213745213896213896745579468132468132579132579468";
	/// var grid3 = Grid.Parse(stringCode); // 'stringCode' is a string, not null.
	/// ]]></code>
	/// </remarks>
	/// <seealso cref="op_Explicit(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string str) => new GridParser(str).Parse();

	/// <summary>
	/// <para>
	/// Parses a string value and converts to this type.
	/// </para>
	/// <para>
	/// If you want to parse a PM grid, you should decide the mode to parse.
	/// If you use compatible mode to parse, all single values will be treated as
	/// given values; otherwise, recommended mode, which uses '<c><![CDATA[<d>]]></c>'
	/// or '<c>*d*</c>' to represent a value be a given or modifiable one. The decision
	/// will be indicated and passed by the second parameter <paramref name="compatibleFirst"/>.
	/// </para>
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the parsing operation should use compatible mode to check PM grid.
	/// </param>
	/// <returns>The result instance had converted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string str, bool compatibleFirst) => new GridParser(str, compatibleFirst).Parse();

	/// <summary>
	/// Parses a string value and converts to this type, using a specified grid parsing type.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="gridParsingOption">The grid parsing type.</param>
	/// <returns>The result instance had converted.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(string str, GridParsingOption gridParsingOption) => new GridParser(str).Parse(gridParsingOption);

	/// <summary>
	/// <para>Parses a string value and converts to this type.</para>
	/// <para>
	/// If you want to parse a PM grid, we recommend you use the method
	/// <see cref="Parse(string, GridParsingOption)"/> instead of this method.
	/// </para>
	/// </summary>
	/// <param name="str">The string.</param>
	/// <returns>The result instance had converted.</returns>
	/// <seealso cref="Parse(string, GridParsingOption)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Parse(scoped ReadOnlySpan<char> str) => new GridParser(str.ToString()).Parse();

	/// <inheritdoc/>
	public static bool TryParse(string str, out Grid result)
	{
		try
		{
			result = Parse(str);
			return !result.IsUndefined;
		}
		catch (FormatException)
		{
			result = Undefined;
			return false;
		}
	}

	/// <summary>
	/// Try to parse a string and converts to this type, and returns a
	/// <see cref="bool"/> value indicating the result of the conversion.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="option">The grid parsing type.</param>
	/// <param name="result">
	/// The result parsed. If the conversion is failed, this argument will be <see cref="Undefined"/>.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <seealso cref="Undefined"/>
	public static bool TryParse(string str, GridParsingOption option, out Grid result)
	{
		try
		{
			result = Parse(str, option);
			return true;
		}
		catch (FormatException)
		{
			result = Undefined;
			return false;
		}
	}

	/// <inheritdoc cref="TryParse(string, out Grid)"/>
	public static bool TryParse(Utf8String str, out Grid result)
	{
		try
		{
			result = Parse(str);
			return !result.IsUndefined;
		}
		catch (FormatException)
		{
			result = Undefined;
			return false;
		}
	}

	/// <inheritdoc cref="TryParse(string, GridParsingOption, out Grid)"/>
	public static bool TryParse(Utf8String str, GridParsingOption option, out Grid result)
	{
		try
		{
			result = Parse(str, option);
			return true;
		}
		catch (FormatException)
		{
			result = Undefined;
			return false;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Grid IParsable<Grid>.Parse(string s, IFormatProvider? provider) => Parse(s);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IParsable<Grid>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Grid result)
		=> s is not null && TryParse(s, out result);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Grid, Grid, bool>.operator ==(Grid left, Grid right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Grid, Grid, bool>.operator !=(Grid left, Grid right) => left != right;


	/// <summary>
	/// Implicit cast from <see cref="string"/> code to its equivalent <see cref="Grid"/> instance representation.
	/// </summary>
	/// <param name="gridCode">The grid code.</param>
	/// <remarks>
	/// <para>
	/// This explicit operator has same meaning for method <see cref="Parse(string)"/>. You can also use
	/// <see cref="Parse(string)"/> to get the same result as this operator.
	/// </para>
	/// <para>
	/// If the argument being passed is <see langword="null"/>, this operator will return <see cref="Undefined"/>
	/// as the final result, whose behavior is the only one that is different with method <see cref="Parse(string)"/>.
	/// That method will throw a <see cref="FormatException"/> instance to report the invalid argument being passed.
	/// </para>
	/// </remarks>
	/// <exception cref="FormatException">
	/// See exception thrown cases for method <see cref="ISimpleParsable{TSimpleParseable}.Parse(string)"/>.
	/// </exception>
	/// <seealso cref="Undefined"/>
	/// <seealso cref="Parse(string)"/>
	/// <seealso cref="ISimpleParsable{TSimpleParseable}.Parse(string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Grid([ConstantExpected] string? gridCode) => gridCode is null ? Undefined : Parse(gridCode);


	/// <summary>
	/// Defines the default enumerator that iterates the <see cref="Grid"/> through the candidates in the current <see cref="Grid"/> instance.
	/// </summary>
	/// <see cref="Grid"/>
	public ref partial struct GridCandidateEnumerator
	{
		/// <summary>
		/// Initializes an instance with the specified reference to an array to iterate.
		/// </summary>
		/// <param name="arr">The reference to an array.</param>
		/// <remarks>
		/// Please note that the argument <paramref name="arr"/> must be a reference instead of a constant,
		/// even though C# allows we passing a constant as an <see langword="in"/> argument.
		/// </remarks>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal GridCandidateEnumerator(ref short arr)
		{
			_refCurrent = ref SubtractByteOffset(ref arr, 1);
			_start = ref _refCurrent;
		}
	}

	/// <summary>
	/// Defines the default enumerator that iterates the <see cref="Grid"/> through the masks in the current <see cref="Grid"/> instance.
	/// </summary>
	/// <seealso cref="Grid"/>
	public ref partial struct GridMaskEnumerator
	{
		/// <summary>
		/// Initializes an instance with the specified pointer to an array to iterate.
		/// </summary>
		/// <param name="arr">The pointer to an array.</param>
		/// <remarks>
		/// Note here we should point at the one-unit-length memory before the array start.
		/// </remarks>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal GridMaskEnumerator(ref short arr)
		{
			_refCurrent = ref SubtractByteOffset(ref arr, 1);
			_start = ref _refCurrent;
		}
	}
}

/// <summary>
/// Indicates the JSON converter of the current type.
/// </summary>
file sealed class Converter : JsonConverter<Grid>
{
	/// <inheritdoc/>
	public override bool HandleNull => true;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Grid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => (Grid)reader.GetString();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Write(Utf8JsonWriter writer, Grid value, JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToString(SusserFormat.Full));
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.Byte.cs,998a36a55f580ab1

/// <summary>
/// Represents a helper method used by <see cref="Grid"/> instances, especially for equality checking method <see cref="Grid.Equals(in Grid)"/>.
/// </summary>
/// <seealso cref="Grid"/>
/// <seealso cref="Grid.Equals(in Grid)"/>
file static unsafe class Helper
{
	/// <summary>
	/// Determines whether two sequences are considered equal on respective bits.
	/// </summary>
	/// <param name="first">The first sequence.</param>
	/// <param name="second">The second sequence.</param>
	/// <param name="length">
	/// The total bits of the sequence to be compared. Please note that two sequences
	/// <paramref name="first"/> and <paramref name="second"/> must hold a same length.
	/// </param>
	/// <returns>A <see cref="bool"/> result indicating whether they are considered equal.</returns>
	/// <remarks>
	/// Optimized byte-based <c>SequenceEquals</c>.
	/// The <paramref name="length"/> parameter for this one is declared a <see langword="nuint"/> rather than <see cref="int"/>
	/// as we also use it for types other than <see cref="byte"/> where the length can exceed 2Gb once scaled by <see langword="sizeof"/>(T).
	/// </remarks>
	public static bool SequenceEqual(ref byte first, ref byte second, nuint length)
	{
		bool result;

		// Use nint for arithmetic to avoid unnecessary 64->32->64 truncations.
		if (length >= (nuint)sizeof(nuint))
		{
			// Conditional jmp forward to favor shorter lengths. (See comment at "Equal:" label)
			// The longer lengths can make back the time due to branch misprediction better than shorter lengths.
			goto Longer;
		}

#if TARGET_64BIT
		// On 32-bit, this will always be true since sizeof(nuint) == 4.
		if (length < sizeof(uint))
#endif
		{
			uint differentBits = 0;
			var offset = length & 2;
			if (offset != 0)
			{
				differentBits = LoadUShort(ref first);
				differentBits -= LoadUShort(ref second);
			}
			if ((length & 1) != 0)
			{
				differentBits |= (uint)AddByteOffset(ref first, offset) - AddByteOffset(ref second, offset);
			}

			result = differentBits == 0;

			goto Result;
		}
#if TARGET_64BIT
		else
		{
			var offset = length - sizeof(uint);
			var differentBits = LoadUInt(ref first) - LoadUInt(ref second);
			differentBits |= LoadUInt(ref first, offset) - LoadUInt(ref second, offset);
			result = differentBits == 0;

			goto Result;
		}
#endif
	Longer:
		// Only check that the ref is the same if buffers are large, and hence its worth avoiding doing unnecessary comparisons.
		if (!AreSame(ref first, ref second))
		{
			// C# compiler inverts this test, making the outer goto the conditional jmp.
			goto Vector;
		}

		// This becomes a conditional jmp forward to not favor it.
		goto Equal;

	Result:
		return result;

	Equal:
		// When the sequence is equal; which is the longest execution, we want it to determine that
		// as fast as possible so we do not want the early outs to be "predicted not taken" branches.
		return true;

	Vector:
		if (Vector128.IsHardwareAccelerated)
		{
			if (Vector256.IsHardwareAccelerated && length >= (nuint)Vector256<byte>.Count)
			{
				var offset = (nuint)0;
				var lengthToExamine = length - (nuint)Vector256<byte>.Count;

				// Unsigned, so it shouldn't have overflowed larger than length (rather than negative).
				Debug.Assert(lengthToExamine < length);
				if (lengthToExamine != 0)
				{
					do
					{
						if (Vector256.LoadUnsafe(ref first, offset) != Vector256.LoadUnsafe(ref second, offset))
						{
							goto NotEqual;
						}

						offset += (nuint)Vector256<byte>.Count;
					} while (lengthToExamine > offset);
				}

				// Do final compare as Vector256<byte>.Count from end rather than start.
				if (Vector256.LoadUnsafe(ref first, lengthToExamine) == Vector256.LoadUnsafe(ref second, lengthToExamine))
				{
					// C# compiler inverts this test, making the outer goto the conditional jmp.
					goto Equal;
				}

				// This becomes a conditional jmp forward to not favor it.
				goto NotEqual;
			}
			else if (length >= (nuint)Vector128<byte>.Count)
			{
				var offset = (nuint)0;
				var lengthToExamine = length - (nuint)Vector128<byte>.Count;

				// Unsigned, so it shouldn't have overflowed larger than length (rather than negative).
				Debug.Assert(lengthToExamine < length);
				if (lengthToExamine != 0)
				{
					do
					{
						if (Vector128.LoadUnsafe(ref first, offset) != Vector128.LoadUnsafe(ref second, offset))
						{
							goto NotEqual;
						}

						offset += (nuint)Vector128<byte>.Count;
					} while (lengthToExamine > offset);
				}

				// Do final compare as Vector128<byte>.Count from end rather than start.
				if (Vector128.LoadUnsafe(ref first, lengthToExamine) == Vector128.LoadUnsafe(ref second, lengthToExamine))
				{
					// C# compiler inverts this test, making the outer goto the conditional jmp.
					goto Equal;
				}

				// This becomes a conditional jmp forward to not favor it.
				goto NotEqual;
			}
		}
		else if (Vector.IsHardwareAccelerated && length >= (nuint)Vector<byte>.Count)
		{
			var offset = (nuint)0;
			var lengthToExamine = length - (nuint)Vector<byte>.Count;

			// Unsigned, so it shouldn't have overflowed larger than length (rather than negative).
			Debug.Assert(lengthToExamine < length);
			if (lengthToExamine > 0)
			{
				do
				{
					if (LoadVector(ref first, offset) != LoadVector(ref second, offset))
					{
						goto NotEqual;
					}

					offset += (nuint)Vector<byte>.Count;
				} while (lengthToExamine > offset);
			}

			// Do final compare as Vector<byte>.Count from end rather than start.
			if (LoadVector(ref first, lengthToExamine) == LoadVector(ref second, lengthToExamine))
			{
				// C# compiler inverts this test, making the outer goto the conditional jmp.
				goto Equal;
			}

			// This becomes a conditional jmp forward to not favor it.
			goto NotEqual;
		}

#if TARGET_64BIT
		if (Vector128.IsHardwareAccelerated)
		{
			Debug.Assert(length <= (nuint)sizeof(nuint) * 2);

			var offset = length - (nuint)sizeof(nuint);
			var differentBits = LoadNUInt(ref first) - LoadNUInt(ref second);
			differentBits |= LoadNUInt(ref first, offset) - LoadNUInt(ref second, offset);
			result = differentBits == 0;
			goto Result;
		}
		else
#endif
		{
			Debug.Assert(length >= (nuint)sizeof(nuint));

			var offset = (nuint)0;
			var lengthToExamine = length - (nuint)sizeof(nuint);
			// Unsigned, so it shouldn't have overflowed larger than length (rather than negative).
			Debug.Assert(lengthToExamine < length);
			if (lengthToExamine > 0)
			{
				do
				{
					// Compare unsigned so not do a sign extend mov on 64 bit.
					if (LoadNUInt(ref first, offset) != LoadNUInt(ref second, offset))
					{
						goto NotEqual;
					}
					offset += (nuint)sizeof(nuint);
				} while (lengthToExamine > offset);
			}

			// Do final compare as sizeof(nuint) from end rather than start.
			result = LoadNUInt(ref first, lengthToExamine) == LoadNUInt(ref second, lengthToExamine);
			goto Result;
		}

	NotEqual:
		// As there are so many true/false exit points the Jit will coalesce them to one location.
		// We want them at the end so the conditional early exit jmps are all jmp forwards so the
		// branch predictor in a uninitialized state will not take them e.g.
		// - loops are conditional jmps backwards and predicted.
		// - exceptions are conditional forwards jmps and not predicted.
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ushort LoadUShort(ref byte start) => ReadUnaligned<ushort>(ref start);

#if TARGET_64BIT
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint LoadUInt(ref byte start) => ReadUnaligned<uint>(ref start);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint LoadUInt(ref byte start, nuint offset) => ReadUnaligned<uint>(ref AddByteOffset(ref start, offset));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static nuint LoadNUInt(ref byte start) => ReadUnaligned<nuint>(ref start);
#endif

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static nuint LoadNUInt(ref byte start, nuint offset) => ReadUnaligned<nuint>(ref AddByteOffset(ref start, offset));

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Vector<byte> LoadVector(ref byte start, nuint offset) => ReadUnaligned<Vector<byte>>(ref AddByteOffset(ref start, offset));
}
