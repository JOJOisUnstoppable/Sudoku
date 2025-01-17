﻿namespace SudokuStudio.Configuration;

using K = CoordinateLabelDisplayKind;
using M = CoordinateLabelDisplayMode;

/// <summary>
/// Defines a list of UI-related preference items. Some items in this group may not be found in settings page
/// because they are controlled by UI only, not by users.
/// </summary>
[DependencyProperty<bool>("DisplayCandidates", DefaultValue = true, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DisplayCandidates")]
[DependencyProperty<bool>("DisplayCursors", DefaultValue = true, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DisplayCursors")]
[DependencyProperty<bool>("DistinctWithDeltaDigits", DefaultValue = true, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.UseDifferentColorToDisplayDeltaDigits")]
[DependencyProperty<bool>("DisableSudokuPaneLayout", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DisableFlyout")]
[DependencyProperty<bool>("PreventConflictingInput", DefaultValue = true, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.PreventConflictingInput")]
[DependencyProperty<bool>("SavePuzzleGeneratingHistory", DefaultValue = true, DocSummary = "Indicates whether the program saves for puzzle-generting history.")]
[DependencyProperty<bool>("EnableDoubleTapFillingForSudokuPane", DefaultValue = true, DocSummary = "Indicates whether sudoku pane in analysis page provides with a simpler way to fill with digits via double tapping.")]
[DependencyProperty<bool>("EnableRightTapRemovingForSudokuPane", DefaultValue = true, DocSummary = "Indicates whether sudoku pane in analysis page provides with a simpler way to delete digits via right tapping.")]
[DependencyProperty<double>("HighlightedPencilmarkBackgroundEllipseScale", DefaultValue = .9, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.HighlightCandidateCircleScale")]
[DependencyProperty<double>("HighlightedBackgroundOpacity", DefaultValue = .15, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.HighlightBackgroundOpacity")]
[DependencyProperty<double>("ChainStrokeThickness", DefaultValue = 1.5, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.ChainStrokeThickness")]
[DependencyProperty<int>("CoordinateLabelDisplayKind", DefaultValue = (int)K.RxCy, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.CoordinateLabelDisplayKind")]
[DependencyProperty<int>("CoordinateLabelDisplayMode", DefaultValue = (int)M.FourDirection, DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.CoordinateLabelDisplayMode")]
[DependencyProperty<Color>("DeltaValueColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DeltaCellColor")]
[DependencyProperty<Color>("DeltaPencilmarkColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DeltaCandidateColor")]
[DependencyProperty<Color>("SudokuPaneBorderColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.BorderColor")]
[DependencyProperty<Color>("CursorBackgroundColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.CursorBackgroundColor")]
[DependencyProperty<Color>("ChainColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.LinkColor")]
[DependencyProperty<Color>("NormalColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.NormalColor")]
[DependencyProperty<Color>("AssignmentColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.AssignmentColor")]
[DependencyProperty<Color>("OverlappedAssignmentColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.OverlappedAssignmentColor")]
[DependencyProperty<Color>("EliminationColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.EliminationColor")]
[DependencyProperty<Color>("CannibalismColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.CannibalismColor")]
[DependencyProperty<Color>("ExofinColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.ExofinColor")]
[DependencyProperty<Color>("EndofinColor", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.EndofinColor")]
[DependencyProperty<DashArray>("StrongLinkDashStyle", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.StrongLinkDashStyle")]
[DependencyProperty<DashArray>("WeakLinkDashStyle", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.WeakLinkDashStyle")]
[DependencyProperty<DashArray>("CyclingCellLinkDashStyle", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.CycleLikeLinkDashStyle")]
[DependencyProperty<DashArray>("OtherLinkDashStyle", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.OtherLinkDashStyle")]
[DependencyProperty<ColorPalette>("AuxiliaryColors", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.AuxiliaryColors")]
[DependencyProperty<ColorPalette>("DifficultyLevelForegrounds", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DifficultyLevelForegrounds")]
[DependencyProperty<ColorPalette>("DifficultyLevelBackgrounds", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.DifficultyLevelBackgrounds")]
[DependencyProperty<ColorPalette>("UserDefinedColorPalette", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.UserDefinedColorPalette")]
[DependencyProperty<ColorPalette>("AlmostLockedSetsColors", DocReferencedMemberName = "global::SudokuStudio.Views.Controls.SudokuPane.AlmostLockedSetsColors")]
[DependencyProperty<FontSerializationData>("GivenFontData")]
[DependencyProperty<FontSerializationData>("ModifiableFontData")]
[DependencyProperty<FontSerializationData>("PencilmarkFontData")]
[DependencyProperty<FontSerializationData>("BabaGroupingFontData")]
[DependencyProperty<FontSerializationData>("CoordinateLabelFontData")]
public sealed partial class UIPreferenceGroup : PreferenceGroup
{
	[DefaultValue]
	private static readonly Color DeltaValueColorDefaultValue = Colors.Red;

	[DefaultValue]
	private static readonly Color DeltaPencilmarkColorDefaultValue = Color.FromArgb(255, 255, 185, 185);

	[DefaultValue]
	private static readonly Color SudokuPaneBorderColorDefaultValue = Colors.Black;

	[DefaultValue]
	private static readonly Color CursorBackgroundColorDefaultValue = Colors.Blue with { A = 32 };

	[DefaultValue]
	private static readonly Color ChainColorDefaultValue = Colors.Red;

	[DefaultValue]
	private static readonly Color NormalColorDefaultValue = Color.FromArgb(255, 63, 218, 101);

	[DefaultValue]
	private static readonly Color AssignmentColorDefaultValue = Color.FromArgb(255, 63, 218, 101);

	[DefaultValue]
	private static readonly Color OverlappedAssignmentColorDefaultValue = Color.FromArgb(255, 0, 255, 204);

	[DefaultValue]
	private static readonly Color EliminationColorDefaultValue = Color.FromArgb(255, 255, 118, 132);

	[DefaultValue]
	private static readonly Color CannibalismColorDefaultValue = Color.FromArgb(255, 235, 0, 0);

	[DefaultValue]
	private static readonly Color ExofinColorDefaultValue = Color.FromArgb(255, 127, 187, 255);

	[DefaultValue]
	private static readonly Color EndofinColorDefaultValue = Color.FromArgb(255, 216, 178, 255);

	[DefaultValue]
	private static readonly DashArray StrongLinkDashStyleDefaultValue = new();

	[DefaultValue]
	private static readonly DashArray WeakLinkDashStyleDefaultValue = new(3, 1.5);

	[DefaultValue]
	private static readonly DashArray CyclingCellLinkDashStyleDefaultValue = new();

	[DefaultValue]
	private static readonly DashArray OtherLinkDashStyleDefaultValue = new(3, 3);

	[DefaultValue]
	private static readonly ColorPalette AuxiliaryColorsDefaultValue = new()
	{
		Color.FromArgb(255, 255, 192, 89),
		Color.FromArgb(255, 127, 187, 255),
		Color.FromArgb(255, 216, 178, 255)
	};

	[DefaultValue]
	private static readonly ColorPalette AlmostLockedSetsColorsDefaultValue = new()
	{
		Color.FromArgb(255, 255, 203, 203),
		Color.FromArgb(255, 178, 223, 223),
		Color.FromArgb(255, 252, 220, 165),
		Color.FromArgb(255, 255, 255, 150),
		Color.FromArgb(255, 247, 222, 143)
	};

	[DefaultValue]
	private static readonly ColorPalette DifficultyLevelForegroundsDefaultValue = new()
	{
		Color.FromArgb(255, 0, 51, 204),
		Color.FromArgb(255, 0, 102, 0),
		Color.FromArgb(255, 102, 51, 0),
		Color.FromArgb(255, 102, 51, 0),
		Color.FromArgb(255, 102, 0, 0),
		Colors.Black
	};

	[DefaultValue]
	private static readonly ColorPalette DifficultyLevelBackgroundsDefaultValue = new()
	{
		Color.FromArgb(255, 204, 204, 255),
		Color.FromArgb(255, 100, 255, 100),
		Color.FromArgb(255, 255, 255, 100),
		Color.FromArgb(255, 255, 150, 80),
		Color.FromArgb(255, 255, 100, 100),
		Color.FromArgb(255, 220, 220, 220)
	};

	[DefaultValue]
	private static readonly ColorPalette UserDefinedColorPaletteDefaultValue = new()
	{
		Color.FromArgb(255, 63, 218, 101),
		Color.FromArgb(255, 255, 192, 89),
		Color.FromArgb(255, 127, 187, 255),
		Color.FromArgb(255, 216, 178, 255),
		Color.FromArgb(255, 197, 232, 140),
		Color.FromArgb(255, 255, 203, 203),
		Color.FromArgb(255, 178, 223, 223),
		Color.FromArgb(255, 252, 220, 165),
		Color.FromArgb(255, 255, 255, 150),
		Color.FromArgb(255, 247, 222, 143),
		Color.FromArgb(255, 220, 212, 252),
		Color.FromArgb(255, 255, 118, 132),
		Color.FromArgb(255, 206, 251, 237),
		Color.FromArgb(255, 215, 255, 215),
		Color.FromArgb(255, 192, 192, 192)
	};

	[DefaultValue]
	private static readonly FontSerializationData GivenFontDataDefaultValue = new()
	{
		FontName = "Cascadia Code",
		FontScale = .85,
		FontColor = Colors.Black
	};

	[DefaultValue]
	private static readonly FontSerializationData ModifiableFontDataDefaultValue = new()
	{
		FontName = "Cascadia Code",
		FontScale = .85,
		FontColor = Colors.Blue
	};

	[DefaultValue]
	private static readonly FontSerializationData PencilmarkFontDataDefaultValue = new()
	{
		FontName = "Cascadia Code",
		FontScale = .3,
		FontColor = Color.FromArgb(255, 100, 100, 100)
	};

	[DefaultValue]
	private static readonly FontSerializationData BabaGroupingFontDataDefaultValue = new()
	{
		FontName = "Times New Roman",
		FontScale = .6,
		FontColor = Colors.Red
	};

	[DefaultValue]
	private static readonly FontSerializationData CoordinateLabelFontDataDefaultValue = new()
	{
		FontName = "Cascadia Code",
		FontScale = .4,
		FontColor = Color.FromArgb(255, 100, 100, 100)
	};
}
