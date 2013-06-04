using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using System.Drawing;


namespace RayTracer_552 {
    public partial class RTCore {

        private KdTree mKdTree;
        private const int kMaxTreeDepth = 30; // This should be ok?
        private const int kMaxPrimitiveInLeave = 20; 

        private void InitializeKdTree()
        {
            mKdTree = new KdTree(kMaxTreeDepth, kMaxPrimitiveInLeave, mSceneDatabase);
        }

        public KdTreeNode GetKdTreeRoot()
        {
            if (null != mKdTree)
                return mKdTree.RootNode();
            else
                return null;
        }

    }
}
