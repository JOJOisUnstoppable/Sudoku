﻿global using System;
global using System.Algorithm;
global using System.Buffers;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using System.Numerics;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;
global using Sudoku.Concepts;
global using Sudoku.Concepts.Handlers;
global using Sudoku.Presentation;
global using Sudoku.Presentation.Nodes;
global using Sudoku.Resources;
global using Sudoku.Runtime.AnalysisServices;
global using Sudoku.Solving.DataRepresentation;
global using Sudoku.Solving.Manual;
global using Sudoku.Solving.Manual.Buffers;
global using Sudoku.Solving.Manual.Searchers;
global using Sudoku.Solving.Manual.Searchers.Specialized;
global using Sudoku.Solving.Manual.Steps;
global using Sudoku.Solving.Manual.Steps.Specialized;
global using Sudoku.Solving.Patterns;
global using Sudoku.Text;
global using Sudoku.Text.Notations;
global using static System.Algorithm.Combinatorics;
global using static System.Algorithm.Sequences;
global using static System.Math;
global using static System.Numerics.BitOperations;
global using static Sudoku.Resources.MergedResources;
global using static Sudoku.Runtime.AnalysisServices.CommonReadOnlies;
global using static Sudoku.Solving.Manual.Buffers.FastProperties;
global using ViewList = System.Collections.Immutable.ImmutableArray<Sudoku.Presentation.View>;
global using ConclusionList = System.Collections.Immutable.ImmutableArray<Sudoku.Solving.Conclusion>;
