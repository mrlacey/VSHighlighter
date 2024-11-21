using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSHighlighter.Commands;
using Task = System.Threading.Tasks.Task;

namespace VSHighlighter;

[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)]
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)] // Info on this package for Help/About
[Guid(PackageGuids.guidVSHighlighterPackageString)]
[ProvideMenuResource("Menus.ctmenu", 1)]
public sealed class VSHighlighterPackage : AsyncPackage
{
	public static AsyncPackage Instance;

	protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
	{
		await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

		Instance = this;

		await HighlightFuchsia.InitializeAsync(this);
		await HighlightGold.InitializeAsync(this);
		await HighlightTurquoise.InitializeAsync(this);
		await HighlightLime.InitializeAsync(this);
		await ClearHighlights.InitializeAsync(this);

		await TrackBasicUsageAnalyticsAsync();
	}

	private static async Task TrackBasicUsageAnalyticsAsync()
	{
		try
		{
#if !DEBUG
		if (string.IsNullOrWhiteSpace(AnalyticsConfig.TelemetryConnectionString))
		{
			return;
		}

		var config = new TelemetryConfiguration
		{
			ConnectionString = AnalyticsConfig.TelemetryConnectionString,
		};

		var client = new TelemetryClient(config);

		var properties = new Dictionary<string, string>
			{
				{ "VsixVersion", Vsix.Version },
				{ "VsVersion", Microsoft.VisualStudio.Telemetry.TelemetryService.DefaultSession?.GetSharedProperty("VS.Core.ExeVersion") },
			};

		client.TrackEvent(Vsix.Name, properties);
#endif
		}
		catch (Exception exc)
		{
			System.Diagnostics.Debug.WriteLine(exc);
			await OutputPane.Instance.WriteAsync("Error tracking usage analytics: " + exc.Message);
		}
	}

	internal static async Task EnsureInstanceLoadedAsync()
	{
		await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

		if (VSHighlighterPackage.Instance == null)
		{
			// Try and force load the project if it hasn't already loaded
			// so can access the configured options.
			if (ServiceProvider.GlobalProvider.GetService(typeof(SVsShell)) is IVsShell shell)
			{
				shell.LoadPackage(ref PackageGuids.guidVSHighlighterPackage, out _);
			}
		}
	}
}
