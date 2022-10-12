﻿using VerifyCS = Sudoku.Diagnostics.CodeAnalysis.Test.CSharpAnalyzerVerifier<
	Sudoku.Diagnostics.CodeAnalysis.Analyzers.SCA0202_UseAddMethodAnalyzer
>;

namespace Sudoku.Diagnostics.CodeAnalysis.Test;

[TestClass]
public sealed class SCA0202UnitTest
{
	[TestMethod]
	public async Task TestCase_EmptyCode() => await VerifyCS.VerifyAnalyzerAsync(@"");

	[TestMethod]
	public async Task TestCase_Normal()
		=> await VerifyCS.VerifyAnalyzerAsync(
			"""
			#nullable enable

			using Sudoku.Concepts;

			file sealed class Program
			{
				private static void Case()
				{
					var a = new CellMap();
					{|#0:a += 42|};
				}
			}

			namespace Sudoku.Concepts
			{
				file readonly struct CellMap
				{
					public void Add(int a) { }
				
					public static CellMap operator +(CellMap a, int b) => new();
				}
			}
			""",
			VerifyCS.Diagnostic(nameof(SCA0202)).WithLocation(0)
		);
}
