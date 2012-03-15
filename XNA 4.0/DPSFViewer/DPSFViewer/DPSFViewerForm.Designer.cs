namespace DPSFViewer
{
	partial class DPSFViewerForm
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
			this.Viewport = new System.Windows.Forms.PictureBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadParticleSystemClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.chkShowText = new System.Windows.Forms.CheckBox();
			this.chkShowFloor = new System.Windows.Forms.CheckBox();
			this.chkPaused = new System.Windows.Forms.CheckBox();
			this.flowPSPs = new System.Windows.Forms.FlowLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.Viewport)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// Viewport
			// 
			this.Viewport.BackColor = System.Drawing.Color.CornflowerBlue;
			this.Viewport.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Viewport.Location = new System.Drawing.Point(4, 27);
			this.Viewport.Name = "Viewport";
			this.Viewport.Size = new System.Drawing.Size(800, 600);
			this.Viewport.TabIndex = 0;
			this.Viewport.TabStop = false;
			this.Viewport.MouseEnter += new System.EventHandler(this.Viewport_MouseEnter);
			this.Viewport.MouseLeave += new System.EventHandler(this.Viewport_MouseLeave);
			// 
			// menuStrip1
			// 
			this.menuStrip1.BackColor = System.Drawing.SystemColors.ScrollBar;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1008, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadParticleSystemClassToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// loadParticleSystemClassToolStripMenuItem
			// 
			this.loadParticleSystemClassToolStripMenuItem.Name = "loadParticleSystemClassToolStripMenuItem";
			this.loadParticleSystemClassToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.loadParticleSystemClassToolStripMenuItem.Text = "Load Particle System Class...";
			this.loadParticleSystemClassToolStripMenuItem.Click += new System.EventHandler(this.loadParticleSystemClassToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// chkShowText
			// 
			this.chkShowText.AutoSize = true;
			this.chkShowText.Checked = true;
			this.chkShowText.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkShowText.Location = new System.Drawing.Point(807, 27);
			this.chkShowText.Name = "chkShowText";
			this.chkShowText.Size = new System.Drawing.Size(77, 17);
			this.chkShowText.TabIndex = 2;
			this.chkShowText.Text = "Show Text";
			this.chkShowText.UseVisualStyleBackColor = true;
			this.chkShowText.CheckedChanged += new System.EventHandler(this.chkShowText_CheckedChanged);
			// 
			// chkShowFloor
			// 
			this.chkShowFloor.AutoSize = true;
			this.chkShowFloor.Checked = true;
			this.chkShowFloor.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkShowFloor.Location = new System.Drawing.Point(893, 27);
			this.chkShowFloor.Name = "chkShowFloor";
			this.chkShowFloor.Size = new System.Drawing.Size(79, 17);
			this.chkShowFloor.TabIndex = 4;
			this.chkShowFloor.Text = "Show Floor";
			this.chkShowFloor.UseVisualStyleBackColor = true;
			this.chkShowFloor.CheckedChanged += new System.EventHandler(this.chkShowFloor_CheckedChanged);
			// 
			// chkPaused
			// 
			this.chkPaused.AutoSize = true;
			this.chkPaused.Location = new System.Drawing.Point(807, 50);
			this.chkPaused.Name = "chkPaused";
			this.chkPaused.Size = new System.Drawing.Size(62, 17);
			this.chkPaused.TabIndex = 5;
			this.chkPaused.Text = "Paused";
			this.chkPaused.UseVisualStyleBackColor = true;
			this.chkPaused.CheckedChanged += new System.EventHandler(this.chkPaused_CheckedChanged);
			// 
			// flowPSPs
			// 
			this.flowPSPs.Location = new System.Drawing.Point(0, 633);
			this.flowPSPs.Name = "flowPSPs";
			this.flowPSPs.Size = new System.Drawing.Size(1008, 92);
			this.flowPSPs.TabIndex = 3;
			// 
			// DPSFViewerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1008, 730);
			this.Controls.Add(this.chkPaused);
			this.Controls.Add(this.chkShowFloor);
			this.Controls.Add(this.flowPSPs);
			this.Controls.Add(this.chkShowText);
			this.Controls.Add(this.Viewport);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "DPSFViewerForm";
			this.Text = "DPSF Viewer";
			((System.ComponentModel.ISupportInitialize)(this.Viewport)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.PictureBox Viewport;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadParticleSystemClassToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkShowText;
		private System.Windows.Forms.CheckBox chkShowFloor;
        private System.Windows.Forms.CheckBox chkPaused;
		private System.Windows.Forms.FlowLayoutPanel flowPSPs;
	}
}