using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// 
    /// Supprots Image-specific information for the Ray Tracer command file:
    /// 
    ///    1. Number of samples to be taken per pixel
    ///    2. X/Y image resolution.
    /// 
    /// </summary>
    public class ImageSpec
    {
        private int mSamplesPerPixel;               // parse result from "samples"
        private int mXResolution, mYResolution;     // parse result from resolution

        /// <summary>
        /// Constructor from parser. 
        /// Please DO NOT change the parser, unless you really know what you are doing!!
        /// </summary>
        /// <param name="parser"></param>
        public ImageSpec(CommandFileParser parser) {
            mXResolution = 64;
            mYResolution = 64;
            mSamplesPerPixel = 1;
            parser.ParserRead();
            while (!parser.IsEndElement("imagespec"))
            {
                if (parser.IsElement() && (!parser.IsElement("imagespec")))
                {
                    if (parser.IsElement("samples"))
                        mSamplesPerPixel = parser.ReadInt();
                    else if (parser.IsElement("resolution"))
                    {
                        Vector2 n = parser.ReadVector2();
                        mXResolution = (int)n.X;
                        mYResolution = (int)n.Y;
                    }
                    else
                        parser.ParserError("ImageSpec");
                }
                else
                    parser.ParserRead();
            }
        }

        /// <summary>
        /// Access functions for the image spec. information
        /// </summary>
        public int XResolution { get { return mXResolution; } }
        public int YResolution { get { return mYResolution; } }
        public int NumSamplesPerPixel { get { return mSamplesPerPixel; } }

    }
}
