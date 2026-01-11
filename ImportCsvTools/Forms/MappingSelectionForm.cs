using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ImportCsvTools;

namespace ImportCsvTools.Forms
{
    /// <summary>
    /// Form for selecting an existing mapping or creating a new one
    /// </summary>
    public partial class MappingSelectionForm : Form
    {
        private string _mappingDirectory;
        private List<MappingFileInfo> _mappingFiles;

        public string SelectedMappingFile { get; private set; }
        public bool CreateNewMapping { get; private set; }
        public bool EditMapping { get; private set; }
        public List<CsvImportColumnInfo> CsvColumns { get; set; }

        public MappingSelectionForm()
        {
            InitializeComponent();
            InitializeMappingDirectory();
        }

        private void InitializeMappingDirectory()
        {
            // If Release build, use AppData; if Debug build, use project directory for easier access
#if DEBUG
            var projectDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            _mappingDirectory = Path.Combine(projectDir, "HSMAdvisorAppData", "CsvImportMappings");
#else
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _mappingDirectory = Path.Combine(appDataPath, "HSMAdvisor", "CSVImportMappings");
#endif
            if (!Directory.Exists(_mappingDirectory))
            {
                try
                {
                    Directory.CreateDirectory(_mappingDirectory);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Failed to create mapping directory:\n{ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            lblDirectory.Text = $"Directory: {_mappingDirectory}";
        }

        private void MappingSelectionForm_Load(object sender, EventArgs e)
        {
            LoadMappingFiles();
        }

        private void LoadMappingFiles()
        {
            _mappingFiles = new List<MappingFileInfo>();
            listBoxMappings.Items.Clear();

            if (!Directory.Exists(_mappingDirectory))
            {
                UpdateDetails(null);
                return;
            }

            try
            {
                var files = Directory.GetFiles(_mappingDirectory, "*.json");
                
                foreach (var file in files.OrderBy(f => Path.GetFileName(f)))
                {
                    try
                    {
                        var config = CsvMappingConfig.Load(file);
                        var info = new MappingFileInfo
                        {
                            FilePath = file,
                            FileName = Path.GetFileName(file),
                            Config = config
                        };
                        
                        // Calculate compatibility with current CSV columns
                        info.CalculateCompatibility(CsvColumns);
                        
                        _mappingFiles.Add(info);
                        listBoxMappings.Items.Add(info);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        // Skip invalid files
                    }
                }

                if (listBoxMappings.Items.Count > 0)
                {
                    listBoxMappings.SelectedIndex = 0;
                }
                else
                {
                    UpdateDetails(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading mapping files:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void listBoxMappings_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = listBoxMappings.SelectedItem as MappingFileInfo;
            UpdateDetails(selected);
        }

        private void UpdateDetails(MappingFileInfo info)
        {
            if (info == null)
            {
                lblLibraryName.Text = "Library Name: N/A";
                lblHasHeaders.Text = "First Row Has Headers: N/A";
                lblMappingCount.Text = "Number of Mappings: 0";
                btnImport.Enabled = false;
                btnEdit.Enabled = false;
                return;
            }

            lblLibraryName.Text = $"Library Name: {info.Config.LibraryName ?? "(not set)"}";
            lblMappingCount.Text = $"Number of Mappings: {info.Config.Mappings?.Count ?? 0}";
            btnImport.Enabled = true;
            btnEdit.Enabled = true;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var selected = listBoxMappings.SelectedItem as MappingFileInfo;
            if (selected == null)
            {
                MessageBox.Show(
                    "Please select a mapping file.",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            SelectedMappingFile = selected.FilePath;
            CreateNewMapping = false;
            EditMapping = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var selected = listBoxMappings.SelectedItem as MappingFileInfo;
            if (selected == null)
            {
                MessageBox.Show(
                    "Please select a mapping file to edit.",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            using (var editorForm = new MappingEditorForm())
            {
                editorForm.CsvColumns = CsvColumns;
                editorForm.LoadMapping(selected.FilePath);
                if (editorForm.ShowDialog() == DialogResult.OK)
                {
                    // Reload the list to reflect any changes
                    LoadMappingFiles();
                }
            }
        }

        private void btnCreateNew_Click(object sender, EventArgs e)
        {
            using (var editorForm = new MappingEditorForm())
            {
                editorForm.MappingDirectory = _mappingDirectory;
                editorForm.CsvColumns = CsvColumns;
                if (editorForm.ShowDialog() == DialogResult.OK)
                {
                    // Reload the list to show the new mapping
                    LoadMappingFiles();
                    
                    // Select the newly created mapping
                    var newFileName = Path.GetFileName(editorForm.CurrentMappingFile);
                    for (int i = 0; i < listBoxMappings.Items.Count; i++)
                    {
                        var item = listBoxMappings.Items[i] as MappingFileInfo;
                        if (item != null && item.FileName == newFileName)
                        {
                            listBoxMappings.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadMappingFiles();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void listBoxMappings_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxMappings.SelectedItem != null)
            {
                btnImport_Click(sender, e);
            }
        }

        private class MappingFileInfo
        {
            public string FilePath { get; set; }
            public string FileName { get; set; }
            public CsvMappingConfig Config { get; set; }
            public int CompatibilityPercentage { get; set; }

            public void CalculateCompatibility(List<CsvImportColumnInfo> csvColumns)
            {
                if (csvColumns == null || csvColumns.Count == 0)
                {
                    CompatibilityPercentage = -1; // Unknown compatibility
                    return;
                }

                if (Config?.Mappings == null || Config.Mappings.Count == 0)
                {
                    CompatibilityPercentage = 0;
                    return;
                }

                // Get the set of CSV column names from the current file
                var csvColumnNames = new HashSet<string>(
                    csvColumns.Select(c => c.ColumnName),
                    StringComparer.OrdinalIgnoreCase);

                // Count how many CSV columns are mapped in this mapping file
                var mappedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var mapping in Config.Mappings)
                {
                    if (!string.IsNullOrWhiteSpace(mapping.CsvColumn))
                    {
                        mappedColumnNames.Add(mapping.CsvColumn);
                    }
                }

                // Calculate how many of the current CSV columns exist in the mapping
                int matchedCount = csvColumnNames.Count(col => mappedColumnNames.Contains(col));

                // Calculate percentage
                CompatibilityPercentage = (int)Math.Round((double)matchedCount / csvColumnNames.Count * 100);
            }

            public override string ToString()
            {
                if (CompatibilityPercentage < 0)
                {
                    // Compatibility not calculated
                    return FileName;
                }
                else if (CompatibilityPercentage == 100)
                {
                    return $"{FileName} (100% compatible)";
                }
                else if (CompatibilityPercentage == 0)
                {
                    return $"{FileName} (Not compatible)";
                }
                else
                {
                    return $"{FileName} ({CompatibilityPercentage}% compatible)";
                }
            }
        }
    }
}
