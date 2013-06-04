namespace RayTracer_552
{
    partial class RTWindow
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
            this.mImage = new System.Windows.Forms.PictureBox();
            this.mMask = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mStatusArea = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.mOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.mCmdFileNameEcho = new System.Windows.Forms.TextBox();
            this.mSelectCmdFile = new System.Windows.Forms.Button();
            this.mReloadCmdFile = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SaveImageButton = new System.Windows.Forms.Button();
            this.mSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.DebugPixels = new System.Windows.Forms.CheckBox();
            this.DebugRays = new System.Windows.Forms.CheckBox();
            this.OrthoRT = new System.Windows.Forms.CheckBox();
            this.ShowDepth = new System.Windows.Forms.CheckBox();
            this.MultiThreadCompute = new System.Windows.Forms.CheckBox();
            this.SkipDraws = new System.Windows.Forms.CheckBox();
            this.PixelInWorld = new System.Windows.Forms.CheckBox();
            this.DrawDB = new System.Windows.Forms.CheckBox();
            this.chkAnaglyph = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.mImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mMask)).BeginInit();
            this.SuspendLayout();
            // 
            // mImage
            // 
            this.mImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mImage.Location = new System.Drawing.Point(2, 51);
            this.mImage.Margin = new System.Windows.Forms.Padding(2);
            this.mImage.Name = "mImage";
            this.mImage.Size = new System.Drawing.Size(486, 361);
            this.mImage.TabIndex = 0;
            this.mImage.TabStop = false;
            // 
            // mMask
            // 
            this.mMask.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mMask.Location = new System.Drawing.Point(492, 51);
            this.mMask.Margin = new System.Windows.Forms.Padding(2);
            this.mMask.Name = "mMask";
            this.mMask.Size = new System.Drawing.Size(475, 365);
            this.mMask.TabIndex = 1;
            this.mMask.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(453, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "RT Status";
            // 
            // mStatusArea
            // 
            this.mStatusArea.Location = new System.Drawing.Point(512, 3);
            this.mStatusArea.Margin = new System.Windows.Forms.Padding(2);
            this.mStatusArea.Name = "mStatusArea";
            this.mStatusArea.Size = new System.Drawing.Size(455, 20);
            this.mStatusArea.TabIndex = 4;
            this.mStatusArea.Text = "No Cmd File Yet";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(4, 30);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(53, 19);
            this.button2.TabIndex = 6;
            this.button2.Text = "BeginRT";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.BeginRT);
            // 
            // mOpenFileDialog
            // 
            this.mOpenFileDialog.DefaultExt = "xml";
            this.mOpenFileDialog.Filter = "xml|";
            this.mOpenFileDialog.InitialDirectory = "./../";
            this.mOpenFileDialog.RestoreDirectory = true;
            // 
            // mCmdFileNameEcho
            // 
            this.mCmdFileNameEcho.Location = new System.Drawing.Point(63, 3);
            this.mCmdFileNameEcho.Margin = new System.Windows.Forms.Padding(2);
            this.mCmdFileNameEcho.Name = "mCmdFileNameEcho";
            this.mCmdFileNameEcho.Size = new System.Drawing.Size(219, 20);
            this.mCmdFileNameEcho.TabIndex = 8;
            this.mCmdFileNameEcho.Text = "No Cmd File Selected";
            // 
            // mSelectCmdFile
            // 
            this.mSelectCmdFile.Location = new System.Drawing.Point(3, 4);
            this.mSelectCmdFile.Margin = new System.Windows.Forms.Padding(2);
            this.mSelectCmdFile.Name = "mSelectCmdFile";
            this.mSelectCmdFile.Size = new System.Drawing.Size(56, 19);
            this.mSelectCmdFile.TabIndex = 9;
            this.mSelectCmdFile.Text = "CmdFile";
            this.mSelectCmdFile.UseVisualStyleBackColor = true;
            this.mSelectCmdFile.Click += new System.EventHandler(this.mSelectCmdFile_Click);
            // 
            // mReloadCmdFile
            // 
            this.mReloadCmdFile.Location = new System.Drawing.Point(286, 4);
            this.mReloadCmdFile.Margin = new System.Windows.Forms.Padding(2);
            this.mReloadCmdFile.Name = "mReloadCmdFile";
            this.mReloadCmdFile.Size = new System.Drawing.Size(50, 19);
            this.mReloadCmdFile.TabIndex = 11;
            this.mReloadCmdFile.Text = "Reload ";
            this.mReloadCmdFile.UseVisualStyleBackColor = true;
            this.mReloadCmdFile.Click += new System.EventHandler(this.mReloadCmdFile_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(61, 30);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(53, 19);
            this.button3.TabIndex = 12;
            this.button3.Text = "Stop RT";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.StopRT);
            // 
            // SaveImageButton
            // 
            this.SaveImageButton.Location = new System.Drawing.Point(346, 4);
            this.SaveImageButton.Margin = new System.Windows.Forms.Padding(2);
            this.SaveImageButton.Name = "SaveImageButton";
            this.SaveImageButton.Size = new System.Drawing.Size(67, 19);
            this.SaveImageButton.TabIndex = 13;
            this.SaveImageButton.Text = "Save Images";
            this.SaveImageButton.UseVisualStyleBackColor = true;
            this.SaveImageButton.Click += new System.EventHandler(this.SaveImageButton_Click);
            // 
            // mSaveFileDialog
            // 
            this.mSaveFileDialog.DefaultExt = "bmp";
            this.mSaveFileDialog.FileName = "RTImage";
            this.mSaveFileDialog.Filter = "bmp|";
            this.mSaveFileDialog.InitialDirectory = "../";
            this.mSaveFileDialog.RestoreDirectory = true;
            // 
            // DebugPixels
            // 
            this.DebugPixels.AutoSize = true;
            this.DebugPixels.Location = new System.Drawing.Point(527, 31);
            this.DebugPixels.Margin = new System.Windows.Forms.Padding(2);
            this.DebugPixels.Name = "DebugPixels";
            this.DebugPixels.Size = new System.Drawing.Size(85, 17);
            this.DebugPixels.TabIndex = 14;
            this.DebugPixels.Text = "DebugPixels";
            this.DebugPixels.UseVisualStyleBackColor = true;
            this.DebugPixels.CheckedChanged += new System.EventHandler(this.DebugPixels_CheckedChanged);
            // 
            // DebugRays
            // 
            this.DebugRays.AutoSize = true;
            this.DebugRays.Location = new System.Drawing.Point(441, 31);
            this.DebugRays.Margin = new System.Windows.Forms.Padding(2);
            this.DebugRays.Name = "DebugRays";
            this.DebugRays.Size = new System.Drawing.Size(82, 17);
            this.DebugRays.TabIndex = 15;
            this.DebugRays.Text = "DebugRays";
            this.DebugRays.UseVisualStyleBackColor = true;
            this.DebugRays.CheckedChanged += new System.EventHandler(this.DebugRays_CheckedChanged);
            // 
            // OrthoRT
            // 
            this.OrthoRT.AutoSize = true;
            this.OrthoRT.Location = new System.Drawing.Point(284, 31);
            this.OrthoRT.Margin = new System.Windows.Forms.Padding(2);
            this.OrthoRT.Name = "OrthoRT";
            this.OrthoRT.Size = new System.Drawing.Size(67, 17);
            this.OrthoRT.TabIndex = 16;
            this.OrthoRT.Text = "OrthoRT";
            this.OrthoRT.UseVisualStyleBackColor = true;
            this.OrthoRT.CheckedChanged += new System.EventHandler(this.OrthoRT_CheckedChanged);
            // 
            // ShowDepth
            // 
            this.ShowDepth.AutoSize = true;
            this.ShowDepth.Location = new System.Drawing.Point(355, 31);
            this.ShowDepth.Margin = new System.Windows.Forms.Padding(2);
            this.ShowDepth.Name = "ShowDepth";
            this.ShowDepth.Size = new System.Drawing.Size(82, 17);
            this.ShowDepth.TabIndex = 17;
            this.ShowDepth.Text = "ShowDepth";
            this.ShowDepth.UseVisualStyleBackColor = true;
            // 
            // MultiThreadCompute
            // 
            this.MultiThreadCompute.AutoSize = true;
            this.MultiThreadCompute.Location = new System.Drawing.Point(118, 31);
            this.MultiThreadCompute.Margin = new System.Windows.Forms.Padding(2);
            this.MultiThreadCompute.Name = "MultiThreadCompute";
            this.MultiThreadCompute.Size = new System.Drawing.Size(78, 17);
            this.MultiThreadCompute.TabIndex = 20;
            this.MultiThreadCompute.Text = "Multithread";
            this.MultiThreadCompute.UseVisualStyleBackColor = true;
            this.MultiThreadCompute.CheckedChanged += new System.EventHandler(this.MultiThreadCompute_CheckedChanged);
            // 
            // SkipDraws
            // 
            this.SkipDraws.AutoSize = true;
            this.SkipDraws.Location = new System.Drawing.Point(200, 31);
            this.SkipDraws.Margin = new System.Windows.Forms.Padding(2);
            this.SkipDraws.Name = "SkipDraws";
            this.SkipDraws.Size = new System.Drawing.Size(80, 17);
            this.SkipDraws.TabIndex = 20;
            this.SkipDraws.Text = "Skip Draws";
            this.SkipDraws.UseVisualStyleBackColor = true;
            // 
            // PixelInWorld
            // 
            this.PixelInWorld.AutoSize = true;
            this.PixelInWorld.Location = new System.Drawing.Point(616, 30);
            this.PixelInWorld.Margin = new System.Windows.Forms.Padding(2);
            this.PixelInWorld.Name = "PixelInWorld";
            this.PixelInWorld.Size = new System.Drawing.Size(85, 17);
            this.PixelInWorld.TabIndex = 21;
            this.PixelInWorld.Text = "PixelInWorld";
            this.PixelInWorld.UseVisualStyleBackColor = true;
            this.PixelInWorld.CheckedChanged += new System.EventHandler(this.PixelInWorld_CheckedChanged);
            // 
            // DrawDB
            // 
            this.DrawDB.AutoSize = true;
            this.DrawDB.Checked = true;
            this.DrawDB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DrawDB.Location = new System.Drawing.Point(705, 30);
            this.DrawDB.Margin = new System.Windows.Forms.Padding(2);
            this.DrawDB.Name = "DrawDB";
            this.DrawDB.Size = new System.Drawing.Size(82, 17);
            this.DrawDB.TabIndex = 22;
            this.DrawDB.Text = "DrawScene";
            this.DrawDB.UseVisualStyleBackColor = true;
            this.DrawDB.CheckedChanged += new System.EventHandler(this.DrawDB_CheckedChanged);
            // 
            // chkAnaglyph
            // 
            this.chkAnaglyph.AutoSize = true;
            this.chkAnaglyph.Location = new System.Drawing.Point(791, 30);
            this.chkAnaglyph.Margin = new System.Windows.Forms.Padding(2);
            this.chkAnaglyph.Name = "chkAnaglyph";
            this.chkAnaglyph.Size = new System.Drawing.Size(70, 17);
            this.chkAnaglyph.TabIndex = 23;
            this.chkAnaglyph.Text = "Anaglyph";
            this.chkAnaglyph.UseVisualStyleBackColor = true;
            this.chkAnaglyph.CheckedChanged += new System.EventHandler(this.chkAnaglyph_CheckedChanged_1);
            // 
            // RTWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.chkAnaglyph);
            this.Controls.Add(this.DrawDB);
            this.Controls.Add(this.PixelInWorld);
            this.Controls.Add(this.SkipDraws);
            this.Controls.Add(this.MultiThreadCompute);
            this.Controls.Add(this.ShowDepth);
            this.Controls.Add(this.OrthoRT);
            this.Controls.Add(this.DebugRays);
            this.Controls.Add(this.DebugPixels);
            this.Controls.Add(this.SaveImageButton);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.mReloadCmdFile);
            this.Controls.Add(this.mSelectCmdFile);
            this.Controls.Add(this.mCmdFileNameEcho);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.mStatusArea);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mMask);
            this.Controls.Add(this.mImage);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RTWindow";
            this.Size = new System.Drawing.Size(969, 418);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.RTWindow_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.mImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mMask)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox mImage;
        private System.Windows.Forms.PictureBox mMask;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mStatusArea;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.OpenFileDialog mOpenFileDialog;
        private System.Windows.Forms.TextBox mCmdFileNameEcho;
        private System.Windows.Forms.Button mSelectCmdFile;
        private System.Windows.Forms.Button mReloadCmdFile;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button SaveImageButton;
        private System.Windows.Forms.SaveFileDialog mSaveFileDialog;
        private System.Windows.Forms.CheckBox DebugPixels;
        private System.Windows.Forms.CheckBox DebugRays;
        private System.Windows.Forms.CheckBox OrthoRT;
        private System.Windows.Forms.CheckBox ShowDepth;
        private System.Windows.Forms.CheckBox MultiThreadCompute;
        private System.Windows.Forms.CheckBox SkipDraws;
        private System.Windows.Forms.CheckBox PixelInWorld;
        private System.Windows.Forms.CheckBox DrawDB;
        private System.Windows.Forms.CheckBox chkAnaglyph;
    }
}

