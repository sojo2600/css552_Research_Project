using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_PrimitiveTriangle : UWB_Primitive
    {
        protected Vector3 mV1, mV2, mV3;
        protected Vector3 m_mouse_down_point;

        public UWB_PrimitiveTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            mV1 = v1;
            mV2 = v2;
            mV3 = v3;
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
            drawHelper.drawTriangle(mV1, mV2, mV3);
        }
    }
}
