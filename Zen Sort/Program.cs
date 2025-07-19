// =====================================================================================
// ZenSortApp.cs
// -------------------------------------------------------------------------------------
// A Windows Forms application that lets the user choose a directory and then
// organises all files inside that directory into sub‐folders according to a selected
// strategy (extension, category, creation‐date, or first letter). Additionally, it watches the
// selected folder in real‐time and automatically organises new or changed files.
// Files with the extension ".crdownload" or ".tmp" are explicitly excluded and will not be moved.
// The app can also be minimized to the system tray using its own icon.
// Written for .NET 6/7 WinForms (or .NET Core 3.1+) in a single source file for learning.
// =====================================================================================

using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;  // for Icon

namespace FileOrganizerGUI
{
    // Entry point for the application
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // Enables visual styles for modern look-and-feel
            Application.EnableVisualStyles();
            // Ensures text rendering is compatible with visual styles
            Application.SetCompatibleTextRenderingDefault(false);
            // Runs the main form of the application
            Application.Run(new MainForm());
        }
    }

    // The main user interface form for folder selection, preview, and file watching
    public sealed class MainForm : Form
    {
        // UI controls and components
        private readonly TextBox _txtPath;          // Displays selected folder path
        private readonly Button _btnBrowse;         // Button to open folder browser dialog
        private readonly Button _btnMinimize;       // Button to minimize application to tray
        private readonly Button _btnAbout;          // Button to show version & author info
        private readonly ListView _lvPreview;       // Shows file preview and target folders
        private readonly ComboBox _cmbStrategy;     // Strategy selection for organizing files
        private readonly Label _lblStatus;          // Displays status messages
        private readonly FolderBrowserDialog _folderDialog; // Dialog to choose folders
        private readonly NotifyIcon _trayIcon;      // System tray icon for minimize/restore
        private FileSystemWatcher _watcher;         // Watches selected folder for changes

        public MainForm()
        {
            // Configure main form properties
            Text = "Zen Sort (Watch Mode)";
            Width = 820;
            Height = 640; // Extra height to accommodate minimize & about buttons
            StartPosition = FormStartPosition.CenterScreen;

            // Initialize and position the path TextBox
            _txtPath = new TextBox
            {
                Left = 10,
                Top = 10,
                Width = 500,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Initialize and position the Browse button
            _btnBrowse = new Button
            {
                Left = 520,
                Top = 8,
                Width = 80,
                Text = "Browse…",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Initialize and position the Minimize button
            _btnMinimize = new Button
            {
                Left = 610,
                Top = 8,
                Width = 80,
                Text = "Minimize",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Initialize and position the About button
            _btnAbout = new Button
            {
                Left = 700,
                Top = 8,
                Width = 80,
                Text = "About",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Set up ListView to preview files and their target folders
            _lvPreview = new ListView
            {
                Left = 10,
                Top = 40,
                Width = 780,
                Height = 520,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _lvPreview.Columns.Add("Filename", 500, HorizontalAlignment.Left);
            _lvPreview.Columns.Add("Target Folder", 260, HorizontalAlignment.Left);

            // ComboBox for choosing organization strategy
            _cmbStrategy = new ComboBox
            {
                Left = 10,
                Top = 570,
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            _cmbStrategy.Items.AddRange(new[]
            {
                "By Extension (default)",
                "By Category (Images, Videos, Documents, Audio, Archives, Code, Torrents, Executables)",
                "By Creation Date (YYYY-MM)",
                "By First Letter (A, B, …)"
            });
            _cmbStrategy.SelectedIndex = 1; // Default: category grouping

            // Status label to inform user of current state
            _lblStatus = new Label
            {
                Left = 370,
                Top = 575,
                Width = 420,
                Height = 15,
                Text = "Select a folder and strategy.",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // Add controls to the form
            Controls.AddRange(new Control[]
            {
                _txtPath,
                _btnBrowse,
                _btnMinimize,
                _btnAbout,
                _lvPreview,
                _cmbStrategy,
                _lblStatus
            });

            // Initialize folder browser dialog
            _folderDialog = new FolderBrowserDialog
            {
                Description = "Choose a folder to organise and watch",
                UseDescriptionForTitle = true
            };

            // Set up system tray icon for minimized state, using the same EXE icon
            _trayIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
                Text = "Zen Sort",
                Visible = false
            };
            var trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Restore", null, (s, e) => RestoreFromTray());
            trayMenu.Items.Add("Exit", null, (s, e) => Application.Exit());
            _trayIcon.ContextMenuStrip = trayMenu;
            _trayIcon.DoubleClick += (s, e) => RestoreFromTray();

            // Wire up event handlers
            _btnBrowse.Click += OnBrowseClicked;
            _btnMinimize.Click += (s, e) => MinimizeToTray();
            _btnAbout.Click += ShowAbout;
            _txtPath.TextChanged += (_, __) => RefreshPreview();
            _cmbStrategy.SelectedIndexChanged += (_, __) => RefreshPreview();
            Resize += (s, e) =>
            {
                if (WindowState == FormWindowState.Minimized)
                    MinimizeToTray();
            };
        }

        // Helper: returns true if file should be ignored
        private static bool IsIgnoredExtension(string filePath)
        {
            string ext = Path.GetExtension(filePath);
            return ext.Equals(".crdownload", StringComparison.InvariantCultureIgnoreCase)
                || ext.Equals(".tmp", StringComparison.InvariantCultureIgnoreCase);
        }

        // Handler for Browse button: opens dialog, sets path, and starts watching
        private void OnBrowseClicked(object sender, EventArgs e)
        {
            if (_folderDialog.ShowDialog(this) == DialogResult.OK)
            {
                string path = _folderDialog.SelectedPath;
                _txtPath.Text = path;       // Update path TextBox
                SetupWatcher(path);          // Begin watching folder
                OrganiseAllFiles(path);      // Immediately organise existing files
            }
        }

        // Hides the form and shows the tray icon
        private void MinimizeToTray()
        {
            Hide();
            _trayIcon.Visible = true;
        }

        // Restores the form from tray state
        private void RestoreFromTray()
        {
            Show();
            WindowState = FormWindowState.Normal;
            _trayIcon.Visible = false;
        }

        // Displays version and author information
        private void ShowAbout(object sender, EventArgs e)
        {
            string version = Application.ProductVersion;  // From assembly metadata
            const string author = "Andrew Forrest";       // ← Change to your name
            MessageBox.Show(
                $"Zen Sort\nVersion: {version}\nWritten by: {author}",
                "About Zen Sort",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        // Configures FileSystemWatcher for the selected path
        private void SetupWatcher(string path)
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
            }

            _watcher = new FileSystemWatcher(path)
            {
                Filter = "*.*",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };

            _watcher.Created += (s, e) => OnFileChanged(e.FullPath);
            _watcher.Renamed += (s, e) => OnFileChanged(e.FullPath);
        }

        // Called when a file is created or renamed: moves it and updates preview
        private void OnFileChanged(string fullPath)
        {
            try { System.Threading.Thread.Sleep(200); } catch { }
            if (File.Exists(fullPath))
            {
                OrganiseSingleFile(fullPath);
                Invoke((Action)RefreshPreview);
            }
        }

        // Organises all current files in the root directory
        private void OrganiseAllFiles(string root)
        {
            try
            {
                foreach (string file in Directory.GetFiles(root))
                    OrganiseSingleFile(file);
                RefreshPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error during organising",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Moves a single file to its destination based on strategy,
        // now with collision-safe renaming and .tmp/.crdownload exclusion.
        private void OrganiseSingleFile(string file)
        {
            if (IsIgnoredExtension(file))
                return;

            try
            {
                string root = _txtPath.Text;
                string destFolder = Path.Combine(root, GetTargetFolderFor(file));
                Directory.CreateDirectory(destFolder);

                // Original filename and initial destination path
                string originalName = Path.GetFileName(file);
                string destination = Path.Combine(destFolder, originalName);

                // If a file with that name already exists, generate a unique new name
                if (File.Exists(destination))
                {
                    string nameOnly = Path.GetFileNameWithoutExtension(originalName);
                    string extension = Path.GetExtension(originalName);
                    int copyCount = 1;
                    string newDest;

                    do
                    {
                        string newName = $"{nameOnly} ({copyCount}){extension}";
                        newDest = Path.Combine(destFolder, newName);
                        copyCount++;
                    }
                    while (File.Exists(newDest));

                    destination = newDest;
                }

                // Move the file (will never overwrite)
                File.Move(file, destination);
            }
            catch
            {
                // Silently ignore any errors during file move
            }
        }

        // Determines folder name for a file according to selected strategy
        private string GetTargetFolderFor(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            switch (_cmbStrategy.SelectedIndex)
            {
                case 0: // By Extension
                    return ext.Trim('.').ToUpperInvariant();

                case 1: // By Category
                    string[] images = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
                    string[] videos = { ".mp4", ".mov", ".avi", ".mkv", ".wmv", ".m4a" };
                    string[] docs = { ".doc", ".docx", ".pdf", ".txt", ".xls", ".xlsx", ".ppt", ".pptx" };
                    string[] audio = { ".mp3", ".wav", ".flac", ".aac", ".ogg" };
                    string[] archives = { ".zip", ".rar", ".7z", ".tar", ".gz" };
                    string[] code = { ".cs", ".js", ".py", ".java", ".html", ".css", ".cpp", ".json", ".xml" };
                    string[] torrents = { ".torrent" };
                    string[] executables = { ".exe", ".msi", ".bat", ".cmd" };

                    if (images.Contains(ext)) return "Images";
                    if (videos.Contains(ext)) return "Videos";
                    if (docs.Contains(ext)) return "Documents";
                    if (audio.Contains(ext)) return "Audio";
                    if (archives.Contains(ext)) return "Archives";
                    if (code.Contains(ext)) return "Code";
                    if (torrents.Contains(ext)) return "Torrents";
                    if (executables.Contains(ext)) return "Executables";
                    return "Others";

                case 2: // By Creation Date
                    DateTime dt = File.GetCreationTime(filePath);
                    return dt.ToString("yyyy-MM");

                case 3: // By First Letter
                    return Path.GetFileName(filePath)
                               .First()
                               .ToString()
                               .ToUpperInvariant();

                default:
                    return "Misc";
            }
        }

        // Updates the preview ListView based on current files in directory
        private void RefreshPreview()
        {
            _lvPreview.BeginUpdate();
            _lvPreview.Items.Clear();

            string root = _txtPath.Text;
            if (Directory.Exists(root))
            {
                foreach (string file in Directory.GetFiles(root))
                {
                    if (IsIgnoredExtension(file))
                        continue;

                    string target = GetTargetFolderFor(file);
                    _lvPreview.Items.Add(new ListViewItem(new[]
                    {
                        Path.GetFileName(file),
                        target
                    }));
                }

                _lblStatus.Text = $"{_lvPreview.Items.Count} file(s) in folder.";
            }
            else
            {
                _lblStatus.Text = "Select a folder to watch and organise.";
            }

            _lvPreview.EndUpdate();
        }
    }
}
