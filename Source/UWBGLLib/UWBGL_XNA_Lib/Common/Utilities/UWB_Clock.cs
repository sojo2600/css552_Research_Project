using System;
using System.Collections.Generic;
//using System.Data;
//using System.Text;

namespace UWBGL_XNA_Lib
{
    public class UWB_Clock
    {
        private DateTime m_previousUpdatetime;

        public UWB_Clock()
        {
            m_previousUpdatetime = DateTime.Now;
        }

        public float getSecondsElapsed()
        {
            DateTime currentUpdateTime = DateTime.Now;
            TimeSpan elapsedTime = currentUpdateTime - m_previousUpdatetime;
            m_previousUpdatetime = currentUpdateTime;
            return (float)elapsedTime.TotalSeconds;
        }
    }
}
