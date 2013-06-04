using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// Scene data base for the RT engine, including:
    /// 
    ///     1. Geometries: 
    ///     2. Materials
    ///     3. Textures
    ///     4. Light sources
    ///     
    /// </summary>

    /*
        For: 
                Geometry and 
                Light, 
            
             you can loop through them as an array:
         
             SceneDatabase myDatabase;  
             ...
             // geometry ...
             for (int i = 0; i < myDatabase.GetNumGeom(); i++) {  
                  RTGeometry g = mSceneDatabase.GetGeom(i);
                  g.Dosomething ...
             }
             ...
             // light ...
             for (int i = 0; i < myDatabase.GetNumLights(); i++) {  
                  RTLight l = mSceneDatabase.GetLight(i);
                  l.Dosomething ...
             }
      
       For: 
                Texture and Materials
        
            you should only access the resource when you know their index!!
    */

    public class SceneDatabase
    {
        private SceneResource<RTGeometry> mAllGeoms;
        private SceneResource<RTMaterial> mAllMaterials;
        private SceneResource<RTTexture> mAllTextures;
        private SceneResource<RTLight> mAllLights;

        public SceneDatabase()
        {
            mAllGeoms = new SceneResource<RTGeometry>();
            mAllMaterials = new SceneResource<RTMaterial>();
            mAllTextures = new SceneResource<RTTexture>();
            mAllLights = new SceneResource<RTLight>();

            // the default material: index of 0
            RTMaterial m = new RTMaterial();
            mAllMaterials.AddResource(m);
        }


        /// <summary>
        /// Geometry: Can be access as an array, the collection is a simple array
        /// </summary>
        public void AddGeom(RTGeometry g)
        {
            g.SetResourceIndex(mAllGeoms.Count);
            mAllGeoms.AddResource(g);
        }
        public RTGeometry GetGeom(int index)
        {
            return (RTGeometry) mAllGeoms.ResourceLookup(index);
        }
        public SceneResource<RTGeometry> GetAllGeom() { return mAllGeoms; }
        public int GetNumGeom() { return mAllGeoms.Count; }



        /// <summary>
        /// Lights: can be access as an array, the collection is a simple array
        /// </summary>
        /// <param name="l"></param>
        public void AddLight(RTLight l)
        {
            l.SetResourceIndex(mAllLights.Count);
            mAllLights.AddResource(l);
        }
        public RTLight GetLight(int index) { return (RTLight)mAllLights.ResourceLookup(index); }
        public int GetNumLights() { return mAllLights.Count; }
            

        /// <summary>
        /// Material: can only be access given an index (the collection may be a sparse array!)
        /// </summary>
        /// <param name="m"></param>
        public void AddMaterial(RTMaterial m)
        {
            mAllMaterials.AddResource(m);
        }
        public RTMaterial GetMaterial(int index) { return (RTMaterial)mAllMaterials.ResourceLookup(index); }


        
        /// <summary>
        /// Texture: can only be access given an index (the collection may be a sparse array!) 
        /// </summary>
        /// <param name="t"></param>
        public void AddTexture(RTTexture t)
        {
            mAllTextures.AddResource(t);
        }
        public RTTexture GetTexture(int index) { return (RTTexture)mAllTextures.ResourceLookup(index); }
    }
}
