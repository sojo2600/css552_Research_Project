using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
using RayTracer_552.RTSupport;

namespace RayTracer_552
{
    /// <summary>
    /// NO CHANGES NECESSARY
    /// 
    /// The scene database for the RT
    /// 
    /// </summary>
    public partial class RTCore
    {
        // RT computation needs
        private RT3DCamera mCamera;
        private ImageSpec mImageSpec;
        private SceneDatabase mSceneDatabase;

        // Spec. for how to trace rays: parsed in RTCore_Parse.cs
        private int mGeneration = 0;
        private bool mComputeShadow = false;
        private bool mComputeReflection = false;
        private Vector3 mBgColor = new Vector3(0f, 0f, 0f);
        

        public void SetCamera(RTCamera c)
        {
            mCamera = new RT3DCamera(c);
        }
        public RTCamera GetCamera() { return mCamera; }

        public void SetImageSpec(ImageSpec s)
        {
            mImageSpec = s;
        }
        
        public int  ImageWidth { get { return mImageSpec.XResolution; } }
        public int  ImageHeight { get { return mImageSpec.YResolution; } }
        public SceneDatabase GetSceneDatabase() { return mSceneDatabase; }
    }
}
