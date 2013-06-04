using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    public class KdTree
    {    
        // stopping criteria
        private int mMaxDepth = 0;
        private int mMaxNumOfPrimitives = 20;

        private KdTreeNode mRoot;
        public KdTreeNode RootNode() { return mRoot; }  

        public KdTree(int maxDepth, int maxLeavePrimitives, SceneDatabase sceneDatabase)
        {
            mMaxNumOfPrimitives = maxLeavePrimitives;
            mMaxDepth = maxDepth;

            // 
            List<RTGeometry> allGeom = new List<RTGeometry>();
            Vector3 worldMin = new Vector3(float.MaxValue);
            Vector3 worldMax = new Vector3(float.MinValue);

            // we need to compute the compute the world min/max
            for (int i = 0; i < sceneDatabase.GetNumGeom(); i++)
            {
                RTGeometry g = sceneDatabase.GetGeom(i);
                allGeom.Add(g);
                worldMin = Vector3.Min(worldMin, g.Min);
                worldMax = Vector3.Max(worldMax, g.Max);
            }
                
            int rootDepth = 0;
            mRoot = new KdTreeNode(worldMin, worldMax, new KdTreeAxis(rootDepth), allGeom);

            BuildKDTree(mRoot, rootDepth, allGeom);
        }

        public void BuildKDTree(KdTreeNode node, int depth, List<RTGeometry> geomList)
        {
            if (MetStopCriteria(node, depth, geomList.Count))
            {
                node.SetGeomList(geomList);
                return;
            }

            List<RTGeometry> leftList = new List<RTGeometry>();
            List<RTGeometry> rightList = new List<RTGeometry>();
            foreach (RTGeometry g in geomList)
            {
                if (g.LeftOf(node.SplitAxis, node.SplitPosition))
                    leftList.Add(g);
                if (g.RightOf(node.SplitAxis, node.SplitPosition))
                    rightList.Add(g);
            }

            Vector3 leftMin, leftMax, rightMin, rightMax;
            leftMin = node.Bounds.Min;
            leftMax = node.Bounds.Max;
            rightMin = leftMin;
            rightMax = leftMax;
            leftMax = node.SplitAxis.SetVectorComponent(leftMax, node.SplitPosition);
            rightMin = node.SplitAxis.SetVectorComponent(rightMin, node.SplitPosition);

            KdTreeAxis nextAxis = node.SplitAxis.NextSplitAxis;
            node.LeftChild = new KdTreeNode(leftMin, leftMax, nextAxis, leftList);
            node.RightChild = new KdTreeNode(rightMin, rightMax, nextAxis, rightList);

            BuildKDTree(node.LeftChild, depth + 1, leftList);
            BuildKDTree(node.RightChild, depth + 1, rightList);
        }

        private bool MetStopCriteria(KdTreeNode node, int depth, int numOfPrimitiveInNode)
        {
            return (depth > mMaxDepth || numOfPrimitiveInNode <= mMaxNumOfPrimitives);
        }
        
        public bool ComputeVisibility(Ray ray, IntersectionRecord rec, int exceptGeom)
        {
            return mRoot.FindNearest(ray, rec, exceptGeom);
        }

    }
}

