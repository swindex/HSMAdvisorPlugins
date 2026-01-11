namespace ImportCsvTools.Forms
{
    partial class MappingEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtLibraryName = new System.Windows.Forms.TextBox();
            this.chkAllowInvalidToolImport = new System.Windows.Forms.CheckBox();
            this.lblCsvInputUnits = new System.Windows.Forms.Label();
            this.radioButtonInches = new System.Windows.Forms.RadioButton();
            this.radioButtonMillimeters = new System.Windows.Forms.RadioButton();
            this.radioButtonMixed = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.btnAddMapping = new System.Windows.Forms.Button();
            this.btnRemoveMapping = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblRequiredFields = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.colCsvColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colToolField = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colDefaultValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEnumType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colExpression = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colExportExpression = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colValueMapButton = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Library Name:";
            // 
            // txtLibraryName
            // 
            this.txtLibraryName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLibraryName.Location = new System.Drawing.Point(81, 3);
            this.txtLibraryName.Name = "txtLibraryName";
            this.txtLibraryName.Size = new System.Drawing.Size(161, 20);
            this.txtLibraryName.TabIndex = 1;
            // 
            // chkAllowInvalidToolImport
            // 
            this.chkAllowInvalidToolImport.AutoSize = true;
            this.chkAllowInvalidToolImport.Location = new System.Drawing.Point(3, 3);
            this.chkAllowInvalidToolImport.Name = "chkAllowInvalidToolImport";
            this.chkAllowInvalidToolImport.Size = new System.Drawing.Size(141, 17);
            this.chkAllowInvalidToolImport.TabIndex = 12;
            this.chkAllowInvalidToolImport.Text = "Allow Invalid Tool Import";
            this.chkAllowInvalidToolImport.UseVisualStyleBackColor = true;
            // 
            // lblCsvInputUnits
            // 
            this.lblCsvInputUnits.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCsvInputUnits.AutoSize = true;
            this.lblCsvInputUnits.Location = new System.Drawing.Point(3, 5);
            this.lblCsvInputUnits.Name = "lblCsvInputUnits";
            this.lblCsvInputUnits.Size = new System.Drawing.Size(85, 13);
            this.lblCsvInputUnits.TabIndex = 13;
            this.lblCsvInputUnits.Text = "CSV Input Units:";
            // 
            // radioButtonInches
            // 
            this.radioButtonInches.AutoSize = true;
            this.radioButtonInches.Checked = true;
            this.radioButtonInches.Location = new System.Drawing.Point(94, 3);
            this.radioButtonInches.Name = "radioButtonInches";
            this.radioButtonInches.Size = new System.Drawing.Size(33, 17);
            this.radioButtonInches.TabIndex = 14;
            this.radioButtonInches.TabStop = true;
            this.radioButtonInches.Text = "in";
            this.radioButtonInches.UseVisualStyleBackColor = true;
            // 
            // radioButtonMillimeters
            // 
            this.radioButtonMillimeters.AutoSize = true;
            this.radioButtonMillimeters.Location = new System.Drawing.Point(133, 3);
            this.radioButtonMillimeters.Name = "radioButtonMillimeters";
            this.radioButtonMillimeters.Size = new System.Drawing.Size(41, 17);
            this.radioButtonMillimeters.TabIndex = 15;
            this.radioButtonMillimeters.Text = "mm";
            this.radioButtonMillimeters.UseVisualStyleBackColor = true;
            // 
            // radioButtonMixed
            // 
            this.radioButtonMixed.AutoSize = true;
            this.radioButtonMixed.Location = new System.Drawing.Point(180, 3);
            this.radioButtonMixed.Name = "radioButtonMixed";
            this.radioButtonMixed.Size = new System.Drawing.Size(53, 17);
            this.radioButtonMixed.TabIndex = 16;
            this.radioButtonMixed.Text = "mixed";
            this.radioButtonMixed.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Field Mappings:";
            // 
            // dataGridView
            // 
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCsvColumn,
            this.colToolField,
            this.colDefaultValue,
            this.colEnumType,
            this.colExpression,
            this.colExportExpression,
            this.colValueMapButton});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 77);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(668, 354);
            this.dataGridView.TabIndex = 4;
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellValueChanged);
            // 
            // btnAddMapping
            // 
            this.btnAddMapping.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddMapping.Location = new System.Drawing.Point(3, 3);
            this.btnAddMapping.Name = "btnAddMapping";
            this.btnAddMapping.Size = new System.Drawing.Size(100, 23);
            this.btnAddMapping.TabIndex = 5;
            this.btnAddMapping.Text = "Add Mapping";
            this.btnAddMapping.UseVisualStyleBackColor = true;
            this.btnAddMapping.Click += new System.EventHandler(this.btnAddMapping_Click);
            // 
            // btnRemoveMapping
            // 
            this.btnRemoveMapping.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveMapping.Location = new System.Drawing.Point(109, 3);
            this.btnRemoveMapping.Name = "btnRemoveMapping";
            this.btnRemoveMapping.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveMapping.TabIndex = 6;
            this.btnRemoveMapping.Text = "Remove";
            this.btnRemoveMapping.UseVisualStyleBackColor = true;
            this.btnRemoveMapping.Click += new System.EventHandler(this.btnRemoveMapping_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 28);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveAs.Location = new System.Drawing.Point(84, 3);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(75, 28);
            this.btnSaveAs.TabIndex = 9;
            this.btnSaveAs.Text = "Save As...";
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(165, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 28);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblRequiredFields
            // 
            this.lblRequiredFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRequiredFields.AutoSize = true;
            this.lblRequiredFields.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRequiredFields.Location = new System.Drawing.Point(3, 474);
            this.lblRequiredFields.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.lblRequiredFields.Name = "lblRequiredFields";
            this.lblRequiredFields.Size = new System.Drawing.Size(231, 13);
            this.lblRequiredFields.TabIndex = 7;
            this.lblRequiredFields.Text = "⚠ Missing required fields: Tool_type_id";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label3.Location = new System.Drawing.Point(3, 492);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(331, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Required fields: Tool_type_id, Tool_material_id, Coating_id, Diameter";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblRequiredFields, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 0, 7);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(674, 545);
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtLibraryName, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(245, 26);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.chkAllowInvalidToolImport, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel6, 1, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 35);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(470, 23);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel6.ColumnCount = 4;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.Controls.Add(this.lblCsvInputUnits, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.radioButtonInches, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.radioButtonMillimeters, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.radioButtonMixed, 3, 0);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(144, 0);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(236, 23);
            this.tableLayoutPanel6.TabIndex = 16;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.btnAddMapping, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnRemoveMapping, 1, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 437);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.Size = new System.Drawing.Size(187, 29);
            this.tableLayoutPanel4.TabIndex = 5;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 3;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.Controls.Add(this.btnCancel, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnSave, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnSaveAs, 1, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(428, 508);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(243, 34);
            this.tableLayoutPanel5.TabIndex = 12;
            // 
            // colCsvColumn
            // 
            this.colCsvColumn.HeaderText = "CSV Column";
            this.colCsvColumn.Name = "colCsvColumn";
            // 
            // colToolField
            // 
            this.colToolField.HeaderText = "Tool Field";
            this.colToolField.Name = "colToolField";
            this.colToolField.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colToolField.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colDefaultValue
            // 
            this.colDefaultValue.HeaderText = "Default Value";
            this.colDefaultValue.Name = "colDefaultValue";
            // 
            // colEnumType
            // 
            this.colEnumType.HeaderText = "Enum Type";
            this.colEnumType.Name = "colEnumType";
            this.colEnumType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colEnumType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colEnumType.Visible = false;
            // 
            // colExpression
            // 
            this.colExpression.HeaderText = "Expression";
            this.colExpression.Name = "colExpression";
            this.colExpression.ToolTipText = "v represents the current CSV cell value\n\nUse standard math: + - * / ( )\n\nUse IIF(" +
    "condition, trueValue, falseValue) for logic\n\nSupported comparisons: = <> < <= > " +
    ">=\n\nLogical operators: AND OR NOT";
            // 
            // colExportExpression
            // 
            this.colExportExpression.HeaderText = "Export Expression";
            this.colExportExpression.Name = "colExportExpression";
            this.colExportExpression.ToolTipText = "Expression for converting HSMAdvisor values back to CSV format during export\n\nva" +
    "lue represents the current Tool field value\n\nUse standard math: + - * / ( )\n\nUse" +
    " IIF(condition, trueValue, falseValue) for logic";
            // 
            // colValueMapButton
            // 
            this.colValueMapButton.HeaderText = "Value Map";
            this.colValueMapButton.Name = "colValueMapButton";
            this.colValueMapButton.Text = "Edit...";
            // 
            // MappingEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(684, 555);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "MappingEditorForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create/Edit Mapping Configuration";
            this.Load += new System.EventHandler(this.MappingEditorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLibraryName;
        private System.Windows.Forms.CheckBox chkAllowInvalidToolImport;
        private System.Windows.Forms.Label lblCsvInputUnits;
        private System.Windows.Forms.RadioButton radioButtonInches;
        private System.Windows.Forms.RadioButton radioButtonMillimeters;
        private System.Windows.Forms.RadioButton radioButtonMixed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button btnAddMapping;
        private System.Windows.Forms.Button btnRemoveMapping;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblRequiredFields;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCsvColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn colToolField;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDefaultValue;
        private System.Windows.Forms.DataGridViewComboBoxColumn colEnumType;
        private System.Windows.Forms.DataGridViewButtonColumn colExpression;
        private System.Windows.Forms.DataGridViewButtonColumn colExportExpression;
        private System.Windows.Forms.DataGridViewButtonColumn colValueMapButton;
    }
}
