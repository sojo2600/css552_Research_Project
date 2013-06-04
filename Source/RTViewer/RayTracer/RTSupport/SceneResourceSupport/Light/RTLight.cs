using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// Light class: has a instance of the actul light.
    /// </summary>
    /// 
    public class RTLight : IndexedResource
    {
        private RTLightType mLight;

        /// <summary>
        /// Constructor from parser. 
        /// Please DO NOT change the parsing routine unless you know what you are doing).
        /// </summary>
        /// <param name="parser"></param>
        public RTLight(CommandFileParser parser)
        {
            parser.ParserRead();
            while (!parser.IsEndElement("light"))
            {
                if (parser.IsElement() && (!parser.IsElement("light")))
                {
                    if (parser.IsElement("type"))
                    {
                        String type = parser.ReadString();
                        if (type.Equals("point"))
                            mLight = new RTLightType_Point(parser);
                        else if (type.Equals("directional"))
                            mLight = new RTLightType_Directional(parser);
                        else if (type.Equals("spot"))
                            mLight = new RTLightType_Spot(parser);
                        else
                            parser.ParserError("Light: Unknown light type");
                    }
                    else
                        parser.ParserError("Light");
                }
                else
                    parser.ParserRead();
            }
        }

        public void InitializeLight(SceneDatabase sceneDataBase) { mLight.InitializeLight(sceneDataBase); }
        public Vector3 GetColor(Vector3 visiblePt) { return mLight.GetColor(visiblePt); }
        public float PercentVisible(Vector3 visiblePt, int exceptGeomIndex, SceneDatabase sceneDatabase) { return mLight.PercentVisible(visiblePt, exceptGeomIndex, sceneDatabase); }
        public Vector3 GetNormalizedDirection(Vector3 visiblePt) { return mLight.GetNormalizedDirection(visiblePt); }
        public Vector3 GetLightPosition() { return mLight.GetLightPosition(); }
        public RTLightType.RTLightSourceType GetLightSourceType() { return mLight.GetLightSourceType(); }
    }
}
