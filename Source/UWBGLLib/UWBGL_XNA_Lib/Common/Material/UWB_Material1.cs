using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace UWBGL_XNA_Lib
{
    public class UWB_Material
    {
        private Vector4 m_Ambient;
        private Vector4 m_Diffuse;
        private Vector4 m_Emissive;
        private Vector4 m_Specular;
        private float m_Power;

        public UWB_Material()
        {
            //
            //
            // nice shade of Husky purple ...
            //
            m_Ambient = new Vector4(0.13f, 0.065f, 0.195f, 1.0f);
            m_Diffuse = new Vector4(0.8f, 0.8f, 0.8f, 0.8f);
            m_Emissive = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);
            m_Specular = new Vector4(0.2f, 0.2f, 0.2f, 0.2f);
            m_Power = 1.0f;
 
        }

        public UWB_Material(Vector4 ambient, Vector4 diffuse, Vector4 emissive, Vector4 specular, float power)
        {
            m_Ambient = ambient;
            m_Diffuse = diffuse;
            m_Emissive = emissive;
            m_Specular = specular;
            m_Power = power;
        }

        public Vector4 Ambient
        {
            get { return m_Ambient; }
            set { m_Ambient = value; }
        }

        public Vector4 Diffuse
        {
            get { return m_Diffuse; }
            set { m_Diffuse = value; }
        }

        public Vector4 Emissive
        {
            get { return m_Emissive; }
            set { m_Emissive = value; }
        }

        public Vector4 Specular
        {
            get { return m_Specular; }
            set { m_Specular = value; }
        }

        public float Power
        {
            get { return m_Power; }
            set { m_Power = value; }
        }
    }
}