﻿#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
#pragma warning disable IDE1006
#endif

namespace Sudoku.UI.Data.Configuration;

/// <summary>
/// Defines the user preferences in the program.
/// </summary>
/// <remarks>
/// Due to the special initialization mechanism, the properties of the type should satisfy the following conditions:
/// <list type="bullet">
/// <item>
/// Only properties that contains <b>both</b> getter and normal setter (and not a <see langword="init"/> setter)
/// can be loaded, initialized and displayed to the settings page.
/// </item>
/// <item>
/// The property must be applied both attribute types <see cref="PreferenceAttribute{TSettingItem}"/>
/// and <see cref="PreferenceGroupAttribute"/>.
/// </item>
/// <item>
/// If the property is a non-formal preference item, a double extra leading underscore characters "<c>__</c>"
/// will be inserted into the property name; otherwise, a normal property name is applied.
/// </item>
/// </list>
/// </remarks>
/// <seealso cref="PreferenceAttribute{TSettingItem}"/>
/// <seealso cref="PreferenceGroupAttribute"/>
public sealed class Preference : IDrawingPreference
{
	#region Basic Options
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 0)]
	public bool ShowCandidates { get; set; } = true;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 1)]
	public bool ShowCandidateBorderLines { get; set; } = false;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see cref="PeerFocusingMode.FocusedCellAndPeerCells"/>.
	/// </remarks>
	[Preference<PeerFocusingModeComboBoxSettingItem>(nameof(PeerFocusingModeComboBoxSettingItem.OptionContents), 3)]
	[PreferenceGroup(PreferenceGroupNames.Basic, 2)]
	public PeerFocusingMode PeerFocusingMode { get; set; } = PeerFocusingMode.FocusedCellAndPeerCells;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#300000FF</c> (i.e. <see cref="Colors.Blue"/> with alpha 48).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 3)]
	public Color FocusedCellColor { get; set; } = Colors.Blue with { A = 48 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#200000FF</c> (i.e. <see cref="Colors.Blue"/> with alpha 32).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 4)]
	public Color PeersFocusedCellColor { get; set; } = Colors.Blue with { A = 32 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 5)]
	public bool EnableDeltaValuesDisplaying { get; set; } = true;

	/// <summary>
	/// Indicates whether the program will use zero character <c>'0'</c> as the placeholder to describe empty cells
	/// in a sudoku grid that we should copied.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 6)]
	public bool PlaceholderIsZero { get; set; } = true;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>{ FontName = "Cascadia Mono", FontScale = .8 }</c> in debugging mode.
	/// </remarks>
	[Preference<FontPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 7)]
	public FontData ValueFont { get; set; } = new()
	{
		FontName =
#if DEBUG
			"Cascadia Mono",
#else
			"Tahoma",
#endif
		FontScale = .8
	};

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>{ FontName = "Cascadia Mono", FontScale = .25 }</c> in debugging mode.
	/// </remarks>
	[Preference<FontPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 8)]
	public FontData CandidateFont { get; set; } = new()
	{
		FontName =
#if DEBUG
			"Cascadia Mono",
#else
			"Tahoma",
#endif
		FontScale = .25
	};

#if AUTHOR_FEATURE_CELL_MARKS || AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <summary>
	/// Indicates whether the old shape should be covered when diffused.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 9)]
	public bool __CoverOldShapeWhenDiffused { get; set; } = false;
