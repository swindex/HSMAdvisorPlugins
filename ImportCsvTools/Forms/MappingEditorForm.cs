using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ImportCsvTools;

namespace ImportCsvTools.Forms
{
    /// <summary>
    /// Form for creating and editing CSV mapping configurations
    /// </summary>
    public partial class MappingEditorForm : Form
    {
        private CsvMappingConfig _config;
        private List<string> _toolProperties;

        public string CurrentMappingFile { get; private set; }
        public string MappingDirectory { get; set; }
        public List<CsvImportColumnInfo> CsvColumns { get; set; }

        public MappingEditorForm()
        {
            InitializeComponent();
            InitializeData();
            
            // Wire up DataError event for graceful error handling
            dataGridView.DataError += dataGridView_DataError;
        }

        private void InitializeData()
        {
            _config = new CsvMappingConfig
            {
                LibraryName = "CSV Import",
                Mappings = new List<CsvMapping>()
            };

            _toolProperties = ReflectionHelpers.GetToolProperties();

            // Insert empty option at the beginning
            _toolProperties.Insert(0, string.Empty);
        }

        private void MappingEditorForm_Load(object sender, EventArgs e)
        {
            LoadConfigToUI();
            SetupDataGridView();
        }

        public void LoadMapping(string filePath)
        {
            try
            {
                _config = CsvMappingConfig.Load(filePath);
                CurrentMappingFile = filePath;
                this.Text = $"Edit Mapping - {Path.GetFileName(filePath)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load mapping file:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadConfigToUI()
        {
            txtLibraryName.Text = _config.LibraryName ?? "CSV Import";
            chkAllowInvalidToolImport.Checked = _config.AllowInvalidToolImport;
            
            // Load CSV Input Units setting
            if (string.Equals(_config.CsvInputUnits, "mm", StringComparison.OrdinalIgnoreCase))
            {
                radioButtonMillimeters.Checked = true;
            }
            else
            {
                radioButtonInches.Checked = true;
            }
        }

        private void SetupDataGridView()
        {
            // Setup combo box columns
            var toolFieldColumn = dataGridView.Columns["colToolField"] as DataGridViewComboBoxColumn;
            if (toolFieldColumn != null)
            {
                toolFieldColumn.DataSource = _toolProperties.ToList();
            }

            // Load existing mappings
            foreach (var mapping in _config.Mappings)
            {
                AddMappingToGrid(mapping);
            }

            // Populate missing CSV columns if provided
            PopulateMissingCsvColumns();

            UpdateRequiredFieldsLabel();
        }

        private void PopulateMissingCsvColumns()
        {
            // If no CSV columns provided, nothing to do
            if (CsvColumns == null || CsvColumns.Count == 0)
            {
                return;
            }

            // Get list of already mapped CSV columns
            var mappedColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow)
                    continue;

                var csvColumn = row.Cells["colCsvColumn"].Value?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(csvColumn))
                {
                    mappedColumns.Add(csvColumn);
                }
            }

            // Add missing CSV columns
            foreach (var columnInfo in CsvColumns)
            {
                if (!mappedColumns.Contains(columnInfo.ColumnName))
                {
                    var newMapping = new CsvMapping
                    {
                        CsvColumn = columnInfo.ColumnName,
                        ValueMap = new Dictionary<string, string>()
                    };
                    AddMappingToGrid(newMapping);
                }
            }
        }

        private void AddMappingToGrid(CsvMapping mapping)
        {
            var rowIndex = dataGridView.Rows.Add(
                mapping.CsvColumn,
                mapping.ToolField,
                mapping.DefaultValue,
                mapping.EnumType,
                string.Empty, // Expression - will be set properly below
                string.Empty, // ExportExpression - will be set properly below
                mapping.ValueMap != null && mapping.ValueMap.Count > 0 ? $"{mapping.ValueMap.Count} item(s)" : string.Empty
            );

            var row = dataGridView.Rows[rowIndex];

            // Store the actual ValueMap in the row's Tag
            row.Tag = mapping.ValueMap ?? new Dictionary<string, string>();

            // Set expressions properly (stores full in tag, displays truncated)
            SetExpression(row, mapping.Expression);
            SetExportExpression(row, mapping.ExportExpression);
        }

        private void btnAddMapping_Click(object sender, EventArgs e)
        {
            var newMapping = new CsvMapping
            {
                ValueMap = new Dictionary<string, string>()
            };
            AddMappingToGrid(newMapping);
        }

