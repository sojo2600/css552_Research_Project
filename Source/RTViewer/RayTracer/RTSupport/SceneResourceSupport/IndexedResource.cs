using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer_552
{
    /// <summary>
    /// Base class for all resources of the RT scene database. All resoruces has an index.
    /// </summary>
    public class IndexedResource
    {
        private int mIndex = 0;

        public void SetResourceIndex(int i) { mIndex = i; }
        public int GetResourceIndex() { return mIndex; }
    }
}
