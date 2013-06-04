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
    public partial class RTTriangle : RTGeometry
    {
        private Vector3[] mVertices;    // always 3 vertices
        private Vector2[] mVertexUV;    
        
        private Vector3 mAVec, mBVec; // A=V1-V0, B=V2-V0
        private float mInvArea2;   // inverse of two times the area of the triangle
        private float mD;   // AX + BY + CZ + D = 0, this is the D
        private Vector3 mNormal; // normal to the polygon

        // support for inverting UV to compute alpha beta
        private float mInvUVDet; // 1/determinent
        private Vector2 mD02, mD12; // delta UV between vertex-0 and 2; and vertex-1 and 2

        /// <summary>
        /// Constructs from parsing file and then intialize for intersection computation.
        /// </summary>
        /// <param name="parser"></param>
        public RTTriangle(CommandFileParser parser)
        {
            mType = RTGeometryType.Triangle;

            // do we have a transform?
            bool hasTransform = false;
            Matrix xform = Matrix.Identity;

            mVertices = new Vector3[3];
            mVertexUV = new Vector2[3];
            mMaterialIndex = 0;
         
            int count = 0;
            parser.ParserRead();
            while (!parser.IsEndElement("triangle"))
            {
                if (parser.IsElement() && (!parser.IsElement("triangle")))
                {
                    if (parser.IsElement("vertices"))
                    {
                        mVertices[count] = parser.ReadVector3();
                    } else
                    if (parser.IsElement("uv"))
                    {
                        mVertexUV[count] = parser.ReadVector2();
                        count++;
                    }
                    
                    else if (parser.IsElement("xform"))
                    {
                        hasTransform = true;
                        xform = ParseTransform(parser);
                    }
                    else if (parser.IsElement("material"))
                        mMaterialIndex = parser.ReadInt();
                    else
                        parser.ParserError("Triangle");
                }
                else
                    parser.ParserRead();
            }
            if (count != 3)
            {
                parser.ParserError("Triangle: vertex indexPtr = " + count);
            }
            else
            {
                if (hasTransform)
                {
                    mVertices[0] = Vector3.Transform(mVertices[0], xform);
                    mVertices[1] = Vector3.Transform(mVertices[1], xform);
                    mVertices[2] = Vector3.Transform(mVertices[2], xform);
                }

                InitializeTriangle();
            }
        }       

        /// <summary>
        /// Initialize internal data structures for intersection
        /// </summary>
        private void InitializeTriangle()
        {
            mAVec = mVertices[1] - mVertices[0];
            mBVec = mVertices[2] - mVertices[0];

            //
            // Normal = v1 cross v2
            // Normalize Normal Vector
            //
            mNormal = Vector3.Cross(mAVec, mBVec);
            mInvArea2 = 1f / mNormal.Length();
            mNormal *= mInvArea2;
            mD = -(Vector3.Dot(mNormal, mVertices[0]));

            // for inverting UV to compute alpha and beta
            mD02 = mVertexUV[0] - mVertexUV[2];
            mD12 = mVertexUV[1] - mVertexUV[2];
            mInvUVDet = 1f / (mD02.Y * mD12.X - mD02.X * mD12.Y); // this _SHOULD_ exist!!
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
            float dist = 0f;
            Vector3 hitPt, n;
            
            n = mNormal;
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
            float u, v, w;
            Vector3 v1 = mVertices[1] - hitPt;
            Vector3 v2 = mVertices[2] - hitPt;
            float areaPV1V2 = Vector3.Dot(mNormal, Vector3.Cross(v1, v2));
                
            u = areaPV1V2 * mInvArea2;
            if ((u < 0f) || (u > 1f))
                return false;
                
            v1 = mVertices[0] - hitPt;
            float areaPV0V2 = Vector3.Dot(mNormal, Vector3.Cross(v2, v1));
            v = areaPV0V2 * mInvArea2;
            if ((v<0f) || (v>1f))
                return false;

            w = 1 - u - v;
            if (w<0f)
                return false;
                
            /*
             *  An actual hit
             */
            record.UpdateBC(u, v, w);

            // now if we have per-vertex normal, use it!
            if (null != mNormalAtVertices)
                n = u * mNormalAtVertices[0] + v * mNormalAtVertices[1] + w * mNormalAtVertices[2];

            // flip the normal if seeing the back face
            if (Vector3.Dot(n, ray.Direction) > 0f)
                n = -n;

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
            u = bc.X * mVertexUV[0].X + bc.Y * mVertexUV[1].X + bc.Z * mVertexUV[2].X;
            v = bc.X * mVertexUV[0].Y + bc.Y * mVertexUV[1].Y + bc.Z * mVertexUV[2].Y;
        }


        /// <summary>
        /// recreives (u,v) and returns the object position that corresponds to the (u,v) coordinate.
        /// </summary>
        /// <param name="u">normalized u</param>
        /// <param name="v">normalized v</param>
        /// <returns>A position that cooresponds to (u,v) on the geometry </returns>
        public override Vector3 GetPosition(float u, float v)
        {
            // 1. compute the (alpha, beta, gamma) cooridnate for UV
            // 2. linear interpolate from vertex positions

            // 1. compute BC coordinate space by remembering:
            //      u = alpha u0 + beta u1 + (1-alpha-beta) u2
            //      v = alpha v0 + beta v1 + (1-alpha-beta) v2
            //   solve for alpha and beta
            float du2 = u - mVertexUV[2].X;
            float dv2 = v - mVertexUV[2].Y;
            float alpha = mInvUVDet * ((mD12.X * dv2) - (mD12.Y * du2));
            float beta = mInvUVDet * ((mD02.X * dv2) - (mD02.Y * du2));

            // now simply linear interpolate
            return alpha * mVertices[0] + beta * mVertices[1] + (1 - alpha - beta) * mVertices[2];
        }

        public Vector3[] GetVertices() { return mVertices; }

        public override Vector3 Max
        {
            get
            {
                Vector3 p = Vector3.Max(mVertices[0], mVertices[1]);
                p = Vector3.Max(p, mVertices[2]);
                return p;
            }
        }

        public override Vector3 Min
        {
            get
            {
                Vector3 p = Vector3.Min(mVertices[0], mVertices[1]);
                p = Vector3.Min(p, mVertices[2]);

                return p;
            }
        }

    }
}