#endif

	/// <summary>
	/// Indicates whether the picture will also be saved when a drawing data file is saved to local.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 10)]
	public bool AlsoSavePictureWhenSaveDrawingData { get; set; } = true;

	/// <summary>
	/// Indicates whether the program always opens the home page if you open the program non-programmatically.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Basic, 11)]
	public bool AlwaysShowHomePageWhenOpen { get; set; } = true;
	#endregion

	#region Rendering Options
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>0</c>.
	/// </remarks>
	[Preference<SliderSettingItem>(
		nameof(SliderSettingItem.StepFrequency), .1, nameof(SliderSettingItem.TickFrequency), .3,
		nameof(SliderSettingItem.MinValue), 0D, nameof(SliderSettingItem.MaxValue), 3D)]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 0)]
	public double OutsideBorderWidth { get; set; } = 0;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>4</c>.
	/// </remarks>
	[Preference<SliderSettingItem>(
		nameof(SliderSettingItem.StepFrequency), .5, nameof(SliderSettingItem.TickFrequency), .5,
		nameof(SliderSettingItem.MinValue), 0D, nameof(SliderSettingItem.MaxValue), 5D)]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 1)]
	public double BlockBorderWidth { get; set; } = 4;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>1</c>.
	/// </remarks>
	[Preference<SliderSettingItem>(
		nameof(SliderSettingItem.StepFrequency), .1, nameof(SliderSettingItem.TickFrequency), .3,
		nameof(SliderSettingItem.MinValue), 0D, nameof(SliderSettingItem.MaxValue), 3D)]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 2)]
	public double CellBorderWidth { get; set; } = 1;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>1</c>.
	/// </remarks>
	[Preference<SliderSettingItem>(
		nameof(SliderSettingItem.StepFrequency), .1, nameof(SliderSettingItem.TickFrequency), .3,
		nameof(SliderSettingItem.MinValue), 0D, nameof(SliderSettingItem.MaxValue), 3D)]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 3)]
	public double CandidateBorderWidth { get; set; } = 1;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 4)]
	public Color OutsideBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFFFFF</c> (i.e. <see cref="Colors.White"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 5)]
	public Color GridBackgroundFillColor { get; set; } = Colors.White;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 6)]
	public Color BlockBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 7)]
	public Color CellBorderColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFD3D3D3</c> (i.e. <see cref="Colors.LightGray"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 8)]
	public Color CandidateBorderColor { get; set; } = Colors.LightGray;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 9)]
	public Color GivenColor { get; set; } = Colors.Black;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF0000FF</c> (i.e. <see cref="Colors.Blue"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 10)]
	public Color ModifiableColor { get; set; } = Colors.Blue;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF696969</c> (i.e. <see cref="Colors.DimGray"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 11)]
	public Color CandidateColor { get; set; } = Colors.DimGray;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c> (i.e. <see cref="Colors.Red"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 12)]
	public Color CellDeltaColor { get; set; } = Colors.Red;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFFB9B9</c>.
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 13)]
	public Color CandidateDeltaColor { get; set; } = Color.FromArgb(255, 255, 185, 185);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF000000</c> (i.e. <see cref="Colors.Black"/>).
	/// </remarks>
	[Preference<ColorPickerSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Rendering, 14)]
	public Color MaskEllipseColor { get; set; } = Colors.Black;
	#endregion

	#region Miscellaneous Options
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Miscellaneous, 0)]
	public bool DescendingOrderedInfoBarBoard { get; set; } = true;

	/// <summary>
	/// Indicates whether the program always check the battery status at first, when you open the program
	/// non-programmatically.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[Preference<ToggleSwitchSettingItem>]
	[PreferenceGroup(PreferenceGroupNames.Miscellaneous, 1)]
	public bool CheckBatteryStatusWhenOpen { get; set; } = true;
	#endregion

	#region Other Options (Only used for program handling, not under the user's control)
	/// <summary>
	/// Indicates whether the program is the first time to be used.
	/// </summary>
	/// <remarks>
	/// The default value is <see langword="true"/>.
	/// </remarks>
	[BackgroundPreference]
	public bool IsFirstMeet { get; set; } = true;
	#endregion

	#region Other Options (Some belong to none of all groups mentioned above)
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>3</c>.
	/// </remarks>
	public double HighlightCellStrokeThickness { get; set; } = 3;

#if AUTHOR_FEATURE_CELL_MARKS
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>4</c>.
	/// </remarks>
	public double __CrossMarkStrokeThickness { get; set; } = 4;
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>2</c>.
	/// </remarks>
	public double __CandidateMarkStrokeThickness { get; set; } = 2;
