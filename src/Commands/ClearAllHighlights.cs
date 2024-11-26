using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace VSHighlighter.Commands;

internal sealed class ClearAllHighlights : BaseHighlightCommand
{
	private ClearAllHighlights(AsyncPackage package, OleMenuCommandService commandService)
	{
		this.package = package ?? throw new ArgumentNullException(nameof(package));
		commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

		var menuCommandID = new CommandID(PackageGuids.guidVSHighlighterPackageCmdSet, PackageIds.ClearAllHighlightsId);
		var menuItem = new MenuCommand(this.Execute, menuCommandID);
		commandService.AddCommand(menuItem);
	}

	public static ClearAllHighlights Instance { get; private set; }

	public static async Task InitializeAsync(AsyncPackage package)
	{
		await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

		OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
		Instance = new ClearAllHighlights(package, commandService);
	}

#pragma warning disable VSTHRD100 // Avoid async void methods
	private async void Execute(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
	{
		try
		{
			if (!(await this.ServiceProvider.GetServiceAsync(typeof(SVsTextManager)) is IVsTextManager textManager))
			{
				// If we fail to get the text manager, we can't get the active view.
				await OutputPane.Instance.WriteAsync("Unable to clear all highlights: failed to get the text manager.");
			}
			else
			{
				textManager.GetActiveView(1, null, out IVsTextView textView);

				if (textView == null)
				{
					// If we fail to get the active view, we can't get the WPF view.
					await OutputPane.Instance.WriteAsync("Unable to clear all highlights: failed to get the text view.");
				}
				else
				{
					if (await this.ServiceProvider.GetServiceAsync(typeof(SComponentModel)) is IComponentModel componentModel)
					{
						var provider = componentModel.GetService<IVsEditorAdaptersFactoryService>();

						var wpftextView = provider.GetWpfTextView(textView);

						// Get the file path of the document
						var textDocumentFactoryService = componentModel.GetService<ITextDocumentFactoryService>();
						if (textDocumentFactoryService.TryGetTextDocument(wpftextView.TextDataModel.DocumentBuffer, out ITextDocument textDocument))
						{
							string filePath = textDocument.FilePath;

							if (filePath is not null)
							{
								await HighlighterService.Instance.RemoveAllAsync(filePath);
							}
							else
							{
								await OutputPane.Instance.WriteAsync("Unable to clear all highlights: file path was unknown.");
							}
						}
						else
						{
							await OutputPane.Instance.WriteAsync("Unable to clear all highlights: failed to get the path of the file.");
						}
					}
					else
					{
						await OutputPane.Instance.WriteAsync("Unable to clear all highlights: failed to get the editor adapter.");
					}
				}
			}
		}
		catch (Exception exc)
		{
			await OutputPane.Instance.WriteAsync("Error in clear all highlights: " + exc.Message);
			await OutputPane.Instance.WriteAsync(exc.StackTrace);
			await OutputPane.Instance.ActivateAsync();
		}

	}
}
