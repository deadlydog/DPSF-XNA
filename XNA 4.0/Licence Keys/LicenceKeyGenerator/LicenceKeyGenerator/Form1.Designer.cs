namespace LicenceKeyGenerator
{
	partial class Form1
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
			this.numericNumberOfKeysToGenerate = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.btnGenerateLicenceKeys = new System.Windows.Forms.Button();
			this.listLicenceKeys = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.listHashedLicenceKeyValues = new System.Windows.Forms.ListBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.btnSelectAllLicenceKeys = new System.Windows.Forms.Button();
			this.btnSelectAllHashedKeyValues = new System.Windows.Forms.Button();
			this.btnCopySelectedLicenceKeysToClipboard = new System.Windows.Forms.Button();
			this.btnCopySelectedHashedKeyValuesToClipboard = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.txtSalt = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.numericNumberOfKeysToGenerate)).BeginInit();
			this.SuspendLayout();
			// 
			// numericNumberOfKeysToGenerate
			// 
			this.numericNumberOfKeysToGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.numericNumberOfKeysToGenerate.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.numericNumberOfKeysToGenerate.Location = new System.Drawing.Point(244, 7);
			this.numericNumberOfKeysToGenerate.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numericNumberOfKeysToGenerate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericNumberOfKeysToGenerate.Name = "numericNumberOfKeysToGenerate";
			this.numericNumberOfKeysToGenerate.Size = new System.Drawing.Size(78, 26);
			this.numericNumberOfKeysToGenerate.TabIndex = 0;
			this.numericNumberOfKeysToGenerate.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(33, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(209, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "Number of keys to generate:";
			// 
			// btnGenerateLicenceKeys
			// 
			this.btnGenerateLicenceKeys.Location = new System.Drawing.Point(496, 6);
			this.btnGenerateLicenceKeys.Name = "btnGenerateLicenceKeys";
			this.btnGenerateLicenceKeys.Size = new System.Drawing.Size(121, 26);
			this.btnGenerateLicenceKeys.TabIndex = 2;
			this.btnGenerateLicenceKeys.Text = "GENERATE";
			this.btnGenerateLicenceKeys.UseVisualStyleBackColor = true;
			this.btnGenerateLicenceKeys.Click += new System.EventHandler(this.btnGenerateLicenceKeys_Click);
			// 
			// listLicenceKeys
			// 
			this.listLicenceKeys.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listLicenceKeys.FormattingEnabled = true;
			this.listLicenceKeys.ItemHeight = 16;
			this.listLicenceKeys.Location = new System.Drawing.Point(16, 105);
			this.listLicenceKeys.Name = "listLicenceKeys";
			this.listLicenceKeys.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listLicenceKeys.Size = new System.Drawing.Size(300, 340);
			this.listLicenceKeys.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(108, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 20);
			this.label2.TabIndex = 4;
			this.label2.Text = "Licence Key";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(427, 55);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(140, 20);
			this.label3.TabIndex = 6;
			this.label3.Text = "Hashed Key Value";
			// 
			// listHashedLicenceKeyValues
			// 
			this.listHashedLicenceKeyValues.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listHashedLicenceKeyValues.FormattingEnabled = true;
			this.listHashedLicenceKeyValues.ItemHeight = 16;
			this.listHashedLicenceKeyValues.Location = new System.Drawing.Point(344, 105);
			this.listHashedLicenceKeyValues.Name = "listHashedLicenceKeyValues";
			this.listHashedLicenceKeyValues.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listHashedLicenceKeyValues.Size = new System.Drawing.Size(300, 340);
			this.listHashedLicenceKeyValues.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(89, 75);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(132, 20);
			this.label4.TabIndex = 7;
			this.label4.Text = "(to give to clients)";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(427, 75);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(144, 20);
			this.label5.TabIndex = 8;
			this.label5.Text = "(to use in software)";
			// 
			// btnSelectAllLicenceKeys
			// 
			this.btnSelectAllLicenceKeys.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSelectAllLicenceKeys.Location = new System.Drawing.Point(22, 448);
			this.btnSelectAllLicenceKeys.Name = "btnSelectAllLicenceKeys";
			this.btnSelectAllLicenceKeys.Size = new System.Drawing.Size(84, 38);
			this.btnSelectAllLicenceKeys.TabIndex = 9;
			this.btnSelectAllLicenceKeys.Text = "Select All";
			this.btnSelectAllLicenceKeys.UseVisualStyleBackColor = true;
			this.btnSelectAllLicenceKeys.Click += new System.EventHandler(this.btnSelectAllLicenceKeys_Click);
			// 
			// btnSelectAllHashedKeyValues
			// 
			this.btnSelectAllHashedKeyValues.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSelectAllHashedKeyValues.Location = new System.Drawing.Point(355, 448);
			this.btnSelectAllHashedKeyValues.Name = "btnSelectAllHashedKeyValues";
			this.btnSelectAllHashedKeyValues.Size = new System.Drawing.Size(84, 38);
			this.btnSelectAllHashedKeyValues.TabIndex = 10;
			this.btnSelectAllHashedKeyValues.Text = "Select All";
			this.btnSelectAllHashedKeyValues.UseVisualStyleBackColor = true;
			this.btnSelectAllHashedKeyValues.Click += new System.EventHandler(this.btnSelectAllHashedKeyValues_Click);
			// 
			// btnCopySelectedLicenceKeysToClipboard
			// 
			this.btnCopySelectedLicenceKeysToClipboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCopySelectedLicenceKeysToClipboard.Location = new System.Drawing.Point(112, 448);
			this.btnCopySelectedLicenceKeysToClipboard.Name = "btnCopySelectedLicenceKeysToClipboard";
			this.btnCopySelectedLicenceKeysToClipboard.Size = new System.Drawing.Size(193, 38);
			this.btnCopySelectedLicenceKeysToClipboard.TabIndex = 11;
			this.btnCopySelectedLicenceKeysToClipboard.Text = "Copy Selected To Clipboard";
			this.btnCopySelectedLicenceKeysToClipboard.UseVisualStyleBackColor = true;
			this.btnCopySelectedLicenceKeysToClipboard.Click += new System.EventHandler(this.btnCopySelectedLicenceKeysToClipboard_Click);
			// 
			// btnCopySelectedHashedKeyValuesToClipboard
			// 
			this.btnCopySelectedHashedKeyValuesToClipboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCopySelectedHashedKeyValuesToClipboard.Location = new System.Drawing.Point(445, 448);
			this.btnCopySelectedHashedKeyValuesToClipboard.Name = "btnCopySelectedHashedKeyValuesToClipboard";
			this.btnCopySelectedHashedKeyValuesToClipboard.Size = new System.Drawing.Size(193, 38);
			this.btnCopySelectedHashedKeyValuesToClipboard.TabIndex = 12;
			this.btnCopySelectedHashedKeyValuesToClipboard.Text = "Copy Selected To Clipboard";
			this.btnCopySelectedHashedKeyValuesToClipboard.UseVisualStyleBackColor = true;
			this.btnCopySelectedHashedKeyValuesToClipboard.Click += new System.EventHandler(this.btnCopySelectedHashedKeyValuesToClipboard_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(328, 9);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(37, 20);
			this.label6.TabIndex = 13;
			this.label6.Text = "Salt";
			// 
			// txtSalt
			// 
			this.txtSalt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSalt.Location = new System.Drawing.Point(371, 7);
			this.txtSalt.MaxLength = 10;
			this.txtSalt.Name = "txtSalt";
			this.txtSalt.Size = new System.Drawing.Size(116, 26);
			this.txtSalt.TabIndex = 14;
			this.txtSalt.Text = "DPSF";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(663, 498);
			this.Controls.Add(this.txtSalt);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.btnCopySelectedHashedKeyValuesToClipboard);
			this.Controls.Add(this.btnCopySelectedLicenceKeysToClipboard);
			this.Controls.Add(this.btnSelectAllHashedKeyValues);
			this.Controls.Add(this.btnSelectAllLicenceKeys);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.listHashedLicenceKeyValues);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listLicenceKeys);
			this.Controls.Add(this.btnGenerateLicenceKeys);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numericNumberOfKeysToGenerate);
			this.Name = "Form1";
			this.Text = "Product Key Generator";
			((System.ComponentModel.ISupportInitialize)(this.numericNumberOfKeysToGenerate)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numericNumberOfKeysToGenerate;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnGenerateLicenceKeys;
		private System.Windows.Forms.ListBox listLicenceKeys;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListBox listHashedLicenceKeyValues;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnSelectAllLicenceKeys;
		private System.Windows.Forms.Button btnSelectAllHashedKeyValues;
		private System.Windows.Forms.Button btnCopySelectedLicenceKeysToClipboard;
		private System.Windows.Forms.Button btnCopySelectedHashedKeyValuesToClipboard;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtSalt;
	}
}

