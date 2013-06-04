using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

 // KdTreeNode - boundary coordinate (BoundingBox) and a list of premitives and left and right neighbors

namespace RayTracer_552
{
    public class KdTreeNode
    {
        private List<RTGeometry> mGeomList;
        private KdTreeNode mLeft;
        private KdTreeNode mRight;
        private float mSplitPos = 0.0f;
        private KdTreeAxis mSplitAxis;
        private BoundingBox mBounds;

        // use for advancing ray origin well into the middle of the volume without going out
        private float mMinDimension;

        public KdTreeNode(Vector3 min, Vector3 max, KdTreeAxis useAxis, List<RTGeometry> list)
        {
            mLeft = null;
            mRight = null;
            mGeomList = null;
            mBounds = new BoundingBox(min, max);
            mSplitAxis = new KdTreeAxis(useAxis);

            ComputeSplitPosition(list);
        }
        
        public KdTreeNode LeftChild { set { mLeft = value; } get { return mLeft; } }
        public KdTreeNode RightChild { set { mRight = value; } get { return mRight; } }

        public BoundingBox Bounds { get { return mBounds; } }

        public float SplitPosition { get { return mSplitPos; } }
        public KdTreeAxis SplitAxis { get { return mSplitAxis; } }

        public bool IsLeafNode() { return mRight == null && mLeft == null; }
        public void SetGeomList(List<RTGeometry> list) { mGeomList = list; }

        private bool PositionLeftOfSplit(Vector3 p)
        {
            return (mSplitAxis.GetVectorComponent(p) < SplitPosition);
        }
        private bool PositionRightOfSplit(Vector3 p)
        {
            return (mSplitAxis.GetVectorComponent(p) > SplitPosition);
        }

        public bool FindNearest(Ray ray, IntersectionRecord rec, int exceptGeom)
        {
            float enterD=0f, exitD=0f;
            if (!ComputeEntryAndExitSignedDistances(ray, out enterD, out exitD))
                return false;

            if (IsLeafNode())
            {
                bool hit = IntersectWithGeom(ray, rec, exceptGeom);
                return (hit &&
                         (rec.HitDistance >= enterD) &&
                         (rec.HitDistance <= exitD)
                         );
            }

            Vector3 enterPos = ray.Origin + enterD * ray.Direction;
            Vector3 exitPos = ray.Origin + exitD * ray.Direction;
            bool rightHit = false;
            bool leftHit = false;

            if (PositionLeftOfSplit(enterPos)) {
                leftHit = LeftChild.FindNearest(ray, rec, exceptGeom);
                if ((!leftHit) && PositionRightOfSplit(exitPos)) 
                    rightHit = RightChild.FindNearest(ray, rec, exceptGeom);
            } else {
                rightHit = RightChild.FindNearest(ray, rec, exceptGeom);
                if ((!rightHit) && PositionLeftOfSplit(exitPos))
                    leftHit = LeftChild.FindNearest(ray, rec, exceptGeom);
            }
            return leftHit || rightHit;
        }

        private bool IntersectWithGeom(Ray ray, IntersectionRecord rec, int exceptGeom)
        {
            bool hitOnce = false;
            if (mGeomList != null)
            {
                foreach (RTGeometry g in mGeomList)
                    if ((g.GetResourceIndex() != exceptGeom) && g.Intersect(ray, rec))
                        hitOnce = true;
            }
            return hitOnce;
        }

