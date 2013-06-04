using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UWBGL_XNA_Lib
{
    public static class UWB_Utility
    {
		public const double zeroTolerance = 1.0e-9;

        public static TextBox mEchoTextBox = null;

        private static Random rand = new Random();
        public static void echoToStatusArea(string echoString)
        {
            if (mEchoTextBox != null)
                mEchoTextBox.Text = echoString;
        }

		public static float UWB_RandomNumber(float range)
		{
			return range * (float)rand.NextDouble();
		}

		public static float UWB_RandomNumber(float minRange, float maxRange)
		{
			float size = maxRange - minRange;
			return minRange + UWB_RandomNumber(size);
		}
    }
}
