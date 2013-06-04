using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552.RTSupport
{
    public class RT3DCamera : RTCamera
    {
        protected Vector3 leftCamera;
        protected Vector3 rightCamera;

        public RT3DCamera(CommandFileParser parser, SceneDatabase sceneDB) 
            : base(parser)
        {
            computeCameraPositions(sceneDB);
        }

        public RT3DCamera(RTCamera c, SceneDatabase sceneDB) 
            : base(c)
        {
            computeCameraPositions(sceneDB);
        }

        public RT3DCamera(RTCamera c)
            : base(c)
        {}

        public void computeCameraPositions(SceneDatabase sceneDB)
        {
            float shortestDist = float.MaxValue;
            float maxDist, minDist, curDist;
            for (int i = 0; i < sceneDB.GetNumGeom(); i++)
            {
                RTGeometry g = sceneDB.GetGeom(i);
                maxDist = (mEye - g.Max).Length();
                minDist = (mEye - g.Min).Length();

                if (maxDist < minDist)
                    curDist = maxDist;
                else
                    curDist = minDist;

                if (curDist < shortestDist)
                    shortestDist = curDist;
            }// end for

            float eyeSeparation = shortestDist / 30;
            leftCamera = mEye + mSideVec * shortestDist / 2;
            rightCamera = mEye - mSideVec * shortestDist / 2;
        }

        public Vector3 RightEyePosition { get { return rightCamera; } }
        public Vector3 LeftEyePosition { get { return leftCamera; } }
    }// end class
}
