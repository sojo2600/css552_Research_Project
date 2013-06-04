using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// A Point light.
    /// </summary>
    /// 
    public class RTLightType_Point : RTLightType
    {  
        /// <summary>
        /// Constructor from parser. 
        /// Please DO NOT change the parsing routine unless you know what you are doing).
        /// </summary>
        /// <param name="parser"></param>
        public RTLightType_Point(CommandFileParser parser)
        {
            mLightSourceType = RTLightSourceType.RTLightSourceTypePoint;

            while (!parser.IsEndElement("light"))
            {
                if (parser.IsElement() && (!parser.IsElement("light")))
                {
                    if (parser.IsElement("color"))
                        mColor = parser.ReadVector3();
                    else if (parser.IsElement("position"))
                        mPosition = parser.ReadVector3();                   
                    else
                        parser.ParserError("Light");
                }
                else
                    parser.ParserRead();
            }
        }
    }
}
