using System;
using System.Collections.Generic;
using System.Text;
using UWBGL_XNA_Lib;
using Microsoft.Xna.Framework;

using RayTracer_552;

namespace RTViewer
{
    internal class RTPixelInWorld : RTPixelToShow
    {
        

        public RTPixelInWorld() : base()
        {
        }


        protected override bool GetPixelPosition(ref Vector3 p, Vector3 cameraPos, float dist)
        {
            bool once = dist < (float.MaxValue / 2f);
            if (once)
            {
                Vector3 v = p - cameraPos;
                v.Normalize();
                p = cameraPos + dist * v;
            }
            return once;
        }
    }
}
