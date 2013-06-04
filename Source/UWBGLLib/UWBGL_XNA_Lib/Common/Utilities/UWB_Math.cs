using System;
using System.Collections.Generic;
using System.Text;

namespace UWBGL_XNA_Lib
{

    public static class UWB_Math
    {
        static float UWB_PI = 3.14159265358979323846f;
        static float UWB_VERY_LARGE = 999999999.0f;
        static float UWB_VERY_SMALL = -UWB_VERY_LARGE;
        static float UWB_ZERO_TOLERANCE = (float)1.0e-9;

        // Level of detail enum for supporting multiple detail levels on graphics objects
        enum eLevelOfDetail { lodLow, lodMedium, lodHigh };

        public static float UWB_ToRadians(float degree)
        {
            return ((degree) * (UWB_PI / 180.0f));
        }

        public static float UWB_ToDegrees(float radian )
        {
            return ((radian) * (180.0f / UWB_PI));
        }
    }
}
