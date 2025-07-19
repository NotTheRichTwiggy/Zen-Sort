# Zen Sort

**Zen Sort** is a Windows Forms desktop application that helps you automatically organize files in a selected folder into subfolders, according to a strategy you choose (by category, extension, creation date, or first letter). The app also watches the folder in real-time and keeps things tidy as new files arrive—so your download or desktop folder stays organized *automatically*.

## Features

- **Organize by Strategy:**  
  - **Extension:** Group files by their file extension (e.g., `.pdf`, `.mp3`, `.jpg`).
  - **Category:** Use smart categories like Images, Documents, Audio, Code, etc.
  - **Creation Date:** Organize by the month and year the file was created.
  - **First Letter:** Sort files by the first letter of their filename.

- **Real-Time Monitoring:**  
  Automatically detects and organizes new or changed files as soon as they appear in the folder.

- **Preview Panel:**  
  See a live preview of how files will be organized before anything is moved.

- **Smart Exclusions:**  
  Files with the `.crdownload` or `.tmp` extensions are ignored (to avoid moving partial/incomplete downloads).

- **Safe Moves:**  
  Never overwrites existing files—renames to avoid collisions.

- **Minimize to System Tray:**  
  Hide the app but keep it running in the background. Restore anytime from the tray icon.

- **Simple Modern UI:**  
  No external dependencies; runs on .NET 6/7 or .NET Core 3.1+.

## Getting Started

### Prerequisites

- Windows 10/11
- [.NET 6 SDK or later](https://dotnet.microsoft.com/download)
- Visual Studio 2022 (recommended, for easy design editing)

### Installation

1. **Clone or Download** this repository.
2. **Open** `Zen Sort.sln` in Visual Studio.
3. **Build** the project (`Build > Build Solution`).
4. **Run** the app (`Debug > Start Debugging` or `F5`).

### Portable Run

You can also build a release and run `Zen Sort.exe` directly on any compatible Windows machine with .NET 6+ installed.

## Usage

1. **Launch** Zen Sort.
2. **Click** `Browse…` and select the folder you want to organize (e.g., your Downloads folder).
3. **Choose a strategy** from the dropdown (e.g., By Category).
4. **Preview**: The main window lists how files will be sorted.
5. **Let it run**: Zen Sort watches your folder and organizes new files automatically.
6. **Minimize** the app to the tray to keep it running in the background.

**Note:**  
- The app will not move files with `.crdownload` or `.tmp` extensions.
- If a file already exists in a target subfolder, Zen Sort will add a number (e.g., `file (1).pdf`) to avoid overwriting.

## Folder Organization Strategies

| Strategy        | Description                                                    |
|-----------------|----------------------------------------------------------------|
| By Extension    | Sorts files into folders by extension (e.g., `PDF/`, `MP3/`)   |
| By Category     | Uses built-in categories: Images, Documents, Audio, Code, etc. |
| By Creation Date| Folders named by year and month (e.g., `2025-07/`)             |
| By First Letter | A-Z folders by first character of filename                     |

## System Tray Support

- Click **Minimize** to send the app to the tray.
- **Double-click** the Zen Sort icon in the system tray to restore the window.
- Right-click the tray icon for restore and exit options.

## Limitations

- Only files in the *selected* folder (not subfolders) are organized.
- Excluded: Partial/incomplete files (`.crdownload`, `.tmp`).
- Moves files—does not copy them.
- Designed for personal/home use, not enterprise deployment.

## License

Copyright (c) 2025
Andrew Forrest
Torrens University — For educational use only.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Credits

- **Author:** Andrew Forrest  
- Windows Forms, .NET 6/7  
- Inspired by the need to keep messy download folders under control!

## Contact

For questions or suggestions, open an issue or email the author.

---

*Happy organizing!*
