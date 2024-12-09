using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using VSHighlighter.Messaging;

namespace VSHighlighter.Visuals;

public class HighlightTagger : ITagger<VsHighlightTag>
{
	private ITextView _textView;
	private ITextBuffer _buffer;
	private string _fileName;

	Dictionary<ITrackingSpan, (string Id, HighlightColor Color)> _trackingSpans = [];

	public HighlightTagger(ITextView textView, ITextBuffer buffer, string fileName)
	{
		_textView = textView;
		_buffer = buffer;

		_fileName = fileName;

#pragma warning disable VSTHRD101 // Avoid unsupported async delegates
		WeakReferenceMessenger.Default.Register<RequestReloadHighlights>(this, OnReloadHighlightsRequested);

		WeakReferenceMessenger.Default.Register<DocumentSavedNotification>(this, async (r, msg) =>
		{
			if (msg.FileName == _fileName)
			{
				try
				{
					await RemoveEmptyTrackingSpansAsync();

					await UpdateSpanPositionsAsync();
				}
				catch (Exception exc)
				{
					await OutputPane.Instance.WriteAsync("Error handling file save for: " + msg.FileName);
					await OutputPane.Instance.WriteAsync(exc.Message);
					await OutputPane.Instance.WriteAsync(exc.StackTrace);
					await OutputPane.Instance.ActivateAsync();
				}
			}
		});
#pragma warning restore VSTHRD101 // Avoid unsupported async delegates

		CreateTrackingSpans();
	}

	// This is like CreateTrackingSpans but adds new ones and removes other ones
	private void OnReloadHighlightsRequested(object recipient, RequestReloadHighlights msg)
	{
		var highlights = HighlighterService.Instance.GetHighlights(_fileName);

		System.Diagnostics.Debug.WriteLine($"Found {highlights.Count()} highlights in '{_fileName}'.");

		bool changesMade = false;

		foreach (var highlight in highlights)
		{
			if (!_trackingSpans.Values.Any(v => v.Id == highlight.Id))
			{
				var newTs = _buffer.CurrentSnapshot.CreateTrackingSpan(highlight.SpanStart, highlight.SpanLength, SpanTrackingMode.EdgeExclusive);

				_trackingSpans.Add(newTs, (highlight.Id, highlight.Color));
				changesMade = true;
			}
		}

		IList<ITrackingSpan> _tsToRemove = [];

		foreach (var ts in _trackingSpans)
		{
			if (msg.WholeDocument || (ts.Key.GetSpan(_buffer.CurrentSnapshot).IntersectsWith(new SnapshotSpan(_buffer.CurrentSnapshot, new Span(msg.RangeStart, msg.RangeLength)))))
			{
				if (!highlights.Any(h => h.Id == ts.Value.Id))
				{
					_tsToRemove.Add(ts.Key);
					changesMade = true;
				}
			}
		}

		foreach (var item in _tsToRemove)
		{
			_trackingSpans.Remove(item);
		}

		if (changesMade)
		{
			TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_buffer.CurrentSnapshot, new Span(msg.RangeStart, msg.RangeLength))));
		}
	}

	private void CreateTrackingSpans()
	{
		var highlights = HighlighterService.Instance.GetHighlights(_fileName);

		System.Diagnostics.Debug.WriteLine($"Found {highlights.Count()} highlights in file.");

		foreach (var highlight in highlights)
		{
			var newTs = _buffer.CurrentSnapshot.CreateTrackingSpan(highlight.SpanStart, highlight.SpanLength, SpanTrackingMode.EdgeExclusive);

			_trackingSpans.Add(newTs, (highlight.Id, highlight.Color));
		}
	}

	private async Task RemoveEmptyTrackingSpansAsync()
	{
		var currentSnapshot = _buffer.CurrentSnapshot;
		var keysToRemove = _trackingSpans.Keys.Where(ts => ts.GetSpan(currentSnapshot).Length == 0).ToList();
		foreach (var key in keysToRemove)
		{
			_trackingSpans.Remove(key);
			await HighlighterService.Instance.RemoveHighlightAsync(_trackingSpans[key].Id);
		}
	}

	private async Task UpdateSpanPositionsAsync()
	{
		var currentSnapshot = _buffer.CurrentSnapshot;

		foreach (var trackingSpan in _trackingSpans)
		{
			var span = trackingSpan.Key.GetSpan(currentSnapshot);
			var (start, length) = HighlighterService.Instance.GetHighlightSpan(_fileName, trackingSpan.Value.Id);

			if (start != span.Start || length != span.Length)
			{
				await HighlighterService.Instance.UpdateHighlightAsync(_fileName, trackingSpan.Value.Id, span.Start.Position, span.Length);
			}
		}
	}

	public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

	public IEnumerable<ITagSpan<VsHighlightTag>> GetTags(NormalizedSnapshotSpanCollection spans)
	{
		List<ITagSpan<VsHighlightTag>> tags = [];

		if (spans.Count == 0)
		{
			return tags;
		}

		var relevantSnapshot = spans.First().Snapshot;

		foreach (var trackingSpan in _trackingSpans)
		{
			var spanInCurrentSnapshot = trackingSpan.Key.GetSpan(relevantSnapshot);

			if (spans.Any(sp => spanInCurrentSnapshot.IntersectsWith(sp)))
			{
				var snapshotSpan = new SnapshotSpan(relevantSnapshot, spanInCurrentSnapshot);

				tags.Add(new TagSpan<VsHighlightTag>(snapshotSpan, GetTagForColor(trackingSpan.Value.Color)));
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
