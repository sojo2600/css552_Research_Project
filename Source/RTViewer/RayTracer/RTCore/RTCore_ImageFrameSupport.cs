using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace RayTracer_552
{
    /// <summary>
    /// You need to implement these!!!
    /// 
    /// Correctly implement the following functions and you will observe the proper red-image frame
    /// in the interactive GUI Window.
    /// </summary>
    public partial class RTCore
    {
        /// <summary>
        /// Returns the (x,y,z) position of the top-left pixel
        /// </summary>
        /// <returns></returns>
        public Vector3 GetTopLeftPixelPosition()
        {
            return mCamera.GetPixelPosition(0, 0);
        }

        /// <summary>
        /// Returns the (x,y,z) position of the top-right pixel
        /// </summary>
        /// <returns></returns>
        public Vector3 GetTopRightPixelPosition()
        {
            return mCamera.GetPixelPosition(0, ImageHeight - 1);
        }

        /// <summary>
        /// Returns the (x,y,z) position of the bottom-left pixel
        /// </summary>
        /// <returns></returns>
        public Vector3 GetBottomLeftPixelPosition()
        {
            return mCamera.GetPixelPosition(ImageWidth - 1, 0);
        }

        /// <summary>
        /// Returns the (x,y,z) position of the bottom-right pixel
        /// </summary>
        /// <returns></returns>
        public Vector3 GetBottomRightPixelPosition()
        {
            return mCamera.GetPixelPosition(ImageWidth - 1, ImageHeight - 1);
        }
    }
}
