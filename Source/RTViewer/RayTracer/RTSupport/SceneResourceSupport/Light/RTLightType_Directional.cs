using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    ///    A directional light.
    /// </summary>
    /// 
    public class RTLightType_Directional : RTLightType
    {
        private Vector3 mDirection;  

        /// <summary>
        /// Constructor from parser. 
        /// Please DO NOT change the parsing routine unless you know what you are doing).
        /// </summary>
        /// <param name="parser"></param>
        public RTLightType_Directional(CommandFileParser parser)
        {
            mLightSourceType = RTLightSourceType.RTLightSourceTypeDirection;

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
                    else
                        parser.ParserError("Light");
                }
                else
                    parser.ParserRead();
            }

            mDirection = -Vector3.Normalize(mDirection);

        }

        protected override float DistanceToLight(Vector3 visiblePt) { return float.MaxValue;  }

        /// <summary>
        /// Returns the normalized L vector for illumination computation.
        /// </summary>
        /// <param name="visiblePt"></param>
        /// <returns></returns>
        public override Vector3 GetNormalizedDirection(Vector3 visiblePt) { return mDirection; }
    }
}
