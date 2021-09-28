﻿namespace Microsoft.CodeAnalysis;

/// <summary>
/// Provides extension methods on <see cref="GeneratorExecutionContext"/>.
/// </summary>
/// <seealso cref="GeneratorExecutionContext"/>
internal static class GeneratorExecutionContextExtensions
{
	/// <summary>
	/// To check whether the context isn't executed on the specified projects.
	/// </summary>
	/// <param name="this">The context.</param>
	/// <param name="projectNames">The project names to check.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNotInProject(this in GeneratorExecutionContext @this, params string[] projectNames)
	{
		string? assemblyName = @this.Compilation.AssemblyName;
		return !Array.Exists(projectNames, projectName => assemblyName == projectName);
	}

	/// <summary>
	/// Simply calls <see cref="GeneratorExecutionContext.AddSource(string, string)"/>,
	/// </summary>
	/// <param name="this">The current context.</param>
	/// <param name="fileName">The file name. The file name may be same as the symbol name</param>
	/// <param name="sourceGeneratorName">The source generator name.</param>
	/// <param name="sourceCode">The source code.</param>
	/// <seealso cref="GeneratorExecutionContext.AddSource(string, string)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AddSource(
		this ref GeneratorExecutionContext @this, string fileName,
		string? sourceGeneratorName, string sourceCode) =>
		@this.AddSource(
			$"{fileName}{(sourceGeneratorName is null ? string.Empty : $".g.{sourceGeneratorName}")}.cs",
			SourceText.From(sourceCode, Encoding.UTF8)
		);

	/// <summary>
	/// Simply calls <see cref="GeneratorExecutionContext.AddSource(string, SourceText)"/>,
	/// </summary>
	/// <param name="this">The current context.</param>
	/// <param name="fileName">The file name. The file name may be same as the symbol name</param>
	/// <param name="sourceGeneratorName">The source generator name.</param>
	/// <param name="sourceCode">The source code.</param>
	/// <param name="encoding">The encoding.</param>
	/// <seealso cref="GeneratorExecutionContext.AddSource(string, SourceText)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AddSource(
		this ref GeneratorExecutionContext @this, string fileName,
		string? sourceGeneratorName, string sourceCode, Encoding encoding) =>
		@this.AddSource(
			$"{fileName}{(sourceGeneratorName is null ? string.Empty : $".g.{sourceGeneratorName}")}.cs",
			SourceText.From(sourceCode, encoding)
		);
}
