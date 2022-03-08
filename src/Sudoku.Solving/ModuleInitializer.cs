﻿using System.Linq.Expressions;
using System.Reflection;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Buffer;
using Sudoku.Solving.Manual.Searchers;

namespace Sudoku.Solving;

/// <include
///     file='../../global-doc-comments.xml'
///     path='g/csharp9/feature[@name="module-initializer"]/target[@name="type"]' />
internal static class ModuleInitializer
{
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp9/feature[@name="module-initializer"]/target[@name="method"]' />
	[ModuleInitializer]
	public static void Initialize() =>
		StepSearcherPool.Collection = (
			from type in typeof(ModuleInitializer).Assembly.GetTypes()
			where type.GetCustomAttribute<StepSearcherAttribute>() is { Deprecated: false }
			let constructors = type.GetConstructors()
			where constructors.Any(static c => c.GetParameters().Length == 0)
			select Activator.CreateInstance(type) as IStepSearcher into instance
			where instance is not null
			orderby instance.Options.Priority ascending
			select instance
		).ToArray();
}
