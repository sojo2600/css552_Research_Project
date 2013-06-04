using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_PrimitivePoint: UWB_Primitive
    {
        protected Vector3 mPoint;
		protected UWB_BoundingBox mBounds;

		public UWB_PrimitivePoint()
        {
            mPoint = Vector3.Zero;
			mBounds = new UWB_BoundingBox();
            mBounds.setCorners(mPoint, mPoint);
        }

		public UWB_PrimitivePoint(float x, float y, float z)
		{
			mPoint = new Vector3(x, y, z);
            mBounds = new UWB_BoundingBox();
            mBounds.setCorners(mPoint, mPoint);
		}

		public override void Update(float elapsedSeconds)
		{
			Vector3 adjVelocity = mVelocity * elapsedSeconds;
			setLocation(mPoint + adjVelocity);
            mBounds.setCorners(mPoint, mPoint);
		}

		public override void MouseDownVertex(int vertexID, float x, float y)
		{
			setLocation(x, y, 0);
		}

		public override void MoveTo(float x, float y)
		{
			setLocation(x, y, 0);
		}

		protected override void SetupDrawAttributes(UWB_DrawHelper drawHelper)
		{
			base.SetupDrawAttributes(drawHelper);
			drawHelper.setColor1(mFlatColor);
			drawHelper.setPointSize(mPointSize);
		}

		protected override void DrawPrimitive(eLevelofDetail lod, UWB_DrawHelper drawHelper)
		{
			drawHelper.drawPoint(mPoint);
		}

        public void setLocation(float x, float y, float z)
        {
			mPoint.X = x;
			mPoint.Y = y;
			mPoint.Z = z;
			mBounds.setCorners(mPoint, mPoint);
        }

		public void setLocation(Vector3 location)
		{
			mPoint.X = location.X;
			mPoint.Y = location.Y;
			mPoint.Z = location.Z;
			mBounds.setCorners(mPoint, mPoint);
		}

        public override Vector3 getLocation()
        {
            return mPoint;
        }

		public override UWB_BoundingVolume getBoundingVolume(eLevelofDetail lod)
		{
			return mBounds;
		}
    }
}
