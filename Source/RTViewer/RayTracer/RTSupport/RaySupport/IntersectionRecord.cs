using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// Records important information at each visible intersection
    /// </summary>
    public class IntersectionRecord
    {
        private Vector3 mPoint;     // Interseciton position
        private Vector3 mBC;        // Barry centric coordinate for triangle
        private Vector3 mNormal;    // Normal at the intersection position
        private Vector3 mRayDir;    // ray direciton that caused the intesection
        private float mHitDistance; // distance from the mPoint to the origin of the interesecting ray
        private int mGeomIndex;     // index into the geometry (of the global geometry) array
        private int mMaterialIndex; // index into the material array (of the global material) array

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public IntersectionRecord()
        {
            InitializeRecord();
        }

        /// <summary>
        /// Contructs with a default hitDistance
        /// </summary>
        /// <param name="hitDist"></param>
        public IntersectionRecord(float hitDist)
        {
            InitializeRecord();
            mHitDistance = hitDist;
        }

        /// <summary>
        /// Initialize the record to nothing is visible (nothing has intersected)
        /// </summary>
        private void InitializeRecord()
        {
            mPoint = new Vector3(0f, 0f, 0f);
            mBC = new Vector3(0f, 0f, 0f);
            mNormal = new Vector3(0f, 1f, 0f);
            mRayDir = new Vector3(0f, 0f, 1f);
            mHitDistance = float.MaxValue;
            mGeomIndex = RTCore.kInvalidIndex;
            mMaterialIndex = RTCore.kInvalidIndex;
        }

        /// <summary>
        /// Sets the record with new data from an valid intersection.
        /// </summary>
        /// <param name="dist">distance to the intersection</param>
        /// <param name="intersectionPt">the visible (intersection) point</param>
        /// <param name="normal">normal vector at the visible point</param>
        /// <param name="ray">ray that caused the intersection</param>
        /// <param name="matIndex">material index at the intersection point</param>
        /// <param name="geomIndex">geometry index at the interesection point</param>
        public void UpdateRecord(float dist, Vector3 intersectionPt, Vector3 normal, Ray ray, int matIndex, int geomIndex)
        {
            mHitDistance = dist;
            mPoint = intersectionPt;
            mNormal = normal;
            mNormal.Normalize();
            mMaterialIndex = matIndex;
            mGeomIndex = geomIndex;
            mRayDir = ray.Direction;
        }
        
        public void UpdateBC(float u, float v, float w) {
            mBC.X = u;
            mBC.Y = v;
            mBC.Z = w;
        }

        /// <summary>
        /// Accessing functions
        /// </summary>
        public float HitDistance { get { return mHitDistance; } }
        public int MaterialIndex { get { return mMaterialIndex; } }
        public int GeomIndex { get { return mGeomIndex; } }
        public Vector3 IntersectPosition { get { return mPoint; } }
        public Vector3 NormalAtIntersect { get { return mNormal; } }
        public Vector3 RayDirection { get { return mRayDir; } }
        public Vector3 HitPtBC { get { return mBC; }}

        public void SetNormalAtIntersection(Vector3 n) { mNormal = n; }
    }
}