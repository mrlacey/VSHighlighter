using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace VSHighlighter.Visuals;

[Export(typeof(IWpfTextViewCreationListener))]
[ContentType(StandardContentTypeNames.Any)]
[TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
internal sealed class HighlightAdornmentTextViewCreationListener : IWpfTextViewCreationListener
{
	[Export(typeof(AdornmentLayerDefinition))]
	[Name("HighlightAdornment")]
	[Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
#pragma warning disable 414 // unused field
	private AdornmentLayerDefinition editorAdornmentLayer = null;
#pragma warning restore 414

	[Import]
	internal ITextDocumentFactoryService docFactory = null;

	public void TextViewCreated(IWpfTextView textView)
	{
		new HighlightAdornment(textView, docFactory);
	}
}
