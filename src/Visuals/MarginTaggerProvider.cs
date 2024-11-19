using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace VSHighlighter.Visuals;

[Export(typeof(ITaggerProvider))]
[ContentType(StandardContentTypeNames.Text)]
[TagType(typeof(MarginTag))]
class MarginTaggerProvider : ITaggerProvider
{
	[Import]
	internal IClassifierAggregatorService AggregatorService = null;

	[Import]
	internal ITextDocumentFactoryService docFactory = null;

	public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}

		return new MarginTagger(docFactory) as ITagger<T>;
	}
}
