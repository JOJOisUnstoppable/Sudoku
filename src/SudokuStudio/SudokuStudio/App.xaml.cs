﻿namespace SudokuStudio;

/// <summary>
/// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
/// </summary>
/// <seealso cref="Application"/>
public partial class App : Application
{
	/// <summary>
	/// Indicates the command-line arguments.
	/// </summary>
	private readonly string[] _commandLineArgs;


	/// <summary>
	/// <para>Initializes the singleton application object via command-line arguments.</para>
	/// <para>
	/// This is the first line of authored code executed, and as such is the logical equivalent of <c>main()</c> or <c>WinMain()</c>.
	/// </para>
	/// </summary>
	/// <param name="args">The command-line arguments.</param>
	public App(string[] args)
	{
		InitializeComponent();

		_commandLineArgs = args;
	}


	/// <summary>
	/// Indicates the program-reserved user preference.
	/// </summary>
	public ProgramPreference Preference { get; } = new();

	/// <summary>
	/// Indicates the program solver.
	/// </summary>
	public LogicalSolver ProgramSolver { get; } = new();

	/// <summary>
	/// Indicates the program step gatherer.
	/// </summary>
	public StepsGatherer ProgramGatherer { get; } = new();

	/// <summary>
	/// Indicates the first-opened grid.
	/// </summary>
	[DisallowNull]
	[DebuggerHidden]
	internal Grid? FirstGrid { get; set; }

	/// <summary>
	/// Indicates the window manager.
	/// </summary>
	internal ProjectWideWindowManager WindowManager { get; } = new();


	/// <summary>
	/// Indicates the assembly version.
	/// </summary>
	[DebuggerHidden]
	internal static Version AssemblyVersion => typeof(App).Assembly.GetName().Version!;


	/// <inheritdoc/>
	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		RegisterResourceFetching();
		HandleOnProgramOpeningEntryCase();
		ActivicateMainWindow();
		LoadConfigurationFileFromLocal();
	}

	/// <summary>
	/// Register resource-fetching service.
	/// </summary>
	private void RegisterResourceFetching() => MergedResources.R.RegisterAssembly(typeof(App).Assembly);

	/// <summary>
	/// Creates a window, and activicate it.
	/// </summary>
	private void ActivicateMainWindow() => WindowManager.CreateWindow<MainWindow>().Activate();

	/// <summary>
	/// Handle the cases how user opens this program.
	/// </summary>
	private void HandleOnProgramOpeningEntryCase()
	{
		if (_commandLineArgs is null or not [])
		{
			return;
		}

		if (AppInstance.GetCurrent().GetActivatedEventArgs() is not
			{
				Kind: ExtendedActivationKind.File,
				Data: IFileActivatedEventArgs { Files: [StorageFile { FileType: CommonFileExtensions.Text, Path: var filePath } file, ..] }
			})
		{
			return;
		}

		if (SudokuFileHandler.Read(filePath) is not [{ GridString: var gridStr }, ..] || !Grid.TryParse(gridStr, out var grid))
		{
			return;
		}

		FirstGrid = grid;
	}

	/// <summary>
	/// Loads configuration file from local path.
	/// </summary>
	private void LoadConfigurationFileFromLocal()
	{
		var targetPath = CommonPaths.UserPreference;
		if (File.Exists(targetPath) && ProgramPreferenceFileHandler.Read(targetPath) is { } loadedConfig)
		{
			Preference.CoverBy(loadedConfig);
		}
	}
}