        private void btnRemoveMapping_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView.SelectedRows)
                {
                    if (!row.IsNewRow)
                    {
                        dataGridView.Rows.Remove(row);
                    }
                }
                UpdateRequiredFieldsLabel();
            }
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            // Handle Expression button click
            if (e.ColumnIndex == dataGridView.Columns["colExpression"].Index)
            {
                var row = dataGridView.Rows[e.RowIndex];
                var valueMap = row.Tag as Dictionary<string, string> ?? new Dictionary<string, string>();

                if (valueMap.Count > 0)
                {
                    var result = MessageBox.Show(
                        "Epressions are not used when a Value Map is defined",
                        "Expression Not Used",
                        MessageBoxButtons.OK);
                    return;
                }

                var expression = GetFullExpression(row);
                var csvColumnName = row.Cells["colCsvColumn"].Value?.ToString()?.Trim();

                using (var editor = new ExpressionEditorForm())
                {
                    editor.Expression = expression;
                    
                    // Find and pass the CSV column info with unique values
                    if (!string.IsNullOrWhiteSpace(csvColumnName) && CsvColumns != null)
                    {
                        var columnInfo = CsvColumns.FirstOrDefault(c => 
                            string.Equals(c.ColumnName, csvColumnName, StringComparison.OrdinalIgnoreCase));
                        
                        if (columnInfo != null)
                        {
                            editor.CsvColumnInfo = columnInfo;
                        }
                    }
                    
                    if (editor.ShowDialog() == DialogResult.OK)
                    {
                        SetExpression(row, editor.Expression);
                    }
                }
            }

            // Handle ExportExpression button click
            if (e.ColumnIndex == dataGridView.Columns["colExportExpression"].Index)
            {
                var row = dataGridView.Rows[e.RowIndex];
                var exportExpression = GetFullExportExpression(row);
                var csvColumnName = row.Cells["colCsvColumn"].Value?.ToString()?.Trim();

                using (var editor = new ExpressionEditorForm())
                {
                    editor.Expression = exportExpression;
                    editor.Text = "Edit Export Expression";
                    
                    // Note: For export expressions, we don't pass CSV column info
                    // because the expression operates on Tool field values, not CSV values
                    
                    if (editor.ShowDialog() == DialogResult.OK)
                    {
                        SetExportExpression(row, editor.Expression);
                    }
                }
            }

            // Handle ValueMap button click
            if (e.ColumnIndex == dataGridView.Columns["colValueMapButton"].Index)
            {
                var row = dataGridView.Rows[e.RowIndex];
                var valueMap = row.Tag as Dictionary<string, string> ?? new Dictionary<string, string>();
                var enumType = row.Cells["colEnumType"].Value?.ToString();
                var csvColumnName = row.Cells["colCsvColumn"].Value?.ToString()?.Trim();
                var cellExpression = row.Cells["colExpression"].Value?.ToString()?.Trim();

                using (var editor = new ValueMapEditorForm())
                {
                    // Pass the EnumType so the editor can show enum values if available
                    editor.EnumTypeName = enumType;
                    editor.ValueMap = new Dictionary<string, string>(valueMap);
                    
                    // Find and pass the CSV column info with unique values
                    if (!string.IsNullOrWhiteSpace(csvColumnName) && CsvColumns != null)
                    {
                        var columnInfo = CsvColumns.FirstOrDefault(c => 
                            string.Equals(c.ColumnName, csvColumnName, StringComparison.OrdinalIgnoreCase));
                        
                        /*if (!String.IsNullOrEmpty(cellExpression) && columnInfo.UniqueValues != null && columnInfo.UniqueValues.Count > 0)
                        {
                            // Create a copy of columnInfo
                            // Exaluate columnInfo.UniqueValues with cellExpression before sending to the mapping form
                            columnInfo = new CsvImportColumnInfo
                            {
                                ColumnName = columnInfo.ColumnName,
                                UniqueValues = columnInfo.UniqueValues.Select(value =>
                                {
                                    return ExpressionEvaluator.EvaluateExpression(cellExpression, value).ToString(); // Placeholder for evaluated value
                                }).ToList()
                            };
                        }*/

                        if (columnInfo != null)
                        {
                            editor.CsvColumnInfo = columnInfo;
                        }
                    }
                    
                    if (editor.ShowDialog() == DialogResult.OK)
                    {
                        row.Tag = editor.ValueMap;
                        row.Cells["colValueMapButton"].Value = editor.ValueMap.Count > 0 
                            ? $"{editor.ValueMap.Count} item(s)" 
                            : string.Empty;

                        if (editor.ValueMap.Count > 0)
                        {
                            // Clear expression if ValueMap is defined
                            SetExpression(row, string.Empty);
                        }
                    }
                }
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var row = dataGridView.Rows[e.RowIndex];

            // Auto-detect enum type when ToolField changes
            if (e.ColumnIndex == dataGridView.Columns["colToolField"].Index)
            {
                var toolField = row.Cells["colToolField"].Value?.ToString();
                if (!string.IsNullOrWhiteSpace(toolField))
                {
                    if (ReflectionHelpers.IsEnumProperty(toolField, out var enumTypeName))
                    {
                        row.Cells["colEnumType"].Value = enumTypeName;
                    }
                }

                UpdateRequiredFieldsLabel();
            }
        }

        private void UpdateRequiredFieldsLabel()
        {
            var mappings = GetMappingsFromGrid();
            var missing = ReflectionHelpers.ValidateRequiredFields(mappings);

            if (missing.Count == 0)
            {
                lblRequiredFields.Text = "✓ All required fields are mapped";
                lblRequiredFields.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lblRequiredFields.Text = $"⚠ Missing required fields: {string.Join(", ", missing)}";
                lblRequiredFields.ForeColor = System.Drawing.Color.Red;
            }
        }

        /// <summary>
        /// Gets the full expression stored in the row's tag
        /// </summary>
        private string GetFullExpression(DataGridViewRow row)
        {
            // Expression is stored in a special tag property to keep the full text
            if (row.Cells["colExpression"].Tag is string fullExpression)
            {
                return fullExpression;
            }
            return "";
            /*else
            {
                // Fallback to cell value if tag not set
                expression = row.Cells["colExpression"].Value?.ToString() ?? string.Empty;
            }

            if (expression == "Edit...")
            {
                expression = null;
            }
            return expression;*/
        }

        /// <summary>
        /// Sets the expression, storing full text in tag and displaying truncated version
        /// </summary>
        private void SetExpression(DataGridViewRow row, string expression)
        {
            // Store full expression in cell's tag
            row.Cells["colExpression"].Tag = expression;
            
            // Display truncated version in the button
            if (string.IsNullOrWhiteSpace(expression))
            {
                row.Cells["colExpression"].Value = "Edit...";
            }
            else
            {
                row.Cells["colExpression"].Value = TruncateExpression(expression);
            }
        }

        /// <summary>
        /// Gets the full export expression stored in the row's tag
        /// </summary>
        private string GetFullExportExpression(DataGridViewRow row)
        {
            // ExportExpression is stored in a special tag property to keep the full text
            if (row.Cells["colExportExpression"].Tag is string fullExportExpression)
            {
                return fullExportExpression;
            }
            return "";
        }

        /// <summary>
        /// Sets the export expression, storing full text in tag and displaying truncated version
        /// </summary>
        private void SetExportExpression(DataGridViewRow row, string exportExpression)
        {
            // Store full export expression in cell's tag
            row.Cells["colExportExpression"].Tag = exportExpression;
            
            // Display truncated version in the button
            if (string.IsNullOrWhiteSpace(exportExpression))
            {
                row.Cells["colExportExpression"].Value = "Edit...";
            }
            else
            {
                row.Cells["colExportExpression"].Value = TruncateExpression(exportExpression);
            }
        }

        /// <summary>
        /// Truncates expression for display
        /// </summary>
        private string TruncateExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return "Edit...";

            const int maxLength = 30;
            if (expression.Length <= maxLength)
                return expression;

            return expression.Substring(0, maxLength) + "...";
        }

        private List<CsvMapping> GetMappingsFromGrid()
        {
            var mappings = new List<CsvMapping>();

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow)
                    continue;

                var mapping = new CsvMapping
                {
                    CsvColumn = row.Cells["colCsvColumn"].Value?.ToString()?.Trim(),
                    ToolField = row.Cells["colToolField"].Value?.ToString()?.Trim(),
                    DefaultValue = row.Cells["colDefaultValue"].Value?.ToString()?.Trim(),
                    EnumType = row.Cells["colEnumType"].Value?.ToString()?.Trim(),
                    Expression = GetFullExpression(row), // Get full expression from tag
                    ExportExpression = GetFullExportExpression(row), // Get full export expression from tag
                    ValueMap = row.Tag as Dictionary<string, string> ?? new Dictionary<string, string>()
                };

                // Only add if at least CsvColumn or ToolField (could have a default value) is specified
                if (!string.IsNullOrWhiteSpace(mapping.CsvColumn) || !string.IsNullOrWhiteSpace(mapping.ToolField))
                {
                    mappings.Add(mapping);
                }
            }

            return mappings;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentMappingFile))
            {
                btnSaveAs_Click(sender, e);
                return;
            }

            SaveMapping(CurrentMappingFile);
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
                saveDialog.DefaultExt = "json";
                saveDialog.AddExtension = true;
                saveDialog.Title = "Save Mapping Configuration";

                if (!string.IsNullOrWhiteSpace(MappingDirectory))
                {
                    saveDialog.InitialDirectory = MappingDirectory;
                }
                else if (!string.IsNullOrWhiteSpace(CurrentMappingFile))
                {
                    saveDialog.InitialDirectory = Path.GetDirectoryName(CurrentMappingFile);
                    saveDialog.FileName = Path.GetFileName(CurrentMappingFile);
                }
                else
                {
                    saveDialog.FileName = "mapping.json";
                }

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveMapping(saveDialog.FileName);
                }
            }
        }

        private void SaveMapping(string filePath)
        {
            try
            {
                // Update config from UI
                _config.LibraryName = txtLibraryName.Text?.Trim();
                _config.AllowInvalidToolImport = chkAllowInvalidToolImport.Checked;
                _config.CsvInputUnits = radioButtonMillimeters.Checked ? "mm" : "in";
                _config.Mappings = GetMappingsFromGrid();

                // Validate required fields
                var missing = ReflectionHelpers.ValidateRequiredFields(_config.Mappings);
                if (missing.Count > 0)
                {
                    var result = MessageBox.Show(
                        $"The following required fields are not mapped:\n{string.Join(", ", missing)}\n\nDo you want to save anyway?",
                        "Missing Required Fields",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.Yes)
                        return;
                }

                // Cleanup expressions from "Edit..."
                foreach (var map in _config.Mappings)
                {
                    if (map.Expression=="Edit...")
                    {
                        map.Expression = null;
                    }
                    if (map.ExportExpression=="Edit...")
                    {
                        map.ExportExpression = null;
                    }
                }

                // Save to file
                var json = DataContractJsonSerializerExtensions.ToJson(_config);
                File.WriteAllText(filePath, json);

                CurrentMappingFile = filePath;
                this.Text = $"Edit Mapping - {Path.GetFileName(filePath)}";

                MessageBox.Show(
                    "Mapping configuration saved successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to save mapping:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Check if it's a ComboBox column error
            if (e.ColumnIndex >= 0 && e.ColumnIndex < dataGridView.Columns.Count &&
                dataGridView.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                // Get details about the error
                var invalidValue = string.Empty;
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView.Rows.Count)
                {
                    var cellValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    invalidValue = cellValue?.ToString() ?? "(empty)";
                }

                var columnName = dataGridView.Columns[e.ColumnIndex].HeaderText;

                // Show friendly error message
                MessageBox.Show(
                    $"Invalid Value in Dropdown Column\n\n" +
                    $"The value '{invalidValue}' in column '{columnName}' is not valid.\n" +
                    $"This value does not exist in the dropdown list.\n\n" +
                    $"The cell has been cleared. Please select a valid value from the dropdown.",
                    "Invalid Dropdown Value",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                // Clear the invalid cell value
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView.Rows.Count)
                {
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = null;
                }

                // Suppress the default error handling
                e.ThrowException = false;
            }
            else
            {
                // For other errors, show a generic message
                MessageBox.Show(
                    $"An error occurred in the data grid:\n{e.Exception?.Message ?? "Unknown error"}",
                    "Data Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                
                e.ThrowException = false;
            }
        }
    }

    /// <summary>
    /// Extension methods for JSON serialization
    /// </summary>
    internal static class DataContractJsonSerializerExtensions
    {
        public static string ToJson(CsvMappingConfig config)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(CsvMappingConfig));
                serializer.WriteObject(stream, config);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
