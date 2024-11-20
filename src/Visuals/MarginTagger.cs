using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace VSHighlighter.Visuals;

internal class MarginTagger : ITagger<MarginTag>
{
	private readonly ITextDocumentFactoryService docFactory;
	private ITextBuffer textBuffer;

#pragma warning disable 67 // unused event - but required by interface
	public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

	internal MarginTagger(ITextDocumentFactoryService docFactory, ITextBuffer textBuffer)
	{
		this.docFactory = docFactory;
		this.textBuffer = textBuffer;

		Messenger.ReloadHighlights += OnReloadHighlightsRequested;
	}

	private void OnReloadHighlightsRequested()
	{
		if (textBuffer.CurrentSnapshot != null)
		{
			var snapshot = textBuffer.CurrentSnapshot;
			var span = new SnapshotSpan(snapshot, 0, snapshot.Length);
			TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
		}
	}

	IEnumerable<ITagSpan<MarginTag>> ITagger<MarginTag>.GetTags(NormalizedSnapshotSpanCollection spans)
	{
		foreach (SnapshotSpan span in spans)
		{
			if (docFactory.TryGetTextDocument(span.Snapshot.TextBuffer, out ITextDocument document))
			{
				var highlights = HighlighterService.Instance.GetHighlights(document.FilePath);

				foreach (var highlight in highlights)
				{
					var highlightSpan = new SnapshotSpan(span.Snapshot, new Span(highlight.SpanStart, highlight.SpanLength));
					if (highlightSpan.IntersectsWith(span))
					{
						yield return new TagSpan<MarginTag>(highlightSpan, new MarginTag { Color = highlight.Color });
					}
				}
			}
		}
	}
}
