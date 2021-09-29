﻿namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Provides a generator that generates the deconstruction methods that are extension methods.
/// </summary>
[Generator]
public sealed partial class ExtensionDeconstructMethodGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var compilation = context.Compilation;
		var attributeSymbol = compilation
			.GetTypeByMetadataName(typeof(AutoDeconstructExtensionAttribute<>).FullName)!
			.ConstructUnboundGenericType();

		foreach (var groupedResult in
			from attributeData in compilation.Assembly.GetAttributes()
			let a = attributeData.AttributeClass
			where a.IsGenericType
			let unboundAttribute = a.ConstructUnboundGenericType()
			where SymbolEqualityComparer.Default.Equals(unboundAttribute, attributeSymbol)
			let typeArgs = a.TypeArguments
			where !typeArgs.IsDefaultOrEmpty
			let typeArg = typeArgs[0]
			let typeArgConverted = typeArg as INamedTypeSymbol
			where typeArgConverted is not null
			let typeArgStr = typeArgConverted.ToDisplayString(FormatOptions.PropertyTypeFormat)
			let arguments = attributeData.ConstructorArguments
			where !arguments.IsDefaultOrEmpty
			let argStrs = from arg in arguments[0].Values select ((string)arg.Value!).Trim()
			let n = attributeData.TryGetNamedArgument(nameof(AutoDeconstructExtensionAttribute<object>.Namespace), out var na) ? ((string)na.Value!).Trim() : null
			group (TypeArgument: typeArgConverted, Members: argStrs, Namespace: n) by typeArgStr)
		{
			var (typeArg, argStrs, n) = groupedResult.First();
			string namespaceResult = n ?? typeArg.ContainingNamespace.ToDisplayString();
			string typeResult = typeArg.Name;
			string deconstructionMethodsCode = string.Join("\r\n\r\n\t", q());
			context.AddSource(
				typeArg.ToFileName(),
				GeneratedFileShortcuts.ExtensionDeconstructionMethod,
				$@"#pragma warning disable CS0618

#nullable enable

namespace {namespaceResult};

/// <summary>
/// Provides the extension methods on this type.
/// </summary>
public static class {typeResult}_DeconstructionMethods
{{
	{deconstructionMethodsCode}
}}
"
			);


			IEnumerable<string> q()
			{
				foreach (var (typeArgument, arguments, namedArgumentNamespace) in groupedResult)
				{
					typeArgument.DeconstructInfo(
						false, out _, out _, out _, out string genericParameterListWithoutConstraint,
						out _, out _, out _
					);
					string fullTypeNameWithoutConstraint = typeArgument.ToDisplayString(FormatOptions.TypeFormat);
					string constraint = fullTypeNameWithoutConstraint.IndexOf("where") is var index and not -1
						? fullTypeNameWithoutConstraint.Substring(index)
						: string.Empty;
					string inModifier = typeArgument.TypeKind == TypeKind.Struct ? "in " : string.Empty;
					string parameterList = string.Join(
						", ",
						from member in arguments
						let memberFound = typeArgument.GetAllMembers().FirstOrDefault(m => m.Name == member)
						where memberFound is not null
						let memberType = memberFound.GetMemberType()
						where memberType is not null
						select $@"out {memberType} {member.ToCamelCase()}"
					);
					string assignments = string.Join(
						"\r\n\t\t",
						from member in arguments select $"{member.ToCamelCase()} = @this.{member};"
					);
					yield return $@"/// <summary>
	/// Deconstruct the instance to multiple elements.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct{genericParameterListWithoutConstraint}(this {inModifier}{fullTypeNameWithoutConstraint} @this, {parameterList}){constraint}
	{{
		{assignments}
	}}";
				}
			}
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}
}
