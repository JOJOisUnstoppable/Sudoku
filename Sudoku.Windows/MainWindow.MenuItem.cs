﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Data.Extensions;
using Sudoku.Data.Stepping;
using Sudoku.Drawing;
using Sudoku.Drawing.Extensions;
using Sudoku.Extensions;
using Sudoku.Solving;
using Sudoku.Solving.BruteForces.Bitwise;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Generating;
using Sudoku.Solving.Manual.Symmetry;
using Sudoku.Windows.Constants;
using static Sudoku.Windows.Constants.Processings;
using AnonymousType = System.Object;
using SudokuGrid = Sudoku.Data.Grid;
#if SUDOKU_RECOGNIZING
using System.Drawing;
#endif
#if DEBUG
using Sudoku.Solving.Manual;
#endif

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		private void MenuItemFileOpen_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				DefaultExt = "sudoku",
				Filter = (string)LangSource["FilterLoadingPuzzles"],
				Multiselect = false,
				Title = (string)LangSource["TitleLoadingPuzzles"]
			};

			if (dialog.ShowDialog() is true)
			{
				using var sr = new StreamReader(dialog.FileName);
				LoadPuzzle(sr.ReadToEnd());
			}
		}

		private void MenuItemFileSave_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog
			{
				AddExtension = true,
				CheckPathExists = true,
				DefaultExt = "sudoku",
				Filter = (string)LangSource["FilterSavingPuzzles"],
				Title = (string)LangSource["TitleSavingPuzzles"]
			};

			if (dialog.ShowDialog() is true)
			{
				using var sw = new StreamWriter(dialog.FileName);
				sw.Write(_puzzle.ToString("#"));
			}
		}

		private void MenuItemFileOpenDatabase_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				DefaultExt = "sudokus",
				Filter = (string)LangSource["FilterOpeningDatabase"],
				Multiselect = false,
				Title = (string)LangSource["TitleOpeningDatabase"]
			};

			if (dialog.ShowDialog() is true)
			{
				using var sr = new StreamReader(Settings.CurrentPuzzleDatabase = _database = dialog.FileName);
				_puzzlesText = sr.ReadToEnd().Split(Splitter, StringSplitOptions.RemoveEmptyEntries);

				Messagings.LoadDatabase(_puzzlesText.Length);

				if (_puzzlesText.Length != 0)
				{
					LoadPuzzle(_puzzlesText[Settings.CurrentPuzzleNumber = 0].TrimEnd(Splitter));
					UpdateDatabaseControls(false, false, true, true);

					_textBoxJumpTo.IsEnabled = true;
					_labelPuzzleNumber.Content = $"1/{_puzzlesText.Length}";
				}
			}
		}

		private void MenuItemBackupConfig_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog
			{
				AddExtension = true,
				CheckPathExists = true,
				DefaultExt = "scfg",
				Filter = (string)LangSource["FilterBackupingConfigurations"],
				Title = (string)LangSource["TitleBackupingConfigurations"]
			};

			if (dialog.ShowDialog() is true)
			{
				try
				{
					SaveConfig(dialog.FileName);
				}
				catch
				{
					Messagings.FailedToBackupConfig();
				}
			}
		}

#if SUDOKU_RECOGNIZING
		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemFileLoadPicture_Click(object sender, RoutedEventArgs e)
#else
		private void MenuItemFileLoadPicture_Click(object sender, RoutedEventArgs e)
