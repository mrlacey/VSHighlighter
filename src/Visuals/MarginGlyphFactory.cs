using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace VSHighlighter.Visuals;

internal class MarginGlyphFactory : IGlyphFactory
{
	const double m_glyphSize = 10.0;

	public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
	{
		// Ensure we can draw a glyph for this marker.
		if (tag != null && tag is MarginTag marginTag)
		{
			System.Windows.Shapes.Ellipse ellipse = new()
			{
				Fill = marginTag.Color.ToBrush(),
				Height = m_glyphSize,
				Width = m_glyphSize,
				Tag = marginTag.HighlightId,
			};

			ellipse.MouseRightButtonDown += OnMouseRightButtonDown;

			return ellipse;
		}
		else
		{
			return null;
		}
	}

#pragma warning disable VSTHRD100 // Avoid async void methods
	private async void OnMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
	{
		try
		{
			await HighlighterService.Instance.RemoveHighlightAsync(((System.Windows.Shapes.Ellipse)sender).Tag.ToString());
		}
		catch (System.Exception exc)
		{
			await OutputPane.Instance.WriteAsync(exc.Message);
			await OutputPane.Instance.WriteAsync(exc.StackTrace);
			await OutputPane.Instance.ActivateAsync();
		}
	}
}
