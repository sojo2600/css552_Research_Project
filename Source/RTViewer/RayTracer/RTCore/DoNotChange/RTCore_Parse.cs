using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Windows.Forms;

namespace RayTracer_552
{
    /// <summary>
    /// 
    /// PLEASE DO NOT CHANGE!!
    /// 
    /// Parse RTWorld specific part of the command file
    /// </summary>
    public partial class RTCore
    {
        // The parser
        private CommandFileParser mParser;

        
        public void Parse(CommandFileParser parser)
        {
            parser.ParserRead();
            while (!parser.IsEndElement("rtspec"))
            {
                if (parser.IsElement() && (!parser.IsElement("rtspec")))
                {
                    if (parser.IsElement("generation"))
                        mGeneration = parser.ReadInt();
                    else if (parser.IsElement("shadow"))
                        mComputeShadow = parser.ReadBool();
                    else if (parser.IsElement("reflection"))
                        mComputeReflection = parser.ReadBool();
                    else if (parser.IsElement("background"))
                        mBgColor = parser.ReadVector3();
                    else
                        parser.ParserError("RTWorld");
                }
                else
                    parser.ParserRead();
            }
        }
    }
}
