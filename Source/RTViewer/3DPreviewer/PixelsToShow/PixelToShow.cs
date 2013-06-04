using System;
using System.Collections.Generic;
using System.Text;
using UWBGL_XNA_Lib;
using Microsoft.Xna.Framework;

using RayTracer_552;

namespace RTViewer
{
    internal class RTPixelToShow
    {
        private UWB_SceneNode mPixelsToShow = null;
        int mShownPixelX, mShownPixelY;
        int mNumPixelNodesUsed = 0; // re-use all the pixel scene nodes, tells us how many has been used
        const float kPixelSize = 0.05f;

        public RTPixelToShow()
        {
            mPixelsToShow = new UWB_SceneNode();
            InitializePixelsToShow();
        }

        public void InitializePixelsToShow()
        {
            if (null != mPixelsToShow)
            {
                mNumPixelNodesUsed = 0;
                mShownPixelX = mShownPixelY = 0;
                int count = mPixelsToShow.numChildren();
                for (int i = count - 1; i >= 0; i--)
                {
                    UWB_SceneNode n = mPixelsToShow.getChildNode(i);
                    n.getPrimitive().setVisible(false);
                }
            }
        }

        public UWB_SceneNode GetAllPixels() { return mPixelsToShow; }


        private UWB_Primitive CreateSphereMesh()
        {
            UWB_XNAPrimitiveMesh m = new UWB_XNAPrimitiveMesh("sphere");
            m.EnableTexturing(false);
            m.EnableLighting(true);
            return m;
        }


        private void ShowOneDebugPixelAt(Vector3 p, System.Drawing.Color c)
        {
            if (mNumPixelNodesUsed >= mPixelsToShow.numChildren())
            {
                // need to create new ones
                UWB_Primitive prim = CreateSphereMesh();
                prim.EnableLighting(false);
                UWB_SceneNode pNode = new UWB_SceneNode();
                pNode.setPrimitive(prim);
                UWB_XFormInfo xf = pNode.getXFormInfo();
                xf.SetTranslation(p);
                xf.SetScale(new Vector3(kPixelSize));
                pNode.setXFormInfo(xf);
                prim.Material.Diffuse = Vector4.Zero;
                prim.Material.Specular = Vector4.Zero;
                prim.Material.Ambient = Vector4.Zero;
                prim.Material.Emissive = new Vector4(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, 1f);
                mPixelsToShow.insertChildNode(pNode);
            }
            else
            {
                // there are more to be reused ...
                UWB_SceneNode n = mPixelsToShow.getChildNode(mNumPixelNodesUsed);
                UWB_XFormInfo xf = n.getXFormInfo();
                xf.SetTranslation(new Vector3(p.X, p.Y, p.Z));
                UWB_Primitive prim = n.getPrimitive();
                prim.setVisible(true);
                prim.Material.Diffuse = Vector4.Zero;
                prim.Material.Specular = Vector4.Zero;
                prim.Material.Ambient = Vector4.Zero;
                prim.Material.Emissive = new Vector4(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, 1f);
            }
        }

        protected virtual bool GetPixelPosition(ref Vector3 p, Vector3 cameraPos, float dist) { return true; }

        public void AddDebugPixel(RayTracer_552.RTCore rtCore, Vector3 cameraPos)
        {
            if (null == rtCore)
                return;

            while ((mShownPixelY < rtCore.CurrentY) ||
                    (((mShownPixelY == rtCore.CurrentY) && (mShownPixelX < rtCore.CurrentX))))
            {
                lock (rtCore)
                {
                    Vector3 p = new Vector3();
                    System.Drawing.Color c = new System.Drawing.Color();
                    float dist;
                    rtCore.GetPixelValues(mShownPixelX, mShownPixelY, out p, out c, out dist);

                    GetPixelPosition(ref p, cameraPos, dist);
                    {
                        ShowOneDebugPixelAt(p, c);
                        mNumPixelNodesUsed++;
                    }
                
                    mShownPixelX++;
                    if (mShownPixelX >= rtCore.ImageWidth)
                    {
                        mShownPixelX = 0;
                        mShownPixelY++;
                    }
                }
            }
        }
    }
}
