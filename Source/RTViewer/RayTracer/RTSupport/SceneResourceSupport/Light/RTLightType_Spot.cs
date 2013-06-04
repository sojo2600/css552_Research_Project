using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    ///    A spot light.
    /// </summary>
    /// 
    public partial class RTLightType_Spot : RTLightType
    {
        private Vector3 mDirection;
        private float mInnerAngle;  // in radian
        private float mCosInAngle;  // Actually, this is half of the angle
        private float mOuterAngle;  // in radian
        private float mCosOutAngle; // Actually, this is half of the angle
        private float mFallOff;
        private float mFallOffDenominator;
        private bool mUseDepthMap = false; // off

        /// <summary>
        /// Constructor from parser. 
        /// Please DO NOT change the parsing routine unless you know what you are doing).
        /// </summary>
        /// <param name="parser"></param>
        public RTLightType_Spot(CommandFileParser parser)
        {
            mLightSourceType = RTLightSourceType.RTLightSourceTypeSpot;

            while (!parser.IsEndElement("light"))
            {
                if (parser.IsElement() && (!parser.IsElement("light")))
                {
                    if (parser.IsElement("color"))
                        mColor = parser.ReadVector3();
                    else if (parser.IsElement("position"))
                        mPosition = parser.ReadVector3();
                    else if (parser.IsElement("direction"))
                        mDirection = parser.ReadVector3();
                    else if (parser.IsElement("innerAngle"))
                        mInnerAngle = MathHelper.ToRadians(parser.ReadFloat());
                    else if (parser.IsElement("outerAngle"))
                        mOuterAngle = MathHelper.ToRadians(parser.ReadFloat());
                    else if (parser.IsElement("falloff"))
                        mFallOff = parser.ReadFloat();
                    else if (parser.IsElement("depthMap"))
                        mUseDepthMap = parser.ReadBool();
                    else if (parser.IsElement("depthMapResolution"))
                        mRes = parser.ReadInt();
                    else if (parser.IsElement("depthMapFilterRes"))
                        mFilterRes = parser.ReadInt();
                    else
                        parser.ParserError("Light");
                }
                else
                    parser.ParserRead();
            }

            mDirection = -Vector3.Normalize(mDirection);
            mCosInAngle = (float) Math.Cos(mInnerAngle / 2);
            mCosOutAngle = (float)Math.Cos(mOuterAngle / 2);
            mFallOffDenominator = 1f / (mCosInAngle - mCosOutAngle);
        }

        public override void InitializeLight(SceneDatabase sceneDatabase)
        {
            InitDepthMap(sceneDatabase);
        }

        private bool InLightCone(Vector3 visiblePt, ref float cosAlpha)
        {
            Vector3 L = Vector3.Normalize(mPosition - visiblePt);
            cosAlpha = Vector3.Dot(L, mDirection);
            return ((cosAlpha > 0) && (cosAlpha > mCosOutAngle));
        }

        /// <summary>
        /// Returns the normalized L vector for illumination computation.
        /// </summary>
        /// <param name="visiblePt"></param>
        /// <returns></returns>
        public override Vector3 GetNormalizedDirection(Vector3 visiblePt) { 
            return Vector3.Normalize(mPosition - visiblePt); }

        /// <summary>
        /// Given the point to be light, returns the color form the light source towards the point
        /// </summary>
        /// <param name="visiblePt"></param>
        /// <returns></returns>
        public override Vector3 GetColor(Vector3 visiblePt) 
        {
            Vector3 useColor = Vector3.Zero;
            float cosAlpha = 0f;

            if (InLightCone(visiblePt, ref cosAlpha)) 
            {
                float frac = 1f;
                if (cosAlpha < mCosInAngle)
                {
                    frac = (cosAlpha - mCosOutAngle) * mFallOffDenominator;
                    frac = (float)Math.Pow(frac, mFallOff);
                }
                useColor = frac * mColor;
            }
            return useColor;
        }

        /// <summary>
        /// How much of the spot light is visible from the visiblePt
        /// </summary>
        /// <param name="visiblePt"></param>
        /// <param name="exceptGeomIndex">this geometry can never block the light (geomIndex of the visiblePt)</param>
        /// <param name="database"></param>
        /// <returns></returns>
        public override float PercentVisible(Vector3 visiblePt, int exceptGeomIndex, SceneDatabase sceneDatabase)
        {
            float cosAlpha = 0f;
            bool canIllum = InLightCone(visiblePt, ref cosAlpha);
            float percent = 0f;

            if (canIllum)
            {
                if (mUseDepthMap)
                {
                    percent = SampleDepthMap(visiblePt, exceptGeomIndex);
                }
                else
                    percent = base.PercentVisible(visiblePt, exceptGeomIndex, sceneDatabase);
            }
            return percent;
        }

    }
}
