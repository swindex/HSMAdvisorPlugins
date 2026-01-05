using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ImportCsvTools.Forms
{
    /// <summary>
    /// Form for editing and testing expressions that transform CSV cell values
    /// </summary>
    public partial class ExpressionEditorForm : Form
    {
        // Cached DataTable for expression evaluation (performance optimization)
        private static readonly System.Data.DataTable _expressionEvaluator = new System.Data.DataTable();

        public string Expression { get; set; }
        public CsvImportColumnInfo CsvColumnInfo { get; set; }

        public ExpressionEditorForm()
        {
            InitializeComponent();
        }

        private void ExpressionEditorForm_Load(object sender, EventArgs e)
        {
            // Load the expression if provided
            if (!string.IsNullOrWhiteSpace(Expression))
            {
                txtExpression.Text = Expression;
            }

            // Load test values from CSV column info
            LoadTestValuesFromCsvColumn();

            // Perform initial evaluation
            EvaluateAllTestValues();
        }

        private void LoadTestValuesFromCsvColumn()
        {
            if (CsvColumnInfo == null || CsvColumnInfo.UniqueValues == null)
                return;

            // Add unique values from the CSV column as test values
            foreach (var uniqueValue in CsvColumnInfo.UniqueValues.Take(20)) // Limit to first 20 for performance
            {
                dataGridViewTestValues.Rows.Add(uniqueValue, string.Empty);
            }
        }

        private void txtExpression_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Real-time evaluation as expression changes
            EvaluateAllTestValues();
        }
        private void txtExpression_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Evaluate on Enter key press
                EvaluateAllTestValues();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void dataGridViewTestValues_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Re-evaluate when a test value is changed
            if (e.RowIndex >= 0 && e.ColumnIndex == 0) // Test Value column
            {
                EvaluateTestValue(e.RowIndex);
            }
        }

        private void btnAddTestValue_Click(object sender, EventArgs e)
        {
            var rowIndex = dataGridViewTestValues.Rows.Add(string.Empty, string.Empty);
            
            // Focus the new row's first cell for editing
            dataGridViewTestValues.CurrentCell = dataGridViewTestValues.Rows[rowIndex].Cells[0];
            dataGridViewTestValues.BeginEdit(true);
        }

        private void btnRemoveTestValue_Click(object sender, EventArgs e)
        {
            if (dataGridViewTestValues.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridViewTestValues.SelectedRows)
                {
                    dataGridViewTestValues.Rows.Remove(row);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Expression = txtExpression.Text?.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void EvaluateAllTestValues()
        {
            foreach (DataGridViewRow row in dataGridViewTestValues.Rows)
            {
                EvaluateTestValue(row.Index);
            }
        }

        private void EvaluateTestValue(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dataGridViewTestValues.Rows.Count)
                return;

            var row = dataGridViewTestValues.Rows[rowIndex];
            var testValue = row.Cells[0].Value?.ToString() ?? string.Empty;
            var expression = txtExpression.Text?.Trim();

            // Evaluate the expression
            var result = EvaluateExpression(expression, testValue);
            
            // Update the result cell
            row.Cells[1].Value = result;

            // Color code the result cell based on success/error
            if (result.StartsWith("ERROR:", StringComparison.OrdinalIgnoreCase))
            {
                row.Cells[1].Style.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                row.Cells[1].Style.ForeColor = System.Drawing.Color.Green;
            }
        }

        /// <summary>
        /// Evaluates a mathematical expression with the CSV value substituted for 'v'
        /// Uses cached DataTable.Compute() for performance
        /// </summary>
        private static string EvaluateExpression(string expression, string value)
        {
            try
            {
                var result = ExpressionEvaluator.EvaluateExpression(expression, value, null, true);
                return result?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }
    }
}
