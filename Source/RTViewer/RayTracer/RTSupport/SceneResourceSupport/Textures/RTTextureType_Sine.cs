using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// Sine function over along U axis, V direction says how many to repeat
    ///     
    ///     Color1/Color2: default to be: White/Black
    /// </summary>
    public class RTTextureType_Sine : RTTextureType 
    {
        private float mPeriod;      // how many periods to be fitted within 0 to 1 U
        private Vector2 mDirection; // in UV space, direction of the sine function

        private float mThetaRange;  // range from 0 to this value (mPeroid * 2PI)

        private Vector3 mColor1 = Vector3.One;  // Checker colors
        private Vector3 mColor2 = Vector3.Zero;

        /// <summary>
        /// Constrcuts from the commandfile.
        /// DO NOT change the pasing loop unless you know what you are doing.
        /// </summary>
        /// <param name="parser"></param>
        public RTTextureType_Sine(CommandFileParser parser)
        {
            mPeriod = 1f;
            mDirection = Vector2.UnitX;

            while (!parser.IsEndElement("texture"))
            {
                if (parser.IsElement() && (!parser.IsElement("texture")))
                {
                    if (parser.IsElement("period"))
                        mPeriod = parser.ReadFloat();
                    else if (parser.IsElement("direction"))
                        mDirection = parser.ReadVector2();
                    else if (parser.IsElement("color1"))
                        mColor1 = parser.ReadVector3();
                    else if (parser.IsElement("color2"))
                        mColor2 = parser.ReadVector3();
                    else
                        parser.ParserError("TextureType_Sine");
                }
                else
                    parser.ParserRead();
            }
            mThetaRange = mPeriod * (float) Math.PI * 2f;
            mDirection = Vector2.Normalize(mDirection);
        }

        /// <summary>
        /// </summary>
        /// <param name="u">value between 0 to 1</param>
        /// <param name="v">value between 0 to 1</param>
        /// <param name="rec">IntersectionRecord to be texture mapped</param>
        /// <param name="g">the geometry</param>
        /// <returns></returns>
        public override Vector3 GetTexile(float u, float v, IntersectionRecord rec, RTGeometry g)
        {
            float useU = u * mDirection.X + v * mDirection.Y;
            float theta = useU * mThetaRange;
            float sineV = 0.5f * (((float) Math.Sin(theta)) + 1);

            return (sineV * mColor1) + (1f - sineV) * mColor2;
        }

    }
}
