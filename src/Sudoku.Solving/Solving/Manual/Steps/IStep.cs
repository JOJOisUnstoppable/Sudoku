﻿namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides a basic manual solving step.
/// </summary>
public interface IStep
{
	/// <summary>
	/// <para>
	/// Indicates whether the difficulty rating of this technique should be
	/// shown in the output screen. Some techniques such as <b>Gurth's symmetrical placement</b>
	/// doesn't need to show the difficulty (because the difficulty of this technique
	/// is unstable).
	/// </para>
	/// <para>
	/// If the value is <see langword="true"/>, the analysis result won't show the difficulty
	/// of this instance.
	/// </para>
	/// <para>The default value is <see langword="true"/>.</para>
	/// </summary>
	bool ShowDifficulty { get; }

	/// <summary>
	/// <para>Indicates whether the step is an EST (i.e. Elementary Sudoku Technique) step.</para>
	/// <para>
	/// Here we define that the techniques often appearing and commonly to be used as below are ESTs:
	/// <list type="bullet">
	/// <item>Full House, Last Digit, Hidden Single, Naked Single</item>
	/// <item>Pointing, Claiming</item>
	/// <item>Naked Pair, Naked Triple, Naked Quarduple</item>
	/// <item>Naked Pair (+), Naked Triple (+), Naked Quarduple (+)</item>
	/// <item>Hidden Pair, Hidden Triple, Hidden Quarduple</item>
	/// <item>Locked Pair, Locked Triple</item>
	/// </list>
	/// </para>
	/// <para>The default value of this property is <see langword="false"/>.</para>
	/// </summary>
	bool IsElementary { get; }

	/// <summary>
	/// Indicates the technique name. The default value is in the resource dictionary.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Gets the format of the current instance.
	/// </summary>
	/// <returns>
	/// Returns a <see cref="string"/> result. If the resource dictionary doesn't contain
	/// any valid formats here, the result value will be <see langword="null"/>.
	/// </returns>
	/// <remarks>
	/// <para>
	/// A <b>format</b> is the better way to format the result text of this technique information instance,
	/// It'll be represented by the normal characters and the placeholders, e.g.
	/// <code><![CDATA["{Name}: Cells {CellsStr} => {ElimsStr}"]]></code>
	/// Here the string result <b>shouldn't</b> be with the leading <c>'$'</c> character, because this is a
	/// format string, rather than a interpolated string.
	/// </para>
	/// <para>
	/// Here the property <c>Name</c>, <c>CellsStr</c> and <c>ElimsStr</c> should be implemented before
	/// the property invoked, you should creates those 3 properties, returns the corresponding correct string
	/// result, makes them <see langword="private"/> or <see langword="protected"/> and marks the attribute
	/// <see cref="FormatItemAttribute"/> to help the code analyzer (if the code analyzer is available).
	/// The recommended implementation pattern is:
	/// <code><![CDATA[
	/// [FormatItem]
	/// private string CellsStr
	/// {
	///     [MethodImpl(MethodImplOptions.AggressiveInlining)]
	///	    get => Cells.ToString();
	/// }
	/// ]]></code>
	/// You can use the code snippet <c>fitem</c> to create the pattern, whose corresponding file is added
	/// into the <c>required/vssnippets</c> folder. For more information, please open the markdown file
	/// <see href="#">README.md</see> in the <c>required</c> folder to learn more information.
	/// </para>
	/// <para>
	/// Because this property will get the value from the resource dictionary, the property supports
	/// multiple language switching, which is better than the normal methods <see cref="ToString"/>
	/// and <see cref="ToFullString"/>. Therefore, this property is the substitution plan of those two methods.
	/// </para>
	/// </remarks>
	/// <seealso cref="ToString"/>
	/// <seealso cref="ToFullString"/>
	string? Format { get; }

	/// <summary>
	/// The difficulty or this step.
	/// </summary>
	decimal Difficulty { get; }

	/// <summary>
	/// The technique code of this instance used for comparison
	/// (e.g. search for specified puzzle that contains this technique).
	/// </summary>
	Technique TechniqueCode { get; }

	/// <summary>
	/// The technique tags of this instance.
	/// </summary>
	TechniqueTags TechniqueTags { get; }

	/// <summary>
	/// The technique group that this technique instance belongs to.
	/// </summary>
	TechniqueGroup TechniqueGroup { get; }

	/// <summary>
	/// The difficulty level of this step.
	/// </summary>
	DifficultyLevel DifficultyLevel { get; }

	/// <summary>
	/// Indicates the stableness of this technique. The default value is <see cref="Stableness.Stable"/>.
	/// </summary>
	/// <seealso cref="Stableness.Stable"/>
	Stableness Stableness { get; }

	/// <summary>
	/// Indicates the rarity of this technique appears.
	/// </summary>
	Rarity Rarity { get; }

	/// <summary>
	/// Indicates the conclusions that the step can be eliminated or assigned to.
	/// </summary>
	ImmutableArray<Conclusion> Conclusions { get; }

	/// <summary>
	/// Indicates the views of the step that may be displayed onto the screen using pictures.
	/// </summary>
	ImmutableArray<PresentationData> Views { get; }

	/// <summary>
	/// Indicates the string representation of the conclusions.
	/// </summary>
	/// <remarks>
	/// Most of techniques uses eliminations
	/// so this property is named <c>ElimStr</c>. In other words, if the conclusion is an assignment one,
	/// the property will still use this name rather than <c>AssignmentStr</c>.
	/// </remarks>
	protected abstract string ElimStr { get; }


	/// <summary>
	/// Put this instance into the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	void ApplyTo(ref Grid grid);

	/// <summary>
	/// Determine whether the current step information instance with the specified flags.
	/// </summary>
	/// <param name="flags">
	/// The flags. If the argument contains more than one set bit, all flags will be checked
	/// one by one.
	/// </param>
	/// <returns>A <see cref="bool"/> result.</returns>
	bool HasTag(TechniqueTags flags);

	/// <summary>
	/// Returns a string that only contains the name and the basic information.
	/// </summary>
	/// <returns>The string instance.</returns>
	/// <remarks>
	/// This method uses <see langword="sealed"/> and <see langword="override"/> modifiers
	/// to prevent the compiler overriding the method; in additional, the default behavior is changed to
	/// output as the method <see cref="Formatize(bool)"/> invoking.
	/// </remarks>
	/// <seealso cref="Formatize(bool)"/>
	string ToString();

	/// <summary>
	/// Returns a string that only contains the name and the conclusions.
	/// </summary>
	/// <returns>The string instance.</returns>
	string ToSimpleString();

	/// <summary>
	/// Returns a string that contains the name, the conclusions and its all details.
	/// This method is used for displaying details in text box control.
	/// </summary>
	/// <returns>The string instance.</returns>
	string ToFullString();

	/// <summary>
	/// Formatizes the <see cref="Format"/> property string and output the result.
	/// </summary>
	/// <param name="handleEscaping">Indicates whether the method will handle the escaping characters.</param>
	/// <returns>The result string.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the format is invalid. The possible cases are:
	/// <list type="bullet">
	/// <item>The format is <see langword="null"/>.</item>
	/// <item>The interpolation part contains the empty value.</item>
	/// <item>Missing the closed brace character <c>'}'</c>.</item>
	/// <item>The number of interpolations failed to match.</item>
	/// </list>
	/// </exception>
	/// <seealso cref="Format"/>
	string Formatize(bool handleEscaping = false);
}
