using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Windows.Forms;

namespace RayTracer_552
{
    /// <summary>
    /// Place where you can initialize your implementation specific data structure
    /// </summary>
    public partial class RTCore
    {
        private static Random mRand;
        private bool mOrthoRT = false;

        private void FirstTimeInit()
        {
            mRand = new Random();

            mCamera.InitializeImage(mImageSpec);
            mCamera.computeCameraPositions(mSceneDatabase);
           
            for (int l = 0; l<mSceneDatabase.GetNumLights(); l++)
                mSceneDatabase.GetLight(l).InitializeLight(mSceneDatabase);

            InitializeKdTree();
        }

        public void SetOrthoRT(bool on)
        {
            mOrthoRT = on;
        }
    }
}
