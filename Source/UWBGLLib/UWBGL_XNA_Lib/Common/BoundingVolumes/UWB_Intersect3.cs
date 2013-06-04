using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace UWBGL_XNA_Lib
{
	public class UWB_BoundingVolume
	{
		protected Vector3 mMin;
		protected Vector3 mMax;

		public enum eVolumeType { box, sphere };

		public virtual eVolumeType getType() 
		{ 
			return eVolumeType.box; 
		}

		public virtual Vector3 getCenter() 
		{
			return Vector3.Zero;
		}

		public virtual void add(UWB_BoundingVolume other) { }

		public virtual bool containsPoint(Vector3 testPoint) 
		{
			return false;
		}

		public virtual bool intersects(UWB_BoundingVolume other) 
		{
			return false;
		}

		public virtual void Draw(ref UWB_DrawHelper drawHelper) { }
	}

	public class UWB_BoundingBox : UWB_BoundingVolume
	{
		public UWB_BoundingBox()
		{
			mMin = Vector3.Zero;
			mMax = Vector3.Zero;
		}

		public UWB_BoundingBox(Vector3 corner1, Vector3 corner2)
		{
			setCorners(corner1, corner2);
		}

		public UWB_BoundingBox(Vector3 center, float width, float height, float depth)
		{
			mMin.X = center.X - width / 2f;
			mMin.Y = center.Y - height / 2f;
			mMin.Z = center.Z - depth / 2f;
			mMax.X = center.X - width / 2f;
			mMax.Y = center.Y - height / 2f;
			mMax.Z = center.Z - depth / 2f;
		}

		public void setCorners(Vector3 corner1, Vector3 corner2)
		{
			mMin.X = corner1.X < corner2.X ? corner1.X : corner2.X;
			mMin.Y = corner1.Y < corner2.Y ? corner1.Y : corner2.Y;
			mMin.Z = corner1.Z < corner2.Z ? corner1.Z : corner2.Z;

			mMax.X = corner1.X > corner2.X ? corner1.X : corner2.X;
			mMax.Y = corner1.Y > corner2.Y ? corner1.Y : corner2.Y;
			mMax.Z = corner1.Z > corner2.Z ? corner1.Z : corner2.Z;
		}

		public void moveToCenter(Vector3 newCenter)
		{
			Vector3 oldCenter = getCenter();
			Vector3 delta = newCenter - oldCenter;

			mMin += delta;
			mMax += delta;
		}

		public override eVolumeType getType()
		{
			return eVolumeType.box; 
		}

		public override bool intersects(UWB_BoundingVolume other)
		{
			eVolumeType vt = other.getType();
			if (eVolumeType.box == vt)
			{
				UWB_BoundingBox otherBox = other as UWB_BoundingBox;
				return intesectBoxBox(mMin, mMax, otherBox.getMin(), otherBox.getMax());
			}

			return false;
		}

		public override Vector3 getCenter()
		{
			Vector3 c = (mMin + mMax) / 2f;
			return c;
		}

		public float width()
		{
			return mMax.X - mMin.X;
		}

		public float height()
		{
			return mMax.Y - mMin.Y;
		}

		public float depth()
		{
			return mMax.Z - mMin.Z;
		}
		
		public override bool containsPoint(Vector3 testPoint)
		{
			bool xInside = testPoint.X >=  mMin.X && testPoint.X <= mMax.X;
			if (!xInside)
				return false;

			bool yInside = testPoint.Y >= mMin.Y && testPoint.Y <= mMax.Y;
			if (!yInside)
				return false;

			bool zInside = testPoint.Z >= mMin.Z && testPoint.Z <= mMax.Z;
			if (!zInside)
				return false;

			return true;
		}

		public override void Draw(ref UWB_DrawHelper drawHelper)
		{
			if (drawHelper != null)
			{
				Vector3 start = mMin;
				Vector3 end = mMin;

				end.X = mMax.X;
				drawHelper.drawLine(start, end);
				start = end;
				end.Y = mMax.Y;
				drawHelper.drawLine(start, end);
				start = end;
				end.X = mMin.X;
				drawHelper.drawLine(start, end);
				start = end;
				end.Y = mMin.Y;
                drawHelper.drawLine(start, end);

                start = mMax;
                end = mMax;
                end.X = mMin.X;
                drawHelper.drawLine(start, end);
                start = end;
                end.Y = mMin.Y;
                drawHelper.drawLine(start, end);
                start = end;
                end.X = mMax.X;
                drawHelper.drawLine(start, end);
                start = end;
                end.Y = mMax.Y;
                drawHelper.drawLine(start, end);

                start = mMax;
                end = mMax;
                end.Z = mMin.Z;
                drawHelper.drawLine(start, end);
                end.Y = mMin.Y;
                start.Y = mMin.Y;
                drawHelper.drawLine(start, end);
                end.X = mMin.X;
                start.X = mMin.X;
                drawHelper.drawLine(start, end);
                end.Y = mMax.Y;
                start.Y = mMax.Y;
                drawHelper.drawLine(start, end);
			}
		}

		public void makeInvalid()
		{
            mMin = new Vector3(1, 1, 1);
			mMax = new Vector3(-1, -1, -1);
		}

		public bool isValid()
		{
			return (mMin.X <= mMax.X && mMin.Y <= mMax.Y && mMin.Z <= mMax.Z);
		}

		public override void add(UWB_BoundingVolume other)
		{
			if(other != null)
			{
				eVolumeType vt = other.getType();
				if(eVolumeType.box == vt)
				{
					UWB_BoundingBox box = other as UWB_BoundingBox;
					add(box);
				}
			}
		}

		public Vector3 getMin()
		{
			return mMin;
		}

		public Vector3 getMax()
		{
			return mMax;
		}

		public void add(UWB_BoundingBox box)
		{
			if (!box.isValid())
				return;

			if (!isValid())
				setCorners(box.getMax(), box.getMin());
			else
			{
				mMin.X = mMin.X < box.mMin.X ? mMin.X : box.mMin.X;
				mMin.Y = mMin.Y < box.mMin.Y ? mMin.Y : box.mMin.Y;
				mMin.Z = mMin.Z < box.mMin.Z ? mMin.Z : box.mMin.Z;
				mMax.X = mMax.X > box.mMax.X ? mMax.X : box.mMax.X;
				mMax.Y = mMax.Y > box.mMax.Y ? mMax.Y : box.mMax.Y;
				mMax.Z = mMax.Z > box.mMax.Z ? mMax.Z : box.mMax.Z;
			}
		}

		private bool intesectBoxBox(Vector3 box1Min, Vector3 box1Max, Vector3 box2Min, Vector3 box2Max)
		{
			bool xOutside = box1Min.X > box2Max.X || box1Max.X < box2Min.X;
			if (xOutside)													
				return false;

			bool yOutside = box1Min.Y > box2Max.Y || box1Max.Y < box2Min.Y;
			if (yOutside)													
				return false;

			bool zOutside = box1Min.Z > box2Max.Z || box1Max.Z < box2Min.Z;
			if (zOutside)
				return false;

			return true;
		}

        public Rectangle Rectangle
        {
            get
            {
                Rectangle rect = new Rectangle((int)Math.Floor(mMin.X), (int)Math.Floor(mMin.Y),
                  (int)Math.Ceiling(width()), (int)Math.Ceiling(height()));

                return rect;
            }
        }
	}
}
