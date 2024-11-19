using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace VSHighlighter.Visuals;

internal sealed class HighlightAdornment
{
	private readonly IAdornmentLayer layer;

	private readonly IWpfTextView view;
	private readonly ITextDocumentFactoryService docFactory;

	private readonly Brush darkTurquoiseBrush;
	private readonly Brush fuchsiaBrush;
	private readonly Brush goldBrush;
	private readonly Brush limeBrush;

	private readonly Pen pen;

	public HighlightAdornment(IWpfTextView view, ITextDocumentFactoryService docFactory)
	{
		if (view == null)
		{
			throw new ArgumentNullException("view");
		}

		this.layer = view.GetAdornmentLayer("HighlightAdornment");

		this.view = view;
		this.docFactory = docFactory;
		this.view.LayoutChanged += this.OnLayoutChanged;

		// Use semi-opaque to not hide underlying text
		byte fiftyPercent = 255 / 2;

		this.fuchsiaBrush = new SolidColorBrush(Color.FromArgb(fiftyPercent, Colors.Fuchsia.R, Colors.Fuchsia.G, Colors.Fuchsia.B));
		this.fuchsiaBrush.Freeze();

		this.darkTurquoiseBrush = new SolidColorBrush(Color.FromArgb(fiftyPercent, Colors.DarkTurquoise.R, Colors.DarkTurquoise.G, Colors.DarkTurquoise.B));
		this.darkTurquoiseBrush.Freeze();

		this.goldBrush = new SolidColorBrush(Color.FromArgb(fiftyPercent, Colors.Gold.R, Colors.Gold.G, Colors.Gold.B));
		this.goldBrush.Freeze();

		this.limeBrush = new SolidColorBrush(Color.FromArgb(fiftyPercent, Colors.Lime.R, Colors.Lime.G, Colors.Lime.B));
		this.limeBrush.Freeze();

		var penBrush = new SolidColorBrush(Colors.Transparent);
		penBrush.Freeze();
		this.pen = new Pen(penBrush, thickness: 0.5);
		this.pen.Freeze();
	}

	/// <summary>
	/// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
	/// </summary>
	/// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
	/// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
	/// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
	/// </remarks>
	/// <param name="sender">The event sender.</param>
	/// <param name="e">The event arguments.</param>
	internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
	{
		foreach (ITextViewLine line in e.NewOrReformattedLines)
		{
			this.CreateVisuals(line);
		}
	}

	private void CreateVisuals(ITextViewLine line)
	{
		IWpfTextViewLineCollection textViewLines = this.view.TextViewLines;

		if (docFactory.TryGetTextDocument(line.Snapshot.TextBuffer, out ITextDocument document))
		{
			var highlights = HighlighterService.Instance.GetHighlights(document.FilePath);

			var lineSpan = new Span(line.Start, line.Length);

			foreach (var highlight in highlights)
			{
				var highlightSpan = new SnapshotSpan(line.Snapshot, new Span(highlight.SpanStart, highlight.SpanLength));

				if (highlightSpan.IntersectsWith(lineSpan))
				{
					// TODO: need to handle highlights that don't cover the whole line.
					SnapshotSpan span = new(this.view.TextSnapshot, lineSpan);

					Geometry geometry = textViewLines.GetMarkerGeometry(span);
					if (geometry != null)
					{
						var brush = highlight.Color switch
						{
							HighlightColor.DarkTurquoise => this.darkTurquoiseBrush,
							HighlightColor.Fuchsia => this.fuchsiaBrush,
							HighlightColor.Gold => this.goldBrush,
							_ => this.limeBrush,
						};

						var drawing = new GeometryDrawing(brush, this.pen, geometry);
						drawing.Freeze();

						var drawingImage = new DrawingImage(drawing);
						drawingImage.Freeze();

						var image = new Image
						{
							Source = drawingImage,
						};

						// Align the image with the top of the bounds of the text geometry
						Canvas.SetLeft(image, geometry.Bounds.Left);
						Canvas.SetTop(image, geometry.Bounds.Top);

						this.layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
					}
				}
			}
		}
	}
}
