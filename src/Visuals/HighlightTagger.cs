using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace VSHighlighter.Visuals;

public class HighlightTagger : ITagger<VsHighlightTag>
{
	private ITextView _textView;
	private ITextBuffer _buffer;

	Dictionary<ITrackingSpan, (string id, HighlightColor color)> _trackingSpans = [];

	public HighlightTagger(ITextView textView, ITextBuffer buffer)
	{
		_textView = textView;
		_buffer = buffer;
		//_documentationAddedListener = new DelegateListener<DocumentationAddedEvent>(OnDocumentationAdded);
		//_eventAggregator.AddListener<DocumentationAddedEvent>(_documentationAddedListener);
		//_documentSavedListener = new DelegateListener<DocumentSavedEvent>(OnDocumentSaved);
		//_eventAggregator.AddListener<DocumentSavedEvent>(_documentSavedListener);

		//_documentatiClosedListener = new DelegateListener<DocumentClosedEvent>(OnDocumentatClosed);

		//_eventAggregator.AddListener<DocumentClosedEvent>(_documentatiClosedListener);
		//_documentationUpdatedListener = new DelegateListener<DocumentationUpdatedEvent>(OnDocumentationUpdated);
		//_eventAggregator.AddListener<DocumentationUpdatedEvent>(_documentationUpdatedListener);
		//_documentationDeletedListener = new DelegateListener<DocumentationDeletedEvent>(OnDocumentationDeleted);
		//_eventAggregator.AddListener<DocumentationDeletedEvent>(_documentationDeletedListener);


		//TODO: Add event aggregator listener to documentation changed on tracking Span X
		//TODO: Add event aggregator listener to documentation deleted on tracking Span X

		CreateTrackingSpans();
	}

	//private void OnDocumentatClosed(DocumentClosedEvent obj)
	//{
	//	if (obj.DocumentFullName == _filename)
	//	{
	//		_eventAggregator.RemoveListener(_documentationAddedListener);
	//		_eventAggregator.RemoveListener(_documentSavedListener);
	//		_eventAggregator.RemoveListener(_documentationUpdatedListener);
	//		_eventAggregator.RemoveListener(_documentatiClosedListener);
	//	}

	//}

	private void CreateTrackingSpans()
	{
		_buffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument document);
		var fileName = document?.FilePath;

		if (string.IsNullOrEmpty(fileName))
		{
			return;
		}

		var highlights = HighlighterService.Instance.GetHighlights(fileName);

		System.Diagnostics.Debug.WriteLine($"Found {highlights.Count()} highlights in file.");

		foreach (var highlight in highlights)
		{
			var newTs = _buffer.CurrentSnapshot.CreateTrackingSpan(highlight.SpanStart, highlight.SpanLength, SpanTrackingMode.EdgeExclusive);

			_trackingSpans.Add(newTs, (highlight.Id, highlight.Color));
		}
	}

	//private void OnDocumentSaved(DocumentSavedEvent documentSavedEvent)
	//{
	//	if (documentSavedEvent.DocumentFullName == _filename)
	//	{
	//		RemoveEmptyTrackingSpans();
	//		FileDocumentation fileDocumentation = CreateFileDocumentationFromTrackingSpans();
	//		DocumentationFileSerializer.Serialize(CodyDocsFilename, fileDocumentation);
	//	}
	//}

	//private void OnDocumentationUpdated(DocumentationUpdatedEvent ev)
	//{
	//	if (_trackingSpans.ContainsKey(ev.TrackingSpan))
	//	{
	//		_trackingSpans[ev.TrackingSpan] = ev.NewDocumentation;
	//		TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
	//			new SnapshotSpan(_buffer.CurrentSnapshot, ev.TrackingSpan.GetSpan(_buffer.CurrentSnapshot))));
	//		MarkDocumentAsUnsaved();
	//	}
	//}

	//private void OnDocumentationDeleted(DocumentationDeletedEvent ev)
	//{
	//	if (_trackingSpans.ContainsKey(ev.Tag.TrackingSpan))
	//	{
	//		_trackingSpans.Remove(ev.Tag.TrackingSpan);
	//		TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
	//			new SnapshotSpan(_buffer.CurrentSnapshot, ev.Tag.TrackingSpan.GetSpan(_buffer.CurrentSnapshot))));
	//		MarkDocumentAsUnsaved();
	//	}
	//}

	private void MarkDocumentAsUnsaved()
	{
		//Document document = DocumentLifetimeManager.GetDocument(_filename);
		//if (document != null)
		//	document.Saved = false;
	}

	private void RemoveEmptyTrackingSpans()
	{
		var currentSnapshot = _buffer.CurrentSnapshot;
		var keysToRemove = _trackingSpans.Keys.Where(ts => ts.GetSpan(currentSnapshot).Length == 0).ToList();
		foreach (var key in keysToRemove)
		{
			_trackingSpans.Remove(key);
		}
	}

	//private FileDocumentation CreateFileDocumentationFromTrackingSpans()
	//{
	//	var currentSnapshot = _buffer.CurrentSnapshot;
	//	List<DocumentationFragment> fragments = _trackingSpans
	//		.Select(ts => new DocumentationFragment()
	//		{
	//			Selection = new TextViewSelection()
	//			{
	//				StartPosition = ts.Key.GetStartPoint(currentSnapshot),
	//				EndPosition = ts.Key.GetEndPoint(currentSnapshot),
	//				Text = ts.Key.GetText(currentSnapshot)
	//			},
	//			Documentation = ts.Value,

	//		}).ToList();

	//	var fileDocumentation = new FileDocumentation() { Fragments = fragments };
	//	return fileDocumentation;
	//}

	//private void OnDocumentationAdded(DocumentationAddedEvent e)
	//{

	//	string filepath = e.Filepath;
	//	if (filepath == CodyDocsFilename)
	//	{
	//		var span = e.DocumentationFragment.GetSpan();
	//		var trackingSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
	//		_trackingSpans.Add(trackingSpan, e.DocumentationFragment.Documentation);
	//		TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(
	//			new SnapshotSpan(_buffer.CurrentSnapshot, span)));
	//		MarkDocumentAsUnsaved();
	//	}
	//}

	public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

	public IEnumerable<ITagSpan<VsHighlightTag>> GetTags(NormalizedSnapshotSpanCollection spans)
	{
		List<ITagSpan<VsHighlightTag>> tags = new List<ITagSpan<VsHighlightTag>>();
		if (spans.Count == 0)
			return tags;

		var relevantSnapshot = spans.First().Snapshot;//_buffer.CurrentSnapshot;

		foreach (var trackingSpan in _trackingSpans)
		{
			var spanInCurrentSnapshot = trackingSpan.Key.GetSpan(relevantSnapshot);

			if (spans.Any(sp => spanInCurrentSnapshot.IntersectsWith(sp)))
			{
				var snapshotSpan = new SnapshotSpan(relevantSnapshot, spanInCurrentSnapshot);

				tags.Add(new TagSpan<VsHighlightTag>(snapshotSpan, GetTagForColor(trackingSpan.Value.color)));
			}
		}

		return tags;
	}

	private VsHighlightTag GetTagForColor(HighlightColor color)
	{
		switch (color)
		{
			case HighlightColor.DarkTurquoise:
				return new TurquoiseHighlightTag();

			case HighlightColor.Fuchsia:
				return new FuchsiaHighlightTag();

			case HighlightColor.Gold:
				return new GoldHighlightTag();

			case HighlightColor.Lime:
				return new LimeHighlightTag();

			default:
				break;
		}

		return null;
	}
}
