using System;
using System.Collections.Generic;
using System.Text;
using UWBGL_WinForms_Lib1;
using UWBGL_XNA_Lib;
using Microsoft.Xna.Framework;
using System.Windows.Forms;


namespace RTViewer
{
    public class DrawOnlyHandler : UWB_XNAWindowHandler
    {
        private UWBGL_UIWindow mWindow;

        public DrawOnlyHandler()
        {
            mWindow = new UWBGL_UIWindow(true);

            m_Camera.CameraPosition = new Vector3(10, 10, 25);
            m_Camera.CameraLookAt = Vector3.Zero;
            m_Camera.CameraUp = Vector3.Up;
            m_Camera.FarClipping = 50.0f;
            m_Camera.NearClipping = 0.1f;
            m_Camera.FoV = (float)Math.PI / 4f;
        }

        public bool Initialize(Control PlaceHolder)
        {
            if (!mWindow.ReplaceFormControl(PlaceHolder.Handle))
                return false;

            if (!mWindow.AttachHandler(this))
                return false;

            return true;
        }


        public override void DrawGraphics()
        {
            BeginScene();
                // for 2D: LoadW2NDCXform();
                LoadViewXform();
                LoadProjectionXform();

                Program.GetModel().DrawModel(false /* don't draw the camera */ );

            EndSceneAndShow();
        }
    }
}