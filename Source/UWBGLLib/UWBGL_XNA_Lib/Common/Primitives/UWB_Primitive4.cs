using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Storage;

namespace UWBGL_XNA_Lib
{
    public class UWB_Primitive
    {
        protected bool mVisible;
        protected Vector3 mVelocity;
        protected Color mFlatColor;
        protected Color mShadingColor;
        protected eFillMode mFillMode;
        protected eShadeMode mShadeMode;
        protected float mPointSize;
        protected bool mDrawBoundingVolume;
        protected bool m_bBlendingEnabled;
        protected bool m_bLightingEnabled;
        protected bool m_bTexturingEnabled;
        protected String m_TexFileName;
        protected UWB_Material m_Material;

        public UWB_Primitive()
        {
            mVisible = true;
            mVelocity = Vector3.Zero;
            mFlatColor = Color.Black;
            mShadingColor = Color.White;
            mFillMode = eFillMode.fmSolid;
            mShadeMode = eShadeMode.smGouraud;
            mPointSize = 2f;
            m_bBlendingEnabled = false;
            m_bLightingEnabled = true;
            m_Material = new UWB_Material();
        }

        public void Draw(eLevelofDetail lod, UWB_DrawHelper drawHelper)
        {
            if (!mVisible)
                return;

            SetupDrawAttributes(drawHelper);

            DrawPrimitive(lod, drawHelper);

            if (isDrawingBoundingVolume())
                drawBoundingVolume(lod, drawHelper, Color.Red);
        }

        public virtual void Update(float elapsedSeconds)
        {

        }

        public virtual void MouseDownVertex(int vertexID, float x, float y)
        {

        }

        public virtual void MoveTo(float x, float y)
        {

        }

        public virtual Vector3 getLocation()
        {
            return Vector3.Zero;
        }

        public void setVisible(bool visible)
        {
            mVisible = visible;
        }

        public bool isVisible()
        {
            return mVisible;
        }

        public void setVelocity(Vector3 velocity)
        {
            mVelocity = velocity;
        }

        public Vector3 getVelocity()
        {
            return mVelocity;
        }

        public bool isStationary()
        {
            return (mVelocity.X <= UWB_Utility.zeroTolerance
                    && mVelocity.Y <= UWB_Utility.zeroTolerance
                    && mVelocity.Z <= UWB_Utility.zeroTolerance);
        }

        public void setFlatColor(Color color)
        {
            mFlatColor = color;
        }

        public Color getFlatColor()
        {
            return mFlatColor;
        }

        public void setShadingColor(Color color)
        {
            mShadingColor = color;
        }

        public Color getShadingColor()
        {
            return mShadingColor;
        }

        public void setFillMode(eFillMode fillMode)
        {
            mFillMode = fillMode;
        }

        public eFillMode getFillMode()
        {
            return mFillMode;
        }

        public void setShadeMode(eShadeMode shadeMode)
        {
            mShadeMode = shadeMode;
        }

        public eShadeMode getShadeMode()
        {
            return mShadeMode;
        }

        public void setPointSize(float size)
        {
            mPointSize = size;
        }

        public float getPointSize()
        {
            return mPointSize;
        }

        protected virtual void SetupDrawAttributes(UWB_DrawHelper drawHelper)
        {
            drawHelper.resetAttributes();
            drawHelper.setColor1(mFlatColor);
            
            drawHelper.SetMaterial(ref m_Material);
            drawHelper.EnableBlending(m_bBlendingEnabled);
            drawHelper.EnableLighting(m_bLightingEnabled);

            drawHelper.SetTextureInfo(ref m_TexFileName);
            if (m_TexFileName != null)
                drawHelper.EnableTexture(m_bTexturingEnabled);


        }

        protected virtual void DrawPrimitive(eLevelofDetail lod, UWB_DrawHelper drawHelper)
        {

        }

        public virtual UWB_BoundingVolume getBoundingVolume(eLevelofDetail lod)
        {
            return null;
        }

        public virtual void collisionResponse(UWB_Primitive other, Vector3 otherLocation)
        {
            Vector3 direction = getLocation() - otherLocation;
            if (direction == Vector3.Zero)
            {
                setVelocity(Vector3.Zero);
                return;
            }

            direction.Normalize();

            float mySpeed = getVelocity().Length();

            setVelocity(mySpeed * direction);
        }

        public void drawBoundingVolume(eLevelofDetail lod, UWB_DrawHelper drawHelper, Color color)
        {
            UWB_BoundingVolume boundingVolume = getBoundingVolume(lod);
            if (boundingVolume != null && drawHelper != null)
            {
                drawHelper.resetAttributes();
                drawHelper.setColor1(color);
                drawHelper.setShadeMode(eShadeMode.smFlat);
                drawHelper.setFillMode(eFillMode.fmWireframe);
                boundingVolume.Draw(ref drawHelper);
            }
        }

        public void setDrawBoundingVolume(bool on)
        {
            mDrawBoundingVolume = on;
        }

        public bool isDrawingBoundingVolume()
        {
            return mDrawBoundingVolume;
        }

        public void EnableBlending(bool on)
        {
            m_bBlendingEnabled = on;
        }
        public bool IsBlendingEnabled()
        {
            return m_bBlendingEnabled;
        }

        public void SetTextureFileName(String n)
        {
            m_TexFileName = n;
        }
        public String GetTextureFileName()
        {
            return m_TexFileName;
        }

        public void EnableTexturing(bool on)
        {
            m_bTexturingEnabled = on;
        }

        public bool IsTexturingEnabled()
        {
            return m_bTexturingEnabled;
        }

        public UWB_Material Material
        {
            get
            {
                return m_Material;
            }
            set
            {
                m_Material = value;
            }
        }

        public void EnableLighting(bool on)
        {
            m_bLightingEnabled = on;
        }

        public bool IsLightingEnabled()
        {
            return m_bLightingEnabled;
        }

    }
}
