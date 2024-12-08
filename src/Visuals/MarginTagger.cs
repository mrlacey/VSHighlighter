using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace VSHighlighter.Visuals;

internal class MarginTagger : ITagger<MarginTag>
{
	private readonly ITextDocumentFactoryService docFactory;
	private readonly ITextBuffer textBuffer;
	private readonly string documentName;

#pragma warning disable 67 // unused event - but required by interface
	public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

	internal MarginTagger(ITextDocumentFactoryService docFactory, ITextBuffer textBuffer)
	{
		this.docFactory = docFactory;
		this.textBuffer = textBuffer;

		//this.textBuffer.Changed += (sender, args) => HandleTextBufferChanged(args);

		if (docFactory.TryGetTextDocument(textBuffer, out ITextDocument document))
		{
			documentName = document.FilePath;
		}

		WeakReferenceMessenger.Default.Register<RequestReloadHighlights>(this, OnReloadHighlightsRequested);
	}

	//private void HandleTextBufferChanged(TextContentChangedEventArgs args)
	//{
	//	System.Diagnostics.Debug.WriteLine("Text buffer changed");
	//	System.Diagnostics.Debug.WriteLine("Changes");
	//	System.Diagnostics.Debug.WriteLine(args.Changes.Count);
	//	if (args.Changes.Count > 0)
	//	{
	//		System.Diagnostics.Debug.WriteLine(args.Changes[0].ToString());
	//	}
	//	System.Diagnostics.Debug.WriteLine("Before");
	//	System.Diagnostics.Debug.WriteLine(args.Before);
	//	System.Diagnostics.Debug.WriteLine("After");
	//	System.Diagnostics.Debug.WriteLine(args.After);
	//}

	~MarginTagger()
	{
		WeakReferenceMessenger.Default.UnregisterAll(this);
	}

	private void OnReloadHighlightsRequested(object recipient, RequestReloadHighlights msg)
	{
		if (textBuffer.CurrentSnapshot != null && msg.FileName == this.documentName)
		{
			var snapshot = textBuffer.CurrentSnapshot;

			var span = msg.WholeDocument
				? new SnapshotSpan(snapshot, 0, snapshot.Length)
				: new SnapshotSpan(snapshot, msg.RangeStart, msg.RangeLength);

			TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
		}
	}

	IEnumerable<ITagSpan<MarginTag>> ITagger<MarginTag>.GetTags(NormalizedSnapshotSpanCollection spans)
	{
		foreach (SnapshotSpan span in spans)
		{
			var highlights = HighlighterService.Instance.GetHighlights(this.documentName);

			foreach (var highlight in highlights)
			{
				var len = span.Snapshot.Length;

				if (highlight.SpanStart >= len || (highlight.SpanStart + highlight.SpanLength) > len)
				{
					continue;
				}

				var highlightSpan = new SnapshotSpan(span.Snapshot, new Span(highlight.SpanStart, highlight.SpanLength));

				if (highlightSpan.IntersectsWith(span))
				{
					yield return new TagSpan<MarginTag>(highlightSpan, new MarginTag(highlight));
				}
			}
		}
	}
}
