using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// Geometry support: only support rectangle and sphere.
    /// </summary>
    public abstract partial class RTGeometry 
    {
        /// <summary>
        /// Parse command line for <xform> </xform> that is embedeed inside 
        /// Geometry.
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        static protected Matrix ParseTransform(CommandFileParser parser)
        {
            Matrix xform = Matrix.Identity;
            Vector3 translation = Vector3.Zero;
            Matrix rotation = Matrix.Identity;
            Vector3 scale = Vector3.One;

            parser.ParserRead();
            while (!parser.IsEndElement("xform"))
            {
                if (parser.IsElement() && (!parser.IsElement("xform")))
                {
                    if (parser.IsElement("translation"))
                        translation = parser.ReadVector3();
                    else if (parser.IsElement("rotationX"))
                    {
                        float rotXInDegree = parser.ReadFloat();
                        rotation = rotation * Matrix.CreateRotationX(MathHelper.ToRadians(rotXInDegree));
                    }
                    else if (parser.IsElement("rotationY"))
                    {
                        float rotXInDegree = parser.ReadFloat();
                        rotation = rotation * Matrix.CreateRotationY(MathHelper.ToRadians(rotXInDegree));
                    }
                    else if (parser.IsElement("rotationZ"))
                    {
                        float rotXInDegree = parser.ReadFloat();
                        rotation = rotation * Matrix.CreateRotationZ(MathHelper.ToRadians(rotXInDegree));
                    }
                    else if (parser.IsElement("scale"))
                        scale = parser.ReadVector3();
                    else
                        parser.ParserError("xform");
                }
                else
                    parser.ParserRead();
            }
            return Matrix.CreateScale(scale) * rotation * Matrix.CreateTranslation(translation);
        }
    }
}
