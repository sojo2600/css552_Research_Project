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
    public class RTTextureType_Ramp : RTTextureType 
    {
        private float mRepeat;      // how many periods to be fitted within 0 to 1 U

        private Vector3 mColor1 = Vector3.One;  // Checker colors
        private Vector3 mColor2 = Vector3.Zero;

        /// <summary>
        /// Constrcuts from the commandfile.
        /// DO NOT change the pasing loop unless you know what you are doing.
        /// </summary>
        /// <param name="parser"></param>
        public RTTextureType_Ramp(CommandFileParser parser)
        {
            mRepeat = 1f;

            while (!parser.IsEndElement("texture"))
            {
                if (parser.IsElement() && (!parser.IsElement("texture")))
                {
                    if (parser.IsElement("repeat"))
                        mRepeat = parser.ReadFloat();
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
            if (mRepeat <= 0)
                mRepeat = 1f;
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
            float useU = u * mRepeat;
            useU = useU - ((int)useU);
            return (useU * mColor2) + (1 - useU) * mColor1;
        }
    }
}
