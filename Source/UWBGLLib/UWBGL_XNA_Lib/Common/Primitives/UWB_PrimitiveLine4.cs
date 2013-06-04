using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_PrimitiveLine: UWB_Primitive
    {
		protected Vector3 mStart;
		protected Vector3 mEnd;
		protected UWB_BoundingBox mBounds;

		public UWB_PrimitiveLine()
		{
			mStart = Vector3.Zero;
			mEnd = Vector3.Zero;
			mBounds = new UWB_BoundingBox();
		}

		protected override void SetupDrawAttributes(UWB_DrawHelper drawHelper)
		{
			base.SetupDrawAttributes(drawHelper);
			drawHelper.setColor1(mFlatColor);
			drawHelper.setColor2(mShadingColor);
			drawHelper.setShadeMode(mShadeMode);
		}

		protected override void DrawPrimitive(eLevelofDetail lod, UWB_DrawHelper drawHelper)
		{
			drawHelper.drawLine(mStart, mEnd);
		}

		public override void MouseDownVertex(int vertexID, float x, float y)
		{
			if (vertexID == 0)
				setStartPoint(x, y, 0f);
			setEndPoint(x, y, 0f);
		}

		public override void MoveTo(float x, float y)
		{
			setEndPoint(x, y, 0f);
		}

		private Vector3 getMidPoint()
		{
			Vector3 m = (mStart + mEnd) / 2f;
			return m;
		}

		public override Vector3 getLocation()
		{
			return getMidPoint();
		}

		public void setStartPoint(float x, float y, float z)
		{
			mStart.X = x;
			mStart.Y = y;
			mStart.Z = z;
		}

		public Vector3 getStartPoint()
		{
			return mStart;
		}

		public void setEndPoint(float x, float y, float z)
		{
			mEnd.X = x;
			mEnd.Y = y;
			mEnd.Z = z;
		}

		public Vector3 getEndPoint()
		{
			return mEnd;
		}

		public override void Update(float elapsedSeconds)
		{
			Vector3 adjVelocity = mVelocity * elapsedSeconds;
			mStart += adjVelocity;
			mEnd += adjVelocity;
		}

		public override UWB_BoundingVolume getBoundingVolume(eLevelofDetail lod)
		{
			mBounds.setCorners(mStart, mEnd);
			return mBounds;
		}
    }
}
