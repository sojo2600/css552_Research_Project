using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UWBGL_XNA_Lib
{
    public class UWB_IWindowHandler
    {
        public virtual bool IsGraphicsDeviceValid() { return false; }
        public virtual bool InitializeHandler(IntPtr h_AttachedWindow) { return false; }

        public virtual void ShutDownHandler() { }

        public virtual void BeginScene() { }
        public virtual void EndSceneAndShow() { }
        public virtual void DrawGraphics() { }

        public virtual void OnMouseButton(bool down, MouseEventArgs e) { }
        public virtual void OnMouseMove(MouseEventArgs e) { }
    }
}
