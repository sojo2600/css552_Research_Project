using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_PrimitiveRectangleXZ : UWB_PrimitiveRectangle
    {
        public UWB_PrimitiveRectangleXZ() 
        {
            m_bounds.setCorners(Vector3.Zero, Vector3.Zero);
        }

        protected override void DrawPrimitive(eLevelofDetail lod, UWB_DrawHelper drawHelper)
        {
            drawHelper.drawRectangleXZ(m_bounds.getMin(), m_bounds.getMax());
        }

        public override void MouseDownVertex(int vertexID, float x, float y)
        {
            if (vertexID == 0)
            {
                m_mouse_down_point = new Vector3(x, 0, y);
                m_bounds.setCorners(m_mouse_down_point, m_mouse_down_point);
            }
            else {
                m_bounds.setCorners(m_mouse_down_point,new Vector3(x,0,y));
            }
        }

        public override void MoveTo(float x, float y)
        {
            Vector3 old_center = m_bounds.getCenter();
            float dx = x - old_center.X;
            float dz = y - old_center.Z;

            Vector3 min_point = m_bounds.getMin();
            Vector3 max_point = m_bounds.getMax();

            min_point.X += dx;
            min_point.Z += dz;
            max_point.X += dx;
            max_point.Z += dz;
            m_bounds.setCorners(min_point, max_point);
        }
    }
}
