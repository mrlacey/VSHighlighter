using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace VSHighlighter.Commands;

internal sealed class HighlightLime : BaseHighlightCommand
{
	private HighlightLime(AsyncPackage package, OleMenuCommandService commandService)
	{
		this.Color = HighlightColor.Lime;
		this.package = package ?? throw new ArgumentNullException(nameof(package));
		commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

		var menuCommandID = new CommandID(PackageGuids.guidVSHighlighterPackageCmdSet, PackageIds.HighlightLimeId);
		var menuItem = new MenuCommand(this.Execute, menuCommandID);
		commandService.AddCommand(menuItem);
	}

	public static HighlightLime Instance { get; private set; }

	public static async Task InitializeAsync(AsyncPackage package)
	{
		await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

		OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
		Instance = new HighlightLime(package, commandService);
	}

	private void Execute(object sender, EventArgs e) => this.BaseExecute(sender, e);
}
