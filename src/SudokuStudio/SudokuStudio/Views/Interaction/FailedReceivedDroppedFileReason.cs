﻿namespace SudokuStudio.Views.Interaction;

/// <summary>
/// Defines an enumeration type that describes reasons why failed to drag a file.
/// </summary>
public enum FailedReceivedDroppedFileReason : int
{
	/// <summary>
	/// Indicates the failed reason is that the file is empty.
	/// </summary>
	FileIsEmpty,

	/// <summary>
	/// Indicates the failed reason is that the file is too large (more than 1MB).
	/// </summary>
	FileIsTooLarge
}