#endif
		{
#if SUDOKU_RECOGNIZING
			await internalOperation();

			async Task internalOperation()
			{
				if (_recognition is null)
				{
					Messagings.FailedToLoadPicture();

					e.Handled = true;
					return;
				}

				var dialog = new OpenFileDialog
				{
					AddExtension = true,
					DefaultExt = "png",
					Filter = (string)LangSource["FilterOpeningPictures"],
					Multiselect = false,
					Title = (string)LangSource["TitleOpeningPictures"]
				};

				if (dialog.ShowDialog() is true)
				{
					try
					{
						if (_recognition.ToolIsInitialized)
						{
							if (Messagings.AskWhileLoadingPicture() == MessageBoxResult.Yes)
							{
								_textBoxInfo.Text = (string)LangSource["TextOpeningPictures"];
								using (var bitmap = new Bitmap(dialog.FileName))
								{
									var grid = (await Task.Run(() => _recognition.Recorgnize(bitmap))).ToMutable();
									grid.Fix();
									Puzzle = new UndoableGrid(grid);
								}

								UpdateUndoRedoControls();
								UpdateImageGrid();
							}
						}
						else
						{
							Messagings.FailedToLoadPictureDueToNotHavingInitialized();
						}
					}
					catch (Exception ex)
					{
						Messagings.ShowExceptionMessage(ex);
					}

					_textBoxInfo.ClearValue(TextBox.TextProperty);
				}
			}
#else
			Messagings.NotSupportedForLoadingPicture();
#endif
		}

		private void MenuItemFileSavePicture_Click(object sender, RoutedEventArgs e) =>
			new PictureSavingPreferencesWindow(_puzzle, Settings, _currentPainter).ShowDialog();

		private void MenuItemFileGetSnapshot_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Clipboard.SetImage((BitmapSource)_imageGrid.Source);
			}
			catch (Exception ex)
			{
				Messagings.ShowExceptionMessage(ex);
			}
		}

		private void MenuItemFileSaveBatch_Click(object sender, RoutedEventArgs e)
		{
			if (!_puzzle.IsValid(out _))
			{
				Messagings.FailedToCheckDueToInvalidPuzzle();
				e.Handled = true;
				return;
			}

			// Batch.
			new PictureSavingPreferencesWindow(_puzzle, Settings, _currentPainter).ShowDialog(); // Save puzzle picture.
			MenuItemFileSave_Click(sender, e); // Save puzzle text.
			MenuItemAnalyzeSolve_Click(sender, e); // Solve the puzzle.
			new PictureSavingPreferencesWindow(_puzzle, Settings, _currentPainter).ShowDialog(); // Save solution picture.
			MenuItemFileSave_Click(sender, e); // Save solution text.
		}

		private void MenuItemFileQuit_Click(object sender, RoutedEventArgs e) => Close();

		private void MenuItemOptionsShowCandidates_Click(object sender, RoutedEventArgs e)
		{
			Settings.ShowCandidates = _menuItemOptionsShowCandidates.IsChecked ^= true;
			_currentPainter.Grid = _puzzle;

			UpdateImageGrid();
		}

		private void MenuItemOptionsSettings_Click(object sender, RoutedEventArgs e)
		{
			var settingsWindow = new SettingsWindow(Settings, _manualSolver);
			if (!(settingsWindow.ShowDialog() is true))
			{
				e.Handled = true;
				return;
			}

			Settings.CoverBy(settingsWindow.Settings);
			UpdateControls();
			UpdateImageGrid();
		}

		private void MenuItemEditUndo_Click(object sender, RoutedEventArgs e)
		{
			if (_puzzle.HasUndoSteps)
			{
				_puzzle.Undo();
				UpdateImageGrid();
			}

			UpdateUndoRedoControls();
		}

		private void MenuItemEditRedo_Click(object sender, RoutedEventArgs e)
		{
			if (_puzzle.HasRedoSteps)
			{
				_puzzle.Redo();
				UpdateImageGrid();
			}

			UpdateUndoRedoControls();
		}

		private void MenuItemEditRecomputeCandidates_Click(object sender, RoutedEventArgs e)
		{
			int[] z = new int[81];
			for (int cell = 0; cell < 81; cell++)
			{
				z[cell] = _puzzle[cell] + 1;
			}

			var grid = SudokuGrid.CreateInstance(z);
			if (new BitwiseSolver().Solve(grid.ToString(), null, 2) == 0)
			{
				Messagings.SukakuCannotUseThisFunction();

				e.Handled = true;
				return;
			}

			_puzzle = new UndoableGrid(grid);
			_puzzle.Unfix();
			_puzzle.ClearStack();

			UpdateImageGrid();
			UpdateUndoRedoControls();
		}

		private void MenuItemEditCopy_Click(object sender, RoutedEventArgs e) =>
			InternalCopy(Settings.TextFormatPlaceholdersAreZero ? "0" : ".");

		private void MenuItemEditCopyCurrentGrid_Click(object sender, RoutedEventArgs e) =>
			InternalCopy(Settings.TextFormatPlaceholdersAreZero ? "#0" : "#.");

		private void MenuItemEditCopyPmGrid_Click(object sender, RoutedEventArgs e) =>
			InternalCopy(Settings.PmGridCompatible ? "@:*!" : "@:*");

		private void MenuItemEditCopyHodokuLibrary_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string z = _puzzle.ToString(Settings.TextFormatPlaceholdersAreZero ? "#0" : "#.");
				Clipboard.SetText($":0000:x:{z}{(z.Contains(':') ? "::" : ":::")}");
			}
			catch (Exception ex)
			{
				Messagings.ShowExceptionMessage(ex);
			}
		}

		private void MenuItemEditCopyAsSukaku_Click(object sender, RoutedEventArgs e) =>
			InternalCopy(
				$"~{(Settings.PmGridCompatible ? string.Empty : "@")}" +
				$"{(Settings.TextFormatPlaceholdersAreZero ? "0" : ".")}");

		private void MenuItemEditCopyAsExcel_Click(object sender, RoutedEventArgs e) => InternalCopy("%");

		private void MenuItemEditPaste_Click(object sender, RoutedEventArgs e)
		{
			string puzzleStr = Clipboard.GetText();
			if (!(puzzleStr is null))
			{
				LoadPuzzle(puzzleStr);

				_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
				_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
				//_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
				_treeViewTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
			}
		}

		private void MenuItemEditPasteAsSukaku_Click(object sender, RoutedEventArgs e)
		{
			string puzzleStr = Clipboard.GetText();
			if (!(puzzleStr is null))
			{
				try
				{
					Puzzle = new UndoableGrid(SudokuGrid.Parse(puzzleStr, GridParsingOption.Sukaku));

					_menuItemEditUndo.IsEnabled = _menuItemEditRedo.IsEnabled = false;
					UpdateImageGrid();
				}
				catch (ArgumentException)
				{
					Messagings.FailedToPasteText();
				}

				_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
				_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
				//_listBoxTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
				_treeViewTechniques.ClearValue(ItemsControl.ItemsSourceProperty);
			}
		}

		private void MenuItemEditFix_Click(object sender, RoutedEventArgs e)
		{
			_puzzle.Fix();

			UpdateImageGrid();
			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
		}

		private void MenuItemEditUnfix_Click(object sender, RoutedEventArgs e)
		{
			_puzzle.Unfix();

			UpdateImageGrid();
			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
		}

		private void MenuItemEditReset_Click(object sender, RoutedEventArgs e)
		{
			_currentPainter.Grid = _puzzle = new UndoableGrid(_initialPuzzle);
			_currentPainter.View = null;

			UpdateImageGrid();
			_listBoxPaths.ClearValue(ItemsControl.ItemsSourceProperty);
			_listViewSummary.ClearValue(ItemsControl.ItemsSourceProperty);
		}

		private void MenuItemClearStack_Click(object sender, RoutedEventArgs e)
		{
			if ((_puzzle.HasUndoSteps || _puzzle.HasRedoSteps)
				&& Messagings.AskWhileClearingStack() == MessageBoxResult.Yes)
			{
				_puzzle.ClearStack();

				UpdateUndoRedoControls();
			}
		}

		private void MenuItemEditClear_Click(object sender, RoutedEventArgs e)
		{
			Puzzle = new UndoableGrid(SudokuGrid.Empty);

			UpdateImageGrid();
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemGenerateWithSymmetry_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				if (_database is null || Messagings.AskWhileGeneratingWithDatabase() == MessageBoxResult.Yes)
				{
					// Disable relative database controls.
					Settings.CurrentPuzzleDatabase = _database = null;
					Settings.CurrentPuzzleNumber = -1;
					UpdateDatabaseControls(false, false, false, false);

					DisableGeneratingControls();

					// These two value should be assigned first, rather than 
					// inlining in the asynchronized environment.
					var symmetry = (SymmetryType)(1 << _comboBoxSymmetry.SelectedIndex + 1);
					//var diff = (DifficultyLevel)_comboBoxDifficulty.SelectedItem;
					Puzzle = new UndoableGrid(await Task.Run(() => new BasicPuzzleGenerator().Generate(33, symmetry)));

					EnableGeneratingControls();
					SwitchOnGeneratingComboBoxesDisplaying();
					ClearItemSourcesWhenGeneratedOrSolving();
					UpdateImageGrid();
				}
			}
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemGenerateHardPattern_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				if (_database is null || Messagings.AskWhileGeneratingWithDatabase() == MessageBoxResult.Yes)
				{
					// Disable relative database controls.
					Settings.CurrentPuzzleDatabase = _database = null;
					Settings.CurrentPuzzleNumber = -1;
					UpdateDatabaseControls(false, false, false, false);
					_labelPuzzleNumber.ClearValue(ContentProperty);

					DisableGeneratingControls();

					// Here two variables cannot be moved into the lambda expression
					// because the lambda expression will be executed in asynchornized way.
					int index = _comboBoxBackdoorFilteringDepth.SelectedIndex;
					Settings.GeneratingDifficultyLevelSelectedIndex = _comboBoxDifficulty.SelectedIndex;

					Puzzle =
						new UndoableGrid(
							await Task.Run(
								() => new HardPatternPuzzleGenerator().Generate(
									index - 1,
									(DifficultyLevel)Settings.GeneratingDifficultyLevelSelectedIndex)));

					EnableGeneratingControls();
					SwitchOnGeneratingComboBoxesDisplaying();
					ClearItemSourcesWhenGeneratedOrSolving();
					UpdateImageGrid();
				}
			}
		}

