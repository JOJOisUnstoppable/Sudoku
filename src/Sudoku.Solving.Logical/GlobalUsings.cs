﻿global using System;
global using System.Algorithm;
global using System.Buffers;
global using System.Collections;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Diagnostics.CodeGen;
global using System.Globalization;
global using System.Linq;
global using System.Numerics;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.Messages;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Timers;
global using Expressive;
global using Expressive.Exceptions;
global using Expressive.Expressions;
global using Expressive.Expressions.Binary;
global using Expressive.Operators;
global using Sudoku.Buffers;
global using Sudoku.Collections.Logical;
global using Sudoku.Compatibility.Hodoku;
global using Sudoku.Compatibility.SudokuExplainer;
global using Sudoku.Concepts;
global using Sudoku.Linq;
global using Sudoku.Preprocessing.Gathering;
global using Sudoku.Presentation;
global using Sudoku.Presentation.Nodes;
global using Sudoku.Resources;
global using Sudoku.Runtime.AnalysisServices;
global using Sudoku.Runtime.DisplayingServices;
global using Sudoku.Solving;
global using Sudoku.Solving.Algorithms.Bitwise;
global using Sudoku.Solving.Logical;
global using Sudoku.Solving.Logical.Annotations;
global using Sudoku.Solving.Logical.Meta;
global using Sudoku.Solving.Logical.Patterns;
global using Sudoku.Solving.Logical.Steps;
global using Sudoku.Solving.Logical.Steps.Specialized;
global using Sudoku.Solving.Logical.StepSearchers;
global using Sudoku.Solving.Logical.StepSearchers.Specialized;
global using Sudoku.Solving.Logical.Techniques;
global using Sudoku.Text;
global using Sudoku.Text.Formatting;
global using Sudoku.Text.Notations;
global using static System.Algorithm.Sequences;
global using static System.Math;
global using static System.Numerics.BitOperations;
global using static System.Runtime.CompilerServices.Unsafe;
global using static Sudoku.Buffers.FastProperties;
global using static Sudoku.Resources.MergedResources;
global using static Sudoku.Runtime.AnalysisServices.CommonReadOnlies;
global using static Sudoku.Runtime.MaskServices.MaskOperations;
global using static Sudoku.SolutionWideReadOnlyFields;
global using static Sudoku.Solving.ConclusionType;
global using ChainBranch = System.Collections.Generic.Dictionary<byte, Sudoku.Collections.Logical.NodeSet>;
global using ViewList = System.Collections.Immutable.ImmutableArray<Sudoku.Presentation.View>;
global using ConclusionList = System.Collections.Immutable.ImmutableArray<Sudoku.Solving.Conclusion>;
