using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// A Sphere.
    /// </summary>
    public class RTSphere : RTGeometry
    {
        private Vector3 mCenter;        //center
        private float mRadius;          // radius
        private float mRadiusSquared;   // r*r

        // to anchor texture map
        private Vector3 kPole = Vector3.UnitY;
        private Vector3 kEquator = Vector3.UnitX;
        private Vector3 kRef = Vector3.UnitZ;

        /// <summary>
        ///  constructs from parsing command file.
        /// </summary>
        /// <param name="parser"></param>
        public RTSphere(CommandFileParser parser)
        {
            mType = RTGeometryType.Sphere;

            // do we have a transform?
            bool hasTransform = false;
            Matrix xform = Matrix.Identity;
            
            mCenter = new Vector3(0f, 0f, 0f);
            mRadius = 1f;
            mMaterialIndex = 0;

            parser.ParserRead();
            while (!parser.IsEndElement("sphere"))
            {
                if (parser.IsElement() && (!parser.IsElement("sphere")))
                {
                    if (parser.IsElement("center"))
                        mCenter = parser.ReadVector3();
                    else if (parser.IsElement("radius"))
                        mRadius = parser.ReadFloat();
                    else if (parser.IsElement("material"))
                        mMaterialIndex = parser.ReadInt();
                    else if (parser.IsElement("xform"))
                    {
                        hasTransform = true;
                        xform = ParseTransform(parser);
                    }
                    else
                        parser.ParserError("Sphere");
                }
                else
                    parser.ParserRead();
            }
            if (hasTransform)
            {
                Vector3 p = mCenter + Vector3.One * mRadius;

                mCenter = Vector3.Transform(mCenter, xform);
                p = Vector3.Transform(p, xform);

                // ****WARNING****: does not handle unporportional scaling!!
                mRadius = (mCenter - p).Length();
            }
            mRadiusSquared = mRadius * mRadius;
        }

        /// <summary>
        /// Intersects the ray with the sphere. If intersection is closer than what is
        /// in the record, updates the record with new intersection information.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        public override bool Intersect(Ray ray, IntersectionRecord record)
        {
            Vector3 v1 = ray.Origin - mCenter;
            float b = 2f * Vector3.Dot(v1, ray.Direction);
            float c = v1.LengthSquared() - mRadiusSquared;
            float root = b * b - 4 * c;

            if (root < 0f)
                return false;

            root = (float) Math.Sqrt(root);
            float t0 = 0.5f * (-b - root);
            float t1 = 0.5f * (-b + root);

            if ((t0 < 0) && (t1 < 0))
                return false;

            float dist;
            if (t0 < t1)
            {
                if (t0 > 0)
                    dist = t0;
                else
                    dist = t1;
            }
            else
            {
                if (t1 > 0)
                    dist = t1;
                else
                    dist = t0;
            }

            if (dist > record.HitDistance)
                return false;

            // intersection found
            Vector3 pt = ray.Origin + dist * ray.Direction;
            Vector3 n = pt - mCenter;
            record.UpdateRecord(dist, pt, n, ray, mMaterialIndex, GetResourceIndex());

            return true;
        }

        /// <summary>
        /// pt is a position on on the sphere, returns the normalized U/V value (between 0 to 1)
        /// No error checking, if pt is not on the geometry, return values are undefined.
        /// </summary>
        /// <param name="pt">position inside the sphere (no error checking!)</param>
        /// <param name="bc">barrycentric coordinate of hit point (for triangle only)</param>
        /// <param name="u">returned normalized u value</param>
        /// <param name="v">returned normalized v value</param>
        public override void GetUV(Vector3 pt, Vector3 bc, ref float u, ref float v)
        {
            Vector3 P = pt - mCenter;
            P.Normalize();
            if (P.Y > 0.99999f)
            {
                // at the pole
                u = v = 0f;
            }
            else
            {
                v = 1f - (float)((Math.Acos(Vector3.Dot(P, kPole)) ) / Math.PI);
                Vector3 Q = new Vector3(P.X, 0f, P.Z);
                Q.Normalize();
                double theta = Math.Acos(Vector3.Dot(Q, kEquator));
                if (Vector3.Dot(Q, kRef) > 0)
                    theta = (2 * Math.PI) - theta;
                u = (float)(theta / (2 * Math.PI));
            }
        }


        /// <summary>
        /// recreives (u,v) and returns the object position that corresponds to the (u,v) coordinate.
        /// </summary>
        /// <param name="u">normalized u</param>
        /// <param name="v">normalized v</param>
        /// <returns>A position that cooresponds to (u,v) on the geometry </returns>
        public override Vector3 GetPosition(float u, float v)
        {
            double phi, theta;
            double x, y, z;    // x, y, and z component of the point

            phi = (1-v) * Math.PI;
            theta = u * 2 * Math.PI;

            y = mRadius * Math.Cos(phi);
            double projectedR = Math.Sin(phi);
            z = mRadius * Math.Sin(theta) * projectedR;
            x = mRadius * Math.Cos(theta) * projectedR;

            return new Vector3((float)x, (float)y, (float)z);
        }


        /// <summary>
        /// Accessing methods
        /// </summary>
        public Vector3 Center { get { return mCenter; } }
        public float Radius { get { return mRadius; } }


        public override Vector3 Max
        {
            get
            {
                return Center + Radius * Vector3.One;
            }
        }


        public override Vector3 Min
        {
            get
            {
                return Center - Radius * Vector3.One;
            }
        }
    }
}
