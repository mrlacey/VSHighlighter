using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace VSHighlighter.Visuals;

[Export(typeof(IWpfTextViewMarginProvider))]
[Name(ScrollBarMargin.MarginName)]
[MarginContainer(PredefinedMarginNames.VerticalScrollBar)]
[Order(Before = PredefinedMarginNames.OverviewChangeTracking)]
[Order(Before = PredefinedMarginNames.OverviewError)]
[Order(Before = PredefinedMarginNames.OverviewMark)]
[Order(Before = PredefinedMarginNames.OverviewSourceImage)]
[ContentType(StandardContentTypeNames.Any)]
[TextViewRole(PredefinedTextViewRoles.Interactive)]
internal sealed class ScrollBarMarginFactory : IWpfTextViewMarginProvider
{
	[Import]
	internal ITextDocumentFactoryService docFactory = null;

	public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
	{
		ITextDocument document;
		if (docFactory.TryGetTextDocument(wpfTextViewHost.TextView.TextBuffer, out document))
		{
			string documentName = document.FilePath;
			return new ScrollBarMargin(marginContainer, documentName);
		}
		return null;
	}
}
