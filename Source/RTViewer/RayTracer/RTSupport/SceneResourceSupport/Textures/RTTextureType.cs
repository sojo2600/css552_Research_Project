using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// Actual texture functionality support. Defines (for now):
    /// 
    ///    1. GetTexile(u,v): returns Vector3 (color)
    ///          For now, this is the only function a texture map needs to know
    ///          how to support.
    ///          
    ///    2. ColorToVec(Color c): given a color convert to a Vector3.
    ///          Utility function for format conversion.
    ///          
    /// </summary>
    public abstract class RTTextureType 
    {
        /// <summary>
        /// UV are values between 0 to 1, maps to texture width/height linearly and returns the
        /// corresponding textile.
        /// </summary>
        /// <param name="u">value between 0 to 1</param>
        /// <param name="v">value between 0 to 1</param>
        /// <param name="rec">IntersectionRecord to be texture mapped</param>
        /// <param name="g">the geometry</param>
        /// <returns></returns>
        public virtual Vector3 GetTexile(float u, float v, IntersectionRecord rec, RTGeometry g) { return Vector3.Zero; }

        /// <summary>
        /// (x,y,z) is the visible pt, returns solid texture value at x,y,z
        /// </summary>
        /// <param name="rec">IntersectionRecord to be texture mapped</param>
        /// <param name="g">The geometry</param>
        /// <returns></returns>
        public virtual Vector3 GetTexile(IntersectionRecord rec, RTGeometry g) { return Vector3.Zero; }

        /// <summary>
        /// Returns if this texture needs UV values to perform lookup, or, alternatively,
        /// this texture can do lookup with 3D position (solid noise)
        /// </summary>
        /// <returns></returns>
        public virtual bool NeedUV() { return true; }
        
        /// <summary>
        /// Utility function for format conversion, from Color to Vector3
        /// </summary>
        /// <param name="c">Input color in RGB bytes (between 0 to 255)</param>
        /// <returns>Output in RGB float normal space (between 0 to 1)</returns>
        protected Vector3 ColorToVec(System.Drawing.Color c)
        {
            float r = ((float)c.R) / 255.0f;
            float g = ((float)c.G) / 255.0f;
            float b = ((float)c.B) / 255.0f;
            return new Vector3(r, g, b);
        }


        protected void GetIndices(float givenI, int res, ref int i0, ref int i1)
        {
            float tmp = givenI * res;
            i0 = ((int)(tmp + 0.5f)) % res;
            i0 = (i0 < 0) ? 0 : i0;
            i1 = (i0 + 1) % res;
            if (i0 > i1)
            {
                int t = i0;
                i0 = i1;
                i1 = t;
            }
        }
    }
}
