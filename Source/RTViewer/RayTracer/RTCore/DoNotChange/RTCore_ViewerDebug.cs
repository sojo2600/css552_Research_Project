using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Windows.Forms;

namespace RayTracer_552
{
    /// <summary>
    /// 
    /// PLEASE DO NOT CHANGE!!
    /// 
    /// RTCore's support for Interactive Debug.
    /// </summary>
    public partial class RTCore
    {
        // Interactive display of computed pixel plane
        private bool mDisplayDebugPixels = false;
        private bool mDisplayDebugRays = false;
        private bool mShowPixelInWorld = false;
        private bool mDrawDB = true;
        private bool mAnaglyph = false;

        public void SetDebugPixels(bool on)
        {
            mDisplayDebugPixels = on;
        }

        public void SetDebugRays(bool on)
        {
            mDisplayDebugRays = on;
        }

        public void SetShowPixelInWorld(bool on)
        {
            mShowPixelInWorld = on;
        }

        public void SetDrawDB(bool on)
        {
            mDrawDB = on;
        }

        public void SetAnaglyph(bool on)
        {
            mAnaglyph = on;
        }

        public bool DisplayDebugPixels() { return mDisplayDebugPixels; }
        public bool DisplayDebugRays() { return mDisplayDebugRays; }
        public bool ShowPixelInWorld() { return mShowPixelInWorld; }
        public bool DrawDB() { return mDrawDB; }
        public bool DrawAnaglyph() { return mAnaglyph; }
    }
}
