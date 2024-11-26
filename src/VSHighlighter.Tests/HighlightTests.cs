using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VSHighlighter.Tests;

[TestClass]
public class HighlightTests
{
	[TestMethod]
	public void Intersects_Not_NegativeStart()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsFalse(sut.Intersects(-2, 20));
	}

	[TestMethod]
	public void Intersects_Not_NegativeLength()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsFalse(sut.Intersects(12, -1));
	}

	[TestMethod]
	public void Intersects_Not_BeforeSpan_ZeroLength()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsFalse(sut.Intersects(0, 0));
	}

	[TestMethod]
	public void Intersects_Not_BeforeSpan_LengthOfOne()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsFalse(sut.Intersects(2, 1));
	}

	[TestMethod]
	public void Intersects_Not_AfterSpan_LengthZero()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsFalse(sut.Intersects(20, 0));
	}

	[TestMethod]
	public void Intersects_Not_AfterSpan_LengthOfOne()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsFalse(sut.Intersects(20, 1));
	}

	[TestMethod]
	public void Intersects_True_WithinSpan_ZeroLength()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsTrue(sut.Intersects(12, 0));
	}

	[TestMethod]
	public void Intersects_True_WithinSpan_LengthOfOne()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsTrue(sut.Intersects(12, 1));
	}

	[TestMethod]
	public void Intersects_True_WithinSpan_FullSize()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsTrue(sut.Intersects(10, 5));
	}

	[TestMethod]
	public void Intersects_True_StartOfSpan_ZeroLength()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsTrue(sut.Intersects(10, 0));
	}

	[TestMethod]
	public void Intersects_True_StartOfSpan_LengthOfOne()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsTrue(sut.Intersects(10, 1));
	}

	[TestMethod]
	public void Intersects_True_EndOfSpan_ZeroLength()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsTrue(sut.Intersects(15, 0));
	}

	[TestMethod]
	public void Intersects_True_EndOfSpan_LengthOfOne()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsTrue(sut.Intersects(11, 1));
	}

	[TestMethod]
	public void Intersects_True_AcrossStartOfSpan()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsTrue(sut.Intersects(8, 5));
	}

	[TestMethod]
	public void Intersects_True_AcrossEndOfSpan()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsTrue(sut.Intersects(12, 8));
	}

	[TestMethod]
	public void Intersects_True_AcrossWholeOfSpan()
	{
		var sut = new Highlight
		{
			SpanStart = 10,
			SpanLength = 5
		};

		Assert.IsTrue(sut.Intersects(5, 20));
	}

	#region Copilot created tests - do not trust their accuracy or completeness!

	[TestMethod]
	public void Intersects_WhenStartIsWithinSpan_ReturnsTrue()
	{
		var highlight = new Highlight { SpanStart = 10, SpanLength = 5 };
		bool result = highlight.Intersects(12, 2);
		Assert.IsTrue(result);
	}

	[TestMethod]
	public void Intersects_WhenEndIsWithinSpan_ReturnsTrue()
	{
		var highlight = new Highlight { SpanStart = 10, SpanLength = 5 };
		bool result = highlight.Intersects(8, 3);
		Assert.IsTrue(result);
	}

	[TestMethod]
	public void Intersects_WhenSpanIsOutside_ReturnsFalse()
	{
		var highlight = new Highlight { SpanStart = 10, SpanLength = 5 };
		bool result = highlight.Intersects(0, 5);
		Assert.IsFalse(result);
	}

	[TestMethod]
	public void Intersects_WhenSpanIsExactlyTheSame_ReturnsTrue()
	{
		var highlight = new Highlight { SpanStart = 10, SpanLength = 5 };
		bool result = highlight.Intersects(10, 5);
		Assert.IsTrue(result);
	}

	[TestMethod]
	public void Intersects_WhenSpanIsCompletelyInside_ReturnsTrue()
	{
		var highlight = new Highlight { SpanStart = 10, SpanLength = 5 };
		bool result = highlight.Intersects(11, 3);
		Assert.IsTrue(result);
	}
	#endregion
}
