using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace VSHighlighter;

public class OutputPane
{
	private static Guid vshPaneGuid = new("ED20FE59-8757-4702-A9AE-6559427F577F");

	private static OutputPane instance;

	private readonly IVsOutputWindowPane pane;

	private OutputPane()
	{
		ThreadHelper.ThrowIfNotOnUIThread();

		if (ServiceProvider.GlobalProvider.GetService(typeof(SVsOutputWindow)) is IVsOutputWindow outWindow
			&& (ErrorHandler.Failed(outWindow.GetPane(ref vshPaneGuid, out this.pane)) || this.pane == null))
		{
			if (ErrorHandler.Failed(outWindow.CreatePane(ref vshPaneGuid, Vsix.Name, fInitVisible: 1, fClearWithSolution: 0)))
			{
				System.Diagnostics.Debug.WriteLine("Failed to create output pane.");
				return;
			}

			if (ErrorHandler.Failed(outWindow.GetPane(ref vshPaneGuid, out this.pane)) || (this.pane == null))
			{
				System.Diagnostics.Debug.WriteLine("Failed to get output pane.");
			}
		}
	}

	public static OutputPane Instance => instance ??= new OutputPane();

	public async Task ActivateAsync()
	{
		await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

		this.pane?.Activate();
	}

	public async Task WriteAsync(string message)
	{
		await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

		this.pane?.OutputStringThreadSafe($"{message}{Environment.NewLine}");
	}
}
