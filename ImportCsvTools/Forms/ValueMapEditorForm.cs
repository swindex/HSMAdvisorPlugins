using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ImportCsvTools.Forms
{
    /// <summary>
    /// Form for editing value mappings (key-value pairs)
    /// </summary>
    public partial class ValueMapEditorForm : Form
    {
        private Dictionary<string, string> _valueMap;
        private string _enumTypeName;
        private CsvImportColumnInfo _csvColumnInfo;

        public Dictionary<string, string> ValueMap
        {
            get { return _valueMap; }
            set
            {
                _valueMap = value ?? new Dictionary<string, string>();
                PopulateGrid();
                PrepopulateMissingValues();
            }
        }

        /// <summary>
        /// If set, the "To Value" column will be a dropdown with enum values
        /// </summary>
        public string EnumTypeName
        {
            get { return _enumTypeName; }
            set
            {
                _enumTypeName = value;
                ConfigureToValueColumn();
            }
        }

        /// <summary>
        /// If set, unique values from this CSV column will be prepopulated
        /// </summary>
        public CsvImportColumnInfo CsvColumnInfo
        {
            get { return _csvColumnInfo; }
            set
            {
                _csvColumnInfo = value;
                PrepopulateMissingValues();
            }
        }

        public ValueMapEditorForm()
        {
            InitializeComponent();
            _valueMap = new Dictionary<string, string>();
            
            // Wire up DataError event for graceful error handling
            dataGridView.DataError += dataGridView_DataError;
        }

        private void ConfigureToValueColumn()
        {
            // Save column properties
            int columnIndex = colToValue.DisplayIndex;
            string headerText = colToValue.HeaderText;
            int width = colToValue.Width;
            
            // Save existing values from the column before replacing it
            var existingValues = new List<string>();
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (!row.IsNewRow && row.Cells.Count > 1)
                {
                    existingValues.Add(row.Cells[1].Value?.ToString() ?? string.Empty);
                }
            }

            // Remove the old column
            dataGridView.Columns.Remove(colToValue);

            // Create the appropriate column type based on whether we have an enum
            DataGridViewColumn newColumn;
            
            if (!string.IsNullOrWhiteSpace(_enumTypeName))
            {
                // Enum mode: strict dropdown with enum values
                var comboColumn = new DataGridViewComboBoxColumn();
                var enumValues = ReflectionHelpers.GetEnumValues(_enumTypeName);
                comboColumn.DataSource = enumValues;
                comboColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
                comboColumn.FlatStyle = FlatStyle.Standard;
                comboColumn.DisplayStyleForCurrentCellOnly = false;
                newColumn = comboColumn;
            }
            else
            {
                // Plain text mode: use a regular TextBox column for free-form entry
                newColumn = new DataGridViewTextBoxColumn();
            }
            
            // Restore column properties
            newColumn.Name = "colToValue";
            newColumn.HeaderText = headerText;
            newColumn.Width = width;
            
            // Add the new column
            dataGridView.Columns.Add(newColumn);
            
            // Store reference to the new column
            colToValue = newColumn;
            
            // Restore the values
            for (int i = 0; i < existingValues.Count && i < dataGridView.Rows.Count; i++)
            {
                if (!dataGridView.Rows[i].IsNewRow)
                {
                    dataGridView.Rows[i].Cells[1].Value = existingValues[i];
                }
            }
        }

        private void PopulateGrid()
        {
            dataGridView.Rows.Clear();
            foreach (var kvp in _valueMap)
            {
                dataGridView.Rows.Add(kvp.Key, kvp.Value);
            }
        }

        private void PrepopulateMissingValues()
        {
            // Only prepopulate if we have CSV column info
            if (_csvColumnInfo == null || _csvColumnInfo.UniqueValues == null || _csvColumnInfo.UniqueValues.Count == 0)
            {
                return;
            }

            // Only prepopulate if we have a value map initialized
            if (_valueMap == null)
            {
                return;
            }

            // Get the set of values already in the ValueMap (case-insensitive)
            var existingFromValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow)
                    continue;

                var fromValue = row.Cells[0].Value?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(fromValue))
                {
                    existingFromValues.Add(fromValue);
                }
            }

            // Add rows for unique values that are not already mapped
            foreach (var uniqueValue in _csvColumnInfo.UniqueValues)
            {
                if (!existingFromValues.Contains(uniqueValue))
                {
                    // Add row with FromValue filled, ToValue empty
                    dataGridView.Rows.Add(uniqueValue, string.Empty);
                }
            }
        }

        private Dictionary<string, string> GetValueMapFromGrid()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow)
                    continue;

                var fromValue = row.Cells[0].Value?.ToString()?.Trim();
                var toValue = row.Cells[1].Value?.ToString()?.Trim();

                if (!string.IsNullOrWhiteSpace(fromValue))
                {
                    result[fromValue] = toValue ?? string.Empty;
                }
            }

            return result;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _valueMap = GetValueMapFromGrid();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            dataGridView.Rows.Add(string.Empty, string.Empty);
        }

        private void btnRemove_Click(object sender, EventArgs e)
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
            }
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

        private void ValueMapEditorForm_Load(object sender, EventArgs e)
        {

        }
    }
}
