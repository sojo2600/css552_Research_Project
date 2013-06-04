using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace RayTracer_552
{
    /// <summary>
    /// 
    /// (NO CHANGES NECESSARY)
    /// Data structures for storing results and supports GUI interactive display of the images.
    /// As you compute each pixel's position, color, and coverage, you MUST update the following
    /// data structures:
    ///         
    ///         mResultImage: store computed color
    ///                       (need to convert Vector3 color representation to Color)
    ///                       (by calling ComputeColor(Vector3 v) function in RTCore_Utilities.cs)
    ///         
    ///         mPixelCoverage: store pixel coverage
    ///         
    ///         mPixelDepth: store visible depth per pixel
    ///        
    ///         mPixelPosition: store the position for the pixel index (i,j)
    ///         
    /// </summary>
    public partial class RTCore
    {
        private Bitmap mResultImage, mPixelCoverage, mPixelDepth; // Actual storage for these are passed in (from GUI)
        private Vector3[][] mPixelPosition; // Pixel position in 3D space, record for debugging purposes!
        private float[][] mPixelHitDistance;

        /// <summary>
        /// Sets the storage for image and pixel coverage arrays
        /// </summary>
        /// <param name="resultImage"></param>
        /// <param name="pixelCoverage"></param>
        public void SetResultColors(Bitmap resultImage, Bitmap pixelCoverage, Bitmap pixelDepth)
        {
            mResultImage = resultImage;
            mPixelCoverage = pixelCoverage;
            mPixelDepth = pixelDepth;

            mPixelPosition = new Vector3[ImageWidth][];
            for (int i = 0; i < ImageWidth; i++)
                mPixelPosition[i] = new Vector3[ImageHeight];

            mPixelHitDistance = new float[ImageWidth][];
            for (int i = 0; i < ImageWidth; i++)
                mPixelHitDistance[i] = new float[ImageHeight];
        }

        /// <summary>
        /// Returns the actualy pixel position and the computed color for pixel index:(x,y)
        /// </summary>
        /// <param name="x">Input: pixel index: x</param>
        /// <param name="y">Input: pixel index: y</param>
        /// <param name="pt">Actual pixel locaiton in 3D world</param>
        /// <param name="c">The computed pixel color</param>
        public void GetPixelValues(int x, int y, out Vector3 pt, out System.Drawing.Color c, out float dist)
        {
            c = mResultImage.GetPixel(x, y);
            pt = mPixelPosition[x][y];
            dist = mPixelHitDistance[x][y];
        }

    }
}
