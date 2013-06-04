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
    public class RTTextureType_Marble : RTTextureType_Noise 
    {
        private float mPeriod;      // how many periods to be fitted within 0 to 1 U
        private Vector3 mDirection; // in XYZ space, direction of the sine function
        
        private float mThetaRange;  // range from 0 to this value (mPeroid * 2PI)

        /// <summary>
        /// Constrcuts from the commandfile.
        /// DO NOT change the pasing loop unless you know what you are doing.
        /// </summary>
        /// <param name="parser"></param>
        public RTTextureType_Marble(CommandFileParser parser)
        {
            // From super class:
            //      Color1/2 
            //      Frequency and
            //      Amplitude 
            
            mPeriod = 1f;
            mDirection = Vector3.UnitY;
            mAmplitude = 1f;


            while (!parser.IsEndElement("texture"))
            {
                if (parser.IsElement() && (!parser.IsElement("texture")))
                {
                    if (parser.IsElement("period"))
                        mPeriod = parser.ReadFloat();
                    else if (parser.IsElement("frequency"))
                        mFrequency = parser.ReadInt();
                    else if (parser.IsElement("amplitude"))
                        mAmplitude = parser.ReadFloat();
                    else if (parser.IsElement("direction"))
                        mDirection = parser.ReadVector3();
                    else if (parser.IsElement("color1"))
                        mColor1 = parser.ReadVector3();
                    else if (parser.IsElement("color2"))
                        mColor2 = parser.ReadVector3();
                    else
                        parser.ParserError("TextureType_Marble");
                }
                else
                    parser.ParserRead();
            }
            mThetaRange = mPeriod * (float) Math.PI * 2f;
            mDirection = Vector3.Normalize(mDirection);
        }

        /// <summary>
        /// (x,y,z) is the visible pt, returns solid texture value at x,y,z
        /// </summary>
        /// <param name="rec">IntersectionRecord to be texture mapped</param>
        /// <param name="g">The geometry</param>
        /// <returns></returns>
        public override Vector3 GetTexile(IntersectionRecord rec, RTGeometry g)
        {
            float x = rec.IntersectPosition.X;
            float y = rec.IntersectPosition.Y;
            float z = rec.IntersectPosition.Z;

            float n = ComputeTurbulanceNoise(x, y, z);

            Vector3 v = new Vector3(x, y, z);
            v.Normalize();
            float useU = Vector3.Dot(mDirection, v) + n;
            float theta = useU * mThetaRange;
            float sineV = 0.5f * (((float) Math.Sin(theta)) + 1);

            return (sineV * mColor1) + (1f - sineV) * mColor2;
        }

    }
}
