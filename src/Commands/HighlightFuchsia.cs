using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace VSHighlighter.Commands;

internal sealed class HighlightFuchsia : BaseHighlightCommand
{
	private HighlightFuchsia(AsyncPackage package, OleMenuCommandService commandService)
	{
		this.Color = HighlightColor.Fuchsia;

		this.package = package ?? throw new ArgumentNullException(nameof(package));
		commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

		var menuCommandID = new CommandID(PackageGuids.guidVSHighlighterPackageCmdSet, PackageIds.HighlightFuchsiaId);
		var menuItem = new MenuCommand(this.Execute, menuCommandID);
		commandService.AddCommand(menuItem);
	}

	public static HighlightFuchsia Instance { get; private set; }

	public static async Task InitializeAsync(AsyncPackage package)
	{
		await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

		OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
		Instance = new HighlightFuchsia(package, commandService);
	}

	private void Execute(object sender, EventArgs e) => this.BaseExecute(sender, e);
}
