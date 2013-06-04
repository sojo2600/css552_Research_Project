using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_XNAEffect : Effect
    {
        private Texture2D m_Texture;
        private bool m_TextureEnabled;

        public UWB_XNAEffect(Effect effect) : base(effect)
        {
            CurrentTechnique = Techniques["MultipleLights"];
        }

        public Matrix World
        {
            set
            {
                Parameters["world"].SetValue(value);
            }
        }

        public Matrix View
        {
            set
            {
                Parameters["view"].SetValue(value);
            }
        }

        public Matrix Projection
        {
            set
            {
                Parameters["projection"].SetValue(value);
            }
        }

        public UWB_Material Material
        {
            set
            {
                Parameters["material"].StructureMembers["ambient"].SetValue(value.Ambient);
                Parameters["material"].StructureMembers["diffuse"].SetValue(value.Diffuse);
                Parameters["material"].StructureMembers["emissive"].SetValue(value.Emissive);
                Parameters["material"].StructureMembers["specular"].SetValue(value.Specular);
                Parameters["material"].StructureMembers["shininess"].SetValue(value.Power);
            }

        }

        //
        // Summary:
        //     Gets or sets a texture to be applied by this effect.
        //
        // Returns:
        //     Texture to be applied by this effect.
        public Texture2D Texture 
        { 
            get
            {
                return m_Texture; 
            }
            set
            {
                m_Texture = value;
                Parameters["textureMap"].SetValue(m_Texture);   
            } 
        }
                        
                
        //
        // Summary:
        //     Enables textures for this effect.
        //
        // Returns:
        //     true to enable textures; false otherwise.
        public bool TextureEnabled 
        { 
            get
            {
                return m_TextureEnabled;   
            } 
            set
            {
                m_TextureEnabled = value;
                Parameters["textureEnable"].SetValue(m_TextureEnabled);   
            } 
        }
    }
}
