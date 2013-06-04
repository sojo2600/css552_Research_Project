using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_XNALight
    {
        private EffectParameter instanceParameter;
        private LightType m_Type;
        private Vector4 m_Ambient;
        private Vector4 m_Diffuse;
        private Vector4 m_Specular;

        private Vector3 m_Direction;
        private Vector3 m_Position;
        private Vector4 m_Color;
        private Vector3 m_Attenuation;

        private float m_Range;
        private float m_Falloff;
        private float m_Theta; // Inner Cone
        private float m_Phi; // Outer Cone

        private bool m_bLightIsOn;
        private bool m_bDrawLight;

        // Must match up with those defined in the UWB_LightingEffect.fx
        public enum LightType
        {
            Point = 0,
            Directional = 1,
            Spot = 2
        };

        public UWB_XNALight(EffectParameter lightParameter)
        {
            instanceParameter = lightParameter;

            Type = LightType.Directional;
            Ambient = Vector4.Zero;
            Diffuse = Vector4.Zero;
            Specular = Vector4.Zero;
            Direction = new Vector3(0.0f, -1.0f, 0.0f);
            Position = new Vector3(0.0f, 0.0f, 0.0f);
            Color = Vector4.Zero;
            Attenuation = new Vector3(1.0f, 0.0f, 0.0f);
            Theta = MathHelper.ToRadians(45.0f); 		// Spot light inner cone radius
            Phi = MathHelper.ToRadians(60.0f);         // Spot light outter cone radius
            Range = 200.0f;
            Falloff = 1.0f;
        }

        public LightType Type
        {
            get { return m_Type; }
            set 
            { 
                m_Type = value;
                instanceParameter.StructureMembers["type"].SetValue((int)m_Type);
            }
        }

        public Vector4 Ambient
        {
            get { return m_Ambient; }
            set 
            {
                m_Ambient = value;
                instanceParameter.StructureMembers["ambient"].SetValue(m_Ambient);
            }
        }

        public Vector4 Diffuse
        {
            get { return m_Diffuse; }
            set 
            { 
                m_Diffuse = value;
                instanceParameter.StructureMembers["diffuse"].SetValue(m_Diffuse);
            }
        }

        public Vector4 Specular
        {
            get { return m_Specular; }
            set 
            { 
                m_Specular = value;
                instanceParameter.StructureMembers["specular"].SetValue(m_Specular);
            }
        }

        public Vector3 Direction
        {
            get { return m_Direction; }
            set 
            { 
                m_Direction = value;
                instanceParameter.StructureMembers["direction"].SetValue(m_Direction);
            }
        }

        public Vector3 Position
        {
            get { return m_Position; }
            set 
            { 
                m_Position = value;
                instanceParameter.StructureMembers["position"].SetValue(m_Position);
            }
        }

        public Vector4 Color
        {
            get { return m_Color; }
            set { m_Color = value; }
        }

        public Vector3 Attenuation
        {
            get { return m_Attenuation; }
            set 
            { 
                m_Attenuation = value;
                instanceParameter.StructureMembers["attenuation"].SetValue(m_Attenuation);
            }
        }

        public float Theta
        {
            get { return m_Theta; }
            set 
            { 
                m_Theta = value;
                instanceParameter.StructureMembers["theta"].SetValue(m_Theta);
            }
        }

        public float Phi
        {
            get { return m_Phi; }
            set 
            { 
                m_Phi = value;
                instanceParameter.StructureMembers["phi"].SetValue(m_Phi);
            }
        }

        public float Range
        {
            get { return m_Range; }
            set
            {
                m_Range = value;
                instanceParameter.StructureMembers["range"].SetValue(m_Range);
            }
        }

        public float Falloff
        {
            get { return m_Falloff; }
            set
            {
                m_Falloff = value;
                instanceParameter.StructureMembers["falloff"].SetValue(m_Falloff);
            }
        }

        public void SwitchOnLight()
        {
            m_bLightIsOn = true;
            instanceParameter.StructureMembers["on"].SetValue(m_bLightIsOn);
        }

        public void SwitchOffLight()
        {
            m_bLightIsOn = false;
            instanceParameter.StructureMembers["on"].SetValue(m_bLightIsOn);
        }

        public bool LightIsSwitchedOn() { return m_bLightIsOn; }

        public void EnableDrawLight() { m_bDrawLight = true; }
        public void DisableDrawLight() { m_bDrawLight = false; }
        public bool ShouldDrawLight() { return m_bDrawLight; }

        // Pure virtual function that must be override!!
        public virtual void DrawLightGeometry(UWB_DrawHelper draw_helper) { }

        void SetToSpotLight() { Type = LightType.Spot; }
        void SetToPointLight() { Type = LightType.Point; }
        void SetToDirectionalLight() { Type = LightType.Directional; }

    }
}