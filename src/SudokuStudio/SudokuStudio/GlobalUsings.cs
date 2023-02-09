﻿global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Collections.ObjectModel;
global using System.Collections.Specialized;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Diagnostics.CodeGen;
global using System.IO;
global using System.Linq;
global using System.Numerics;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.InteropServices.WindowsRuntime;
global using System.Runtime.Messages;
global using System.Runtime.Versioning;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Windows.Input;
global using Expressive.Exceptions;
global using LiveChartsCore;
global using LiveChartsCore.Drawing;
global using LiveChartsCore.Kernel;
global using LiveChartsCore.Kernel.Sketches;
global using LiveChartsCore.Measure;
global using LiveChartsCore.SkiaSharpView;
global using LiveChartsCore.SkiaSharpView.Drawing;
global using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
global using LiveChartsCore.SkiaSharpView.Painting;
global using Microsoft.Graphics.Canvas.Text;
global using Microsoft.UI;
global using Microsoft.UI.Composition;
global using Microsoft.UI.Composition.SystemBackdrops;
global using Microsoft.UI.Dispatching;
global using Microsoft.UI.Input;
global using Microsoft.UI.Text;
global using Microsoft.UI.Windowing;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Automation;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Controls.Primitives;
global using Microsoft.UI.Xaml.Data;
global using Microsoft.UI.Xaml.Documents;
global using Microsoft.UI.Xaml.Input;
global using Microsoft.UI.Xaml.Markup;
global using Microsoft.UI.Xaml.Media;
global using Microsoft.UI.Xaml.Media.Animation;
global using Microsoft.UI.Xaml.Media.Imaging;
global using Microsoft.UI.Xaml.Navigation;
global using Microsoft.UI.Xaml.Shapes;
global using Microsoft.Windows.AppLifecycle;
global using SkiaSharp;
global using Sudoku.Checking;
global using Sudoku.Concepts;
global using Sudoku.Filtering;
global using Sudoku.Generating.Puzzlers;
global using Sudoku.Presentation;
global using Sudoku.Presentation.Nodes;
global using Sudoku.Rating;
global using Sudoku.Resources;
global using Sudoku.Runtime.AnalysisServices;
global using Sudoku.Solving;
global using Sudoku.Solving.Algorithms.Bitwise;
global using Sudoku.Solving.Logical;
global using Sudoku.Solving.Logical.StepGatherers;
global using Sudoku.Text;
global using Sudoku.Text.Formatting;
global using Sudoku.Text.Notations;
global using SudokuStudio.Collection;
global using SudokuStudio.Configuration;
global using SudokuStudio.Input;
global using SudokuStudio.Interaction;
global using SudokuStudio.Interaction.Conversions;
global using SudokuStudio.Interop;
global using SudokuStudio.Models;
global using SudokuStudio.Storage;
global using SudokuStudio.Views.Controls;
global using SudokuStudio.Views.Pages;
global using SudokuStudio.Views.Pages.Analyze;
global using SudokuStudio.Views.Pages.Operation;
global using SudokuStudio.Views.Windows;
global using Windows.ApplicationModel.Activation;
global using Windows.ApplicationModel.DataTransfer;
global using Windows.Foundation;
global using Windows.Graphics;
global using Windows.Graphics.Display;
global using Windows.Graphics.Imaging;
global using Windows.Storage;
global using Windows.Storage.Pickers;
global using Windows.Storage.Search;
global using Windows.Storage.Streams;
global using Windows.System;
global using Windows.UI;
global using Windows.UI.Core;
global using Windows.UI.Text;
global using Windows.UI.ViewManagement;
global using WinRT;
global using WinRT.Interop;
global using static System.Math;
global using static System.Numerics.BitOperations;
global using static System.Text.Json.JsonSerializer;
global using static Microsoft.UI.Xaml.DependencyPropertyRegistering;
global using static Sudoku.SolutionWideReadOnlyFields;
global using static Sudoku.Solving.ConclusionType;
global using static SudokuStudio.Resources.ResourceDictionary;
global using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;
global using GridLayout = Microsoft.UI.Xaml.Controls.Grid;
global using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;
global using Geometry = Microsoft.UI.Xaml.Media.Geometry;
global using LineGeometry = Microsoft.UI.Xaml.Media.LineGeometry;
global using PathGeometry = Microsoft.UI.Xaml.Media.PathGeometry;
global using Path = Microsoft.UI.Xaml.Shapes.Path;
global using MicrosoftXamlWindowActivatedEventArgs = Microsoft.UI.Xaml.WindowActivatedEventArgs;
global using Grid = Sudoku.Concepts.Grid;
global using VisualUnit = Sudoku.Presentation.IVisual;
global using TechniqueGroupModel = SudokuStudio.Models.TechniqueGroup;
global using SpecialFolder = System.Environment.SpecialFolder;
global using SystemPath = System.IO.Path;
global using WinSysDispatcherQueue = Windows.System.DispatcherQueue;
global using WinSysDispatcherQueueController = Windows.System.DispatcherQueueController;
