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
    public class RTTextureType_Grid : RTTextureType 
    {
        private float mURepeat;      // how many periods to be fitted within 0 to 1 U
        private float mVRepeat;   // inverse of repeat

        private Vector3 mColor1 = Vector3.One;  // Checker colors
        private Vector3 mColor2 = Vector3.Zero;

        /// <summary>
        /// Constrcuts from the commandfile.
        /// DO NOT change the pasing loop unless you know what you are doing.
        /// </summary>
        /// <param name="parser"></param>
        public RTTextureType_Grid(CommandFileParser parser)
        {
            mURepeat = 1f;
            mVRepeat = 1f;

            while (!parser.IsEndElement("texture"))
            {
                if (parser.IsElement() && (!parser.IsElement("texture")))
                {
                    if (parser.IsElement("urepeat"))
                        mURepeat = parser.ReadFloat();
                    if (parser.IsElement("vrepeat"))
                        mVRepeat = parser.ReadFloat();
                    else if (parser.IsElement("color1"))
                        mColor1 = parser.ReadVector3();
                    else if (parser.IsElement("color2"))
                        mColor2 = parser.ReadVector3();
                    else
                        parser.ParserError("TextureType_Ramp");
                }
                else
                    parser.ParserRead();
            }
        }


        private const float kLowBound = 0.45f;
        private const float kUpBound = 0.55f;
        private const float kInvWidth = 1f / (kUpBound - kLowBound);
        private const float kMidPt = 0.5f * (kLowBound + kUpBound);

        private Vector3 GetGridColor(float useU)
        {
            Vector3 resultColor = mColor1;
            useU = useU - kLowBound;
            useU *= kInvWidth;
            if (useU < kMidPt)
                resultColor = useU * mColor2 + (1 - useU) * mColor1;
            else
                resultColor = useU * mColor1 + (1 - useU) * mColor2;
            return resultColor;
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
            float useU = u * mURepeat;
            useU = useU - ((int)useU);
            float useV = v * mVRepeat;
            useV = useV - ((int)useV);

            Vector3 resultColor = mColor1;
            if ((useU > kLowBound) && (useU < kUpBound))
            {
                resultColor = GetGridColor(useU);
            }
            else if ((useV > kLowBound) && (useV < kUpBound))
            {
                resultColor = GetGridColor(useV);
            }

            return resultColor;
        }
    }
}
