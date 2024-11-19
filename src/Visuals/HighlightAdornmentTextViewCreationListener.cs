using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace VSHighlighter.Visuals;

[Export(typeof(IWpfTextViewCreationListener))]
[ContentType(StandardContentTypeNames.Text)]
[TextViewRole(PredefinedTextViewRoles.Document)]
internal sealed class HighlightAdornmentTextViewCreationListener : IWpfTextViewCreationListener
{
	[Export(typeof(AdornmentLayerDefinition))]
	[Name("HighlightAdornment")]
	[Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
	private AdornmentLayerDefinition editorAdornmentLayer;

	[Import]
	internal ITextDocumentFactoryService docFactory = null;

	public void TextViewCreated(IWpfTextView textView)
	{
		new HighlightAdornment(textView, docFactory);
	}
}
