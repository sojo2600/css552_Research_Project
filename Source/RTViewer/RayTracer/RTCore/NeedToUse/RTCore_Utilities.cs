using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace RayTracer_552
{
    /// <summary>
    /// Local utilities, converts Vector3 to Color.
    /// 
    /// you DO NOT NEED TO CHANGE anything in this file, but you will probably find the following
    /// functions useful:
    /// 
    ///     ComputeColor(): convert Vector3 to Color for mResultImage
    ///     ComputePixelCoverage(): covert float to Gray color for mPixelCoverage
    /// 
    /// Note: both mResultImage and mPixelCoverage are in RTCore_ResultStorage.cs
    /// 
    /// </summary>
    public partial class RTCore
    {
        /// <summary>
        /// Returns a gray color: 
        ///    if p = 0: Black
        ///    if p = 1: white
        ///    otherwise, linearly compute a gray color
        /// </summary>
        /// <param name="p">normalized number between 0 and 1</param>
        /// <returns></returns>
        private System.Drawing.Color ComputePixelCoverage(float p)
        {
            int c = FloatToColorChannel(p);
            return System.Drawing.Color.FromArgb(c, c, c);
        }

        /// <summary>
        /// Convert 0 to 1 to 0 to 255, x/y/z to r/g/b.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private System.Drawing.Color ComputeColor(Vector3 v)
        {
            return System.Drawing.Color.FromArgb(
                        FloatToColorChannel(v.X),
                        FloatToColorChannel(v.Y),
                        FloatToColorChannel(v.Z) );
        }

        /// <summary>
        /// convert 0 to 1 to 0 to 255 with clamping such that no negative nubmers or numbers
        /// greater than 255 will happen.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private int FloatToColorChannel(float f)
        {
            return (int)((ClampToNormal(f) * 255f) + 0.5f);
        }

        /// <summary>
        /// clamp a float to beween 0 to 1
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private float ClampToNormal(float f)
        {
            if (f > 1f)
                f = 1f;
            else if (f < 0f)
                f = 0f;
            return f;
        }
    }
}
