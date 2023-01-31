﻿namespace SudokuStudio.Storage;

/// <summary>
/// Provides with some paths that is used for the whole program.
/// </summary>
internal static class CommonPaths
{
	/// <summary>
	/// Indicates the user preference path.
	/// </summary>
	public static readonly string UserPreference;


	/// <include file='../../../global-doc-comments.xml' path='g/static-constructor' />
	static CommonPaths()
	{
		var documents = Environment.GetFolderPath(SpecialFolder.MyDocuments);

		UserPreference = $@"{documents}\SudokuStudio\user-preference{CommonFileExtensions.UserPreference}";
	}
}
