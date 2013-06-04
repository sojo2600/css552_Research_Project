using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace RayTracer_552
{
    /// <summary>
    /// Internal support for texturable attribute. Assumption is that "texture" returns a normalized RGB, 
    /// but it is possible to abstract this RGB into float (e.g., by taking the length()). Lookup always return
    /// the Vector3 from texture map. Caller should determine what to do with the result.
    /// </summary>
    /// <typeparam name="DataType">data type of the attribute: for now, float or Vector3.</typeparam>
    class TexturableAttribute<DataType>
    {
        private DataType mAttribute;   // the actual attribute, e.g. Ka is a Vector3, or reflectivity is a float 
        private int mTextureIndex;     // index for the attribute

        public TexturableAttribute(DataType initValue)
        {
            mTextureIndex = RTCore.kInvalidIndex;
            mAttribute = initValue;
        }
        public void SetAttribute(DataType value) { mAttribute = value; }
        public void SetTextureIndex(int i) { mTextureIndex = i; }
        public bool IsTextureMapped() { return (RTCore.kInvalidIndex != mTextureIndex); }
        public DataType AttributeValue() { return mAttribute; }

        /// <summary>
        /// Looks up attribute from the texture table! NO ERROR CHECKINGG! If mTextureIndex is invalid, this function will crash!!
        /// </summary>
        /// <param name="database"></param>
        /// <param name="rec"></param>
        /// <returns></returns>
        public Vector3 AttributeLookup(SceneDatabase database, IntersectionRecord rec)
        {
            return database.GetTexture(mTextureIndex).TextureLookup(rec, database.GetGeom(rec.GeomIndex));
        }
    }

    /// <summary>
    /// Material support, nothing fancy, construct from parsing the command file, 
    /// with accessing methods.
    /// </summary>
    public class RTMaterial : IndexedResource
    {
        // classic Ambient, Diffuse, Specular
        private TexturableAttribute<Vector3> mKa, mKs, mKd; // Ambient, Diffuse, Specular coefients
        private float mN;              // Shinngingness

        // support for transparency and refractiveIndex
        private TexturableAttribute<float> mTransparency;
        private float mRefractiveIndex; 

        // support for reflection
        private TexturableAttribute<float> mReflectivity;   // how reflective
        
        // support for normal and positional map
        private TexturableAttribute<Vector3> mNormal, mPosition;

        public RTMaterial(CommandFileParser parser)
        {
            InitializeMaterial();

            parser.ParserRead();
            while (!parser.IsEndElement("material"))
            {
                if (parser.IsElement() && (!parser.IsElement("material")))
                {
                    if (parser.IsElement("index"))
                    {
                        int i = parser.ReadInt();
                        SetResourceIndex(i);
                    }
                    else if (parser.IsElement("ka"))
                        mKa.SetAttribute(parser.ReadVector3());
                    else if (parser.IsElement("ks"))
                        mKs.SetAttribute(parser.ReadVector3());
                    else if (parser.IsElement("kd"))
                        mKd.SetAttribute(parser.ReadVector3());
                    else if (parser.IsElement("n"))
                        mN = parser.ReadFloat();
                    else if (parser.IsElement("textureindex"))  // backward compatibility
                        mKd.SetTextureIndex(parser.ReadInt());
                    else if (parser.IsElement("kaIndex"))
                        mKa.SetTextureIndex(parser.ReadInt());
                    else if (parser.IsElement("kdIndex"))
                        mKd.SetTextureIndex(parser.ReadInt());
                    else if (parser.IsElement("ksIndex"))
                        mKs.SetTextureIndex(parser.ReadInt());
                    else if (parser.IsElement("tIndex"))
                        mTransparency.SetTextureIndex(parser.ReadInt());
                    else if (parser.IsElement("rIndex"))
                        mReflectivity.SetTextureIndex(parser.ReadInt());
                    else if (parser.IsElement("nIndex"))
                        mNormal.SetTextureIndex(parser.ReadInt());
                    else if (parser.IsElement("ptIndex"))
                        mPosition.SetTextureIndex(parser.ReadInt());
                    else if (parser.IsElement("reflectivity"))
                        mReflectivity.SetAttribute(parser.ReadFloat());
                    else if (parser.IsElement("transparency"))
                        mTransparency.SetAttribute(parser.ReadFloat());
                    else if (parser.IsElement("refractiveindex"))
                        mRefractiveIndex = parser.ReadFloat();
                    else
                        parser.ParserError("Material");
                }
                else
                    parser.ParserRead();
            }
        }

        public RTMaterial()
        {
            InitializeMaterial();
        }

        public void InitializeMaterial()
        {
            mKa = new TexturableAttribute<Vector3>(new Vector3(0.1f));
            mKd = new TexturableAttribute<Vector3>(new Vector3(0.1f));
            mKs = new TexturableAttribute<Vector3>(new Vector3(0.1f));
            mN = 1f;

            mTransparency = new TexturableAttribute<float>(0f);
            mRefractiveIndex = 1f;

            mReflectivity = new TexturableAttribute<float>(0f);

            mNormal = new TexturableAttribute<Vector3>(Vector3.Zero);
            mPosition = new TexturableAttribute<Vector3>(Vector3.Zero);
            
            SetResourceIndex(0);
        }
        

        public float GetRefractiveIndex { get { return mRefractiveIndex; } }
        public float GetN { get { return mN; } }
        public Vector3 GetAmbientColor { get { return mKa.AttributeValue(); } }
        public Vector3 GetDiffuseColor { get { return mKd.AttributeValue(); } }
        public Vector3 GetSpecularColor { get { return mKs.AttributeValue(); } }


        /// <summary>
        /// Ambient color lookup: if texture mapped, scale the texture value by mKa.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="rec"></param>
        /// <returns></returns>
        public Vector3 GetAmbient(SceneDatabase database, IntersectionRecord rec)
        {
            Vector3 result = mKa.AttributeValue();
            if (mKa.IsTextureMapped())
                result *= mKa.AttributeLookup(database, rec);
            return result;
        }
        /// <summary>
        /// Diffuse color lookup: if texture mapped, scale the texture value by mKs
        /// </summary>
        /// <param name="database"></param>
        /// <param name="rec"></param>
        /// <returns></returns>
        public Vector3 GetSpecular(SceneDatabase database, IntersectionRecord rec)
        {
            Vector3 result = mKs.AttributeValue();
            if (mKs.IsTextureMapped())
                result *= mKs.AttributeLookup(database, rec);
            return result;
        }
        /// <summary>
        /// Diffuse color lookup: if texture mapped, scale the texture value by mKd
        /// </summary>
        /// <param name="database"></param>
        /// <param name="rec"></param>
        /// <returns></returns>
        public Vector3 GetDiffuse(SceneDatabase database, IntersectionRecord rec)
        {
            Vector3 result = mKd.AttributeValue();
            if (mKd.IsTextureMapped())
                result *= mKd.AttributeLookup(database, rec);
            return result;
        }

        /// <summary>
        /// Normal vector lookup, if texture mapped, replaces the intersection record's normal vector!
        /// </summary>
        /// <param name="database"></param>
        /// <param name="rec"></param>
        /// <returns></returns>
        public Vector3 GetNormal(SceneDatabase database, IntersectionRecord rec)
        {
            Vector3 result;
            if (mNormal.IsTextureMapped())
            {
                result = mNormal.AttributeLookup(database, rec);
                if (Vector3.Dot(result, rec.NormalAtIntersect) < 0)
                    result = -result;
                rec.SetNormalAtIntersection(result);
            }
            else
            {
                result = rec.NormalAtIntersect;
            }
            
            return result;
        }

        /// <summary>
        /// Reflectivity look up: if mapped, scales Avg(RBG) by mReflectivity
        /// </summary>
        /// <param name="database"></param>
        /// <param name="rec"></param>
        /// <returns></returns>
        public float GetReflectivity(SceneDatabase database, IntersectionRecord rec)
        {
            float result = mReflectivity.AttributeValue();
            if (mReflectivity.IsTextureMapped())
            {
                Vector3 v = mReflectivity.AttributeLookup(database, rec);
                result *= ((v.X + v.Y + v.Z)/3f);
            }
            return result;
        }

        /// <summary>
        /// Transparency look up: if mapped, scales Avg(RBG) by transparency
        /// </summary>
        /// <param name="database"></param>
        /// <param name="rec"></param>
        /// <returns></returns>
        public float GetTransparency(SceneDatabase database, IntersectionRecord rec)
        {
            float result = mTransparency.AttributeValue();
            if (mTransparency.IsTextureMapped())
            {
                Vector3 v = mTransparency.AttributeLookup(database, rec);
                result *= ((v.X + v.Y + v.Z) / 3f);
            }
            return result;
        }
    }
}