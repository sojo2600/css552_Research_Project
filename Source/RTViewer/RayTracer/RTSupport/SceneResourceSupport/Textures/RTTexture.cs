using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// Texutre support. Notice the constructor creates the actual TextureType
    /// and continues parsing. It is assumed that in the command file, xml element types:
    ///      index
    ///      type
    ///  are specifiec before any other elements!!
    /// </summary>
    public class RTTexture : IndexedResource
    {
            
        private RTTextureType mTexture; // The actualy texture functionality.

        /// <summary>
        /// Constrctus from parsing command file. xlm elements:
        ///     index, type
        /// MUST exist before any other texture type-specific elements!!
        /// </summary>
        /// <param name="parser"></param>
        public RTTexture(CommandFileParser parser)
        {
            parser.ParserRead();
            while (!parser.IsEndElement("texture"))
            {
                if (parser.IsElement() && (!parser.IsElement("texture")))
                {
                    if (parser.IsElement("index")) {
                        int i = parser.ReadInt();
                        SetResourceIndex(i);
                    } 
                    else if (parser.IsElement("type")) {
                        String type = parser.ReadString();
                        if (type.Equals("color"))
                            mTexture = new RTTextureType_Color(parser);
                        else if (type.Equals("checker"))
                            mTexture = new RTTextureType_Checker(parser);
                        else if (type.Equals("noise"))
                            mTexture = new RTTextureType_Noise(parser);
                        else if (type.Equals("sine"))
                            mTexture = new RTTextureType_Sine(parser);
                        else if (type.Equals("marble"))
                            mTexture = new RTTextureType_Marble(parser);
                        else if (type.Equals("ramp"))
                            mTexture = new RTTextureType_Ramp(parser);
                        else if (type.Equals("grid"))
                            mTexture = new RTTextureType_Grid(parser);
                        else if (type.Equals("bump"))
                            mTexture = new RTTextureType_Bump(parser);
                        else
                            parser.ParserError("Texture: unknown type");
                    }
                    else
                        parser.ParserError("Texture");
                }
                else
                    parser.ParserRead();
            }
        }

        public Vector3 TextureLookup(IntersectionRecord rec, RTGeometry g) {
            if (mTexture.NeedUV())
            {
                float u = 0f, v = 0f;
                g.GetUV(rec.IntersectPosition, rec.HitPtBC, ref u, ref v);
                return mTexture.GetTexile(u, v, rec, g);
            }
            else
            {
                return mTexture.GetTexile(rec, g);
            }
        }

    }
}
