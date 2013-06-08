using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using System.Drawing;


/// This multi-threaded version is copy whole-sale from Samual Cook, CSS451 Winter 2010
///         Feb 2, 2010

namespace RayTracer_552
{
    public partial class RTCore
    {
        public enum Anaglyph
        {
            No3D,
            RedGreen,
            RedBlue,
            RedCyan,
            GreenRed,
            GreenMagenta
        };
        private Vector3 leftLens = Vector3.Zero;
        private Vector3 rightLens = Vector3.Zero;
        private const int SINGLE = 0;
        private const int LEFT = 1;
        private const int RIGHT = 2;
        private const float kUseMaxDepth = 20f; // for now, assume max distance
        public static Anaglyph mAnaglyphList;
        public void SetAnaglyph(String input)
        {
            if (input == "No3D")
            {
                mAnaglyphList = Anaglyph.No3D;
            }
            else if (input == "RedGreen")
            {
                mAnaglyphList = Anaglyph.RedGreen;

            }
            else if (input == "RedBlue")
            {
                mAnaglyphList = Anaglyph.RedBlue;
            }
            else if (input == "RedCyan")
            {
                mAnaglyphList = Anaglyph.RedCyan;
            }
            else if (input == "GreenRed")
            {
                mAnaglyphList = Anaglyph.GreenRed;
            }
            else if (input == "GreenMagenta")
            {
                mAnaglyphList = Anaglyph.GreenMagenta;
            }
            else
            {
                mAnaglyphList = Anaglyph.No3D;
            }
            SetFilters();
        }

