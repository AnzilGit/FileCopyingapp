using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FileCopyDesktopApp
{
    public partial class MainForm : Form
    {
        private List<string> filesToCopy = new List<string>
        {
          
        };
        private List<string> filesReady = new List<string>();
        private string[] _args;
        public MainForm(string[] args)
        {
            InitializeComponent();
            _args = args;
            InitDestTect();
        }

        private void InitDestTect()
        {
            string targetFolder = @"C:\Program Files\";
            txtTarget.Text = targetFolder;
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtSource.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnBrowseTarget_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtTarget.Text = dialog.SelectedPath;
                    UpdateFileList();
                }
            }
        }

        private void UpdateFileList()
        {
            string targetFolder = txtTarget.Text;
            if (string.IsNullOrWhiteSpace(targetFolder) || !Directory.Exists(targetFolder))
            {
                return;
            }

            for (int i = 0; i < filesToCopy.Count; i++)
            {
                string targetFilePath = Path.Combine(targetFolder, filesToCopy[i]);
                checkedListBoxFiles.SetItemChecked(i, File.Exists(targetFilePath));
            }
        }

        private void btnCopyFiles_Click(object sender, EventArgs e)
        {
            string sourceFolder = txtSource.Text;
            string targetFolder = txtTarget.Text;

            if (string.IsNullOrWhiteSpace(sourceFolder) || string.IsNullOrWhiteSpace(targetFolder))
            {
                MessageBox.Show("Please specify both source and target folders.");
                return;
            }

            if (!Directory.Exists(sourceFolder) || !Directory.Exists(targetFolder))
            {
                MessageBox.Show("Source or Target folder does not exist.");
                return;
            }

            try
            {
                foreach (var item in checkedListBoxFiles.CheckedItems)
                {
                    string fileName = item.ToString();
                    // Do something with fileName
                    Console.WriteLine("Checked: " + fileName);

                    string sourceFile = Path.Combine(sourceFolder, fileName);
                    string destFile = Path.Combine(targetFolder, fileName);

                    if (File.Exists(sourceFile))
                    {
                        File.Copy(sourceFile, destFile, true);                      
                    }

                }

                MessageBox.Show("Files copied successfully!");
                UpdateFileList();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access denied. Please run as administrator.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sourceFolder = txtSource.Text;

            if (string.IsNullOrWhiteSpace(sourceFolder))
            {
                MessageBox.Show("Please specify both source and target folders.");
                return;
            }
            if (!Directory.Exists(sourceFolder))
            {
                MessageBox.Show("Source or Target folder does not exist.");
                return;
            }
            try
            {
                var files = Directory.GetFiles(sourceFolder);
                checkedListBoxFiles.Items.Clear();

                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    if (filesToCopy.Contains(fileName))
                    {
                        checkedListBoxFiles.Items.Add(fileName, true);
                    }
                    else {
                        checkedListBoxFiles.Items.Add(fileName, false);
                    }
                      
                }

                MessageBox.Show($"Loaded {files.Length} file(s) from source folder.");
            }
            catch (Exception eX)
            {
                MessageBox.Show("file was unable to be checked");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (_args != null && _args.Length > 0)
            {
                string firstPath = _args[0];

                if (File.Exists(firstPath))
                {
                    // If it's a file, get its directory
                    txtSource.Text = Path.GetDirectoryName(firstPath);
                }
                else if (Directory.Exists(firstPath))
                {
                    // If it's already a directory
                    txtSource.Text = firstPath;
                }
                else
                {
                    MessageBox.Show("Invalid path received from Send To.");
                }
            }
        }
    }
}
