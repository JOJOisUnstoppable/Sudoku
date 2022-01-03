﻿namespace Nano;

/// <summary>
/// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
/// </summary>
public partial class App : Application
{
	/// <summary>
	/// Indicates the main window.
	/// </summary>
	private Window _window = null!;


	/// <summary>
	/// <para>Initializes the singleton application object.</para>
	/// <para>
	/// This is the first line of authored code
	/// executed, and as such is the logical equivalent of <c>main()</c> or <c>WinMain()</c>.
	/// </para>
	/// </summary>
	public App() => InitializeComponent();


	/// <summary>
	/// Invoked when the application is launched normally by the end user.
	/// Other entry points will be used such as when the application is launched to open a specific file.
	/// </summary>
	/// <param name="args">Details about the launch request and process.</param>
	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		_window = new MainWindow();
		_window.Activate();
	}
}