        public void SetAnaglypg(Anaglyph input)
        {
            mAnaglyphList = input;
            SetFilters();
        }
        /// 
        /// Continuously calls ComputeNextPixel until there is no more pixel to compute.
        /// This function is meant to be called and started as a separate thread (from the GUI).
        ///         
        private void SetFilters()
        {
            // using vectors to multiple with inputed vector color 1 = color 0 = no color set at x = red y = green z = blue (RGB)
            if (mAnaglyphList == Anaglyph.No3D)
            {
                leftLens = new Vector3(1, 1, 1);
                rightLens = leftLens;
                mAnaglyph = false;
            }
            else
            {    
                if (mAnaglyphList == Anaglyph.RedGreen)
                {
                    leftLens = new Vector3(1, 0, 0);
                    rightLens = new Vector3(0, 1, 0);
                }
                else if (mAnaglyphList == Anaglyph.RedBlue)
                {
                    leftLens = new Vector3(1, 0, 0);
                    rightLens = new Vector3(0, 0, 1);
                }
                else if (mAnaglyphList == Anaglyph.RedCyan)
                {
                    leftLens = new Vector3(1, 0, 0);
                    rightLens = new Vector3(0, 1, 1);
                }
                else if (mAnaglyphList == Anaglyph.GreenRed)
                {
                    leftLens = new Vector3(0, 1, 0);
                    rightLens = new Vector3(1, 0, 0);
                }
                else //if (mAnaglyphList == Anaglyph.GreenMagenta)
                {
                    leftLens = new Vector3(0, 1, 0);
                    rightLens = new Vector3(1, 0, 1);
                }
                mAnaglyph = true;
            }
        }
        public void ComputeImage(Object parm)
        {
            int startX, startY, endX, endY, threadID;
            ParseParm(parm, out startX, out startY, out endX, out endY, out threadID);

            // Console.WriteLine("Start: Thread:" + threadID + " - y(" + startY + " " + endY + ")");

            int currentX = startX;
            int currentY = startY;

            while (currentY < endY && currentY < ImageHeight)
            {
                while (currentX < ImageWidth)
                {
                    Vector3 pixelColor = Vector3.Zero;
                    float pixelCoverage = 0f;
                    float pixelDepth = 0f;

                    for (int i = 0; i < mImageSpec.NumSamplesPerPixel; i++)
                    {
                        float dx;
                        float dy;
                        lock (this)
                        {
                            dx = (float)mRand.NextDouble();
                            dy = (float)mRand.NextDouble();
                        }
                        Vector3 pixelPos = mCamera.GetPixelPosition(currentX + dx, currentY + dy);
                        Ray r;
                        if (!mOrthoRT)
                        {
                            r = new Ray(mCamera.EyePosition, pixelPos);
                        }
                        else
                        {
                            pixelPos -= 10f * mCamera.ViewDirection;
                            r = Ray.CrateRayFromPtDir(pixelPos, mCamera.ViewDirection);
                        }

                        // #if RT_PIXEL_DEBUG
                        if ((currentX == 348) && (currentY == 196))
                        {
                            int a = 4;
                        }
                        // #endif

                        if (!mAnaglyph)
                        {
                            computePixelColor(r, ref pixelColor, SINGLE);
                        }
                        else
                        {
                            Ray left = new Ray(mCamera.LeftEyePosition, pixelPos);
                            computePixelColor(left, ref pixelColor, LEFT);
                            Ray right = new Ray(mCamera.RightEyePosition, pixelPos);
                            computePixelColor(right, ref pixelColor, RIGHT);
                        }

                        float sampleCoverage = 0f;
                        float sampleDepth = 0f;

                        // what coverage and depth?
                        IntersectionRecord rec = new IntersectionRecord();
                        ComputeVisibility(r, rec, RTCore.kInvalidIndex);
                        if (rec.GeomIndex != RTCore.kInvalidIndex)
                        {
                            sampleCoverage = 1f;
                            sampleDepth = kUseMaxDepth - rec.HitDistance;
                        }
                        pixelCoverage += sampleCoverage;
                        pixelDepth += sampleDepth;
                    }

                    pixelColor /= (float)mImageSpec.NumSamplesPerPixel;
                    pixelCoverage /= (float)mImageSpec.NumSamplesPerPixel;
                    pixelDepth /= (float)mImageSpec.NumSamplesPerPixel;

                    System.Drawing.Color c = ComputeColor(pixelColor); // convert Vector3 (0-1 float) to Color (0 - 255)
                    System.Drawing.Color p = ComputePixelCoverage(pixelCoverage);
                    float useDepth = pixelDepth / kUseMaxDepth;
                    System.Drawing.Color z = ComputePixelCoverage(useDepth);

                    lock (this)
                    {
                        mResultImage.SetPixel(currentX, currentY, c);
                        mPixelCoverage.SetPixel(currentX, currentY, p);
                        mPixelDepth.SetPixel(currentX, currentY, z);

                        if (mShowPixelInWorld || mDisplayDebugPixels || mDisplayDebugRays)
                        {
                            Vector3 pixelPos = mCamera.GetPixelPosition(currentX, currentY);
                            Ray r = new Ray(mCamera.EyePosition, pixelPos);
                            IntersectionRecord rec = new IntersectionRecord();
                            Vector3 resultColor = mBgColor;
                            ComputeVisibility(r, rec, RTCore.kInvalidIndex);
                            if (rec.GeomIndex != RTCore.kInvalidIndex)
                            {
                                resultColor = ComputeShading(rec, 0);
                            }

                            mPixelPosition[currentX][currentY] = pixelPos;
                            mPixelHitDistance[currentX][currentY] = rec.HitDistance;
                            mResultImage.SetPixel(currentX, currentY, ComputeColor(resultColor));
                        }
                    }

                    // next pixel
                    currentX++;
                }

                // next scanline
                currentX = startX;
                currentY++;

                lock (this)
                {
                    if (currentY > mCurrentY)
                        mCurrentY = currentY;
                }
            }

            // Console.WriteLine("Start: Thread:" + threadID + " - y(" + startY + " " + endY + ")");

            if (threadID != -1)
                ThreadNextWorkLoad(threadID);
            else
                RTRenderingDone();
        }

        private void computePixelColor(Ray r, ref Vector3 pixelColor, int channel)
        {
            IntersectionRecord rec = new IntersectionRecord();
            // what can we see?
            ComputeVisibility(r, rec, RTCore.kInvalidIndex);

            Vector3 sampleColor =  mBgColor;
            if (pixelColor != Vector3.Zero)
            {
                int a = 5;
            }
            // what color should it be?
            if (rec.GeomIndex != RTCore.kInvalidIndex)
                sampleColor = ComputeShading(rec, 0);

                if (channel == SINGLE)
                    pixelColor += sampleColor;

                else if (channel == LEFT)
                    pixelColor += sampleColor * leftLens;

                else if (channel == RIGHT)
                    pixelColor += sampleColor * rightLens;
            
            //throws off the filters
            // This prevents a yellow background by bringing in the color blue where there are no intersections
            // if (channel != SINGLE && rec.GeomIndex == RTCore.kInvalidIndex)
            //   pixelColor.Z += sampleColor.Z / 2;

        }
    }
}
