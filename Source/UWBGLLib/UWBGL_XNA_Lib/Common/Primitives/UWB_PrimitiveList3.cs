using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
	public class UWB_PrimitiveList : UWB_Primitive
	{
		UWB_BoundingBox mBounds;
		UWB_Array<UWB_Primitive> mList;

		public UWB_PrimitiveList()
		{
			mBounds = new UWB_BoundingBox();
			mList = new UWB_Array<UWB_Primitive>();
		}

		protected override void SetupDrawAttributes(UWB_DrawHelper drawHelper)
		{
			//nothing to do here
		}

		protected override void DrawPrimitive(eLevelofDetail lod, UWB_DrawHelper drawHelper)
		{
			int count = mList.count();
			for (int i = 0; i < count; i++)
			{
				UWB_Primitive primitive = mList.getItem(i);
				if (primitive != null)
				{
					primitive.Draw(lod, drawHelper);
				}
			}
		}

		public override void Update(float elapsedSeconds)
		{
			int count = mList.count();
			for (int i = 0; i < count; i++)
			{
				UWB_Primitive primitive = mList.getItem(i);
				if (primitive != null)
				{
					primitive.Update(elapsedSeconds);
				}
			}
		}

		public override UWB_BoundingVolume getBoundingVolume(eLevelofDetail lod)
		{
			int count = mList.count();

			if (count == 0)
				return null;
			mBounds.makeInvalid();

			// NOTE: This is pretty inefficient. We could speed this up by maintaining
			// some sort of "changed" flag and only updating the bounds when this flag changes			
			for (int i = 0; i < count; i++)
			{
				UWB_Primitive child = mList.getItem(i);
				mBounds.add(child.getBoundingVolume(lod));
			}

			return mBounds;
		}

		public int count()
		{
			return mList.count();
		}

		public UWB_Primitive primitiveAt(int index)
		{
			if (index < 0 || index >= mList.count())
			{
				return null;
			}

			return mList.getItem(index);
		}

		public void append(UWB_Primitive primitive)
		{
			mList.append(primitive);
		}

		public void deletePrimitiveAt(int index)
		{
			mList.deleteItem(index);
		}

		public void drawChildBoundingVolumes(eLevelofDetail lod, UWB_DrawHelper drawHelper, Color color)
		{
			if (drawHelper == null)
				return;

			int count = this.count();
			for (int i = 0; i < count; i++)
			{
				UWB_Primitive primitive = mList.getItem(i);
				if (primitive != null)
				{
					primitive.drawBoundingVolume(lod, drawHelper, color);
				}
			}
		}
	}
}
