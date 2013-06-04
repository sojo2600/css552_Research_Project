using System;
using System.Drawing;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using UWBGL_XNA_Lib;

namespace UWBGL_WinForms_Lib1
{
    public class UWBGL_UIWindow: Panel 
    {
        private bool m_TimerBased;
		private UWB_IWindowHandler m_Handler;

        public UWBGL_UIWindow(bool timerbased): base()
        {
            m_TimerBased = timerbased;
            m_Handler = new UWB_IWindowHandler();
        }

        public bool ReplaceFormControl(IntPtr PlaceHolderHandle)
        {
            Width = Control.FromHandle(PlaceHolderHandle).Width;
            Height = Control.FromHandle(PlaceHolderHandle).Height;

            Control.FromHandle(PlaceHolderHandle).Controls.Add(this);
            return true;
        }

        public bool AttachHandler(UWB_IWindowHandler Handler)
        {
            if (Handler == m_Handler)
                return true;

            if (m_Handler != null)
            {
                m_Handler.ShutDownHandler();
                m_Handler = null;
            }
            bool attachSuccess = Handler.InitializeHandler(Handle);

            if(attachSuccess)
                m_Handler = Handler;
            else
                MessageBox.Show("Error attaching viewport to window!", "UWBGLE_UIWindow::AttachViewport", MessageBoxButtons.OK, MessageBoxIcon.Stop);

            return attachSuccess;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_Handler == null)
            {
                Rectangle rect = Bounds;
                e.Graphics.FillRectangle(Brushes.White, rect);
                Font drawFont = new Font("Arial", 16);
                e.Graphics.DrawString("No Graphics Handler Attached", drawFont, Brushes.Black, rect);
            }
            else
            {
                if (!m_TimerBased)
                    m_Handler.DrawGraphics();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (m_Handler != null)
            {
                m_Handler.OnMouseButton(true, e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (m_Handler != null)
            {
                m_Handler.OnMouseMove(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (m_Handler != null)
            {
                m_Handler.OnMouseButton(false, e);
            }
        }
    }
}
