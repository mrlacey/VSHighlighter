using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Formatting;
using System.Windows.Shapes;
using System.ComponentModel.Composition;

namespace VSHighlighter;

internal class Highlight
{
	public string FileName { get; set; }
	public int SpanStart { get; set; }
	public int SpanLength { get; set; }
	public HighlightColor Color { get; set; }
	public string Content { get; set; }
}
