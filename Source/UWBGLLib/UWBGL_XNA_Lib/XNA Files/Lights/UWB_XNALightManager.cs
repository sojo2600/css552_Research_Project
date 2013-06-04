using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    
    public class UWB_XNALightManager
    {
        public static int MAX_LIGHTS = 8;

        private int numLights;
        private Effect m_LightingEffect;

        public UWB_XNALightManager(Effect lightingEffect)
        {
            m_LightingEffect = lightingEffect;
        }

        public void ResetAllLights()
        {
            numLights = 0;
        }

        public UWB_XNAPointLight CreatePointLight()
        {
            if (numLights < MAX_LIGHTS)
            {
                UWB_XNAPointLight tempLight = new UWB_XNAPointLight(m_LightingEffect.Parameters["lights"].Elements[numLights]);
                IncrementLightCount();
                return tempLight;
            }
            else
            {
                UWBGL_XNA_Lib.UWB_Utility.echoToStatusArea("Failed to create light. Max lights reached.");
                return null;
            }
        }

        public UWB_XNADirectionalLight CreateDirectionalLight()
        {
            if (numLights < MAX_LIGHTS)
            {
                UWB_XNADirectionalLight tempLight = new UWB_XNADirectionalLight(m_LightingEffect.Parameters["lights"].Elements[numLights]);
                IncrementLightCount();
                return tempLight;
            }
            else
            {
                UWBGL_XNA_Lib.UWB_Utility.echoToStatusArea("Failed to create light. Max lights reached.");
                return null;
            }
        }

        public UWB_XNASpotLight CreateSpotLight()
        {
            if (numLights < MAX_LIGHTS)
            {
                UWB_XNASpotLight tempLight = new UWB_XNASpotLight(m_LightingEffect.Parameters["lights"].Elements[numLights]);
                IncrementLightCount();
                return tempLight;
            }
            else
            {
                UWBGL_XNA_Lib.UWB_Utility.echoToStatusArea("Failed to create light. Max lights reached.");
                return null;
            }
        }

        private void IncrementLightCount()
        {
            numLights++;
            m_LightingEffect.Parameters["numLights"].SetValue(numLights);
        }

        public int NumLights
        {
            get{ return numLights; }
        }

    }
}