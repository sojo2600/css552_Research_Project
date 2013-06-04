using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// LightType class: super class for all light source types: 
    /// 
    /// Point, Directional, and Spotlight
    /// 
    /// </summary>
    public abstract class RTLightType
    {
        public enum RTLightSourceType
        {
            RTLightSourceTypeDirection,
            RTLightSourceTypePoint,
            RTLightSourceTypeSpot
        };

        protected RTLightSourceType mLightSourceType;
        protected Vector3 mPosition = Vector3.Zero;
        protected Vector3 mColor = Vector3.Zero;


        protected virtual float DistanceToLight(Vector3 visiblePt) { return (visiblePt - mPosition).Length(); }

        /// <summary>
        /// Initialization of light after the entire scene has been read in
        /// </summary>
        /// <param name="sceneDatabase"></param>
        public virtual void InitializeLight(SceneDatabase sceneDatabase) { }

        /// <summary>
        /// Given the point to be light, returns the color form the light source towards the point
        /// </summary>
        /// <param name="visiblePt"></param>
        /// <returns></returns>
        public virtual Vector3 GetColor(Vector3 visiblePt) { return mColor; }

        /// <summary>
        /// Percentage of the light that is visible from the visiblePt
        /// </summary>
        /// <param name="visiblePt"></param>
        /// <param name="exceptGeomIndex">this geometry can never block the light (geomIndex of the visiblePt)</param>
        /// <param name="database"></param>
        /// <returns></returns>
        public virtual float PercentVisible(Vector3 visiblePt, int exceptGeomIndex, SceneDatabase sceneDatabase)
        {
            IntersectionRecord rec = new IntersectionRecord(DistanceToLight(visiblePt));
            Ray r = Ray.CrateRayFromPtDir(visiblePt, GetNormalizedDirection(visiblePt));

            bool blocked = false;
            int count = 0;

            while ((!blocked) && (count < sceneDatabase.GetNumGeom()))
            {
                if (count != exceptGeomIndex)
                    blocked = sceneDatabase.GetGeom(count).Intersect(r, rec);
                count++;
            }
            return (blocked ? 0f : 1f);
        }

        /// <summary>
        /// Returns the normalized L vector for illumination computation.
        /// </summary>
        /// <param name="visiblePt"></param>
        /// <returns></returns>
        public virtual Vector3 GetNormalizedDirection(Vector3 visiblePt) { return Vector3.Normalize(mPosition - visiblePt); }


        /// <summary>
        /// returns the position of the light source
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLightPosition() { return mPosition; }

        /// <summary>
        /// returns the type of light source: Directional, Point, or Spot
        /// </summary>
        public RTLightSourceType GetLightSourceType() { return mLightSourceType; }
    }
}