#endif

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF3FDA65</c>.
	/// </remarks>
	public Color NormalColor { get; set; } = Color.FromArgb(255, 63, 218, 101);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF7684</c>.
	/// </remarks>
	public Color EliminationColor { get; set; } = Color.FromArgb(255, 255, 118, 132);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FF7FBBFF</c>.
	/// </remarks>
	public Color ExofinColor { get; set; } = Color.FromArgb(255, 127, 187, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFD8B2FF</c>.
	/// </remarks>
	public Color EndofinColor { get; set; } = Color.FromArgb(255, 216, 178, 255);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFEB0000</c>.
	/// </remarks>
	public Color CannibalismColor { get; set; } = Color.FromArgb(255, 235, 0, 0);

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#FFFF0000</c>.
	/// </remarks>
	public Color LinkColor { get; set; } = Color.FromArgb(255, 255, 0, 0);

#if AUTHOR_FEATURE_CELL_MARKS
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __CellRectangleFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __CellCircleFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __CrossMarkStrokeColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __StarFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __TriangleFillColor { get; set; } = Colors.Black with { A = 64 };

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#40000000</c> (i.e. <see cref="Colors.Black"/> with alpha 64).
	/// </remarks>
	public Color __DiamondFillColor { get; set; } = Colors.Black with { A = 64 };
#endif

#if AUTHOR_FEATURE_CANDIDATE_MARKS
	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>#80000000</c> (i.e. <see cref="Colors.Black"/> with alpha 128).
	/// </remarks>
	public Color __CandidateMarkStrokeColor { get; set; } = Colors.Black with { A = 128 };
#endif

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is an array of 3 elements:
	/// <list type="number">
	/// <item>#FF7FBBFF</item>
	/// <item>#FFD8B2FF</item>
	/// <item>#FFFFFF96</item>
	/// </list>
	/// </remarks>
	public Color[] AuxiliaryColors { get; set; } =
	{
		Color.FromArgb(255, 127, 187, 255), // FF7FBBFF
		Color.FromArgb(255, 216, 178, 255), // FFD8B2FF
		Color.FromArgb(255, 255, 255, 150) // FFFFFF96
	};

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is an array of 5 elements:
	/// <list type="number">
	/// <item>#FFC5E88C</item>
	/// <item>#FFFFCBCB</item>
	/// <item>#FFB2DFDF</item>
	/// <item>#FFFCDCA5</item>
	/// <item>#FFFFFF96</item>
	/// </list>
	/// </remarks>
	public Color[] AlmostLockedSetColors { get; set; } =
	{
		Color.FromArgb(255, 197, 232, 140), // FFC5E88C
		Color.FromArgb(255, 255, 203, 203), // FFFFCBCB
		Color.FromArgb(255, 178, 223, 223), // FFB2DFDF
		Color.FromArgb(255, 252, 220, 165), // FFFCDCA5
		Color.FromArgb(255, 255, 255, 150) // FFFFFF96
	};

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is an array of 10 elements:
	/// <list type="number">
	/// <item>#FFFFC059 (Orange)</item>
	/// <item>#FFB1A5F3 (Light purple)</item>
	/// <item>#FFF7A5A7 (Red)</item>
	/// <item>#FF86E8D0 (Sky blue)</item>
	/// <item>#FF86F280 (Light green)</item>
	/// <item>#FFF7DE8F (Light orange)</item>
	/// <item>#FFDCD4FC (Whitey purple)</item>
	/// <item>#FFFFD2D2 (Light red)</item>
	/// <item>#FFCEFBED (Whitey blue)</item>
	/// <item>#FFD7FFD7 (Whitey green)</item>
	/// </list>
	/// All values of this array are referenced from sudoku project
	/// <see href="https://sourceforge.net/projects/hodoku/">Hodoku</see>.
	/// </remarks>
	public Color[] PaletteColors { get; set; } =
	{
		Color.FromArgb(255, 255, 192, 89), // FFFFC059
		Color.FromArgb(255, 177, 165, 243), // FFB1A5F3
		Color.FromArgb(255, 247, 165, 167), // FFF7A5A7
		Color.FromArgb(255, 134, 232, 208), // FF86E8D0
		Color.FromArgb(255, 134, 242, 128), // FF86F280
		Color.FromArgb(255, 247, 222, 143), // FFF7DE8F
		Color.FromArgb(255, 220, 212, 252), // FFDCD4FC
		Color.FromArgb(255, 255, 210, 210), // FFFFD2D2
		Color.FromArgb(255, 206, 251, 237), // FFCEFBED
		Color.FromArgb(255, 215, 255, 215) // FFD7FFD7
	};
	#endregion


	/// <summary>
	/// Covers the config file by the specified preference instance.
	/// </summary>
	/// <param name="preference">
	/// The preference instance. If the value is <see langword="null"/>, no operation will be handled.
	/// </param>
	public void CoverPreferenceBy(Preference? preference)
	{
		if (preference is null)
		{
			return;
		}

		((IDrawingPreference)this).CoverPreferenceBy(preference);
	}
}