using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// This is the RAY in ray tracing.
    /// </summary>
    public class Ray
    {
        private Vector3 mOrigin;        // origin of the ray
        private Vector3 mDirection;     // direction of the ray, normalized

        /// <summary>
        /// constructor based on two points.
        /// </summary>
        /// <param name="o">Origin of ray.</param>
        /// <param name="at">Any point in the path of the ray.</param>
        public Ray(Vector3 o, Vector3 at)
        {
            mOrigin = o;
            mDirection = at - o;
            mDirection.Normalize();
        }

        /// <summary>
        /// Disallow the creation of an uninitialized ray.
        /// </summary>
        private Ray(){}


        /// <summary>
        /// Accessing functions.
        /// </summary>
        public Vector3 Origin   { get { return mOrigin; } }
        public Vector3 Direction { get { return mDirection; } }

        /// <summary>
        /// Creates a ray from orgin and a direction.
        /// </summary>
        /// <param name="org">Origin of the ray.</param>
        /// <param name="dir">Direction of the ray (assumed to be normalized!!)</param>
        /// <returns>A Ray</returns>
        static public Ray CrateRayFromPtDir(Vector3 org, Vector3 dir)
        {
            Ray r = new Ray();
            r.mOrigin = org;
            r.mDirection = dir;
            return r;
        }

    }
}