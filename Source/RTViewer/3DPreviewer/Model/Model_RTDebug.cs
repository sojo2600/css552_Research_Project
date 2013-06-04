using System;
using System.Collections.Generic;
using System.Text;
using UWBGL_XNA_Lib;
using Microsoft.Xna.Framework;

using RayTracer_552;

namespace RTViewer
{
    public partial class RTModelViewer
    {
        private UWB_SceneNode mDebugInfo = null;
            // Child node is collection of all pixels
            // PrimitiveList is all the Rays

        private RTPixelToShow mPixelsToShow = new RTPixelToShow();

        private RTPixelToShow mPixelInWorld = new RTPixelInWorld();
        
        private UWB_PrimitiveList mRaysToShow = null;
        int mShownRayX, mShownRayY;


        public void InitializeRaysToShow()
        {
            if (null != mRaysToShow)
            {
                mShownRayX = mShownRayY = 0;
                int count = mRaysToShow.count();
                for (int i = count - 1; i >= 0; i--)
                    mRaysToShow.deletePrimitiveAt(i);
            }
        }

        public void InitializePixelToShow()
        {
            mPixelsToShow.InitializePixelsToShow();
        }

        public void InitializePixelInWorld()
        {
            mPixelInWorld.InitializePixelsToShow();
        }

        internal void AddDebugPixel(RayTracer_552.RTCore rtCore)
        {
            if (null == rtCore)
                return;

            mPixelsToShow.AddDebugPixel(rtCore, mCameraPosition);
        }

        internal void AddPixelInWorld(RayTracer_552.RTCore rtCore)
        {
            if (null == rtCore)
                return;
            mPixelInWorld.AddDebugPixel(rtCore, mCameraPosition);
        }

        internal void AddDebugRays(RayTracer_552.RTCore rtCore)
        {
            if (null == rtCore)
                return;

            while ((mShownRayY < rtCore.CurrentY) ||
                    (((mShownRayY == rtCore.CurrentY) && (mShownRayX < rtCore.CurrentX))))
            {
                lock (rtCore)
                {
                    Vector3 p = new Vector3();
                    System.Drawing.Color c = new System.Drawing.Color();
                    float dist;
                    rtCore.GetPixelValues(mShownRayX, mShownRayY, out p, out c, out dist);

                    if (rtCore.DisplayDebugRays())
                    {
                        UWB_PrimitiveLine line = new UWB_PrimitiveLine();
                        Vector3 v = p - mCameraPosition;
                        float len = v.Length();
                        v /= len;
                        if (dist > (float.MaxValue / 2f))
                            dist = len;
                        p = mCameraPosition + dist * v;
                        line.setStartPoint(mCameraPosition.X, mCameraPosition.Y, mCameraPosition.Z);
                        line.setEndPoint(p.X, p.Y, p.Z);
                        line.Material.Diffuse = Vector4.Zero;
                        line.Material.Specular = Vector4.Zero;
                        line.Material.Ambient = Vector4.Zero;
                        line.Material.Emissive = Vector4.One;
                        mRaysToShow.append(line);
                    }

                    mShownRayX++;
                    if (mShownRayX >= rtCore.ImageWidth)
                    {
                        mShownRayX = 0;
                        mShownRayY++;
                    }
                }
            }
        }

        internal void AddImageFrame(RayTracer_552.RTCore rtCore)
        {
            if (null == rtCore)
                return;

            // construct the image frame
            Vector3 tl = rtCore.GetTopLeftPixelPosition();
            Vector3 tr = rtCore.GetTopRightPixelPosition();
            Vector3 bl = rtCore.GetBottomLeftPixelPosition();
            Vector3 br = rtCore.GetBottomRightPixelPosition();
            UWB_PrimitiveLine t, b, l, r;
            t = new UWB_PrimitiveLine();
            b = new UWB_PrimitiveLine();
            l = new UWB_PrimitiveLine();
            r = new UWB_PrimitiveLine();

            t.setStartPoint(tr.X, tr.Y, tr.Z);
            t.setEndPoint(tl.X, tl.Y, tl.Z);
            t.Material.Emissive = Vector4.UnitX;

            l.setStartPoint(tl.X, tl.Y, tl.Z);
            l.setEndPoint(bl.X, bl.Y, bl.Z);
            l.Material.Emissive = Vector4.UnitX;

            b.setStartPoint(br.X, br.Y, br.Z);
            b.setEndPoint(bl.X, bl.Y, bl.Z);
            b.Material.Emissive = Vector4.UnitX;

            r.setStartPoint(tr.X, tr.Y, tr.Z);
            r.setEndPoint(br.X, br.Y, br.Z);
            r.Material.Emissive = Vector4.UnitX;

            mCameraPrimitives.append(t);
            mCameraPrimitives.append(b);
            mCameraPrimitives.append(l);
            mCameraPrimitives.append(r);
        }
    }
}
