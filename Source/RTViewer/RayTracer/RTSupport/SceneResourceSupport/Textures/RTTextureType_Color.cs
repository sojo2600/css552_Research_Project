using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;

/// only available to Vista and later
// using System.Runtime.CompilerOptions;

namespace RayTracer_552
{
    /// <summary>
    /// Typical "file color map" 
    /// </summary>
    public class RTTextureType_Color : RTTextureType {
    
        private Bitmap mTextureImage; // the bitmap image file.

        /// <summary>
        /// Constructs from parsing the command file.
        /// DO NOT change the parser loop unless you know what you are doing.
        /// </summary>
        /// <param name="parser"></param>
        public RTTextureType_Color(CommandFileParser parser)
        {
            while (!parser.IsEndElement("texture"))
            {
                if (parser.IsElement() && (!parser.IsElement("texture")))
                {
                    if (parser.IsElement("filename")) {
                        String name = parser.ReadString();
                        name = parser.FullPath() + "\\" + name;
                        if (System.IO.File.Exists(name))
                            mTextureImage = new Bitmap(name, true);
                        else
                            parser.ParserError("TextureType_Color filename");
                    }
                    else
                        parser.ParserError("TextureType_Color");
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
        /// 
        /// only available to Vista and later
        /// [MethodImpl(MethodImplOptions.Synchronized)]
        public override Vector3 GetTexile(float u, float v, IntersectionRecord rec, RTGeometry g)
        {
            System.Drawing.Color c = System.Drawing.Color.Black;
            if (null != mTextureImage)
            {
                if ((u >= 0) && (u <= 1f) && (v >= 0) && (v <= 1f))
                {
                    lock (mTextureImage)
                    {
                        int x = (int)(u * (mTextureImage.Width - 1) + 0.5f);
                        int y = (int)(v * (mTextureImage.Height - 1) + 0.5f);
                        y = mTextureImage.Height - y - 1;
                        c = mTextureImage.GetPixel(x, y);
                    }
                }
            }
            return ColorToVec(c);
        }
    }
}