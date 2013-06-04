using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// A Rectangle
    /// </summary>
    public class RTRectangle : RTGeometry
    {
        private Vector3[] mVertices;    // always 4 vertices
        private int mUAxisIndex, mVAxisIndex; // 0 is X, 1 is Y, and 2 is Z
        private float mD;   // AX + BY + CZ + D = 0, this is the D
        private Vector3 mNormal; // normal to the polygon

        // for texture mapping
        private Vector3 muVec, mvVec; // Normalized vector in the U and V direction
        private float muSize, mvSize; // entire object size in the U and V direction
        
        /// <summary>
        /// Constructs from parsing file and then intialize for intersection computation.
        /// </summary>
        /// <param name="parser"></param>
        public RTRectangle(CommandFileParser parser)
        {
            mType = RTGeometryType.Rectangle;

            // do we have a transform?
            bool hasTransform = false;
            Matrix xform = Matrix.Identity;
            
            mVertices = new Vector3[4];
            mMaterialIndex = 0;

            int count = 0;
            parser.ParserRead();
            while (!parser.IsEndElement("rectangle"))
            {
                if (parser.IsElement() && (!parser.IsElement("rectangle")))
                {
                    if (parser.IsElement("vertices"))
                    {
                        mVertices[count] = parser.ReadVector3();
                        count++;
                    }
                    else if (parser.IsElement("material"))
                        mMaterialIndex = parser.ReadInt();
                    else if (parser.IsElement("xform"))
                    {
                        hasTransform = true;
                        xform = ParseTransform(parser);
                    }
                    else
                        parser.ParserError("Rectangle");
                }
                else
                    parser.ParserRead();
            }
            if (count != 4)
            {
                parser.ParserError("Rectangle: vertex indexPtr = " + count);
            }
            else
            {
                if (hasTransform)
                {
                    for (int i = 0; i < 4; i++)
                        mVertices[i] = Vector3.Transform(mVertices[i], xform);
                }
                InitializeRectangle();
            }
        }

        /// <summary>
        /// Construting a rectange from given vertices.
        /// </summary>
        /// <param name="v"></param>
        public RTRectangle(Vector3[] v)
        {
            mType = RTGeometryType.Rectangle;
            mMaterialIndex = 0;

            mVertices = new Vector3[4];
            for (int i = 0; i<4; i++)
                mVertices[i] = v[i];

            InitializeRectangle();
        }

        /// <summary>
        /// Initialize internal data structures for intersection and texture support
        /// </summary>
        private void InitializeRectangle()
        {
            Vector3 v1, v2;
            muVec = v1 = mVertices[1] - mVertices[0];
            mvVec = v2 = mVertices[3] - mVertices[0];
            muSize = muVec.Length();
            mvSize = mvVec.Length();
            muVec /= muSize;
            mvVec /= mvSize;

            //
            // Normal = v1 cross v2
            // Normalize Normal Vector
            //
            mNormal = Vector3.Cross(v1, v2);
            mNormal.Normalize();
            mD = -(Vector3.Dot(mNormal, mVertices[0]));
            if (Math.Abs(mNormal.X) > Math.Abs(mNormal.Y))
            {
                /*  X > Y */
                if (Math.Abs(mNormal.X) > Math.Abs(mNormal.Z))
                {
                    /*  X > Y and X > Z ===> X Dominates  */
                    mUAxisIndex = 1; mVAxisIndex = 2;
                }
                else
                {
                    /* X > Y and X < Z  ===> X Dominates */
                    mUAxisIndex = 0; mVAxisIndex = 1;
                }
            }
            else
            {
                /*  Y > X */
                if (Math.Abs(mNormal.Y) > Math.Abs(mNormal.Z))
                {
                    /*  Y > X and Y > Z ===> Y Dominates  */
                    mUAxisIndex = 0; mVAxisIndex = 2;
                }
                else
                {
                    /* Y > X and Y < Z  ===> Z Dominates */
                    mUAxisIndex = 0; mVAxisIndex = 1;
                }
            }
        }

        /// <summary>
        /// Returns if the given pt is inside(true) or outside(false) of the polygon.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private bool InsidePolygon(Vector3 pt)
        {
            float[] va, vb, transVector;
            int NC; /* Number of crossings */
            int NSH, SH; /* Sign holder */
            int i, b;
            float uIntersect;

            va = new float[3];
            vb = new float[3];
            transVector = new float[3];

            transVector[0] = -pt.X;
            transVector[1] = -pt.Y;
            transVector[2] = -pt.Z;
            NC = 0;

            va[0] = mVertices[0].X + transVector[0];
            va[1] = mVertices[0].Y + transVector[1];
            va[2] = mVertices[0].Z + transVector[2];

            if (va[mVAxisIndex] < 0)
                SH = -1;
            else
                SH = 1;

            for (i = 0; i < 4; i++)
            {
                b = (i + 1) % 4;
                vb[0] = mVertices[b].X + transVector[0];
                vb[1] = mVertices[b].Y + transVector[1];
                vb[2] = mVertices[b].Z + transVector[2];
                if (vb[mVAxisIndex] < 0)
                    NSH = -1;
                else
                    NSH = 1;

                if (SH != NSH)
                {
                    if ((va[mUAxisIndex] > 0) && (vb[mUAxisIndex] > 0))
                    {
                        /* 
                         * Line crossed +U
                         */
                        NC++;
                    }
                    else
                    {
                        if ((va[mUAxisIndex] > 0) || (vb[mUAxisIndex] > 0))
                        {
                            //line might cross +U, so compute U intersectoin
                            uIntersect = va[mUAxisIndex] - (va[mVAxisIndex] *
                                         (vb[mUAxisIndex] - va[mUAxisIndex]) /
                                         (vb[mVAxisIndex] - va[mVAxisIndex]));
                            if (uIntersect > 0)
                            {
                                // Line crossed +U
                                NC++;
                            }
                        }
                    }
                }
                SH = NSH;
                va[0] = vb[0];
                va[1] = vb[1];
                va[2] = vb[2];
            }

            if ((NC % 2) == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Intersects the ray, if intersection is closer than the one inside the record,
        /// sets the records with intersection information.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public override bool Intersect(Ray ray, IntersectionRecord record)
        {
            float dist= 0f;
            Vector3 hitPt, n;
            
            n = mNormal;    // because ray/plane intersection may flip the normal!
            if (!RayPlaneIntersection(ray, ref n, mD, ref dist))
                return false;

            /*
             * rectangle behind the ray or there are other closer intersections
             */
            if ((dist < 0) || (dist > record.HitDistance))
                return false;

            hitPt = ray.Origin + (ray.Direction * dist);

            /*
             * Now need to decide inside or outside
             */
            if (!InsidePolygon(hitPt))
                return false;

            record.UpdateRecord(dist, hitPt, n, ray, mMaterialIndex, GetResourceIndex());
            return true;
        }
        
        /// <summary>
        /// pt is a position on in the rectangle, returns the normalized U/V value (between 0 to 1)
        /// No error checking, if pt is not on the geometry, return values are undefined.
        /// </summary>
        /// <param name="pt">position inside the rectangle (no error checking!)</param>
        /// <param name="bc">barrycentric coordinate of hit point (for triangle only)</param>
        /// <param name="u">returned normalized u value</param>
        /// <param name="v">returned normalized v value</param>
        public override void GetUV(Vector3 pt, Vector3 bc, ref float u, ref float v) {
            Vector3 P = pt - mVertices[0];
            u = Vector3.Dot(P, muVec) / muSize;
            v = Vector3.Dot(P, mvVec) / mvSize;
        }


                /// <summary>
        /// recreives (u,v) and returns the object position that corresponds to the (u,v) coordinate.
        /// </summary>
        /// <param name="u">normalized u</param>
        /// <param name="v">normalized v</param>
        /// <returns>A position that cooresponds to (u,v) on the geometry </returns>
        public override Vector3 GetPosition(float u, float v)
        {
            return mVertices[0] + (u * muSize * muVec) + (v * mvSize * mvVec);
        }

        /// <summary>
        /// Returns the "approximated center" of the rectangle. By simple averaging.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCenter() { 
            Vector3 c = mVertices[0];
            for (int i=1; i<4; i++)
                c+=mVertices[i];
            c /= 4f;
            return c;
        }

        /// <summary>
        /// Accessing functions:
        /// </summary>
        public float GetUSize() { return muSize; }
        public float GetVSize() { return mvSize; }
        public Vector3 GetNormal() { return mNormal; }

        public override Vector3 Max
        {
            get
            {
                Vector3 p = Vector3.Max(mVertices[0], mVertices[1]);
                p = Vector3.Max(p, mVertices[2]);
                p = Vector3.Max(p, mVertices[3]);
                return p;
            }
        }

        public override Vector3 Min
        {
            get
            {
                Vector3 p = Vector3.Min(mVertices[0], mVertices[1]);
                p = Vector3.Min(p, mVertices[2]);
                p = Vector3.Min(p, mVertices[3]);
                return p;
            }
        }

    }
}
