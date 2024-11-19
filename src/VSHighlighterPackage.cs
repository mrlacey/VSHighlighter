using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace VSHighlighter;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[Guid(VSHighlighterPackage.PackageGuidString)]
public sealed class VSHighlighterPackage : AsyncPackage
{
	public const string PackageGuidString = "bdb718d6-5369-48b4-9185-f27c969759b2";

	// TODO: Need to ensure package loaded before accessing this.
	public static AsyncPackage Instance;

	// TODO: Add buttons for each color
	// TODO: add menu options to highlight selection
	// TODO: add menu options to clear all highlights in the document

	protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
	{
		await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

		Instance = this;

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
}