        // Algorithm copied from: http://www.cs.utah.edu/~awilliam/box/box.pdf
        // This is the non-optimized version of:
        /*
         * Ray-box intersection using IEEE numerical properties to ensure that the
         * test is both robust and efficient, as described in:
         *
         *      Amy Williams, Steve Barrus, R. Keith Morley, and Peter Shirley
         *      "An Efficient and Robust Ray-Box Intersection Algorithm"
         *      Journal of graphics tools, 10(1):49-54, 2005
         *
         */
        private bool ComputeEntryAndExitSignedDistances(Ray rtRay, out float tMin, out float tMax)
        {
            float yMin = float.MaxValue, zMin = float.MaxValue;
            float yMax = float.MinValue, zMax = float.MinValue;
            tMin = float.MaxValue;
            tMax = float.MinValue; 
            if (rtRay.Direction.X > 0)
            {
                tMin = (Bounds.Min.X - rtRay.Origin.X) / rtRay.Direction.X;
                tMax = (Bounds.Max.X - rtRay.Origin.X) / rtRay.Direction.X;
            }
            else if (rtRay.Direction.X < 0)
            {
                tMin = (Bounds.Max.X - rtRay.Origin.X) / rtRay.Direction.X;
                tMax = (Bounds.Min.X - rtRay.Origin.X) / rtRay.Direction.X;
            }
            if (rtRay.Direction.Y > 0)
            {
                yMin = (Bounds.Min.Y - rtRay.Origin.Y) / rtRay.Direction.Y;
                yMax = (Bounds.Max.Y - rtRay.Origin.Y) / rtRay.Direction.Y;
            }
            else if (rtRay.Direction.Y < 0)
            {
                yMin = (Bounds.Max.Y - rtRay.Origin.Y) / rtRay.Direction.Y;
                yMax = (Bounds.Min.Y - rtRay.Origin.Y) / rtRay.Direction.Y;
            }

            if ((tMin > yMax) || (yMin > tMax))
                return false;
            if (yMin > tMin)
                tMin = yMin;
            if (yMax < tMax)
                tMax = yMax;

            if (rtRay.Direction.Z > 0)
            {
                zMin = (Bounds.Min.Z - rtRay.Origin.Z) / rtRay.Direction.Z;
                zMax = (Bounds.Max.Z - rtRay.Origin.Z) / rtRay.Direction.Z;
            }
            else if (rtRay.Direction.Z < 0)
            {
                zMin = (Bounds.Max.Z - rtRay.Origin.Z) / rtRay.Direction.Z;
                zMax = (Bounds.Min.Z - rtRay.Origin.Z) / rtRay.Direction.Z;
            }

            if ((tMin > zMax) || (zMin > tMax))
                return false;
            if (zMin > tMin)
                tMin = zMin;
            if (zMax < tMax)
                tMax = zMax;

            return true;

#if THIS_IS_USELESS
            // apparantly ray/BoundingBox returns _one_ of the intersection positions, but not always the closest one!! 
            // USELESS!!
            Microsoft.Xna.Framework.Ray r = new Microsoft.Xna.Framework.Ray(rtRay.Origin, rtRay.Direction);

            float? dist = r.Intersects(Bounds);
            if (null == dist)
                return null;
            
            float[] distances = new float[2];
            if (Bounds.Contains(rtRay.Origin) == ContainmentType.Contains)
            { // originated from inside, let's find the back-end existing position
                Microsoft.Xna.Framework.Ray backRay = new Microsoft.Xna.Framework.Ray(rtRay.Origin, -rtRay.Direction);
                float? backDist = backRay.Intersects(Bounds);
                distances[0] = (float)-backDist; // this will be a negative value
                distances[1] = (float)dist;
            }
            else
            {
                // dist is the entry position, now find the exist position
                distances[0] = (float)dist;
                // advance the ray position without going out of this node
                r.Position = r.Position + (distances[0] + float.Epsilon) * r.Direction;
                if (Bounds.Contains(r.Position) == ContainmentType.Contains)
                {
                    distances[1] = (float)r.Intersects(Bounds);
                }
                else
                {
                    distances[1] = distances[0] + float.Epsilon;
                }
            }
            return distances;
#endif
        }
        
        private void ComputeSplitPosition(List<RTGeometry> list)
        {
            float bestPos = 0.0f;
            float bestCost = 1000000f;

            float leftExtreme = 0.0f;
            float rightExtreme = 0.0f;

            // THIS OPTIMAL SPLITTING PLANE ALGORIHM IS TAKEN OUT FOR TIME CONSTRAINT -
            // Takes WAY too long to do in complex scenes..
            //foreach (RTGeometry g in node.GeomList)
            //{
            //    leftExtreme = g.GetBoundingBox().GetAxisAlignedMinOn(axis);
            //    rightExtreme = g.GetBoundingBox().GetAxisAlignedMaxOn(axis);
            //    float leftCost = CaculateCost(node, axis, leftExtreme);
            //    if (leftCost < bestCost)
            //    {
            //        bestCost = leftCost;
            //        bestPos = leftExtreme;
            //    }
            //    float rightCost = CaculateCost(node, axis, rightExtreme);
            //    if (rightCost <= bestCost)
            //    {
            //        bestCost = rightCost;
            //        bestPos = rightExtreme;
            //    }
            //}   

            // Use uniform subdivision for final demo purposes
            mSplitPos = 0.5f * (mSplitAxis.GetVectorComponent(mBounds.Min) + mSplitAxis.GetVectorComponent(mBounds.Max));
        }
    }

}