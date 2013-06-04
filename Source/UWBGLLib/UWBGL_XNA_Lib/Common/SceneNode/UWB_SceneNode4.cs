using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_SceneNode
    {
        protected string mName;
        protected bool mPivotVisible;
        protected UWB_XFormInfo mXFormInfo;
        protected UWB_Array<UWB_SceneNode> mChildNodes;
        protected UWB_Primitive mPrimitive;
        protected Vector3 mVelocity;

        public UWB_SceneNode(string name)
        {
            mName = name;
            Init();
        }

		public UWB_SceneNode()
		{
            mName = "SceneNode";
            Init();
		}

        private void Init()
        {
            mVelocity = new Vector3(0, 0, 0);
            mPrimitive = null;
            mPivotVisible = false;

            mXFormInfo = new UWB_XFormInfo();
            mChildNodes = new UWB_Array<UWB_SceneNode>();
        }
       
        public void Draw(eLevelofDetail lod, UWB_DrawHelper drawHelper)
        {
            drawHelper.pushModelTransform();
            mXFormInfo.setupModelStack(drawHelper);

            if (mPrimitive != null)
            {
                mPrimitive.Draw(lod, drawHelper);
            }

            int count = mChildNodes.count();
            for (int i = 0; i < count; i++)
            {
                mChildNodes.getItem(i).Draw(lod, drawHelper);
            }

            if (mPivotVisible)
            {
                mXFormInfo.drawPivot(drawHelper, .2f);
            }

            drawHelper.popModelTransform();
        }

        public void setPrimitive(UWB_Primitive primitive)
        {
            mPrimitive = primitive;
        }

        public UWB_Primitive getPrimitive()
        {
            return mPrimitive;
        }

        public void insertChildNode(UWB_SceneNode childNode)
        {
            mChildNodes.append(childNode);
        }

        public int numChildren()
        {
            return mChildNodes.count();
        }

        public UWB_SceneNode getChildNode(int index)
        {
            if (index < 0 || index >= numChildren())
            {
                return null;
            }

            return mChildNodes.getItem(index);
        }

        public void MoveNodeByVelocity(float elapsed_seconds)
        {
            Vector3 translate = mXFormInfo.GetTranslation();
            translate += (mVelocity * elapsed_seconds);
            mXFormInfo.SetTranslation(translate);
        }

        public Vector3 Velocity
        {
            get { return mVelocity; }
            set { mVelocity = value; }
        }

        public UWB_XFormInfo getXFormInfo()
        {
            return mXFormInfo;
        }

        public void setXFormInfo(UWB_XFormInfo xForm)
        {
            mXFormInfo = xForm;
        }

        public void setPivotVisible(bool visible)
        {
            mPivotVisible = visible;
        }

        public bool isPivotVisible()
        {
            return mPivotVisible;
        }

        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        public UWB_BoundingBox GetBounds(ref UWB_DrawHelper helper, bool bDraw)
        {
            UWB_BoundingBox box = new UWB_BoundingBox();
            box.makeInvalid();
            if (mPrimitive != null)
                box.add(mPrimitive.getBoundingVolume(eLevelofDetail.lodLow));

            helper.pushModelTransform();
            {
                mXFormInfo.setupModelStack(helper);
                //Draw the box for debugging
                if (bDraw)
                    box.Draw(ref helper);
                Vector3 minPt = box.getMin();
                Vector3 maxPt = box.getMax();
                Vector3 pt1 = new Vector3(minPt.X, minPt.Y, minPt.Z);
                Vector3 pt2 = new Vector3(maxPt.X, minPt.Y, minPt.Z);
                Vector3 pt3 = new Vector3(maxPt.X, maxPt.Y, minPt.Z);
                Vector3 pt4 = new Vector3(minPt.X, maxPt.Y, minPt.Z);
                Vector3 pt5 = new Vector3(minPt.X, minPt.Y, maxPt.Z);
                Vector3 pt6 = new Vector3(maxPt.X, minPt.Y, maxPt.Z);
                Vector3 pt7 = new Vector3(maxPt.X, maxPt.Y, maxPt.Z);
                Vector3 pt8 = new Vector3(minPt.X, maxPt.Y, maxPt.Z);
                helper.transformPoint(ref pt1);
                helper.transformPoint(ref pt2);
                helper.transformPoint(ref pt3);
                helper.transformPoint(ref pt4);
                helper.transformPoint(ref pt5);
                helper.transformPoint(ref pt6);
                helper.transformPoint(ref pt7);
                helper.transformPoint(ref pt8);

                box.makeInvalid();
                box.add(new UWB_BoundingBox(pt1, pt2));
                box.add(new UWB_BoundingBox(pt3, pt4));
                box.add(new UWB_BoundingBox(pt5, pt6));
                box.add(new UWB_BoundingBox(pt7, pt8));

                int count = mChildNodes.count();
                for (int i = 0; i < count; i++)
                    box.add(mChildNodes.getItem(i).GetBounds(ref helper, bDraw));
            }
            helper.popModelTransform();

            return box;
        }

        protected bool GetNodeBoundsHelper(ref UWB_SceneNode pSearchNode,
                                         ref UWB_BoundingBox box,
                                         ref UWB_DrawHelper helper, int level, bool bDraw) 
        {
          bool found = false;
          if( Object.ReferenceEquals(this, pSearchNode))
          {
            box = this.GetBounds( ref helper, bDraw );
            found = true;
          }
          else
          {
            helper.pushModelTransform();
            {
              level++;
              this.getXFormInfo().setupModelStack(helper);

              int count = this.numChildren();
              for(int i=0; i<count; i++)
              {
                UWB_SceneNode pChildNode = this.getChildNode( i );
                if( pChildNode.GetNodeBoundsHelper(  ref pSearchNode, ref box, ref helper, level, bDraw ) )
                {
                  found = true;
                  break;
                }
              }
              level--;
            }
            helper.popModelTransform();
          }
          if (0 == level && found && bDraw)
          {
              helper.resetAttributes();
              helper.setColor1(new Color(0,0,255));
              helper.setShadeMode(eShadeMode.smFlat);
              helper.setFillMode(eFillMode.fmWireframe);
              box.Draw(ref helper);
          }

          return found;
        }

        public bool GetNodeBounds( ref UWB_SceneNode pSearchNode,
                                           ref UWB_BoundingBox box,
                                           ref UWB_DrawHelper helper,
                                           bool bDraw)
        {
          return GetNodeBoundsHelper(ref pSearchNode, ref box, ref helper, 0, bDraw );
        }

    }
}
