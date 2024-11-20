using System;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;

namespace VSHighlighter.Commands;

internal class BaseHighlightCommand
{
	protected AsyncPackage package;

	protected Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => this.package;

	protected HighlightColor Color { get; set; }

#pragma warning disable VSTHRD100 // Avoid async void methods
	protected async void BaseExecute(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
	{
		try
		{
			if (!(await this.ServiceProvider.GetServiceAsync(typeof(SVsTextManager)) is IVsTextManager textManager))
			{
				// If we fail to get the text manager, we can't get the active view.
				await OutputPane.Instance.WriteAsync("Unable to highlight the selection: failed to get the text manager.");
			}
			else
			{
				textManager.GetActiveView(1, null, out IVsTextView textView);

				if (textView == null)
				{
					// If we fail to get the active view, we can't get the WPF view.
					await OutputPane.Instance.WriteAsync("Unable to highlight the selection: failed to get the text view.");
				}
				else
				{
					if (await this.ServiceProvider.GetServiceAsync(typeof(SComponentModel)) is IComponentModel componentModel)
					{
						var provider = componentModel.GetService<IVsEditorAdaptersFactoryService>();

						var wpftextView = provider.GetWpfTextView(textView);

						var selection = wpftextView.Selection;

						// TODO: review handling of multiple selections
						var selectionSpan = selection.SelectedSpans[0];

						// Get the file path of the document
						var textDocumentFactoryService = componentModel.GetService<ITextDocumentFactoryService>();
						if (textDocumentFactoryService.TryGetTextDocument(wpftextView.TextDataModel.DocumentBuffer, out ITextDocument textDocument))
						{
							string filePath = textDocument.FilePath;

							HighlighterService.Instance.AddHighlight(filePath, this.Color, selectionSpan.Start.Position, selectionSpan.Length);

							// TODO: review including the selection in the highlight record
							//textView.GetSelectedText(out var selText);
						}
						else
						{
							await OutputPane.Instance.WriteAsync("Unable to highlight the selection: failed to get the path of the file.");
						}
					}
					else
					{
						await OutputPane.Instance.WriteAsync("Unable to highlight the selection: failed to get the editor adapter.");
					}
				}
			}
		}
		catch (Exception exc)
		{
			await OutputPane.Instance.WriteAsync("Error highlighting text: " + exc.Message);
		}
	}
}
