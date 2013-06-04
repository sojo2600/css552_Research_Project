using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_XNAPrimitiveMeshArrow : UWB_Primitive
    {
        protected float m_length;
        public UWB_XNAPrimitiveMeshArrow()
        {
        }
        
        protected override void DrawPrimitive(eLevelofDetail lod, UWB_DrawHelper draw_helper)
        {
            UWB_XNAGraphicsDevice graphics = UWB_XNAGraphicsDevice.m_TheAPI;
            UWB_XNAPrimitiveMesh pCylinderMesh = new UWB_XNAPrimitiveMesh("cylinder");
            UWB_XNAPrimitiveMesh pConeMesh = new UWB_XNAPrimitiveMesh("cone");

            if (pCylinderMesh != null && pConeMesh != null)
            {
                pCylinderMesh.Material = Material;
                pConeMesh.Material = Material;
                draw_helper.pushModelTransform();
                {
                    // Need to add support for finding the bounding box of a mesh
                    // the Z value is hard coded until then
                    //UWB_BoundingBox box = (pCylinderMesh.getBoundingVolume(draw_helper.getLod()) as UWB_BoundingBox);
                    UWB_XFormInfo cylinder_xform = new UWB_XFormInfo();
                    cylinder_xform.SetScale(new Vector3(0.025f, 0.025f, 0.2666666666f));
                    cylinder_xform.UpdateRotationYByDegree(-90.0f);
                    cylinder_xform.SetTranslation(new Vector3(0.4f, 0.0f, 0.0f));
                    cylinder_xform.setupModelStack(draw_helper);
                    try
                    {
                        pCylinderMesh.setMesh(graphics.mResources.Load<Model>("cylinder"));
                        pCylinderMesh.DrawMesh(true, mFlatColor, draw_helper);
                    }
                    catch (Exception e) { UWBGL_XNA_Lib.UWB_Utility.echoToStatusArea((e.Message)); }
                    //pCylinderMesh.DrawMesh(false, mFlatColor,draw_helper);
                }
                draw_helper.popModelTransform();

                draw_helper.pushModelTransform();
                {
                    //const UWB_BoundingBox box = pConeMesh.getBoundingBox();
                    UWB_XFormInfo cone_xform = new UWB_XFormInfo();
                    cone_xform.SetScale(new Vector3(0.08f,0.08f, 0.1f));
                    cone_xform.UpdateRotationYByDegree(-90.0f);
                    cone_xform.SetTranslation(new Vector3(0.85f,0.0f,0.0f));
                    cone_xform.setupModelStack(draw_helper);
                    try
                    {
                        pConeMesh.setMesh(graphics.mResources.Load<Model>("cone"));
                        pConeMesh.DrawMesh(true, mFlatColor, draw_helper);
                    }
                    catch (Exception e) { UWBGL_XNA_Lib.UWB_Utility.echoToStatusArea((e.Message)); }
                    //pConeMesh.DrawMesh(false, mFlatColor, draw_helper);
                }
                draw_helper.popModelTransform();
                
            }            
        }        
    }
}
