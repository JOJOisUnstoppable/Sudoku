﻿namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0209")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSymbolAction), typeof(SymbolKind), nameof(SymbolKind.NamedType))]
public sealed partial class SCA0209_ExtensionMethodTypesShouldBeSameAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SymbolAnalysisContext context)
	{
		if (context is not
			{
				Symbol: INamedTypeSymbol { IsStatic: true, DeclaringSyntaxReferences: [var syntaxRef] } symbol,
				CancellationToken: var ct
			})
		{
			return;
		}

		var methods = symbol.GetMembers().OfType<IMethodSymbol>();
		var dic = new Dictionary<string, bool>();
		foreach (var method in methods)
		{
			if (method is not
				{
					DeclaredAccessibility: not Accessibility.Private, // We intend to ignore private methods.
					Parameters: [{ IsThis: true, Type.Name: var typeName }, ..]
				})
			{
				continue;
			}

			if (dic.ContainsKey(typeName))
			{
				continue;
			}

			dic.Add(typeName, true);
		}

		if (dic.Count == 1)
		{
			return;
		}

		if (syntaxRef.GetSyntax(ct) is not BaseTypeDeclarationSyntax { Identifier: var identifier })
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0209, identifier.GetLocation()));
	}
}
