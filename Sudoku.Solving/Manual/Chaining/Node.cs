﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides an elementary unit in a chain.
	/// </summary>
	public sealed class Node : IEquatable<Node>
	{
		/// <summary>
		/// Initializes an instance with a specified candidate and its type.
		/// </summary>
		/// <param name="candidate">The candidates.</param>
		/// <param name="nodeType">The type of this node.</param>
		public Node(int candidate, NodeType nodeType)
			: this(new FullGridMap(stackalloc[] { candidate }), nodeType)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified candidates and its type.
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		/// <param name="nodeType">The type of this node.</param>
		public Node(IEnumerable<int> candidates, NodeType nodeType)
			: this(new FullGridMap(candidates), nodeType)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified map and its type.
		/// </summary>
		/// <param name="candidatesMap">The map of candidates.</param>
		/// <param name="nodeType">The node type.</param>
		public Node(FullGridMap candidatesMap, NodeType nodeType) =>
			(CandidatesMap, NodeType) = (candidatesMap, nodeType);


		/// <summary>
		/// Indicates all candidates used in this node.
		/// </summary>
		public FullGridMap CandidatesMap { get; }

		/// <summary>
		/// Indicates the type of this current node.
		/// </summary>
		public NodeType NodeType { get; }


		/// <summary>
		/// Get a candidate offset in the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The candidate offset.</returns>
		public int this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => CandidatesMap.SetAt(index);
		}

		/// <summary>
		/// Get a candidate offset in the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The candidate offset.</returns>
		public int this[Index index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => CandidatesMap.SetAt(index);
		}


		/// <include file='../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="map">(<see langword="out"/> parameter) The map.</param>
		/// <param name="nodeType">
		/// (<see langword="out"/> parameter) Indicates the node type.
		/// </param>
		public void Deconstruct(out FullGridMap map, out NodeType nodeType) =>
			(map, nodeType) = (CandidatesMap, NodeType);

		/// <summary>
		/// Checks whether all candidates used in this instance is collide with
		/// the other one. If two candidates hold at least one same candidate,
		/// we will say the node is collide with the other node (or reversely).
		/// </summary>
		/// <param name="other">The other node.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public bool IsCollideWith(Node other) => (this & other).IsNotEmpty;

		/// <summary>
		/// Checks whether all candidates used in this instance fully contains
		/// the other one.
		/// </summary>
		/// <param name="other">The other node.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		public bool Contains(Node other) =>
			CandidatesMap.Count <= other.CandidatesMap.Count ? false : (this | other) == CandidatesMap;

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Node comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(Node other) => CandidatesMap == other.CandidatesMap;

		/// <inheritdoc/>
		/// <remarks>
		/// If you get a derived class, we recommend you override this method
		/// to describe the type of the node.
		/// </remarks>
		public override int GetHashCode() => CandidatesMap.GetHashCode();

		/// <inheritdoc/>
		/// <remarks>
		/// If you get a derived class, we recommend you override this method
		/// to describe the type of the node.
		/// </remarks>
		public override string ToString() =>
			CandidateCollection.ToString(CandidatesMap.Offsets);

		/// <summary>
		/// Indicates all candidates used.
		/// </summary>
		public IEnumerable<int> GetCandidates() => CandidatesMap.Offsets;


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Node left, Node right) => left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Node left, Node right) => !(left == right);

		/// <summary>
		/// Get all candidates that <paramref name="left"/> and <paramref name="right"/>
		/// map both contain.
		/// </summary>
		/// <param name="left">The left map.</param>
		/// <param name="right">The right map.</param>
		/// <returns>All candidates that satisfied the condition.</returns>
		public static FullGridMap operator &(Node left, Node right) =>
			left.CandidatesMap & right.CandidatesMap;

		/// <summary>
		/// Get all candidates from <paramref name="left"/> and <paramref name="right"/>
		/// maps.
		/// </summary>
		/// <param name="left">The left map.</param>
		/// <param name="right">The right map.</param>
		/// <returns>All candidates.</returns>
		public static FullGridMap operator |(Node left, Node right) =>
			left.CandidatesMap | right.CandidatesMap;

		/// <summary>
		/// Get all candidates that satisfy the formula <c>(a - b) | (b - a)</c>.
		/// </summary>
		/// <param name="left">The left map.</param>
		/// <param name="right">The right map.</param>
		/// <returns>All candidates.</returns>
		public static FullGridMap operator ^(Node left, Node right) =>
			left.CandidatesMap ^ right.CandidatesMap;

		/// <summary>
		/// Get all candidates that is in the <paramref name="left"/> map but not in
		/// the <paramref name="right"/> map (i.e. formula <c>a &#38; ~b</c>).
		/// </summary>
		/// <param name="left">The left map.</param>
		/// <param name="right">The right map.</param>
		/// <returns>All candidates.</returns>
		public static FullGridMap operator -(Node left, Node right) =>
			left.CandidatesMap - right.CandidatesMap;
	}
}
