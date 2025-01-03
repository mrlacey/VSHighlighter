using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace VSHighlighter.Visuals;

[Export]
[Export(typeof(IViewTaggerProvider))]
[ContentType(StandardContentTypeNames.Any)]
[TagType(typeof(VsHighlightTag))]
public class HighlightTaggerProvider : IViewTaggerProvider
{
	[ImportingConstructor]
	public HighlightTaggerProvider()
	{
	}

	public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
	{
		// Only provide highlighting on the top-level buffer
		if (textView.TextBuffer != buffer)
			return null;

		buffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument document);
		
		if (string.IsNullOrWhiteSpace(document?.FilePath ?? string.Empty))
		{
			return null;
		}

		return buffer.Properties.GetOrCreateSingletonProperty(
			nameof(HighlightTaggerProvider),
			() => new HighlightTagger(textView, buffer, document.FilePath) as ITagger<T>);
	}
}
