﻿namespace System.Reflection;

/// <summary>
/// Provides with extension methods on <see cref="MemberInfo"/> instances.
/// </summary>
/// <seealso cref="MemberInfo"/>
public static class MemberInfoExtensions
{
	/// <summary>
	/// Gets the type arguments of the specified attribute type applied to the specified property.
	/// </summary>
	/// <typeparam name="TMemberInfo">The type of the member information.</typeparam>
	/// <param name="this">The <see cref="MemberInfo"/> instance.</param>
	/// <param name="genericAttributeType">The generic attribute type.</param>
	/// <returns>The types of the generic type arguments.</returns>
	public static Type[] GetGenericAttributeTypeArguments<TMemberInfo>(this TMemberInfo @this, Type genericAttributeType)
		where TMemberInfo : MemberInfo
	{
		if (genericAttributeType is not { IsGenericType: true, FullName: { } genericTypeName })
		{
			goto ReturnEmptyArray;
		}

		if (
			(
				from a in @this.GetCustomAttributes()
				where a.GetType() is { IsGenericType: var g, FullName: { } f } && g && p(genericTypeName) == p(f)
				select a
			).ToArray() is not [var attribute]
		)
		{
			goto ReturnEmptyArray;
		}

		return attribute.GetType().GenericTypeArguments;

	ReturnEmptyArray:
		return Array.Empty<Type>();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int p(string s) => s.IndexOf('`');
	}
}