namespace RTViewer
{
    partial class RTViewer
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
            this.components = new System.ComponentModel.Container();
            this.RTContainer = new System.Windows.Forms.Panel();
            this.mRTPreviewArea = new System.Windows.Forms.Panel();
            this.mInteractiveViewArea = new System.Windows.Forms.Panel();
            this.mTimer = new System.Windows.Forms.Timer(this.components);
            this.XNAUIDraw = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // RTContainer
            // 
            this.RTContainer.AutoSize = true;
            this.RTContainer.Location = new System.Drawing.Point(2, 372);
            this.RTContainer.Name = "RTContainer";
            this.RTContainer.Size = new System.Drawing.Size(980, 424);
            this.RTContainer.TabIndex = 0;
            // 
            // mRTPreviewArea
            // 
            this.mRTPreviewArea.Location = new System.Drawing.Point(2, 1);
            this.mRTPreviewArea.Name = "mRTPreviewArea";
            this.mRTPreviewArea.Size = new System.Drawing.Size(484, 368);
            this.mRTPreviewArea.TabIndex = 1;
            // 
            // mInteractiveViewArea
            // 
            this.mInteractiveViewArea.Location = new System.Drawing.Point(490, 1);
            this.mInteractiveViewArea.Name = "mInteractiveViewArea";
            this.mInteractiveViewArea.Size = new System.Drawing.Size(491, 368);
            this.mInteractiveViewArea.TabIndex = 2;
            // 
            // mTimer
            // 
            this.mTimer.Interval = 25;
            this.mTimer.Tick += new System.EventHandler(this.mTimer_Tick);
            // 
            // XNAUIDraw
            // 
            this.XNAUIDraw.AutoSize = true;
            this.XNAUIDraw.Checked = true;
            this.XNAUIDraw.CheckState = System.Windows.Forms.CheckState.Checked;
            this.XNAUIDraw.Location = new System.Drawing.Point(451, 349);
            this.XNAUIDraw.Name = "XNAUIDraw";
            this.XNAUIDraw.Size = new System.Drawing.Size(99, 17);
            this.XNAUIDraw.TabIndex = 3;
            this.XNAUIDraw.Text = "InteractiveView";
            this.XNAUIDraw.UseVisualStyleBackColor = true;
            // 
            // RTViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(984, 799);
            this.Controls.Add(this.XNAUIDraw);
            this.Controls.Add(this.RTContainer);
            this.Controls.Add(this.mRTPreviewArea);
            this.Controls.Add(this.mInteractiveViewArea);
            this.Name = "RTViewer";
            this.Text = "Ray Trace Previewer";
            this.Load += new System.EventHandler(this.RTViewer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel RTContainer;
        private System.Windows.Forms.Panel mRTPreviewArea;
        private System.Windows.Forms.Panel mInteractiveViewArea;
        private System.Windows.Forms.Timer mTimer;
        private System.Windows.Forms.CheckBox XNAUIDraw;
    }
}

