using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using UWBGL_XNA_Lib;
using Microsoft.Xna.Framework;

namespace RTViewer
{
    public class DrawAndMouseHandler : DrawOnlyHandler
    {
        public DrawAndMouseHandler()
            :base() { }

        public override void OnMouseButton(bool down, MouseEventArgs e)
        {
            int deviceX = 0, deviceY = 0;
            HardwareToDevice(e.X, e.Y, ref deviceX, ref deviceY);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (down) 
                    {
                        m_Camera.BeginMouseRotate(deviceX, deviceY);
                    }
                    break;
                case MouseButtons.Middle:
                    if (down)
                    {
                        m_Camera.BeginMouseTrack(deviceX, deviceY);
                    }
                    break;
                case MouseButtons.Right:
                    if (down)
                    {
                        m_Camera.BeginMouseZoom(deviceX);
                    }
                    break;
            }
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            int deviceX = 0, deviceY = 0;
            HardwareToDevice(e.X, e.Y, ref deviceX, ref deviceY);

            switch (e.Button)
            {
                case MouseButtons.Left:
                     m_Camera.MouseRotate(deviceX, deviceY);
                    break;
                case MouseButtons.Middle:
                    m_Camera.MouseTrack(deviceX, deviceY);
                    break;
                case MouseButtons.Right:
                    m_Camera.MouseZoom(deviceX);
                    break;
            }
        }

        public override void DrawGraphics()
        {
             BeginScene();
                // 2D: LoadW2NDCXform();
                LoadViewXform();
                LoadProjectionXform();

                Program.GetModel().DrawModel(true /* draw the camera */ );

             EndSceneAndShow();
        }
    }
}
