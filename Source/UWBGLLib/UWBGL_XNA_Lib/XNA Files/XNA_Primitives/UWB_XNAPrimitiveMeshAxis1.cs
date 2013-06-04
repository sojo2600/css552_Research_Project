using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_XNAPrimitiveMeshAxis : UWB_Primitive
    {
        protected UWB_XNAPrimitiveMeshArrow m_xaxis;
        protected UWB_XNAPrimitiveMeshArrow m_yaxis;
        protected UWB_XNAPrimitiveMeshArrow m_zaxis;

        public UWB_XNAPrimitiveMeshAxis()
        {
            m_xaxis = new UWB_XNAPrimitiveMeshArrow();
            m_yaxis = new UWB_XNAPrimitiveMeshArrow();
            m_zaxis = new UWB_XNAPrimitiveMeshArrow();
            m_xaxis.setFlatColor(new Color(255, 0, 0));
            SetMaterial(1f, 0f, 0f, m_xaxis);
            m_yaxis.setFlatColor(new Color(0, 255, 0));
            SetMaterial(0f, 1f, 0f, m_yaxis);
            m_zaxis.setFlatColor(new Color(0, 0, 255));
            SetMaterial(0f, 0f, 1f, m_zaxis);
        }

        private void SetMaterial(float r, float g, float b, UWB_Primitive p)
        {
            p.Material.Ambient = Vector4.Zero;
            p.Material.Diffuse = Vector4.Zero;
            p.Material.Specular = Vector4.Zero;
            p.Material.Emissive = new Vector4(r, g, b, 1.0f);
        }

        protected override void DrawPrimitive(eLevelofDetail lod, UWB_DrawHelper draw_helper)
        {
            draw_helper.pushModelTransform();
            {
                m_xaxis.Draw(lod, draw_helper);
            }
            draw_helper.popModelTransform();

            UWB_XFormInfo yrot = new UWB_XFormInfo();
            yrot.UpdateRotationZByDegree(-90.0f);
            draw_helper.pushModelTransform();
            {
                yrot.setupModelStack(draw_helper);
                m_yaxis.Draw(lod, draw_helper);
            }
            draw_helper.popModelTransform();

            UWB_XFormInfo zrot = new UWB_XFormInfo();
            zrot.UpdateRotationYByDegree(90.0f);
            draw_helper.pushModelTransform();
            {
                zrot.setupModelStack(draw_helper);
                m_zaxis.Draw(lod, draw_helper);
            }
            draw_helper.popModelTransform();
        }
        

    }

}
