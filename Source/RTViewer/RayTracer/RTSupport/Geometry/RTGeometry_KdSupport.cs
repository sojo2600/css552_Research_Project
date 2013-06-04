using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// Geometry support: only support rectangle and sphere.
    /// </summary>
    public abstract partial class RTGeometry 
    {
        public bool LeftOf(KdTreeAxis axis, float splitValue)
        {
            return (Overlaps(axis, splitValue) ||
                (
                 (axis.IsSplitOnX && Max.X < splitValue) ||
                 (axis.IsSplitOnY && Max.Y < splitValue) ||
                 (axis.IsSplitOnZ && Max.Z < splitValue))
                 );
        }

        public bool RightOf(KdTreeAxis axis, float splitValue)
        {
            return (Overlaps(axis, splitValue) ||
                (
                 (axis.IsSplitOnX && Min.X > splitValue) ||
                 (axis.IsSplitOnY && Min.Y > splitValue) ||
                 (axis.IsSplitOnZ && Min.Z > splitValue)
                ));
        }
        public bool Overlaps(KdTreeAxis axis, float splitValue)
        {
            return (
               (axis.IsSplitOnX && Min.X <= splitValue && Max.X >= splitValue) ||
               (axis.IsSplitOnY && Min.Y <= splitValue && Max.Y >= splitValue) ||
               (axis.IsSplitOnZ && Min.Z <= splitValue && Max.Z >= splitValue));
        }
    }
}
