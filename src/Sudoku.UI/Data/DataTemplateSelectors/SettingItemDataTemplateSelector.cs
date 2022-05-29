﻿namespace Sudoku.UI.Data.DataTemplateSelectors;

/// <summary>
/// Defines a data template selector that determines which data template can be used on a setting item.
/// </summary>
public sealed class SettingItemDataTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the template that is used for a toggle switch.
	/// </summary>
	public DataTemplate ToggleSwitchTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the default template.
	/// </summary>
	public DataTemplate DefaultTemplate { get; set; } = null!;


	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">Throws when data template cannot be found.</exception>
	protected override DataTemplate SelectTemplateCore(object item)
		=> item switch
		{
			ToggleSwitchSettingItem => ToggleSwitchTemplate,
			_ => DefaultTemplate
		};
}
