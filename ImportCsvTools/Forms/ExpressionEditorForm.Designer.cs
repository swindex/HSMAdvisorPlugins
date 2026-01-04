namespace ImportCsvTools.Forms
{
    partial class ExpressionEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpressionEditorForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxInstructions = new System.Windows.Forms.GroupBox();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.groupBoxExpression = new System.Windows.Forms.GroupBox();
            this.txtExpression = new System.Windows.Forms.TextBox();
            this.groupBoxTestValues = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewTestValues = new System.Windows.Forms.DataGridView();
            this.colTestValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddTestValue = new System.Windows.Forms.Button();
            this.btnRemoveTestValue = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxInstructions.SuspendLayout();
            this.groupBoxExpression.SuspendLayout();
            this.groupBoxTestValues.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTestValues)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBoxInstructions, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxExpression, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxTestValues, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(564, 541);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBoxInstructions
            // 
            this.groupBoxInstructions.AutoSize = true;
            this.groupBoxInstructions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxInstructions.Controls.Add(this.lblInstructions);
            this.groupBoxInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxInstructions.Location = new System.Drawing.Point(3, 3);
            this.groupBoxInstructions.Name = "groupBoxInstructions";
            this.groupBoxInstructions.Padding = new System.Windows.Forms.Padding(8);
            this.groupBoxInstructions.Size = new System.Drawing.Size(558, 237);
            this.groupBoxInstructions.TabIndex = 0;
            this.groupBoxInstructions.TabStop = false;
            this.groupBoxInstructions.Text = "Instructions";
            // 
            // lblInstructions
            // 
            this.lblInstructions.AutoSize = true;
            this.lblInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInstructions.Location = new System.Drawing.Point(8, 21);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(352, 208);
            this.lblInstructions.TabIndex = 0;
            this.lblInstructions.Text = resources.GetString("lblInstructions.Text");
            // 
            // groupBoxExpression
            // 
            this.groupBoxExpression.AutoSize = true;
            this.groupBoxExpression.Controls.Add(this.txtExpression);
            this.groupBoxExpression.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxExpression.Location = new System.Drawing.Point(3, 246);
            this.groupBoxExpression.Name = "groupBoxExpression";
            this.groupBoxExpression.Padding = new System.Windows.Forms.Padding(8, 8, 8, 35);
            this.groupBoxExpression.Size = new System.Drawing.Size(558, 56);
            this.groupBoxExpression.TabIndex = 1;
            this.groupBoxExpression.TabStop = false;
            this.groupBoxExpression.Text = "Expression";
            // 
            // txtExpression
            // 
            this.txtExpression.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtExpression.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExpression.Location = new System.Drawing.Point(8, 21);
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExpression.Size = new System.Drawing.Size(542, 23);
            this.txtExpression.TabIndex = 0;
            this.txtExpression.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtExpression_KeyDown);
            this.txtExpression.Validating += new System.ComponentModel.CancelEventHandler(this.txtExpression_Validating);
            // 
            // groupBoxTestValues
            // 
            this.groupBoxTestValues.Controls.Add(this.tableLayoutPanel2);
            this.groupBoxTestValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxTestValues.Location = new System.Drawing.Point(3, 308);
            this.groupBoxTestValues.Name = "groupBoxTestValues";
            this.groupBoxTestValues.Padding = new System.Windows.Forms.Padding(8);
            this.groupBoxTestValues.Size = new System.Drawing.Size(558, 191);
            this.groupBoxTestValues.TabIndex = 2;
            this.groupBoxTestValues.TabStop = false;
            this.groupBoxTestValues.Text = "Test Values";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.dataGridViewTestValues, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(8, 21);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(542, 162);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // dataGridViewTestValues
            // 
            this.dataGridViewTestValues.AllowUserToResizeRows = false;
            this.dataGridViewTestValues.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewTestValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTestValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTestValue,
            this.colResult});
            this.dataGridViewTestValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewTestValues.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewTestValues.Name = "dataGridViewTestValues";
            this.dataGridViewTestValues.Size = new System.Drawing.Size(536, 121);
            this.dataGridViewTestValues.TabIndex = 0;
            this.dataGridViewTestValues.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewTestValues_CellValueChanged);
            // 
            // colTestValue
            // 
            this.colTestValue.HeaderText = "Test Value";
            this.colTestValue.Name = "colTestValue";
            // 
            // colResult
            // 
            this.colResult.HeaderText = "Result";
            this.colResult.Name = "colResult";
            this.colResult.ReadOnly = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnAddTestValue);
            this.flowLayoutPanel1.Controls.Add(this.btnRemoveTestValue);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 130);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(536, 29);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnAddTestValue
            // 
            this.btnAddTestValue.Location = new System.Drawing.Point(3, 3);
            this.btnAddTestValue.Name = "btnAddTestValue";
            this.btnAddTestValue.Size = new System.Drawing.Size(100, 23);
            this.btnAddTestValue.TabIndex = 0;
            this.btnAddTestValue.Text = "Add Test Value";
            this.btnAddTestValue.UseVisualStyleBackColor = true;
            this.btnAddTestValue.Click += new System.EventHandler(this.btnAddTestValue_Click);
            // 
            // btnRemoveTestValue
            // 
            this.btnRemoveTestValue.Location = new System.Drawing.Point(109, 3);
            this.btnRemoveTestValue.Name = "btnRemoveTestValue";
            this.btnRemoveTestValue.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveTestValue.TabIndex = 1;
            this.btnRemoveTestValue.Text = "Remove";
            this.btnRemoveTestValue.UseVisualStyleBackColor = true;
            this.btnRemoveTestValue.Click += new System.EventHandler(this.btnRemoveTestValue_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.btnCancel);
            this.flowLayoutPanel2.Controls.Add(this.btnSave);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(399, 505);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(162, 33);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(84, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 27);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "OK";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // ExpressionEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(500, 500);
            this.Name = "ExpressionEditorForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Expression Editor";
            this.Load += new System.EventHandler(this.ExpressionEditorForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBoxInstructions.ResumeLayout(false);
            this.groupBoxInstructions.PerformLayout();
            this.groupBoxExpression.ResumeLayout(false);
            this.groupBoxExpression.PerformLayout();
            this.groupBoxTestValues.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTestValues)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBoxInstructions;
        private System.Windows.Forms.Label lblInstructions;
        private System.Windows.Forms.GroupBox groupBoxExpression;
        private System.Windows.Forms.TextBox txtExpression;
        private System.Windows.Forms.GroupBox groupBoxTestValues;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView dataGridViewTestValues;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnAddTestValue;
        private System.Windows.Forms.Button btnRemoveTestValue;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTestValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colResult;
    }
}
