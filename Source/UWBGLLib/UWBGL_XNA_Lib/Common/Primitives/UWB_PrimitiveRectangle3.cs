using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
	public class UWB_PrimitiveRectangle : UWB_Primitive
	{
		protected float m_width;
        protected float m_height;
        protected UWB_BoundingBox m_bounds;
        protected Vector3 m_mouse_down_point;

		public UWB_PrimitiveRectangle()
		{
			m_width = m_height = 0;
			m_bounds = new UWB_BoundingBox();
		}

		protected override void SetupDrawAttributes(UWB_DrawHelper drawHelper)
		{
			base.SetupDrawAttributes(drawHelper);
			drawHelper.setColor1(mFlatColor);
			drawHelper.setColor2(mShadingColor);
			drawHelper.setShadeMode(mShadeMode);
			drawHelper.setFillMode(mFillMode);
		}

		protected override void DrawPrimitive(eLevelofDetail lod, UWB_DrawHelper drawHelper)
		{
			drawHelper.drawRectangle(m_bounds.getMin(), m_bounds.getMax());
		}

		public override void MouseDownVertex(int vertexID, float x, float y)
		{
			if (vertexID == 0)
			{
				m_mouse_down_point = new Vector3(x, y, 0);
				m_bounds.setCorners(m_mouse_down_point, m_mouse_down_point);
			}
			else
			{
				m_bounds.setCorners(m_mouse_down_point, new Vector3(x, y, 0));
			}
		}

		public override void MoveTo(float x, float y)
		{
			Vector3 old_center = m_bounds.getCenter();
			float dx = x - old_center.X;
			float dy = y - old_center.Y;

			Vector3 min_point = m_bounds.getMin();
			Vector3 max_point = m_bounds.getMax();

			min_point.X += dx;
			min_point.Y += dy;
			max_point.X += dx;
			max_point.Y += dy;
			m_bounds.setCorners(min_point, max_point);
		}

		public override Vector3 getLocation()
		{
			return m_bounds.getCenter();
		}

		public void setCorners(Vector3 corner1, Vector3 corner2)
		{
			m_bounds.setCorners(corner1, corner2);
		}

		public override void Update(float elapsed_seconds)
		{
			Vector3 adjust_vec = mVelocity * elapsed_seconds;
			Vector3 min_point = m_bounds.getMin();
			Vector3 max_point = m_bounds.getMax();

			min_point += adjust_vec;
			max_point += adjust_vec;
			m_bounds.setCorners(min_point, max_point);
		}

		public override UWB_BoundingVolume getBoundingVolume(eLevelofDetail lod)
		{
			return m_bounds;
		}


	}
}
