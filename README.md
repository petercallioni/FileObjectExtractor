# File Object Extractor

**F**ile **O**bject E**X**tractor (FOX) is a powerful tool that makes it easy to view and extract embedded files from Microsoft Office documents such as DOCX, XLSX, and PPTX. Whether you need to extract documents, images, PDFs, binaries, or even content that’s embedded directly without an icon, FOX has you covered. **Please note: FOX is designed for 64-bit platforms only.**

## Overview

In many Office documents, files can be embedded within the document structure. FOX allows you to effortlessly locate and extract any embedded file. It supports a wide range of file types including:
- **Documents**
- **Images**
- **PDFs**
- **Binaries**
- **Directly embedded files** (content embedded without an icon)

## Features

- **Multi-Format Extraction**  
  Extract embedded files from:
  - Word Documents (DOCX, DOCM, DOTM, DOTX)
  - Excel Spreadsheets (XLSX, XLXM, XLTM, XLTX, XLW)
  - Powerpoint Presentations (PPTX, POTX, PPTM, PPSM, PPSX)

- **Graphical User Interface (GUI) Operation Mode**  
  For everyday users, FOX provides an intuitive GUI. Launch FOX from your Start Menu or desktop shortcut to easily navigate, preview, and extract embedded files using a visual approach.

- **Command-Line Interface (CLI) Operation Mode**  
  For advanced users and power users, FOX supports a CLI mode. Run FOX from the terminal and use the `-h` flag to view available commands and options.

- **Direct File Opening**  
  Open the embedded file immediately without needing to extract it first.

- **File Renaming Before Saving**  
  Choose a custom name for the extracted file.

- **Filtering**  
  Multiple options for filtering embedded files to quickly select all relevant files.

## Usage

### Graphical User Interface (GUI) Operation

The GUI mode is geared toward regular users who prefer a visual experience. To use the GUI:
- Launch FOX from the Start Menu or via a desktop shortcut.
- Use the intuitive menus and dialogs to open Office documents and view the embedded files.
- Select one or more files to preview, extract, or directly open them.

### Command-Line Interface (CLI) Operation

The CLI mode is intended for power users who favor terminal-based workflows. To view all available commands and options in CLI mode, run:

```bash
FileObjectExtractor -h