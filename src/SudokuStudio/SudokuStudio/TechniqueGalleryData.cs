﻿namespace SudokuStudio;

/// <summary>
/// Provides with a field that stores the techniques data used by page <see cref="TechniqueGalleryPage"/>.
/// </summary>
/// <seealso cref="TechniqueGalleryPage"/>
internal static class TechniqueGalleryData
{
	/// <summary>
	/// The routing groups.
	/// </summary>
	public static readonly ObservableCollection<TechniquePageRoutingDataGroup> RoutingGroups = new()
	{
		new()
		{
			Title = GetString("TechniqueGalleryPage_Singles"),
			Collection = new TechniquePageRoutingData[]
			{
				new() { IsEnabled = false, Color = Colors.Green, Technique = Technique.FullHouse, RoutingPageTypeName = typeof(FullHousePage).Name },
				new() { IsEnabled = false, Color = Colors.Green, Technique = Technique.LastDigit },
				new() { IsEnabled = false, Color = Colors.Green, Technique = Technique.HiddenSingleBlock },
				new() { IsEnabled = false, Color = Colors.Green, Technique = Technique.HiddenSingleRow },
				new() { IsEnabled = false, Color = Colors.Green, Technique = Technique.HiddenSingleColumn },
				new() { IsEnabled = false, Color = Colors.Green, Technique = Technique.NakedSingle }
			}
		},
		new()
		{
			Title = GetString("TechniqueGalleryPage_Intersections"),
			Collection = new TechniquePageRoutingData[]
			{
				new() { IsEnabled = false, Color = Colors.Aqua, Technique = Technique.Pointing },
				new() { IsEnabled = false, Color = Colors.Aqua, Technique = Technique.Claiming }
			}
		},
		new()
		{
			Title = GetString("TechniqueGalleryPage_Subsets"),
			Collection = new TechniquePageRoutingData[]
			{
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.NakedPair },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.NakedTriple },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.NakedQuadruple },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.HiddenPair },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.HiddenTriple },
				new() { IsEnabled = false, Color = Colors.Yellow, Technique = Technique.HiddenQuadruple }
			}
		}
	};
}
