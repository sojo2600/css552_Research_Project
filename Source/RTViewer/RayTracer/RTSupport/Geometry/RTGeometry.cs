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
    public abstract partial class RTGeometry : IndexedResource
    {
        public enum RTGeometryType
        {
            Sphere,
            Rectangle,
            Triangle
        };

        public RTGeometry()
        { }

        /// <summary>
        /// Returns status of if Ray intersects with this Geom. If so, details of intersection
        /// is returned in the IntersectionRecord.
        /// </summary>
        /// <param name="ray">Incoming ray.</param>
        /// <param name="record">If intersect, this record has the details.</param>
        /// <returns>T/F: intersect or not.</returns>
        public abstract bool Intersect(Ray ray, IntersectionRecord record);

        /// <summary>
        /// Min position
        /// </summary>
        /// <returns></returns>
        public abstract Vector3 Min { get; }

        /// <summary>
        /// Min position
        /// </summary>
        /// <returns></returns>
        public abstract Vector3 Max { get; }


        /// <summary>
        /// pt is a position on the geometry, returns the normalized U/V value (between 0 to 1)
        /// No error checking, if pt is not on the geometry, return values are undefined.
        /// </summary>
        /// <param name="pt">position on the geometry (no error checking!)</param>
        /// <param name="bc">barrycentric coordinate of hit point (for triangle only)</param>
        /// <param name="u">returned normalized u value</param>
        /// <param name="v">returned normalized v value</param>
        public abstract void GetUV(Vector3 pt, Vector3 bc, ref float u, ref float v);


        /// <summary>
        /// recreives (u,v) and returns the object position that corresponds to the (u,v) coordinate.
        /// </summary>
        /// <param name="u">normalized u</param>
        /// <param name="v">normalized v</param>
        /// <returns>A position that cooresponds to (u,v) on the geometry </returns>
        public abstract Vector3 GetPosition(float u, float v);

        protected RTGeometryType mType;
        public RTGeometryType GeomType() { return mType; }

        protected int mMaterialIndex;
        public int GetMaterialIndex() { return mMaterialIndex; }


        /// <summary>
        /// Dot(norm, P) + d = 0 is the plane equation
        /// Intersects the ray with the plane returns T/F
        /// if T 
        ///     dist is the distance between the ray and the plane (may be negative)
        /// if F
        ///     dist is undefined. Ray and Plane is parallel
        ///  
        /// **WARNING**: this function flips the norm vector if the plane is facing
        ///              away from the ray directoin!!
        /// </summary>
        /// <param name="r">Ray to intersect the plane</param>
        /// <param name="norm">Normal (normalized) vector of the plane</param>
        /// <param name="d">Dot(norm, P) + d = 0.</param>
        /// <param name="dist">Returned: Distance between the ray to the plane if the ray is not paralle with the plane</param>
        /// <returns>True, if there is an interesection, False, if ray is parallel to the plane</returns>
        protected bool RayPlaneIntersection(Ray r, ref Vector3 norm, float d, ref float dist)
        {
            float denomenator;

            denomenator = Vector3.Dot(norm, r.Direction);

            /*
             * Ray parallel to normal
             */
            if (Math.Abs(denomenator) < float.Epsilon)
                return false;

            float NdotO = Vector3.Dot(norm, r.Origin);
            dist = -(NdotO + d) / denomenator;

            if (denomenator > 0)
                norm = -norm;

            return true;
        }
    }
}
