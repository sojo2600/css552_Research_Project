using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UWBGL_XNA_Lib
{
    public class UWB_XNAWindowHandler:UWB_WindowHandler
    {
        protected GraphicsDevice m_XNA_GraphicsDevice;
        protected PresentationParameters pp;

        public override bool IsGraphicsDeviceValid()
        {
            return (m_XNA_GraphicsDevice != null);
        }

        public override bool InitializeHandler(IntPtr hAttachedWindow)
        {
            m_XNA_GraphicsDevice = null;
            pp = null;

            if (!base.InitializeHandler(hAttachedWindow))
                return false;

            UWB_XNAGraphicsDevice.m_TheAPI.CreateGraphicsContext(hAttachedWindow, ref pp);
            m_XNA_GraphicsDevice = UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice;
            return true;
        }

        public override void BeginScene()
        {
            if (m_XNA_GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal)
            {
                UWB_XNAGraphicsDevice.m_TheAPI.BeginScene(pp);
            }
        }

        public override void EndSceneAndShow()
        {
            if (m_XNA_GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal)
                UWB_XNAGraphicsDevice.m_TheAPI.EndSceneAndShow(pp);
        }

		public override void LoadW2NDCXform()
		{
			Matrix world2ndc;
			Matrix toSize;
			Matrix toPlace;

			Vector3 center = m_WCWindow.getCenter();

			toPlace = Matrix.CreateTranslation(-center);
			toSize = Matrix.CreateScale(2.0f / m_WCWindow.width(), 2.0f / m_WCWindow.height(), 1.0f);
			world2ndc = toPlace * toSize;

            UWB_XNAGraphicsDevice.m_TheAPI.ViewMatrix = world2ndc;
		}
        
        protected void ComputeViewMatrix(ref Matrix mat)
        {   
          mat = Matrix.CreateLookAt(m_Camera.CameraPosition,m_Camera.CameraLookAt,m_Camera.CameraUp);       
        }

        public virtual void LoadViewXform()
        {
           Matrix viewMat = Matrix.Identity;
           // if(viewMat != null)
                ComputeViewMatrix(ref viewMat);
           UWB_XNAGraphicsDevice.m_TheAPI.ViewMatrix = viewMat;
        }

        protected void ComputeProjectionMatrix(ref Matrix mat)
        {
            mat = Matrix.CreatePerspectiveFieldOfView(m_Camera.FoV,getAspectRatio(),m_Camera.NearClipping,m_Camera.FarClipping);
        }

        public virtual void LoadProjectionXform()
        {
          Matrix projMat = Matrix.Identity;
          //  if(projMat != null)
                ComputeProjectionMatrix(ref projMat);
           UWB_XNAGraphicsDevice.m_TheAPI.ProjectionMatrix = projMat;
        }

        public override void DeviceToWorld(int dcX, int dcY, ref Vector3 wcPt, ref Vector3 wcRay)
        {
          Vector3 ecPt, wcRayPtD3D = Vector3.Zero;

          Matrix viewMat = Matrix.Identity, invView = Matrix.Identity;
          ComputeViewMatrix(ref viewMat);
          Matrix.Invert(ref viewMat,out invView);
          
          int deviceW, deviceH;
          deviceW = Control.FromHandle(m_hAttachedWindow).Size.Width;
          deviceH = Control.FromHandle(m_hAttachedWindow).Size.Height; 
          
          float fov = m_Camera.FoV;
          float tanAlpha = (float)Math.Tan((fov/2.0f));
          float tanBeta = ((float)deviceW/(float)deviceH) * tanAlpha;

          ecPt.X = (((2.0f * (float)dcX) / (float)deviceW) - 1f) * tanBeta;
          ecPt.Y = (((2.0f * (float)dcY) / (float)deviceH) - 1f) * tanAlpha;
	      ecPt.Z = -1.0f;

          //transform ray to 3d space
          wcRayPtD3D.X = ecPt.X * invView.M11 + ecPt.Y * invView.M21 + ecPt.Z * invView.M31;
          wcRayPtD3D.X = ecPt.X * invView.M12 + ecPt.Y * invView.M22 + ecPt.Z * invView.M32;
          wcRayPtD3D.X = ecPt.X * invView.M13 + ecPt.Y * invView.M23 + ecPt.Z * invView.M33;

          wcPt.X = invView.M41;
          wcPt.Y = invView.M42;
          wcPt.Z = invView.M43;

          wcRay.X = wcRayPtD3D.X - wcPt.X;
          wcRay.Y = wcRayPtD3D.Y - wcPt.Y;
          wcRay.Z = wcRayPtD3D.Z - wcPt.Z;
        }
    }
}