#if DEBUG
		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemGenerateWithTechniqueFiltering_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				DisableGeneratingControls();

				Puzzle = new UndoableGrid(
					await Task.Run(() =>
						new TechniqueFilteringPuzzleGenerator().Generate(
							new TechniqueCodeFilter { TechniqueCode.AlmostLockedPair })));

				EnableGeneratingControls();
				SwitchOnGeneratingComboBoxesDisplaying();
				ClearItemSourcesWhenGeneratedOrSolving();
				UpdateImageGrid();
			}
		}
#else
		private void MenuItemGenerateWithTechniqueFiltering_Click(object sender, RoutedEventArgs e) =>
			Messagings.NotSupportedWhileGeneratingWithFilter();
#endif

		private void MenuItemAnalyzeSolve_Click(object sender, RoutedEventArgs e)
		{
			if (!applyNormal() && !applySukaku())
			{
				Messagings.FailedToApplyPuzzle();
				e.Handled = true;
				return;
			}

			bool applySukaku()
			{
				var sb = new StringBuilder(SudokuGrid.EmptyString);
				if (new SukakuBitwiseSolver().Solve(
					_puzzle.ToString($"~{(Settings.TextFormatPlaceholdersAreZero ? "0" : ".")}"), sb, 2) != 1)
				{
					return !(e.Handled = true);
				}

				var grid = SudokuGrid.Parse(sb.ToString());
				grid.Unfix();

				Puzzle = new UndoableGrid(grid);
				UpdateImageGrid();
				return true;
			}

			bool applyNormal()
			{
				var solutionSb = new StringBuilder();
				string puzzleStr = _puzzle.ToString("0+");
				if (new BitwiseSolver().Solve(_puzzle.ToString(), solutionSb, 2) != 1)
				{
					return !(e.Handled = true);
				}

				var newSb = new StringBuilder();
				for (int i = 0, cell = 0, length = puzzleStr.Length; i < length; cell++)
				{
					char c = solutionSb[cell];
					switch (puzzleStr[i])
					{
						case '+':
						{
							newSb.Append($"+{c}");
							i += 2;
							break;
						}
						case '0':
						{
							newSb.Append($"+{c}");
							i++;
							break;
						}
						default:
						{
							newSb.Append(c);
							i++;
							break;
						}
					}
				}

				Puzzle = new UndoableGrid(SudokuGrid.Parse(newSb.ToString()));
				UpdateImageGrid();
				return true;
			}
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemAnalyzeAnalyze_Click(object sender, RoutedEventArgs e)
		{
			if (!await internalOperation(false) && !await internalOperation(true))
			{
				Messagings.FailedToApplyPuzzle();
				e.Handled = true;
				return;
			}

			async Task<bool> internalOperation(bool sukakuMode)
			{
				if (_puzzle.HasSolved)
				{
					Messagings.PuzzleAlreadySolved();
					return !(e.Handled = true);
				}

				var sb = new StringBuilder(SudokuGrid.EmptyString);
				if (sukakuMode)
				{
					string puzzleString = _puzzle.ToString("~");
					if (new SukakuBitwiseSolver().Solve(puzzleString, sb, 2) != 1)
					{
						return !(e.Handled = true);
					}
				}
				else
				{
					for (int cell = 0; cell < 81; cell++)
					{
						sb[cell] += (char)(_puzzle[cell] + 1);
					}

					if (new BitwiseSolver().Solve(sb.ToString(), null, 2) != 1)
					{
						return !(e.Handled = true);
					}
				}

				// Update status.
				ClearItemSourcesWhenGeneratedOrSolving();
				_textBoxInfo.Text = (string)LangSource["WhileSolving"];
				DisableSolvingControls();

				// Run the solver asynchronizedly, during solving you can do other work.
				var dialog = new ProgressWindow();
				dialog.Show();

				_analyisResult =
					await Task.Run(() =>
					{
						if (_puzzle.GivensCount == 0)
						{
							_puzzle.Fix();
						}

						if (!Settings.SolveFromCurrent && !sukakuMode)
						{
							_puzzle.Reset();
						}

						_puzzle.ClearStack();

						return _manualSolver.Solve(_puzzle, dialog.DefaultReporting, Settings.LanguageCode);
					});

				UpdateImageGrid();

				dialog.CloseAnyway();

				// Solved. Now update the technique summary.
				EnableSolvingControls();
				SwitchOnGeneratingComboBoxesDisplaying();

				if (_analyisResult.HasSolved)
				{
					_textBoxInfo.Text =
						$"{_analyisResult.SolvingStepsCount} " +
						$@"{(
							_analyisResult.SolvingStepsCount == 1 ? LangSource["StepSingular"] : LangSource["StepPlural"]
						)}" +
						$"{LangSource["Comma"]}" +
						$"{LangSource["TimeElapsed"]}" +
						$"{_analyisResult.ElapsedTime:hh':'mm'.'ss'.'fff}" +
						$"{LangSource["Period"]}";

					int i = 0;
					var pathList = new List<ListBoxItem>();
					foreach (var step in _analyisResult.SolvingSteps!)
					{
						var (fore, back) = Settings.DiffColors[step.DifficultyLevel];
						pathList.Add(
							new ListBoxItem
							{
								Foreground = new SolidColorBrush(fore.ToWColor()),
								Background = new SolidColorBrush(back.ToWColor()),
								Content =
									new PrimaryElementTuple<string, int, TechniqueInfo>(
										$"(#{i + 1}, {step.Difficulty}) {step.ToSimpleString()}", i++, step),
								BorderThickness = default
							});
					}
					_listBoxPaths.ItemsSource = pathList;

					// Gather the information.
					// GridView should list the instance with each property, not fields,
					// even if fields are public.
					// Therefore, here may use anonymous type is okay, but using value tuples
					// is bad.
					var collection = new List<AnonymousType>();
					decimal summary = 0, summaryMax = 0;
					int summaryCount = 0;
					foreach (var techniqueGroup in
						from solvingStep in _analyisResult.SolvingSteps!
						orderby solvingStep.Difficulty
						group solvingStep by solvingStep.Name)
					{
						string name = techniqueGroup.Key;
						int count = techniqueGroup.Count();
						decimal total = 0, maximum = 0;
						foreach (var step in techniqueGroup)
						{
							summary += step.Difficulty;
							summaryCount++;
							total += step.Difficulty;
							maximum = Math.Max(step.Difficulty, maximum);
							summaryMax = Math.Max(step.Difficulty, maximum);
						}

						collection.Add(new { Technique = name, Count = count, Total = total, Max = maximum });
					}

					collection.Add(
						new
						{
							Technique = default(string?),
							Count = summaryCount,
							Total = summary,
							Max = summaryMax
						});

					GridView view;
					_listViewSummary.ItemsSource = collection;
					_listViewSummary.View = view = new GridView();
					view.Columns.AddRange(
						new[]
						{
							createGridViewColumn(LangSource["TechniqueHeader"], "Technique", .6),
							createGridViewColumn(LangSource["TechniqueCount"], "Count", .1),
							createGridViewColumn(LangSource["TechniqueTotal"], "Total", .15),
							createGridViewColumn(LangSource["TechniqueMax"], "Max", .15)
						});
					view.AllowsColumnReorder = false;
				}
				else
				{
					Messagings.FailedToSolveWithMessage(_analyisResult);
				}

				return true;
			}

			GridViewColumn createGridViewColumn(object header, string name, double widthScale) =>
				new GridViewColumn
				{
					Header = header,
					DisplayMemberBinding = new Binding(name),
					Width = _tabControlInfo.ActualWidth * widthScale - 4,
				};
		}

		private void MenuItemAnalyzeSeMode_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.AnalyzeDifficultyStrictly = _menuItemAnalyzeSeMode.IsChecked ^= true;

		private void MenuItemAnalyzeFastSearch_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.FastSearch = _menuItemAnalyzeFastSearch.IsChecked ^= true;

		private void MenuItemCheckConclusionValidityAfterSearched_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.CheckConclusionValidityAfterSearched = _menuItemAnalyzeCheckConclusionValidityAfterSearched.IsChecked ^= true;

		private void MenuItemCheckGurthSymmetricalPlacement_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.CheckGurthSymmetricalPlacement = _menuItemAnalyzeCheckGurthSymmetricalPlacement.IsChecked ^= true;

		private void MenuItemShowFullHouses_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.EnableFullHouse = _menuItemAnalyzeShowFullHouses.IsChecked ^= true;

		private void MenuItemShowLastDigits_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.EnableLastDigit = _menuItemAnalyzeShowLastDigits.IsChecked ^= true;

		private void MenuItemOptimizeApplyingOrder_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.OptimizedApplyingOrder = _menuItemAnalyzeOptimizeApplyingOrder.IsChecked ^= true;

		private void MenuItemUseCalculationPriority_Click(object sender, RoutedEventArgs e) =>
			_manualSolver.UseCalculationPriority = _menuItemAnalyzeUseCalculationPriority.IsChecked ^= true;

		private void MenuItemExport_Click(object sender, RoutedEventArgs e)
		{
			if (_analyisResult is null)
			{
				Messagings.YouShouldSolveFirst();
				e.Handled = true;
				return;
			}

			new ExportAnalysisResultWindow(_analyisResult).Show();
		}

		private void MenuItemAnalyzeBackdoor_Click(object sender, RoutedEventArgs e) =>
			new BackdoorWindow(_puzzle).ShowDialog();

		private void MenuItemAnalyzeBugN_Click(object sender, RoutedEventArgs e)
		{
			if (!_puzzle.IsValid(out _))
			{
				Messagings.FailedToCheckDueToInvalidPuzzle();
				e.Handled = true;
				return;
			}

			new BugNSearchWindow(_puzzle).ShowDialog();
		}

		private void MenuItemTransformMirrorLeftRight_Click(object sender, RoutedEventArgs e) =>
			Transform(p => new UndoableGrid(p.MirrorLeftRight()));

		private void MenuItemTransformMirrorTopBotton_Click(object sender, RoutedEventArgs e) =>
			Transform(p => new UndoableGrid(p.MirrorTopBottom()));

		private void MenuItemTransformMirrorDiagonal_Click(object sender, RoutedEventArgs e) =>
			Transform(p => new UndoableGrid(p.MirrorDiagonal()));

		private void MenuItemTransformMirrorAntidiagonal_Click(object sender, RoutedEventArgs e) =>
			Transform(p => new UndoableGrid(p.MirrorAntidiagonal()));

		private void MenuItemTransformRotateClockwise_Click(object sender, RoutedEventArgs e) =>
			Transform(p => new UndoableGrid(p.RotateClockwise()));

		private void MenuItemTransformRotateCounterclockwise_Click(object sender, RoutedEventArgs e) =>
			Transform(p => new UndoableGrid(p.RotateCounterclockwise()));

		private void MenuItemTransformRotatePi_Click(object sender, RoutedEventArgs e) =>
			Transform(p => new UndoableGrid(p.RotatePi()));

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemViewsShowBugN_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async ValueTask internalOperation()
			{
				if (!_puzzle.IsValid(out _))
				{
					Messagings.FailedToCheckDueToInvalidPuzzle();
					e.Handled = true;
					return;
				}

				_textBoxInfo.Text = (string)LangSource["WhileCalculatingTrueCandidates"];

				var trueCandidates = await Task.Run(() => new BugChecker(_puzzle).GetAllTrueCandidates(64));

				_textBoxInfo.ClearValue(TextBox.TextProperty);
				if (trueCandidates.Count == 0)
				{
					Messagings.DoesNotContainBugMultiple();
					e.Handled = true;
					return;
				}

				_currentPainter.View = new View((from candidate in trueCandidates select (0, candidate)).ToArray());
				_currentPainter.Conclusions = null;

				UpdateImageGrid();

				_textBoxInfo.Text =
					$"{LangSource["AllTrueCandidates"]}" +
					$"{new CandidateCollection(trueCandidates).ToString()}";
			}
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void MenuItemViewsBackdoorView_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async ValueTask internalOperation()
			{
				if (!_puzzle.IsValid(out _))
				{
					Messagings.FailedToCheckDueToInvalidPuzzle();
					e.Handled = true;
					return;
				}

				_textBoxInfo.Text = (string)LangSource["WhileCalculatingBackdoorsLevel0Or1"];

				var backdoors = new List<Conclusion>();
				for (int level = 0; level <= 1; level++)
				{
					foreach (var backdoor in
						await Task.Run(() => new BackdoorSearcher().SearchForBackdoorsExact(_puzzle, level)))
					{
						backdoors.AddRange(backdoor);
					}
				}

				_textBoxInfo.ClearValue(TextBox.TextProperty);
				if (backdoors.Count == 0)
				{
					Messagings.DoesNotContainBackdoor();
					e.Handled = true;
					return;
				}

				var highlightCandidates = new List<(int, int)>();
				int currentLevel = 0;
				foreach (var (_, candidate) in backdoors)
				{
					highlightCandidates.Add((currentLevel, candidate));

					currentLevel++;
				}

				_currentPainter.View =
					new View(
						new List<(int, int)>(
							from conclusion in backdoors
							where conclusion.ConclusionType == ConclusionType.Assignment
							select (0, conclusion.CellOffset * 9 + conclusion.Digit)));
				_currentPainter.Conclusions = backdoors;

				UpdateImageGrid();

				_textBoxInfo.Text =
					$"{LangSource["AllBackdoorsAtLevel0Or1"]}" +
					$"{new ConclusionCollection(backdoors).ToString()}";
			}
		}

		private void MenuItemViewsGspView_Click(object sender, RoutedEventArgs e)
		{
			if (Enumerable.Range(0, 81).All(i => _puzzle.GetStatus(i) != CellStatus.Given))
			{
				Messagings.SukakuCannotUseGspChecking();
				e.Handled = true;
				return;
			}

			if (!(new GspTechniqueSearcher().GetOne(_puzzle) is GspTechniqueInfo info))
			{
				Messagings.DoesNotContainGsp();
				e.Handled = true;
				return;
			}

			bool[] series = new bool[9];
			int?[] mapping = info.MappingTable;
			var cellOffsets = new List<(int, int)>();
			for (int i = 0, p = 0; i < 9; i++)
			{
				if (series[i])
				{
					continue;
				}

				int? value = mapping[i];
				if (value is null)
				{
					continue;
				}

				int j = value.Value;
				(series[i], series[j]) = (true, true);
				for (int cell = 0; cell < 81; cell++)
				{
					int cellValue = _puzzle[cell];
					if (cellValue == i || cellValue == j)
					{
						cellOffsets.Add((p, cell));
					}
				}

				p++;
			}

			_textBoxInfo.Text = info.ToString();
			_currentPainter.View = new View(cellOffsets, null, null, null);
			_currentPainter.Conclusions = info.Conclusions;

			UpdateImageGrid();
		}

		private void MenuItemLanguagesChinese_Click(object sender, RoutedEventArgs e) => ChangeLanguage("zh-cn");

		private void MenuItemLanguagesEnglish_Click(object sender, RoutedEventArgs e) => ChangeLanguage("en-us");

		private void MenuItemAboutMe_Click(object sender, RoutedEventArgs e) => new AboutMeWindow().Show();

		private void MenuItemAboutSpecialThanks_Click(object sender, RoutedEventArgs e) =>
			new SpecialThanksWindow().Show();

		private void MenuItemAboutImplementedTechniques_Click(object sender, RoutedEventArgs e) =>
			new TechniquesWindow().Show();

		private void ContextListBoxPathsCopyCurrentStep_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem)
			{
				try
				{ 
					if (_listBoxPaths.SelectedItem is ListBoxItem listBoxItem
						&& listBoxItem.Content is PrimaryElementTuple<string, int, TechniqueInfo> triplet)
					{
						Clipboard.SetText(triplet.Value3.ToFullString());
					}
				}
				catch
				{
					Messagings.CannotCopyStep();
				}
			}
		}

		[SuppressMessage("Style", "IDE0038:Use pattern matching", Justification = "<Pending>")]
		private void ContextListBoxPathsCopyAllSteps_Click(object sender, RoutedEventArgs e)
		{
			if (sender is MenuItem)
			{
				var sb = new StringBuilder();
				foreach (string step in
					from ListBoxItem item in _listBoxPaths.Items
					let Content = item.Content
					where Content is PrimaryElementTuple<string, int, TechniqueInfo>
					select ((PrimaryElementTuple<string, int, TechniqueInfo>)Content).Value3.ToFullString())
				{
					sb.AppendLine(step);
				}

				try
				{
					Clipboard.SetText(sb.ToString());
				}
				catch
				{
					Messagings.CannotCopyStep();
				}
			}
		}

		private void MenuItemImageGridSet1_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 0);

		private void MenuItemImageGridSet2_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 1);

		private void MenuItemImageGridSet3_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 2);

		private void MenuItemImageGridSet4_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 3);

		private void MenuItemImageGridSet5_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 4);

		private void MenuItemImageGridSet6_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 5);

		private void MenuItemImageGridSet7_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 6);

		private void MenuItemImageGridSet8_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 7);

		private void MenuItemImageGridSet9_Click(object sender, RoutedEventArgs e) =>
			SetADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 8);

		private void MenuItemImageGridDelete1_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 0);

		private void MenuItemImageGridDelete2_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 1);

		private void MenuItemImageGridDelete3_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 2);

		private void MenuItemImageGridDelete4_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 3);

		private void MenuItemImageGridDelete5_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 4);

		private void MenuItemImageGridDelete6_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 5);

		private void MenuItemImageGridDelete7_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 6);

		private void MenuItemImageGridDelete8_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 7);

		private void MenuItemImageGridDelete9_Click(object sender, RoutedEventArgs e) =>
			DeleteADigit(_pointConverter.GetCellOffset(_currentRightClickPos.ToDPointF()), 8);
	}
}
