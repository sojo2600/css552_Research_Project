using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RTViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        static RTModelViewer mModel = null;
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RTViewer());
        }

        public static RTModelViewer GetModel()
        {
            if (null == mModel)
                mModel = new RTModelViewer();

            return mModel;
        }
    }
}
