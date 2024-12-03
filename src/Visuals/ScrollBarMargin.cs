using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace VSHighlighter.Visuals;

internal class ScrollBarMargin : Canvas, IWpfTextViewMargin
{
	private bool _isDisposed;
	private static double _lineWidth = 200, _lineHeight = 5;
	private readonly IVerticalScrollBar _scrollBar;
	private readonly string _documentName;
	private bool _isDoingDelayRetry = false;

	public const string MarginName = "VsHighlighterScrollBarMargin";

	public ScrollBarMargin(IWpfTextViewMargin marginContainer, string documentName)
	{
		_scrollBar = marginContainer as IVerticalScrollBar;
		_documentName = documentName;

		// Prevent the control from intercepting clicks
		this.IsHitTestVisible = false;

		WeakReferenceMessenger.Default.Register<RequestReloadHighlights>(this, OnReloadHighlightsRequested);
	}

	~ScrollBarMargin()
	{
		WeakReferenceMessenger.Default.UnregisterAll(this);
	}

	public FrameworkElement VisualElement
	{
		get
		{
			ThrowIfDisposed();
			return this;
		}
	}

	public double MarginSize
	{
		get
		{
			ThrowIfDisposed();
			return ActualHeight;
		}
	}

	public bool Enabled
	{
		get
		{
			ThrowIfDisposed();
			return true;
		}
	}

	public ITextViewMargin GetTextViewMargin(string marginName)
	{
		return string.Equals(marginName, ScrollBarMargin.MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
	}

	public void Dispose()
	{
		if (!_isDisposed)
		{
			GC.SuppressFinalize(this);
			_isDisposed = true;
		}
	}

	private void ThrowIfDisposed()
	{
		if (_isDisposed)
		{
			throw new ObjectDisposedException(MarginName);
		}
	}

	protected override void OnRender(DrawingContext drawingContext)
	{
		base.OnRender(drawingContext);

		// This may be called before the scrollbar knows how big it is and so can calculate the Y-offset correctly
		if (((System.Windows.FrameworkElement)_scrollBar).ActualHeight > 0)
		{
			DrawMarkers(drawingContext);
		}
		else if (!_isDoingDelayRetry)
		{
			_isDoingDelayRetry = true;

			// If opening a solution with a document open that includes highlights,
			// we won't get an automatic render parse for this margin after the scrollbar is ready.
			// Add an artificial delay (to allow other layout & rendering a chance to finish) before retrying.
			// Not ideal, but it appears to work.
			_ = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
			{
				System.Diagnostics.Debug.WriteLine("Doing delay retry for scrollbar margin");

				await Task.Delay(500);
				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
				_isDoingDelayRetry = false;
				InvalidateVisual();
			});
		}
	}

	private void OnReloadHighlightsRequested(object recipient, RequestReloadHighlights msg)
	{
		if (msg.FileName == this._documentName)
		{
			InvalidateVisual();
		}
	}

	void DrawMarkers(DrawingContext drawingContext)
	{
		foreach (var highlight in HighlighterService.Instance.GetHighlights(_documentName))
		{
			if (highlight.LineNumber > -1)
			{
				double y = _scrollBar.GetYCoordinateOfScrollMapPosition(highlight.LineNumber);

				if (highlight.LineNumber > 1 && y < 1)
				{
					System.Diagnostics.Debug.WriteLine($"Scroll position not found for line: {highlight.LineNumber}, y: {y}");
					
					// Don't bother trying to draw something if not in the "correct" position.
					continue;
				}

				drawingContext.DrawRectangle(highlight.Color.ToBrush(), null, new Rect(0, y, _lineWidth, _lineHeight));
			}
		}
	}
}
