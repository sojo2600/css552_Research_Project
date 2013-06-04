using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Windows.Forms;

namespace RayTracer_552
{
    /// <summary>
    /// 
    /// PLEASE DO NOT CHANGE!!
    /// 
    /// Constructor and data you will need to use:
    /// 
    ///     1. mStatsArea.Text: is the text debug output place!
    ///     2. mCurrentX/mCurrentY: 
    ///         is the pixel you need to loop through to compute the image
    ///         refer to RTCore_Compute.cs::ComputeImage() image function
    ///     
    ///     3. the constructor calls FirstTimeInit() (defined in RTCore_Init.cs)
    ///         you can initialize any your implementation specific internal data structure here!!
    /// 
    /// </summary>
    public partial class RTCore
    {
        public const int kInvalidIndex = -1;

        private RTWindow mRTWindows = null; 

        // current pixel being computed. These two variables only make sense during single threaded operation!!
        private int mCurrentX, mCurrentY;

        public RTCore(RTWindow w, String cmdFile, ContentManager meshLoader)
        {
            mRTWindows = w;
            
            mSceneDatabase = new SceneDatabase();
            mParser = new CommandFileParser(cmdFile, meshLoader, w.StatusArea(), this, mSceneDatabase);
            mCurrentX = mCurrentY = 0;

            //
            FirstTimeInit();
        }
 
        public int CurrentX { get { return mCurrentX; } }
        public int CurrentY { get { return mCurrentY; } }
    }
}
