# VS Highlighter

![Works with Visual Studio 2022](https://img.shields.io/static/v1.svg?label=VS&message=2022&color=A853C7)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

[![Build](https://github.com/mrlacey/VSHighlighter/actions/workflows/build.yaml/badge.svg)](https://github.com/mrlacey/VSHighlighter/actions/workflows/build.yaml)
![Tests](https://gist.githubusercontent.com/mrlacey/c586ff0f495b4a8dd76ab0dbdf9c89e0/raw/VSHighlighter.badge.svg)

---

This is an _experimental_ Visual Studio extension that enables the arbitrary highlighting of lines in the editor.

Highlighted lines are remembered and automatically shown when the document is open.

![A document shown with multiple highlights](art/example-light.png)

## Add a highlight

Add a highlight by right-clicking on a line in the editor and selecting a color from the context menu.

![Context menu showing highlight options](art/contextmenu-dark.png)]

Or by clicking a color on the Toolbar:

![Toolbar showing highlight options](art/toolbar-dark.png)

The selected text is highlighted (including multiple lines) or the current line is highlighted if no selection is made.

## Remove highlights

Remove an individual highlight by right-clicking on the circle in the margin.

Or, put the cursor within a highlight and select the "Clear highlight" option in the Highlighting context menu.

Or click the "X" in the toolbar.

It is possible to remove all highlights from the document by selecting the "Clear All in document" option in the context menu.
