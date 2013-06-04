using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// Checker repeats over UV space:
    ///     UV repeats
    ///     Color1/Color2: default to be: White/Black
    /// </summary>
    public class RTTextureType_Checker : RTTextureType 
    {
        private int mURepeat, mVRepeat;         // UV Repeat from command file
        private Vector3 mColor1 = Vector3.One;  // Checker colors
        private Vector3 mColor2 = Vector3.Zero;

        /// <summary>
        /// Constrcuts from the commandfile.
        /// DO NOT change the pasing loop unless you know what you are doing.
        /// </summary>
        /// <param name="parser"></param>
        public RTTextureType_Checker(CommandFileParser parser)
        {
            while (!parser.IsEndElement("texture"))
            {
                if (parser.IsElement() && (!parser.IsElement("texture")))
                {
                    if (parser.IsElement("urepeat"))
                        mURepeat = parser.ReadInt();
                    else if (parser.IsElement("vrepeat"))
                        mVRepeat = parser.ReadInt();
                    else if (parser.IsElement("color1"))
                        mColor1 = parser.ReadVector3();
                    else if (parser.IsElement("color2"))
                        mColor2 = parser.ReadVector3();
                    else
                        parser.ParserError("TextureType_Checker");
                }
                else
                    parser.ParserRead();
            }
        }

        /// <summary>
        /// UV are values between 0 to 1, maps to texture width/height linearly and returns the
        /// corresponding textile.
        /// </summary>
        /// <param name="u">value between 0 to 1</param>
        /// <param name="v">value between 0 to 1</param>
        /// <param name="rec">IntersectionRecord to be texture mapped</param>
        /// <param name="g">the geometry</param>
        /// <returns></returns>
        public override Vector3 GetTexile(float u, float v, IntersectionRecord rec, RTGeometry g)
        {
            float uVal = u * mURepeat;
            float vVal = v * mVRepeat;

            int uBit = ((int)uVal) % 2;
            int vBit = ((int)vVal) % 2;

            Vector3 c = mColor1;
            if (uBit == vBit)
                c = mColor2;

            return c;
        }

    }
}
