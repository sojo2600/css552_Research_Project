using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content;

using System.Threading;


namespace RayTracer_552
{
    /// <summary>
    /// RTWindow is the main window for the app. 
    /// 
    /// 1. the top two sub-windows are interactive views of the RT scene (implemented in XNA)
    ///         LEFT: view from the RT camera
    ///         RIGHT: third camera view of the scene
    ///     
    /// 2. the bottom two sub-windows are:
    ///         LEFT: ray trace image
    ///         RIGHT: mask (coverage) for the ray trace image
    ///            
    /// Service routine for all the buttons/check boxes are defined in this file.
    /// 
    /// </summary>
    public partial class RTWindow : UserControl
    {
        /// <summary>
        ///  Constants for GUI layout (space between sub-windows)
        /// </summary>
        private const int kSpace = 2;
        private const int kXOffset = 2;
        private const int kYOffset = 60;

        /// <summary>
        /// Defualt Image size
        /// </summary>
        public int kInitImageW = 500;
        public int kInitImageH = 350;
        
        /// <summary>
        /// RT engine
        /// </summary>
        private RTCore mRT;
        private ContentManager mMeshLoader = null;
        public void SetMeshLoader(ContentManager cm) { mMeshLoader = cm; }

        /// <summary>
        /// Default command file for RT
        /// </summary>
        private String mCmdFileName = "../CommandFile/CommandFile.xml";
        private bool mNewSceneForGUI = false;

        /// <summary>
        /// Result image/mask for GUI display
        /// </summary>
        Bitmap mResultImage = null, mResultPixelCoverage = null, mResultPixelDepth = null;
        Graphics mImageGraphics, mMaskGraphics;

        /// <summary>
        /// Constructor (called from RayTraceViewer::Initialization. Build a temporary GUI frame.
        /// </summary>
        public RTWindow()
        {
            InitializeComponent();

            mRT = null;

            int w = kInitImageW;
            int h = kInitImageH;

            int totalW = 2 * w + kXOffset + 3 * kSpace;
            int totalH = h + kYOffset + 2 * kSpace;
            SetClientSizeCore(totalW, totalH);            
        }


        public RTCore GetRTCore() { return mRT; }

        /// <summary>
        /// When app quits.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (null != mRT)
                mRT.AbortRTThread();
        }


        /// <summary>
        /// Service routine for "BeginRT" Button.
        ///    1. Parse command file, quits without doing anything if no command file is defined
        ///    2. Stop existing RT engine (if one is running)
        ///    3. Allocate result image/mask buffers
        ///    4. Sets the image/mask buffers to the RT engine
        ///    5. Begins the RT engine as a separate thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeginRT(object sender, EventArgs e)
        {
            // 1. Parse command file: quit if none is there
            mReloadCmdFile_Click(sender, e);
            if (null == mRT)
                return;

            // 2. stop existing RT engine, if it is running
            StopRT(sender, e);
            
            // 3. set up GUI support based on the parsing results
            int w = mRT.ImageWidth;
            int h = mRT.ImageHeight;
               
            mImage.SetBounds(kXOffset, kYOffset, w, h);
            mImageGraphics = mImage.CreateGraphics();
            mImageGraphics.Clear(System.Drawing.Color.Beige);

            mMask.SetBounds(kXOffset + w + kSpace + kSpace, kYOffset, w, h);
            mMaskGraphics = mMask.CreateGraphics();
            mMaskGraphics.Clear(System.Drawing.Color.Beige);

 
            this.Size = new System.Drawing.Size(2*w+kXOffset+kSpace+kSpace, h+kSpace+kSpace);
            this.PerformLayout();


            int totalW = 2 * w + kXOffset + 3 * kSpace;
            int totalH = h + kYOffset + 2 * kSpace;

            if (totalW < (kInitImageW * 2))
                totalW = kInitImageW * 2;
            SetClientSizeCore(totalW, totalH);

            mResultImage = new Bitmap(w, h);
            mResultPixelCoverage = new Bitmap(w, h);
            mResultPixelDepth = new Bitmap(w, h);

            // 4. sets the storage buffer to the RT engine
            mRT.SetResultColors(mResultImage, mResultPixelCoverage, mResultPixelDepth);

            // 5. begin the RT engine
            mRT.BeginRTThread();
        }

        /// <summary>
        /// This is the service routine for the "StopRT" button.
        /// Stops the RT engine if it is running. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopRT(object sender, EventArgs e)
        {
            if (mRT != null) {
                mRT.AbortRTThread();
            }
        }

        /// <summary>
        /// Serice routine for "SaveImage" button.
        /// Save the image/mask buffers as .bmp files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveImageButton_Click(object sender, EventArgs e)
        {
            if (null == mRT) {
                mStatusArea.Text = "No Image to save!";
                return;
            }
            StopRT(sender, e);

            mSaveFileDialog.ShowDialog();
            String[] allFiles = mSaveFileDialog.FileNames;
            if (allFiles.Length > 0)
            {
                String saveFile = allFiles[0];
                mStatusArea.Text = "Saving image to: " + saveFile;
                String filePath = System.IO.Path.GetFullPath(saveFile);
                String fileName = System.IO.Path.GetFileNameWithoutExtension(saveFile);
                String extension = System.IO.Path.GetExtension(saveFile);
                String maskFile = filePath + ".Mask" + extension;
                String depthFile = filePath + ".Depth" + extension;

                mResultImage.Save(saveFile);
                mResultPixelCoverage.Save(maskFile);
                mResultPixelDepth.Save(depthFile);
            }

        }

