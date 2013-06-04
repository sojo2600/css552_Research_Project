using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_PrimitiveCircle: UWB_Primitive
    {
		protected Vector3 mCenter;
		protected float mRadius;
		protected UWB_BoundingBox mBounds;

		public UWB_PrimitiveCircle()
		{
			mCenter = Vector3.Zero;
			mRadius = 0;
			mBounds = new UWB_BoundingBox();
			mBounds.setCorners(
				new Vector3(mCenter.X - mRadius, mCenter.Y - mRadius, mCenter.Z),
                new Vector3(mCenter.X + mRadius, mCenter.Y + mRadius, mCenter.Z));
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
			eLevelofDetail oldLod = drawHelper.getLod();
			drawHelper.setLod(lod);
			drawHelper.drawCircle(mCenter, mRadius);
			drawHelper.setLod(oldLod);
		}

		public override void MouseDownVertex(int vertexID, float x, float y)
		{
			if (vertexID == 0)
			{
				setCenter(x, y, 0f);
				setRadius(0);
			}
			else
			{
				float dx = x - mCenter.X;
				float dy = y - mCenter.Y;
				float radius = (float)Math.Sqrt(dx * dx + dy * dy);
				setRadius(radius);
			}
		}

		public void setCenter(Vector3 center)
		{
			mCenter = center;
			mBounds.moveToCenter(center);
		}

		public void setCenter(float x, float y, float z)
		{
			mCenter.X = x;
			mCenter.Y = y;
			mCenter.Z = z;
			mBounds.moveToCenter(mCenter);
		}

		public override Vector3 getLocation()
		{
			return mCenter;
		}

		public void setRadius(float radius)
		{
			mRadius = radius;
			mBounds.setCorners(
				new Vector3(mCenter.X - mRadius, mCenter.Y - mRadius, mCenter.Z),
				new Vector3(mCenter.X + mRadius, mCenter.Y + mRadius, mCenter.Z));
		}

		public float getRadius()
		{
			return mRadius;
		}

		public override void Update(float elapsedSeconds)
		{
			Vector3 adjVelocity = mVelocity * elapsedSeconds;
			mCenter += adjVelocity;
			mBounds.moveToCenter(mCenter);
		}

		public override UWB_BoundingVolume getBoundingVolume(eLevelofDetail lod)
		{
			return mBounds;
		}
    }
}
