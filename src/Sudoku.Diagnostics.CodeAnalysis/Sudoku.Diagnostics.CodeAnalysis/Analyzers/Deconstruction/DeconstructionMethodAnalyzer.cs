﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for deconstruction methods.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class DeconstructionMethodAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.MethodDeclaration });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode) = context;
			if (
				originalNode is not MethodDeclarationSyntax
				{
					Parent: { } parentNode,
					Identifier: { ValueText: "Deconstruct" } identifier,
					ParameterList: { Parameters: { Count: var parametersCount } parameters },
					Modifiers: var modifiers,
					ReturnType: var returnType,
					Body: var body,
					ExpressionBody: var expressionBody
				} node
			)
			{
				return;
			}

			if (semanticModel.GetDeclaredSymbol(parentNode) is { IsStatic: true })
			{
				return;
			}

			if (semanticModel.GetTypeInfo(returnType).Type is not { } type)
			{
				return;
			}

			CheckSS0501AndSS0505(context, parameters, identifier);
			CheckSudokuSS0502AndSS0504(context, modifiers, identifier);
			CheckSS0503(context, returnType, type, compilation);

			var members =
				from member in semanticModel.GetDeclaredSymbol(originalNode)!.ContainingType.GetAllMembers()
				where member switch
				{
					IFieldSymbol { IsStatic: false } => true,
					IPropertySymbol { IsStatic: false, Parameters: { IsEmpty: true } } => true,
					_ => false
				}
				select member;
			CheckSS0506AndSS0507(context, parameters, body, expressionBody, members);
			CheckSS0508(context, semanticModel, node, identifier, type, parametersCount);
		}

		private static void CheckSS0501AndSS0505(
			SyntaxNodeAnalysisContext context, SeparatedSyntaxList<ParameterSyntax> parameters,
			SyntaxToken identifier)
		{
			if (parameters.Count < 2)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0501,
						location: identifier.GetLocation(),
						messageArgs: null
					)
				);
			}

			foreach (var parameter in parameters)
			{
				if (parameter.Modifiers.All(DoesNotContainOutKeyword))
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0505,
							location: parameter.GetLocation(),
							messageArgs: null
						)
					);
				}
			}
		}

		private static void CheckSudokuSS0502AndSS0504(
			SyntaxNodeAnalysisContext context, SyntaxTokenList modifiers, SyntaxToken identifier)
		{
			if (modifiers.Any(ContainsStaticKeyword))
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0502,
						location: identifier.GetLocation(),
						messageArgs: null,
						additionalLocations: new[] { context.Node.GetLocation() }
					)
				);
			}

			if (modifiers.All(DoesNotContainPublicKeyword))
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0504,
						location: identifier.GetLocation(),
						messageArgs: null
					)
				);
			}
		}

		private static void CheckSS0503(
			SyntaxNodeAnalysisContext context, TypeSyntax returnType, ITypeSymbol type,
			Compilation compilation)
		{
			if (
				SymbolEqualityComparer.Default.Equals(
					type,
					compilation.GetSpecialType(SpecialType.System_Void)
				)
			)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0503,
					location: returnType.GetLocation(),
					messageArgs: null
				)
			);
		}

		private static void CheckSS0506AndSS0507(
			SyntaxNodeAnalysisContext context, SeparatedSyntaxList<ParameterSyntax> parameters,
			BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, IEnumerable<ISymbol> members)
		{
			checkSS0507();

			if (body is not null)
			{
				checkSS0506(body);
			}
			else if (expressionBody is not null)
			{
				checkSS0506(expressionBody);
			}

			void checkSS0507()
			{
				foreach (var parameter in parameters)
				{
					string text = parameter.Identifier.ValueText;
					if (
						members.Any(
							member => member.Name is var n && (
								n == text.ToCamelCase()
								|| n == $"_{text.ToCamelCase()}"
								|| n == text.ToPascalCase()
							)
						)
					)
					{
						continue;
					}

					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0507,
							location: parameter.GetLocation(),
							messageArgs: new[] { text }
						)
					);
				}
			}

			void checkSS0506(SyntaxNode body)
			{
				foreach (var descendant in
					from descendant in body.DescendantNodes().OfType<BinaryExpressionSyntax>()
					where descendant.RawKind == (int)SyntaxKind.SimpleAssignmentExpression
					select descendant)
				{
					var rightExpr = descendant.Right;
					switch (rightExpr)
					{
						case IdentifierNameSyntax
						{
							Identifier: { ValueText: var possibleDataMemberName }
						} identifierName
						when context.SemanticModel.GetOperation(identifierName) is IFieldReferenceOperation:
						{
							continue;
						}
						default:
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0506,
									location: rightExpr.GetLocation(),
									messageArgs: new[] { rightExpr.ToString() }
								)
							);

							break;
						}
					}
				}
			}
		}

		private static void CheckSS0508(
			SyntaxNodeAnalysisContext context, SemanticModel semanticModel, SyntaxNode node,
			SyntaxToken identifier, ITypeSymbol type, int parametersCount)
		{
			var methodSymbol = (IMethodSymbol)semanticModel.GetDeclaredSymbol(node)!;
			foreach (var deconstructionMethod in type.GetAllDeconstructionMethods())
			{
				if (deconstructionMethod.ToDisplayString() == methodSymbol.ToDisplayString())
				{
					continue;
				}

				if (deconstructionMethod.Parameters.Length != parametersCount)
				{
					continue;
				}

				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0508,
						location: identifier.GetLocation(),
						messageArgs: null
					)
				);

				// Only reports once is okay.
				return;
			}
		}

		private static bool ContainsStaticKeyword(SyntaxToken token) => token.RawKind == (int)SyntaxKind.StaticKeyword;
		private static bool DoesNotContainPublicKeyword(SyntaxToken token) => token.RawKind != (int)SyntaxKind.PublicKeyword;
		private static bool DoesNotContainOutKeyword(SyntaxToken token) => token.RawKind != (int)SyntaxKind.OutKeyword;
	}
}
