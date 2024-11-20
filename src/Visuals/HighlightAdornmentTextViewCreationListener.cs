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
#pragma warning disable IDE0044 // unused field
#pragma warning disable 414 // unused field
#pragma warning disable IDE0051 // Remove unused private members
	private AdornmentLayerDefinition editorAdornmentLayer = null;
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore 414
#pragma warning restore IDE0044

	[Import]
	internal ITextDocumentFactoryService docFactory = null;

	public void TextViewCreated(IWpfTextView textView)
	{
		new HighlightAdornment(textView, docFactory);
	}
}
