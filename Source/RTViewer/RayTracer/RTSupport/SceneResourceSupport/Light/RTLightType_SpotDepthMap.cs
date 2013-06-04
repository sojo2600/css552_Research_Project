using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    ///    A spot light.
    /// </summary>
    /// 
    public partial class RTLightType_Spot
    {
        private float[][] mDepthMap = null;
        private int[][] mGeomID;            // Geom who's depth is in the map
        private RTRectangle mDepthMapGeom;  // the rectangle representing the map
        private int mRes = 128;             // depth map resolution is Res x Res
        private int mFilterRes = 1;         // filter size when sampling the depth map

        private const float kDepthImageDist = 1f; // depth map image plane distance from the light source
        
        private void InitDepthMap(SceneDatabase sceneDatabase)
        {
            if (null != mDepthMap)
                return; // done

            // remember!! mDireciton is L, or a vector from Visible point to the light.
            // Here we want direction from light source towards the scene
            Vector3 useDir = -mDirection;

            // 1. Find a Up direction
            //         guess up direction to be (0, 1, 0), if view direction is also this,
            //         use (1, 0, 0) as view direction
            Vector3 up = Vector3.UnitY;
            if (Math.Abs(Vector3.Dot(up, useDir)) > 0.99999)
                up = Vector3.UnitX;


            // 2. define Orthonormal base
            Vector3 sideV = Vector3.Cross(up, useDir);
            up = Vector3.Cross(useDir, sideV);
            sideV.Normalize();
            up.Normalize();

            // 3. compute the depth map image plane, 
            //    define the image plane to be located at ... a distance of kDepthImageDist away
            Vector3 ptOnImage = mPosition + kDepthImageDist * useDir;

            // 4. compute depth map image size
            float halfImageSize = kDepthImageDist * (float)Math.Tan(mOuterAngle/2f);

            // 5. Compute the 4 vertices the defines the depth map
            Vector3[] v = new Vector3[4];
            v[0] = ptOnImage - halfImageSize * up - halfImageSize * sideV;
            v[1] = ptOnImage - halfImageSize * up + halfImageSize * sideV;
            v[2] = ptOnImage + halfImageSize * up + halfImageSize * sideV;
            v[3] = ptOnImage + halfImageSize * up - halfImageSize * sideV;
            
            // 6. create a Geometry that represents the map
            // ** Be caureful **!!
            //     RTRectangle uses v[0] as the origin for texture lookup
            //     we _MUST_ follow the same in order to take advante of GetUV() function!
            mDepthMapGeom = new RTRectangle(v);

            // 7. Now allocate memory for the actual map
            mDepthMap = new float[mRes][];
            mGeomID = new int[mRes][];
            for (int i = 0; i<mRes; i++) {
                mDepthMap[i] = new float[mRes];
                mGeomID[i] = new int[mRes];
            }

            // now, trace rays through each of the pixels in the depth map and record the depth and geomID
            float pixelSize = halfImageSize / (0.5f * mRes);
            Vector3 upPixelVector = pixelSize * up;
            Vector3 sidePixelVector = pixelSize * sideV;
            for (int y = 0; y < mRes; y++) {
                Vector3 yDisp = v[0] + (y+0.5f) * upPixelVector;
                for (int x = 0; x < mRes; x++) {
                    Vector3 pixelPos = ((x+0.5f) * sidePixelVector) + yDisp;
                    Ray r = new Ray(mPosition, pixelPos);
                    IntersectionRecord rec = new IntersectionRecord();

                    for (int i = 0; i < sceneDatabase.GetNumGeom(); i++)
                    {
                        RTGeometry g = sceneDatabase.GetGeom(i);
                        g.Intersect(r, rec);
                    }
                    mDepthMap[x][y] = rec.HitDistance;
                    // closes intersection distance, any object that is 
                    // further away from the light than this distance is in the shadow of this light
                    mGeomID[x][y] = rec.GeomIndex;
                    // this object can never be in shadow, becuase it is the closest to the light!
                }
            }
        }

        /// <summary>
        /// Samples the depth map, returns the percentage of samples that is closer to the light than the map
        /// </summary>
        /// <param name="visiblePt"></param>
        /// <param name="visibleObj"></param>
        /// <returns></returns>
        private float SampleDepthMap(Vector3 visiblePt, int visibleObj)
        {
            float samplesTaken = 0f;
            float count = 0f;

            // intersect a ray with the depthMap to get the intersection position
            Ray r = new Ray(visiblePt, mPosition);
            IntersectionRecord rec = new IntersectionRecord();
            if (mDepthMapGeom.Intersect(r, rec))
            {
                float x = 0, y = 0;
                mDepthMapGeom.GetUV(rec.IntersectPosition, rec.HitPtBC, ref x, ref y);

                int lowX = (int)(x * mRes) - mFilterRes;
                int hiX = lowX + mFilterRes;
                int lowY = (int)(y * mRes) - mFilterRes;
                int hiY = lowY + mFilterRes;

                //float distToPt = (visiblePt - mPosition).Length();

                // depth map look up
                for (int i = lowX; i <=hiX; i++)
                {
                    for (int j = lowY; j <=hiY; j++)
                    {
                        if ((i >=0) && (j>=0) && (i < mRes) && (j < mRes))
                        {
                            samplesTaken += 1f;
                            if (visibleObj == mGeomID[i][j])
                                    count += 1f;
                        }
                    }
                }
            }
            return count /= samplesTaken;
        }
    }
}
