using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace UWBGL_XNA_Lib
{
    public class UWB_WindowHandler: UWB_IWindowHandler
    {
        protected IntPtr m_hAttachedWindow;
		protected UWB_BoundingBox m_WCWindow;
        protected UWB_Camera m_Camera;

		public UWB_WindowHandler()
		{
			m_WCWindow = new UWB_BoundingBox();
            m_Camera = new UWB_Camera();
		}

        public override bool InitializeHandler(IntPtr hAttachedWindow)
        {
            if (!Control.FromHandle(hAttachedWindow).IsHandleCreated)
                return false;

            m_hAttachedWindow = hAttachedWindow;
            return true;
        }

        public bool HardwareToDevice(int hardwareX, int hardwareY, ref int deviceX, ref int deviceY)
        {
            if (!Control.FromHandle(m_hAttachedWindow).IsHandleCreated)
                return false;

            deviceX = hardwareX;
            deviceY = Control.FromHandle(m_hAttachedWindow).Height - hardwareY;
            return true;
        }

        public float getAspectRatio()
        {
            if (!Control.FromHandle(m_hAttachedWindow).IsHandleCreated)
                return 0.0f;

            Size size = Control.FromHandle(m_hAttachedWindow).Size;

            return (float)size.Width / (float)size.Height;
        }

		public void setWCWindow(ref UWB_BoundingBox w)
		{
			m_WCWindow = w;
		}

		public virtual void LoadW2NDCXform()
		{
			return;
		}

        public virtual void DeviceToWorld(int dcX, int dcY, ref Vector3 wcPt, ref Vector3 wcRay) 
        { 
          wcPt.X = wcPt.Y = wcPt.Z = 0.0f;
          wcRay.X = wcRay.Y = wcRay.Z = 0.0f;
        }

        public virtual void SetCamera(ref UWB_Camera c)
        {
          m_Camera = c;
        }

        public virtual UWB_Camera GetCamera() 
        {
          return m_Camera;
        }
    }
}
