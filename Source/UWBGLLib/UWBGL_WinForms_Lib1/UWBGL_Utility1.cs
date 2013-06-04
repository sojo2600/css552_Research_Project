using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace UWBGL_WinForms_Lib1
{
    public static class Utility
    {
        public static void UWBGL_ReplaceFormControl(Control Replacement, Control PlaceHolder)
        {
            Replacement.Width = PlaceHolder.Width;
            Replacement.Height = PlaceHolder.Height;
            PlaceHolder.Controls.Add(Replacement);
        }
    }
}
