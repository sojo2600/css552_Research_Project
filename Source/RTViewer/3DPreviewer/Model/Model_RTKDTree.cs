using System;
using System.Collections.Generic;
using System.Text;
using UWBGL_XNA_Lib;
using Microsoft.Xna.Framework;


using RayTracer_552;

namespace RTViewer
{
    public partial class RTModelViewer
    {
        private void defineBox(UWB_PrimitiveList list, Vector3 min, Vector3 max)
        {
            UWB_PrimitiveLine l = new UWB_PrimitiveLine();
            l.setStartPoint(min.X, min.Y, min.Z);
            l.setEndPoint(max.X, min.Y, min.Z);
            list.append(l);
            l = new UWB_PrimitiveLine();
            l.setStartPoint(min.X, min.Y, min.Z);
            l.setEndPoint(min.X, max.Y, min.Z);
            list.append(l);
            l = new UWB_PrimitiveLine();
            l.setStartPoint(min.X, min.Y, min.Z);
            l.setEndPoint(min.X, min.Y, max.Z);
            list.append(l);

            l = new UWB_PrimitiveLine();
            l.setStartPoint(max.X, max.Y, max.Z);
            l.setEndPoint(min.X, max.Y, max.Z);
            list.append(l);
            l = new UWB_PrimitiveLine();
            l.setStartPoint(max.X, max.Y, max.Z);
            l.setEndPoint(max.X, min.Y, max.Z);
            list.append(l);
            l = new UWB_PrimitiveLine();
            l.setStartPoint(max.X, max.Y, max.Z);
            l.setEndPoint(max.X, max.Y, min.Z);
            list.append(l);

            l = new UWB_PrimitiveLine();
            l.setStartPoint(min.X, max.Y, min.Z);
            l.setEndPoint(max.X, max.Y, min.Z);
            list.append(l);
            l = new UWB_PrimitiveLine();
            l.setStartPoint(min.X, max.Y, min.Z);
            l.setEndPoint(min.X, max.Y, max.Z);
            list.append(l);
            
            l = new UWB_PrimitiveLine();
            l.setStartPoint(min.X, min.Y, max.Z);
            l.setEndPoint(max.X, min.Y, max.Z);
            list.append(l);
            l = new UWB_PrimitiveLine();
            l.setStartPoint(min.X, min.Y, max.Z);
            l.setEndPoint(min.X, max.Y, max.Z);
            list.append(l);
            
            l = new UWB_PrimitiveLine();
            l.setStartPoint(max.X, min.Y, min.Z);
            l.setEndPoint(max.X, max.Y, min.Z);
            list.append(l);
        }

        internal void AddBox(RayTracer_552.KdTreeNode node, UWB_PrimitiveList list)
        {
            defineBox(list, node.Bounds.Min, node.Bounds.Max);
            if (!node.IsLeafNode()) {
                AddBox(node.LeftChild, list);
                AddBox(node.RightChild, list);
            }
        }

        internal void AddRTKdTree(RayTracer_552.KdTreeNode rootNode)
        {
            UWB_SceneNode n = new UWB_SceneNode();
            m_SceneDatabase.insertChildNode(n);
            UWB_PrimitiveList list = new UWB_PrimitiveList();
            n.setPrimitive(list);
            AddBox(rootNode, list);
        }
        
	}
}
