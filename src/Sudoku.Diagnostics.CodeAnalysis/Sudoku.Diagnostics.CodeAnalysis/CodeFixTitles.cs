﻿namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Provides the titles for the code fix solutions.
	/// </summary>
	internal static class CodeFixTitles
	{
		public const string SD0101 = "Append default technique property";
		public const string SD0302 = "Use property 'IsEmpty' property";
		public const string SD0303 = "Use default field instead";
		public const string SD0304 = "Use property instead";
		public const string SD0306_1 = "Remove operator ~";
		public const string SD0306_2 = "Remove the whole expression";
		public const string SD0307 = "Simplify the initialization expression";
		public const string SD0308 = "Remove duplicate items";
		public const string SD0309 = "Use object initializer instead";
		
		public const string SS0102 = "Remove redundant '$'";
		public const string SS0502 = "Remove keyword 'static'";
		public const string SS9001 = "Move the expression to the initializer";
		public const string SS9002 = "Remove redundant array creation statement";
	}
}
