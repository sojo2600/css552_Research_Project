using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

using System.Threading;

namespace RayTracer_552
{
    /// <summary>
    /// 
    /// PLEASE DO NOT CHANGE
    /// 
    /// Supports running the RayTracer as a separate thread.
    /// </summary>
    public partial class RTCore
    {
        //MultiThread compute boolean
        private bool mMultiThreadCompute = true;
        public void SetMultiThreadCompute(bool on) { mMultiThreadCompute = on; }

        //ComputeImage() now starts threads which run ComputeImagePart()
        private Thread[] mComputeThreads = null;
        private System.Diagnostics.Stopwatch mStopWatch = new System.Diagnostics.Stopwatch();
        private int mYStep = 1;

        private Object mWorkYLock = new object();
        private int mThreadWorkedY;  // max Y-scanline that has been assigned to working Threads.

        private void SetParm(int[] parm, int startX, int startY, int endX, int endY, int tid)
        {
            parm[0] = startX;
            parm[1] = startY;
            parm[2] = endX;
            parm[3] = endY;
            parm[4] = tid;
        }

        /// <summary>
        /// Parse the parameter passed in.
        /// </summary>
        /// <param name="parm">Parameter received.</param>
        /// <param name="startX">X pixel position to begin tracing ray</param>
        /// <param name="startY">Y pixel position to begin tracing ray</param>
        /// <param name="endX">X pixel position to end tracing ray</param>
        /// <param name="endY">Y pixel position to end tracing ray</param>
        /// <param name="tid">This thread's ID</param>
        private void ParseParm(Object parm, out int startX, out int startY, out int endX, out int endY, out int tid) 
        {
            int[] p = (int[])parm;
            startX = p[0];
            startY = p[1];
            endX = p[2];
            endY = p[3];
            tid = p[4];
        }

        /// <summary>
        ///  Beginning a new Ray Tracing thread (ParseCmdFile must have been called!)
        /// </summary>
        public void BeginRTThread()
        {
            mStopWatch.Reset();
            mStopWatch.Start();

            mThreadWorkedY = 0;

            if (!mMultiThreadCompute)
            {
                mRTWindows.ThreadSafeEchoToStatus("SingleThreaded initializing ...");

                mComputeThreads = new Thread[1];
                int[] parm = new int[5];
                SetParm(parm, 0, 0, ImageWidth, ImageHeight, -1);
                mComputeThreads[0] = new Thread(ComputeImage);
                mComputeThreads[0].SetApartmentState(ApartmentState.STA);
                mComputeThreads[0].Start(parm);
            }
            else
            {
                mComputeThreads = new Thread[Environment.ProcessorCount * 2];
                mRTWindows.ThreadSafeEchoToStatus("MultiThreaded starting " + mComputeThreads.Length + " threads");

                //!!!!number of rows to compute per thread
                mYStep = (ImageHeight / Environment.ProcessorCount) / 4;
                if (mYStep <= 0) mYStep = 1;//minumum one pixel row per thread..(obviously)
                
                //create and start initial threads
                for (int i = 0; i < mComputeThreads.Length; i++)
                {
                    int endY;
                    int startY;
                    lock (mWorkYLock)
                    {
                        startY = mThreadWorkedY;
                        mThreadWorkedY += mYStep;
                        endY = mThreadWorkedY;
                    }
                    int[] parm = new int[5];
                    mRTWindows.ThreadSafeEchoToStatus("MultiThreaded thread(" + i + ") getting new job scanlines(" + startY + "-" + (endY-1) + ")");
                    SetParm(parm, 0, startY, ImageWidth, endY, i);
                    mComputeThreads[i] = new Thread(ComputeImage);
                    mComputeThreads[i].Start(parm);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadNumber"></param>
        private void ThreadNextWorkLoad(int threadNumber)
        {
            if (mThreadWorkedY >= ImageHeight)
            {   //all needed threads have been created
                mComputeThreads[threadNumber] = null;  // we are done
                if (!RTThreadRunning())
                    RTRenderingDone();
            }
            else
            { // more work to be done
                int[] parm = new int[5];
                int startY, endY;
                lock (mWorkYLock)
                {
                    startY = mThreadWorkedY;
                    mThreadWorkedY += mYStep;
                    endY = mThreadWorkedY;
                }
                mRTWindows.ThreadSafeEchoToStatus("MultiThreaded thread(" + threadNumber + ") getting new job scanlines(" + startY + "-" + (endY-1) + ")");
                SetParm(parm, 0, startY, ImageWidth, endY, threadNumber);
                ComputeImage(parm);
            }
        }

        private void RTRenderingDone()
        {
            mStopWatch.Stop();
            mRTWindows.ThreadSafeEchoToStatus("Total Rendering Time(s): " + (((decimal) mStopWatch.ElapsedMilliseconds) / 1000).ToString());
            mThreadWorkedY = 0;
        }


        /// <summary>
        /// Current ray tracer thread is running
        /// </summary>
        /// <returns></returns>
        public bool RTThreadRunning()
        {
            if (null == mComputeThreads)
                return false;

            bool oneRunning = false;
            int count = 0;
            while ( (!oneRunning) && (count < mComputeThreads.Length) )
            {
                oneRunning = (mComputeThreads[count] != null) && (mComputeThreads[count].IsAlive);
                count++;
            }
            return oneRunning;
        }

        public void AbortRTThread()
        {
            if (null != mComputeThreads)
            {
                for (int i = 0; i < mComputeThreads.Length; i++)
                {
                    if (mComputeThreads[i] != null)
                    {
                        mComputeThreads[i].Abort();
                        while (mComputeThreads[i].IsAlive) ; // wait to make sure abort has happened
                        mComputeThreads[i] = null;
                    }
                }
                mComputeThreads = null;
            }
        }
    }
}