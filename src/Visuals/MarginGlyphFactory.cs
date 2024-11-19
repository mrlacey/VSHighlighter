using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Windows.Shapes;
using System.Windows;

namespace VSHighlighter.Visuals;

internal class MarginGlyphFactory : IGlyphFactory
{
	const double m_glyphSize = 10.0;

	public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
	{
		// Ensure we can draw a glyph for this marker.
		if (tag != null && tag is MarginTag marginTag)
		{
			System.Windows.Shapes.Ellipse ellipse = new Ellipse();
			ellipse.Fill = marginTag.Color.ToBrush();
			ellipse.Height = m_glyphSize;
			ellipse.Width = m_glyphSize;

			ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;

			return ellipse;
		}
		else
		{
			return null;
		}
	}

	private void Ellipse_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		// Show a message box to prove we were here
		VsShellUtilities.ShowMessageBox(
			VSHighlighterPackage.Instance,
			"ellipse clicked",
			"msgbox title",
			OLEMSGICON.OLEMSGICON_INFO,
			OLEMSGBUTTON.OLEMSGBUTTON_OK,
			OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
	}
}
