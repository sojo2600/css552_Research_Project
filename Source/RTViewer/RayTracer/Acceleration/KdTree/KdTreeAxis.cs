using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    public class KdTreeAxis
    {
        private enum KdSplitAxis
        {
            KdSplitXAxis = 0,
            KdSplitYAxis = 1,
            KdSplitZAxis = 2
        };
    
        private KdSplitAxis mSplitAxis = KdSplitAxis.KdSplitXAxis;
        
        public KdTreeAxis(KdTreeAxis axis)
        {
            mSplitAxis = axis.mSplitAxis;
        }

        public KdTreeAxis(int split)
        {
            mSplitAxis = (KdSplitAxis)(split % 3);
        }
        
        public KdTreeAxis NextSplitAxis
        {
            get
            {
                return new KdTreeAxis((int)(mSplitAxis + 1));
            }
        }
        public KdTreeAxis PrevSplitAxis
        {
            get
            {
                int pre = (int)(mSplitAxis - 1);
                return new KdTreeAxis((pre < 0 ? 2 : pre));
            }
        }
        public bool IsSplitOnX { get { return mSplitAxis == KdSplitAxis.KdSplitXAxis; } }
        public bool IsSplitOnY { get { return mSplitAxis == KdSplitAxis.KdSplitYAxis; } }
        public bool IsSplitOnZ { get { return mSplitAxis == KdSplitAxis.KdSplitZAxis; } }

        public float GetVectorComponent(Vector3 p)
        {
            float v = -1f;
            switch (mSplitAxis)
            {
                case KdSplitAxis.KdSplitXAxis:
                    v = p.X;
                    break;
                case KdSplitAxis.KdSplitYAxis:
                    v = p.Y;
                    break;
                case KdSplitAxis.KdSplitZAxis:
                    v = p.Z;
                    break;
            }
            return v;
        }

        public Vector3 SetVectorComponent(Vector3 p, float v)
        {
            switch (mSplitAxis)
            {
                case KdSplitAxis.KdSplitXAxis:
                    p.X = v;
                    break;
                case KdSplitAxis.KdSplitYAxis:
                    p.Y = v;
                    break;
                case KdSplitAxis.KdSplitZAxis:
                    p.Z = v;
                    break;
            }
            return p;
        }
    }
}