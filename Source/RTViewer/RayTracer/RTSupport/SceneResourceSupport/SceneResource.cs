using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer_552
{
    /// <summary>
    /// Template class for storing resources in the scene database.
    /// Notice, this is an un-ordered collection of resources with index.
    /// e.g., when we want to find Light, index 2, we DO NOT look for it under
    ///     mCollection[2]
    /// instead, we loop through the mCollection and look at the index of each resource.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SceneResource<T> 
    {
        private List<IndexedResource> mCollection;

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneResource() {
            mCollection = new List<IndexedResource>();
        }
            
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="r"></param>
        public void AddResource(IndexedResource r)  { mCollection.Add(r);  }

        /// <summary>
        /// Number of resources in the collection
        /// </summary>
        public int Count { get { return mCollection.Count; } }

        /// <summary>
        /// Find/return a resource with index. Null if none found
        /// </summary>
        /// <param name="index">Index of the resource to be found</param>
        /// <returns></returns>
        public IndexedResource ResourceLookup(int index) {
            bool found = false;
            int i = 0;
            IndexedResource r = null;

            if (index == RTCore.kInvalidIndex)
                return r;

            while ((!found) && (i < mCollection.Count))
            {
                r = mCollection.ElementAt(i);
                found = (r.GetResourceIndex() == index);
                i++;
            }
            return r;
        }
    }
}