        /// <summary>
        /// Service routine for "CmdFile" button.
        ///   1. Let's user select a command file
        ///   2. Parse the command file, create RT engine (by calling "ReloadCmdFile_Click()) function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSelectCmdFile_Click(object sender, EventArgs e)
        {
            mOpenFileDialog.ShowDialog();
            String[] allFiles = mOpenFileDialog.FileNames;
            if (allFiles.Length > 0)
            {
                mCmdFileName = allFiles[0];

                mReloadCmdFile_Click(sender, e);
            }
            else
            {
                mCmdFileNameEcho.Text = null;
                mCmdFileName = null;
            }
        }

        /// <summary>
        /// Service routine for "Reload" button.
        ///     1. Create a new RT engine to parse command file
        ///     2. Sets the Debug state to the RT engine (Debug pixel/rays)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mReloadCmdFile_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(mCmdFileName)) {
                
                StopRT(sender, e);

                String fileName = System.IO.Path.GetFileName(mCmdFileName);
                mCmdFileNameEcho.Text = fileName;
                mStatusArea.Text = "Parsing command file: " + fileName;

                if (null != mRT)
                    mRT.AbortRTThread();
                mRT = new RTCore(this, mCmdFileName, mMeshLoader);
                mRT.SetDebugPixels(DebugPixels.Checked);
                mRT.SetDebugRays(DebugRays.Checked);
                mRT.SetShowPixelInWorld(PixelInWorld.Checked);
                mRT.SetDrawDB(DrawDB.Checked);
                mRT.SetOrthoRT(OrthoRT.Checked);
                mRT.SetMultiThreadCompute(MultiThreadCompute.Checked);
                mRT.SetAnaglyph(listAnaglyph.SelectedItem.ToString());
                mNewSceneForGUI = true;

            } else {
                mStatusArea.Text = "Invalid Command File: " + mCmdFileName;
            }
        }

        /// <summary>
        /// Set/Get function for mNewSceneForGUI: this flags informs RayTraceViewer to re-build
        /// the interactive viewer (Bottom two windows)
        /// </summary>
        public void GUIUpdatedScene() { mNewSceneForGUI = false;     }
        public bool NewSceneForGUI() { return mNewSceneForGUI; }

        /// <summary>
        /// Default service routine for repaint.
        /// Echos the image/mask images to the two top sub-windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        const int skipRate = 20;
        int SkipCounter = skipRate;
        public void RTWindow_Paint(object sender, PaintEventArgs e) {
            if (null == mRT)
                return;

            if (null != mResultImage) {
                if (!this.SkipDraws.Checked || SkipCounter == 0 || mRT.CurrentY == -1) {
                    SkipCounter = skipRate;
                    lock (mRT) {
                        mImageGraphics.DrawImage(mResultImage, 0, 0);
                        if (ShowDepth.Checked)
                            mMaskGraphics.DrawImage(mResultPixelDepth, 0, 0);
                        else
                            mMaskGraphics.DrawImage(mResultPixelCoverage, 0, 0);
                    }
                }
                else
                    SkipCounter--;
            }
        }

        /// <summary>
        /// Service routine for showing/hiding debug pixels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DebugPixels_CheckedChanged(object sender, EventArgs e)
        {
            if (mRT !=  null)
                mRT.SetDebugPixels(DebugPixels.Checked);
        }

        private void PixelInWorld_CheckedChanged(object sender, EventArgs e)
        {
            if (mRT != null)
                mRT.SetShowPixelInWorld(PixelInWorld.Checked);
        }

        /// <summary>
        ///  service routine for showing/hiding debug rays
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DebugRays_CheckedChanged(object sender, EventArgs e)
        {
            if (mRT != null) 
                mRT.SetDebugRays(DebugRays.Checked);
        }


        private void DrawDB_CheckedChanged(object sender, EventArgs e)
        {
            if (mRT != null)
                mRT.SetDrawDB(DrawDB.Checked);
        }

        private void OrthoRT_CheckedChanged(object sender, EventArgs e)
        {
            if (mRT != null)
                mRT.SetOrthoRT(OrthoRT.Checked);
        }

        private void MultiThreadCompute_CheckedChanged(object sender, EventArgs e)
        {
            if (mRT != null)
            {
                mRT.SetMultiThreadCompute(MultiThreadCompute.Checked);
                DebugPixels.Checked = false;
                DebugRays.Checked = false;
                PixelInWorld.Checked = false;
                DebugRays_CheckedChanged(null, null);
                DebugPixels_CheckedChanged(null, null);
                DebugRays.Enabled = !MultiThreadCompute.Checked;
                DebugPixels.Enabled = !MultiThreadCompute.Checked;
                PixelInWorld.Enabled = !MultiThreadCompute.Checked;
            }
        }

        public TextBox StatusArea()
        {
            return mStatusArea;
        }

        // this delegate enables asynchronous calls for setting the text property on a TextBox Control
        private delegate void SetTextCallback(string text);

        public void ThreadSafeEchoToStatus(string msg)
        {
            if (mStatusArea.InvokeRequired)
            {
                // It's on a different thread, so use Invoke.
                SetTextCallback d = new SetTextCallback(SetEchoStatus);
                Invoke(d, new object[] { msg });
            }
            else
            {
                // It's on the same thread, no need for Invoke
                SetEchoStatus(msg);
            }
        }

        // This method is passed in to the SetTextCallBack delegate
        // to set the Text property of textBox1.
        private void SetEchoStatus(string text)
        {
             mStatusArea.Text = text;
             // Console.WriteLine(text);
        }

        // Sets whether image should be rendered in Anaglyph stereoscopic mode (3D)
        private void chkAnaglyph_CheckedChanged_1(object sender, EventArgs e)
        {
            if (mRT != null)
                mRT.SetAnaglyph(listAnaglyph.SelectedItem.ToString());
            if (listAnaglyph.SelectedIndex != 0)
                OrthoRT.Checked = false;
        }
    }
}