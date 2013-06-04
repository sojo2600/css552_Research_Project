using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

using RayTracer_552;
using UWBGL_XNA_Lib;

namespace RTViewer
{
    public partial class RTViewer : Form
    {
        private RTWindow mRTWindows;    // Windows showing RT results
        private DrawAndMouseHandler mInteractiveWindow; // For Interactive viewing
        private DrawOnlyHandler mRTPreviewWindow;       // For Previewing RT 

        public RTViewer()
        {
            InitializeComponent();
            mRTWindows = new RTWindow();
            RTContainer.Controls.Add(mRTWindows);
        }

        private void RTViewer_Load(object sender, EventArgs e)
        {
            mInteractiveWindow = new DrawAndMouseHandler();
            mInteractiveWindow.Initialize(mInteractiveViewArea);
            mInteractiveWindow.GetCamera().CameraLookAt = Vector3.Zero;
            mInteractiveWindow.GetCamera().CameraPosition = new Vector3(-30, 30, -10);
            mInteractiveWindow.GetCamera().FoV = (float)Math.PI / 4f;
            mInteractiveWindow.GetCamera().NearClipping = 0.5f;
            mInteractiveWindow.GetCamera().FarClipping = 150.0f;

            mRTPreviewWindow = new DrawOnlyHandler();
            mRTPreviewWindow.Initialize(mRTPreviewArea);
            mRTPreviewWindow.GetCamera().CameraLookAt = Vector3.Zero;
            mRTPreviewWindow.GetCamera().CameraPosition = new Vector3(0, 5, 10);
            mRTPreviewWindow.GetCamera().FoV = (float)Math.PI / 4f;
            mRTPreviewWindow.GetCamera().NearClipping = 0.5f;
            mRTPreviewWindow.GetCamera().FarClipping = 100.0f;

            UWB_XNAGraphicsDevice.m_TheAPI.Initialize();
            mRTWindows.SetMeshLoader(UWB_XNAGraphicsDevice.m_TheAPI.mResources);

            mTimer.Start();
            mRTWindows.Show();
        }

        private void mTimer_Tick(object sender, EventArgs e)
        {
            if (null != mRTWindows.GetRTCore())
            {
                if (mRTWindows.NewSceneForGUI())
                {
                    RTCamera c = mRTWindows.GetRTCore().GetCamera();
                    mRTPreviewWindow.GetCamera().CameraLookAt = c.AtPosition;
                    mRTPreviewWindow.GetCamera().CameraPosition = c.EyePosition;
                    mRTPreviewWindow.GetCamera().FoV = MathHelper.ToRadians(c.FOV);
                    mRTPreviewWindow.GetCamera().NearClipping = 0.5f;
                    mRTPreviewWindow.GetCamera().FarClipping = 100.0f;

                    Program.GetModel().AddRTScene(c, mRTWindows.GetRTCore().GetSceneDatabase());
                    Program.GetModel().AddImageFrame(mRTWindows.GetRTCore());
                    Program.GetModel().AddRTKdTree(mRTWindows.GetRTCore().GetKdTreeRoot());

                    this.PerformLayout();
                    mRTWindows.GUIUpdatedScene();
                }
                else
                {
                    if (mRTWindows.GetRTCore().DisplayDebugPixels())
                        Program.GetModel().AddDebugPixel(mRTWindows.GetRTCore());
                    else
                        Program.GetModel().InitializePixelToShow();

                    if (mRTWindows.GetRTCore().DisplayDebugRays())
                        Program.GetModel().AddDebugRays(mRTWindows.GetRTCore());
                    else
                        Program.GetModel().InitializeRaysToShow();

                    if (mRTWindows.GetRTCore().ShowPixelInWorld())
                        Program.GetModel().AddPixelInWorld(mRTWindows.GetRTCore());
                    else
                        Program.GetModel().InitializePixelInWorld();

                    Program.GetModel().SetDrawDB(mRTWindows.GetRTCore().DrawDB());

                    mRTWindows.RTWindow_Paint(null, null);
                }
            }

            if ((null != mRTWindows.GetRTCore()) && (!mRTWindows.GetRTCore().RTThreadRunning()))
            {
                if (XNAUIDraw.Checked)
                {
                mInteractiveWindow.DrawGraphics();
                mRTPreviewWindow.DrawGraphics();
                }
            }
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
