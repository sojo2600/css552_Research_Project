using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_PrimitiveAxisFrame : UWB_Primitive
    {
        private float m_size;   // len of axis lines

        public UWB_PrimitiveAxisFrame()
        {
            m_size = 10;
        }

        protected override void SetupDrawAttributes(UWB_DrawHelper drawHelper)
        {
            base.SetupDrawAttributes(drawHelper);
            drawHelper.setShadeMode(eShadeMode.smFlat);
        }

        protected override void DrawPrimitive(eLevelofDetail lod, UWB_DrawHelper drawHelper)
        {
            Vector3 origin = new Vector3();
            Vector3 xaxis = new Vector3(m_size, 0, 0);
            Vector3 yaxis = new Vector3(0, m_size, 0);
            Vector3 zaxis = new Vector3(0, 0, m_size);
            eLevelofDetail oldLod = drawHelper.getLod();
            drawHelper.setLod(lod);
            drawHelper.setColor1(Color.Red);
            drawHelper.drawLine(origin, xaxis);
            drawHelper.setColor1(Color.Green);
            drawHelper.drawLine(origin, yaxis);
            drawHelper.setColor1(Color.Blue);
            drawHelper.drawLine(origin, zaxis);
            drawHelper.setLod(oldLod);
        }

        public void setSize(float s)
        {
            m_size = s;
        }

        public float getSize()
        {
            return m_size;
        }
    }
}
