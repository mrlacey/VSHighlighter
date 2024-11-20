using System.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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
				Width = m_glyphSize
			};

			ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;

			return ellipse;
		}
		else
		{
			return null;
		}
	}

#pragma warning disable VSTHRD100 // Avoid async void methods
	private async void Ellipse_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
	{
		try
		{
			// TODO: remove relevant highlight

			await VSHighlighterPackage.EnsureInstanceLoadedAsync();

			// Show a message box to prove we were here
			VsShellUtilities.ShowMessageBox(
				VSHighlighterPackage.Instance,
				"ellipse clicked",
				"msgbox title",
				OLEMSGICON.OLEMSGICON_INFO,
				OLEMSGBUTTON.OLEMSGBUTTON_OK,
				OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
		}
		catch (System.Exception exc)
		{
			await OutputPane.Instance.WriteAsync(exc.Message);
		}
	}
}
