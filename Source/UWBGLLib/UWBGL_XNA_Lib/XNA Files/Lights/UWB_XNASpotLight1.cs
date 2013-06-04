using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_XNASpotLight : UWB_XNALight
    {
        public UWB_XNASpotLight(EffectParameter lightParameter) : base(lightParameter) 
        {
            Type = LightType.Spot;
        }
    }
}