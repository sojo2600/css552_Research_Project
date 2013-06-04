using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
    public class UWB_XNADirectionalLight : UWB_XNALight
    {
        public UWB_XNADirectionalLight(EffectParameter lightParameter) : base(lightParameter)
        {
            Type = LightType.Directional;
        }
    }
}