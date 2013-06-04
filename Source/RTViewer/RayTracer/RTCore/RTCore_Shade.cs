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
        /// <summary>
        ///
        /// </summary>
        private Vector3 ComputeShading(IntersectionRecord rec, int generation)
        {
            if (rec.GeomIndex == RTCore.kInvalidIndex)
                return mBgColor;

            Vector3 resultColor = Vector3.Zero;
            Vector3 V = -Vector3.Normalize(rec.RayDirection);

            RTMaterial m = mSceneDatabase.GetMaterial(rec.MaterialIndex);
            Vector3 useNormal = m.GetNormal(mSceneDatabase, rec);

            for (int l = 0; l < mSceneDatabase.GetNumLights(); l++)
            {
                RTLight lgt = mSceneDatabase.GetLight(l);
                resultColor += ShadeRec(V, lgt, rec);
            }

            resultColor += mSceneDatabase.GetMaterial(rec.MaterialIndex).GetAmbient(mSceneDatabase, rec);
            int nextGen = generation + 1;

            // now take care of reflection
            float reflectivity = m.GetReflectivity(mSceneDatabase, rec);
            Vector3 reflColor = Vector3.Zero;
            if ( mComputeReflection && (reflectivity > 0f) && (generation < mGeneration) )
            {
                IntersectionRecord refRec = new IntersectionRecord();
                Vector3 refDir = Vector3.Reflect(rec.RayDirection, useNormal);
                Ray refRay = Ray.CrateRayFromPtDir(rec.IntersectPosition, refDir);
                // Now compute new visibility
                ComputeVisibility(refRay, refRec, rec.GeomIndex);
                if (refRec.GeomIndex != RTCore.kInvalidIndex)
                    reflColor = ComputeShading(refRec, nextGen) * reflectivity;
                else
                    reflColor = mBgColor * reflectivity;
            }


            // now do transparency
            //
            // The following code: 
            //     1. only supports refractive index > 1 (from less dense e.g.: air, to more dense: e.g., glass)
            //     2. cannot go from dense material into less dense material (once enter cannot exit)
            //     3. once entered: do not know how to go from one transparent object into another transparent object
            //
            float transparency = m.GetTransparency(mSceneDatabase, rec);
            Vector3 transColor = Vector3.Zero; 
            if (mComputeReflection && (transparency > 0f) && (generation < mGeneration))
            {
                float cosThetaI = Vector3.Dot(V, useNormal);
                float invN = 1f / m.GetRefractiveIndex;
                float cosThetaT = (float)Math.Sqrt(1 - ((invN * invN) * (1 - (cosThetaI * cosThetaI))));
                Vector3 transDir = -((invN * V) + ((cosThetaT - (invN * cosThetaI)) * useNormal));
                Ray transRay = Ray.CrateRayFromPtDir(rec.IntersectPosition, transDir);
                // Now compute new visibility
                IntersectionRecord transRec = new IntersectionRecord();
                ComputeVisibility(transRay, transRec, rec.GeomIndex);  // here we assume single layer geometries
                if (transRec.GeomIndex != RTCore.kInvalidIndex)
                    transColor = ComputeShading(transRec, nextGen) * transparency;
                else
                    transColor = mBgColor * transparency;
            }
            resultColor = (1 - transparency - reflectivity) * resultColor + transColor + reflColor;

            return resultColor;
        }

        private Vector3 ShadeRec(Vector3 V, RTLight lgt, IntersectionRecord rec)
        {
            RTMaterial m = mSceneDatabase.GetMaterial(rec.MaterialIndex);
            Vector3 r = Vector3.Zero;

            float percentToUse = 1f;
            if (mComputeShadow) 
                percentToUse = lgt.PercentVisible(rec.IntersectPosition, rec.GeomIndex, mSceneDatabase);

            if (percentToUse > 0f)
            {
                Vector3 L = lgt.GetNormalizedDirection(rec.IntersectPosition);
                float NdotL = Vector3.Dot(rec.NormalAtIntersect, L);

                if (NdotL > 0)
                {
                    Vector3 diffColor = m.GetDiffuse(mSceneDatabase, rec);
                    Vector3 lightColor = percentToUse * lgt.GetColor(rec.IntersectPosition);
                    r += diffColor * NdotL * lightColor;

                    Vector3 R = 2 * NdotL * rec.NormalAtIntersect - L;
                    float VdotR = Vector3.Dot(V, R);

                    if (VdotR > 0)
                        r += m.GetSpecular(mSceneDatabase, rec) * (float)Math.Pow(VdotR, m.GetN) * lightColor;
                }
            }
            
            return r;
        }

    }
}