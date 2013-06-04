using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_XNAPrimitiveMesh : UWB_Primitive
    {
        protected string filename;
        public UWB_BoundingBox m_bounds;
        Model mesh;
        //protected Model mesh;

        public UWB_XNAPrimitiveMesh(string fn)
        {
            filename = fn;
            m_bounds = new UWB_BoundingBox();
        }

        public override UWB_BoundingVolume getBoundingVolume(eLevelofDetail lod)
        {

            UWB_XNAGraphicsDevice graphics = UWB_XNAGraphicsDevice.m_TheAPI;
            try
            {
                mesh = graphics.mResources.Load<Model>(filename);
                return m_bounds;
            }
            catch (Exception e)
            {
                UWBGL_XNA_Lib.UWB_Utility.echoToStatusArea((e.Message));
                return null;
            }
        }
        public void setMesh(Model m)
        {
            mesh = m;
        }
        protected override void DrawPrimitive(eLevelofDetail lod, UWB_DrawHelper drawHelper)
        {
            UWB_XNAGraphicsDevice graphics = UWB_XNAGraphicsDevice.m_TheAPI;
            try
            {
                mesh = graphics.mResources.Load<Model>(filename);
                DrawMesh(true, this.mFlatColor, drawHelper);
            }
            catch (Exception e)
            {
                UWBGL_XNA_Lib.UWB_Utility.echoToStatusArea((e.Message));
            }
        }
        public virtual void DrawMesh(bool bShowflatColor, Color MaterialColor, UWB_DrawHelper drawHelper)
        {
            UWB_XNAGraphicsDevice graphics = UWB_XNAGraphicsDevice.m_TheAPI;
            GraphicsDevice device = UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice;

            UWB_XNAEffect effect = UWB_XNAGraphicsDevice.m_TheAPI.LightingEffect;

            SetupDrawAttributes(drawHelper);

            UWB_Material material = this.m_Material;

            //if (bShowflatColor && !m_bTexturingEnabled)
            //{
            //    material = new UWB_Material(Color.Gray.ToVector4(),
            //        Color.Gray.ToVector4(),
            //        Color.Gray.ToVector4(),
            //        Color.White.ToVector4(), 1f);

            //    material.Emissive = MaterialColor.ToVector4();
            //}

            /* If texturing is enabled, there are two option... one is that
             there has been a texture set for the mesh, and another is that
             the mesh has a texture defined inside the mesh file. In the 
             second case, this only checks the FIRST mesh part and uses the
             FIRST texture. */

            Texture2D texture = null;
            if (this.m_bTexturingEnabled)
            {
                texture = UWB_XNAGraphicsDevice.m_TheAPI.RetrieveTexture(m_TexFileName);

                BasicEffect meshEffect = (mesh.Meshes[0].MeshParts[0].Effect as BasicEffect);
                Texture2D partTexture = meshEffect.Texture;

                // If there is no supplied texture then use the texture and material of the first mesh part
                if (texture == null && partTexture != null)
                {
                    texture = partTexture;
                    material.Ambient = new Vector4(meshEffect.AmbientLightColor, meshEffect.Alpha);
                    material.Diffuse = new Vector4(meshEffect.DiffuseColor, meshEffect.Alpha);
                    material.Emissive = new Vector4(meshEffect.EmissiveColor, meshEffect.Alpha);
                    material.Specular = new Vector4(meshEffect.SpecularColor, meshEffect.Alpha);
                    material.Power = meshEffect.SpecularPower;
                }
            }

            effect.Material = material;

            if (texture != null)
            {
                effect.Texture = texture;
                effect.TextureEnabled = true;
            }
            else
            {
                effect.Texture = texture;
                effect.TextureEnabled = false;
            }

           
            for (int m = 0; m < mesh.Meshes.Count; m++)
            {
                for (int part = 0; part < mesh.Meshes[m].MeshParts.Count; part++)
                {
                    ModelMeshPart mp = mesh.Meshes[m].MeshParts[part];

                    device.Indices = mp.IndexBuffer;
                    device.SetVertexBuffer(mp.VertexBuffer);

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        
                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                            mp.VertexOffset, 0,
                            mp.NumVertices, mp.StartIndex, mp.PrimitiveCount);
                    }
                }
            }
        }

        protected override void SetupDrawAttributes(UWB_DrawHelper drawHelper)
        {
            base.SetupDrawAttributes(drawHelper);
            drawHelper.setColor2(mShadingColor);
            drawHelper.setShadeMode(mShadeMode);
            drawHelper.setFillMode(mFillMode);
        }

    }
}
