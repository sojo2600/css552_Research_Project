using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace UWBGL_XNA_Lib
{
    public enum eLevelofDetail { lodLow, lodMed, lodHigh };
	public enum eShadeMode { smFlat, smGouraud };
	public enum eFillMode { fmPoint, fmWireframe, fmSolid };

    public class UWB_DrawHelper
    {
        protected eLevelofDetail mLod;
        protected eShadeMode mShadeMode;
        protected eFillMode mFillMode;
        protected UWB_MatrixStack m_MatrixStack;

        protected float mPointSize;
        protected Color mColor1;
        protected Color mColor2;
        public String m_TexFileName;
        public bool m_bTexturingEnabled;
        public bool m_bLightingEnabled;
        public bool m_bBlendingEnabled;
        protected UWB_Material m_Material;

        public UWB_DrawHelper()
        {
            mLod = eLevelofDetail.lodHigh;
            mShadeMode = eShadeMode.smGouraud;
            mFillMode = eFillMode.fmSolid;
            m_TexFileName = null;
            resetAttributes();
            m_MatrixStack = new UWB_MatrixStack();
        }

        public void resetAttributes()
        {
            mPointSize = 1.0f;
            mColor1 = Color.Black;
            mColor2 = Color.Black;
            EnableBlending(false);
            EnableTexture(false);
        }

        public void setLod(eLevelofDetail lod)
        {
            mLod = lod;
        }

        public eLevelofDetail getLod()
        {
            return mLod;
        }

        public virtual void setShadeMode(eShadeMode shadeMode)
        {
            mShadeMode = shadeMode;
        }

        public eShadeMode getShadeMode()
        {
            return mShadeMode;
        }

        public virtual void setFillMode(eFillMode fillMode)
        {
            mFillMode = fillMode;
        }

        public eFillMode getFillMode()
        {
            return mFillMode;
        }

        public Color setColor1(Color color1)
        {
            Color oldColor = mColor1;
            mColor1 = color1;
            return oldColor;
        }

        public Color setColor2(Color color2)
        {
            Color oldColor = mColor2;
            mColor2 = color2;
            return oldColor;
        }

        public float setPointSize(float ptSize)
        {
            float oldPtSize = mPointSize;
            mPointSize = ptSize;
            return oldPtSize;
        }

        /// Set the current texture that will be used for mapping
        /// to  primitive (if textureing is enabled)
        /// \param texFile The path to the image file used for texture mapping
        ///        If NULL is passed, then there is no current texture
        public virtual void SetTextureInfo(ref String texFile)
        {
            m_TexFileName = texFile;
        }

        /// Turn on/off blending. Blending makes primitive object colors
        /// blend with the existing color in the color buffer
        public virtual bool EnableBlending(bool on) { return false; }

        /// Turn on/off texture mapping. A texture must be assigned if this is
        /// turned on in order to get texture mapping to work
        public virtual bool EnableTexture(bool on) { return false; }

        /// Turn on/off lighting computation. 
        public virtual bool EnableLighting(bool on) { return false; }

        public virtual UWB_Material SetMaterial(ref UWB_Material m)
        {
            UWB_Material old = m_Material;
            m_Material = m;
            return old;
        }

        // Subclasses return true if they support the draw operation
        // The base class just always returns false for all draw functions
        public virtual bool drawPoint(Vector3 position)
        {
            return false;
        }

        public virtual bool drawLine(Vector3 start, Vector3 end)
        {
            return false;
        }

        public virtual bool drawCircle(Vector3 center, float radius)
        {
            return false;
        }

        public virtual bool drawRectangle(Vector3 corner1, Vector3 corner2)
        {
            return false;
        }

        public virtual bool drawRectangleXZ(Vector3 corner1, Vector3 corner2)
        {
            return false;
        }

        public virtual bool drawTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return false;
        }

        public virtual bool accumulateModelTransform(
           Vector3 translation,
           Vector3 scale,
           Matrix rotation,
           Vector3 pivot)
        {
            return false;
        }


        public virtual bool accumulateModelTransform(
            Vector3 translation,
            Vector3 scale,
            Vector3 rotationRadians,
            Vector3 pivot)
        {
            return false;
        }
        
        public virtual bool accumulateModelTransform(
            Vector3 translation,
            Vector3 scale,
            float rotationRadians,
            Vector3 rotation_axis,
            Vector3 pivot)
        {
            return false;
        }

        public virtual bool pushModelTransform()
        {
            return false;
        }

        public virtual bool popModelTransform()
        {
            return false;
        }

        public virtual bool initializeModelTransform()
        {
            return false;
        }

        public virtual bool transformPoint(ref Vector3 point)
        {
            return false;
        }

        public virtual UWB_MatrixStack MatrixStack
        {
            get
            {
                return m_MatrixStack;
            }
        }

    }
        
}
