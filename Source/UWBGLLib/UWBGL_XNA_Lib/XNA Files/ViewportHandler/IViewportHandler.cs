using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace UWBGL_XNA_Lib
{
    public abstract class IViewportHandler
    {
        private Matrix m_ViewMatrix, m_ProjectionMatrix;
        private Viewport m_Viewport;
        protected bool m_bVisible;

        private Rectangle m_WorldBounds;
        private Matrix m_WorldMatrix;
        private Color m_BackgroundColor;
        protected UWB_Camera m_Camera;

        public IViewportHandler(Rectangle deviceBounds, Rectangle worldBounds)
        {
            m_WorldBounds = worldBounds;

            // initialize the transformation pipeline
            m_WorldMatrix = new Matrix();
            m_ViewMatrix = new Matrix();
            m_ProjectionMatrix = new Matrix();

            m_Viewport = new Viewport();
            m_Viewport.X = deviceBounds.Left;
            m_Viewport.Y = deviceBounds.Top;
            m_Viewport.Width = deviceBounds.Width;
            m_Viewport.Height = deviceBounds.Height;

            m_BackgroundColor = new Color(new Vector4(.8f, .8f, .8f, 1f));

            m_WorldMatrix = Matrix.Identity;
            m_ViewMatrix = Matrix.CreateScale(2.0f / UWB_XNAGraphicsDevice.m_TheAPI.DeviceWidth, 2.0f / UWB_XNAGraphicsDevice.m_TheAPI.DeviceHeight, 1.0f) * Matrix.CreateTranslation(-1.0f, -1.0f, 0.0f);
            m_ProjectionMatrix = Matrix.Identity;

            m_bVisible = true;
        }

        // perform all drawing operations for this Viewport
        public virtual void BeginDraw()
        {
            UWB_XNAGraphicsDevice.m_TheAPI.BeginDraw(m_Viewport, m_BackgroundColor);
        }

        public virtual void EndDraw()
        {
            UWB_XNAGraphicsDevice.m_TheAPI.EndDraw();
        }

        public virtual void DrawGraphics() { }

        ///// Mouse Events /////
        /// The attached UWB_UI_Listener passes mouse events to its viewport. This makes dealing
        /// with mouse input much easier as you only have to subclass a single viewport class to
        /// perform custom drawing and mouse code

        /// Called when the mouse is clicked down or up on a graphics window
        /// Note: Currently, this implementation only handles left mouse button
        /// \param down [in]: true if the left mouse button is clicked down
        /// \param hwX, hwY [in]: client window coordinates of the cursor
        public virtual void OnMouseButton(bool down, int hwX, int hwY) { }

        /// Called when the mouse is moved over a graphics window
        /// Note: Currently, this implementation only handles left mouse button
        /// \param down [in]: true if the left mouse button is clicked down
        /// \param hwX, hwY [in]: client window coordinates of the cursor
        public virtual void OnMouseMove(bool down, int hwX, int hwY) { }

        /// Transform point from hardware coordinates to device coordinates
        /// \param hardware_x, hardware_y [in]: hardware coordinate to transform
        /// \param device_x, device_y [out]: resulting device coordinate
        /// return true if successful
        public virtual bool HardwareToDevice(int hardwareX, int hardwareY, out int deviceX, out int deviceY)
        {
            deviceX = hardwareX;
            deviceY = UWB_XNAGraphicsDevice.m_TheAPI.DeviceHeight - hardwareY;
            return true;
        }

        public virtual void DeviceToWorld(int dcX, int dcY, ref Vector3 wcPt, ref Vector3 wcRay)
        {
            Vector3 ecPt, wcRayPtD3D = Vector3.Zero;

            Matrix viewMat = Matrix.Identity, invView = Matrix.Identity;
            ComputeViewMatrix(ref viewMat);
            Matrix.Invert(ref viewMat, out invView);

            int deviceW, deviceH;
            deviceW = UWB_XNAGraphicsDevice.m_TheAPI.DeviceWidth;
            deviceH = UWB_XNAGraphicsDevice.m_TheAPI.DeviceHeight;

            float fov = m_Camera.FoV;
            float tanAlpha = (float)Math.Tan((fov / 2.0f));
            float tanBeta = ((float)deviceW / (float)deviceH) * tanAlpha;

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

        public virtual void SetCamera(ref UWB_Camera c)
        {
            m_Camera = c;
        }

        public virtual UWB_Camera GetCamera()
        {
            return m_Camera;
        }

        protected void LoadW2NDCXform()
        {
            int height = m_WorldBounds.Height;
            int width = m_WorldBounds.Width;
            int centerX = (int)(width / 2f) + m_WorldBounds.Left;
            int centerY = (int)(height / 2f) + m_WorldBounds.Top;

            Vector3 translate = new Vector3(-centerX, -centerY, 0f);
            Vector3 scale = new Vector3(2.0f / width, 2.0f / height, 0f);
            m_ViewMatrix = Matrix.CreateTranslation(translate) * Matrix.CreateScale(scale);
            UWB_XNAGraphicsDevice.m_TheAPI.ViewMatrix = m_ViewMatrix;

            m_WorldBounds.Width = width;
            m_WorldBounds.Height = height;
        }

        protected void ComputeViewMatrix(ref Matrix mat)
        {
            mat = Matrix.CreateLookAt(m_Camera.CameraPosition, m_Camera.CameraLookAt, m_Camera.CameraUp);

            // Now pre-concatenate the camera X and Y rotation
            float xRot = 0, yRot = 0;
            Vector3 xRef = Vector3.Zero, yRef = Vector3.Zero;
            Matrix xRotMat, yRotMat, rotMat;
            m_Camera.GetMouseRotationsInRadian(ref xRot, ref xRef, ref yRot, ref yRef);

            xRotMat = Matrix.CreateFromAxisAngle(xRef, xRot);
            yRotMat = Matrix.CreateFromAxisAngle(yRef, yRot);

            rotMat = yRotMat * xRotMat;// My * Mx
            mat = rotMat * mat;// My * Mx * rotMat (Y will be rotated first)         
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
            mat = Matrix.CreatePerspectiveFieldOfView(m_Camera.FoV, AspectRatio, m_Camera.NearClipping, m_Camera.FarClipping);
        }

        public virtual void LoadProjectionXform()
        {
            Matrix projMat = Matrix.Identity;
            //  if(projMat != null)
            ComputeProjectionMatrix(ref projMat);
            UWB_XNAGraphicsDevice.m_TheAPI.ProjectionMatrix = projMat;
        }

        public void DrawWCWindow(UWB_DrawHelper drawHelper)
        {
            drawHelper.resetAttributes();
            drawHelper.setShadeMode(eShadeMode.smFlat);
            drawHelper.setFillMode(eFillMode.fmWireframe);
            drawHelper.setColor1(new Color(new Vector3(1.0f, 0.0f, 0.0f)));
            drawHelper.drawRectangle(WCWindowMin, WCWindowMax);
        }

        public Rectangle WorldBounds
        {
            get { return m_WorldBounds; }
            set { m_WorldBounds = value; }
        }

        public Vector3 WCWindowMin
        {
            get { return new Vector3((float)m_WorldBounds.Left, (float)m_WorldBounds.Top, 0f); }
            set
            {
                m_WorldBounds.X = (int)value.X;
                m_WorldBounds.Y = (int)value.Y;
            }
        }

        public Vector3 WCWindowMax
        {
            get { return new Vector3((float)m_WorldBounds.Right, (float)m_WorldBounds.Bottom, 0f); }
            set
            {
                m_WorldBounds.Width = (int)value.X - m_WorldBounds.X;
                m_WorldBounds.Height = (int)value.Y - m_WorldBounds.Y;
            }
        }

        public UWB_BoundingBox WCWindow
        {
            get { return new UWB_BoundingBox(WCWindowMin, WCWindowMax); }
            set { WCWindowMin = value.getMin(); WCWindowMax = value.getMax(); }
        }

        public virtual float AspectRatio
        {
            get { return (float)m_Viewport.Width / (float)m_Viewport.Height; }
        }

        public virtual Color BackgroundColor
        {
            get { return m_BackgroundColor; }
            set { m_BackgroundColor = value; }
        }

        public virtual bool Visible
        {
            get { return m_bVisible; }
            set { m_bVisible = value; }
        }

        public virtual Rectangle Bounds
        {
            get { return new Rectangle(m_Viewport.X, m_Viewport.Y, m_Viewport.Width, m_Viewport.Height); }
        }

        public virtual Viewport Viewport
        {
            get { return m_Viewport; }
        }

    }
}
