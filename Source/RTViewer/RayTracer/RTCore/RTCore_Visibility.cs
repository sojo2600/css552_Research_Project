#define KDTREE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using System.Drawing;

namespace RayTracer_552
{

    public partial class RTCore
    {
        private void ComputeVisibility(Ray r, IntersectionRecord rec, int exceptGeomIndex)
        {
#if KDTREE
            mKdTree.ComputeVisibility(r, rec, exceptGeomIndex);
#else
            for (int i = 0; i < mSceneDatabase.GetNumGeom(); i++)
            {
                if (i != exceptGeomIndex)
                {
                    RTGeometry g = mSceneDatabase.GetGeom(i);
                    g.Intersect(r, rec);
                }
            }
#endif
        }
        
        private bool IsBlocked(Vector3 org, Vector3 target, int exceptGeomIndex) 
        {
            Vector3 d = target - org;
            float dist = d.Length();
            IntersectionRecord rec = new IntersectionRecord(dist);
            d /= dist;
            Ray r = Ray.CrateRayFromPtDir(org, d);
#if KDTREE

            mKdTree.ComputeVisibility(r, rec, exceptGeomIndex);
            return (rec.GeomIndex != RTCore.kInvalidIndex);
#else   
            bool blocked = false;
            int count = 0;
            
            while ((!blocked) && (count < mSceneDatabase.GetNumGeom())) {
                if (count != exceptGeomIndex)
                    blocked = mSceneDatabase.GetGeom(count).Intersect(r, rec);
                count++;
            }
            return blocked;
#endif
        }
    }
}