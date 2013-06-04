using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// Typical "file color map" 
    /// </summary>
    public class RTTextureType_Bump : RTTextureType {
    
        private Bitmap mTextureImage; // the bitmap image file.
        private float mInvWidth, mInvHeight;
        private float mGain;

        /// <summary>
        /// Constructs from parsing the command file.
        /// DO NOT change the parser loop unless you know what you are doing.
        /// </summary>
        /// <param name="parser"></param>
        public RTTextureType_Bump(CommandFileParser parser)
        {
            mGain = 1.0f;

            while (!parser.IsEndElement("texture"))
            {
                if (parser.IsElement() && (!parser.IsElement("texture")))
                {
                    if (parser.IsElement("filename")) {
                        String name = parser.ReadString();
                        name = parser.FullPath() + "\\" + name;
                        if (System.IO.File.Exists(name))
                            mTextureImage = new Bitmap(name, true);
                        else
                            parser.ParserError("TextureType_Bump filename");
                    }
                    else if (parser.IsElement("gain"))
                        mGain = parser.ReadFloat();
                    else
                        parser.ParserError("TextureType_Bump");
                }
                else
                    parser.ParserRead();
            }
            mInvWidth = 1f / (float)(mTextureImage.Width - 1);
            mInvHeight = 1f / (float)(mTextureImage.Height - 1);
        }


        /// <summary>
        /// UV are values between 0 to 1, maps to texture width/height linearly and returns the
        /// corresponding textile.
        /// </summary>
        /// <param name="u">value between 0 to 1</param>
        /// <param name="v">value between 0 to 1</param>
        /// <param name="rec">IntersectionRecord to be texture mapped</param>
        /// <param name="g">the geometry</param>
        /// <returns></returns>
        public override Vector3 GetTexile(float u, float v, IntersectionRecord rec, RTGeometry g)
        {
            Vector3 norm = Vector3.UnitY;
            if ((u >= 0) && (u <= 1f) && (v >= 0) && (v <= 1f))
            {
                int u0Index=0, u1Index=0, v0Index=0, v1Index=0;
                float uIndex, vIndex;
                float bumpAtu0v0, bumpAtu0v1, bumpAtu1v0, bumpAtu1v1;
                lock (mTextureImage)
                {
                    GetIndices(u, mTextureImage.Width, ref u0Index, ref u1Index);
                    GetIndices(v, mTextureImage.Height, ref v0Index, ref v1Index);
                    v0Index = mTextureImage.Height - 1 - v0Index;
                    v1Index = mTextureImage.Height - 1 - v1Index;
                    uIndex = u * mTextureImage.Width;
                    vIndex = mTextureImage.Height - 1 - (v * mTextureImage.Height);

                    bumpAtu0v0 = ColorToVec(mTextureImage.GetPixel(u0Index, v0Index)).Length();
                    bumpAtu1v0 = ColorToVec(mTextureImage.GetPixel(u1Index, v0Index)).Length();
                    bumpAtu0v1 = ColorToVec(mTextureImage.GetPixel(u0Index, v1Index)).Length();
                    bumpAtu1v1 = ColorToVec(mTextureImage.GetPixel(u1Index, v1Index)).Length();
                }
                float du = uIndex - u0Index;
                float dv = vIndex - v1Index;
                float bumpAtV0 = du * bumpAtu1v0 + (1 - du) * bumpAtu0v0;
                float bumpAtV1 = du * bumpAtu1v1 + (1 - du) * bumpAtu0v1;
                float bumpAtU0 = dv * bumpAtu0v1 + (1 - dv) * bumpAtu0v0;
                float bumpAtU1 = dv * bumpAtu1v1 + (1 - dv) * bumpAtu1v0;

                float dDu = mGain * (bumpAtV1 - bumpAtV0); 
                float dDv = mGain * (bumpAtU1 - bumpAtU0);
                
                float u0Ref, u1Ref, v0Ref, v1Ref;
                u0Ref = u0Index * mInvWidth;
                u1Ref = u1Index * mInvWidth;
                v0Ref = v0Index * mInvHeight;
                v1Ref = v1Index * mInvHeight;

                Vector3 Pu0 = g.GetPosition(u0Ref, v);
                Vector3 Pu1 = g.GetPosition(u1Ref, v);
                Vector3 Pv0 = g.GetPosition(u, v0Ref);
                Vector3 Pv1 = g.GetPosition(u, v1Ref);
                Vector3 Pu = Pu1 - Pu0;
                Vector3 Pv = Pv1 - Pv0;

                Vector3 aDir = Vector3.Normalize(Vector3.Cross(Pu, rec.NormalAtIntersect));
                Vector3 bDir = Vector3.Normalize(Vector3.Cross(Pv, rec.NormalAtIntersect));
                Vector3 D = (dDv * aDir) - (dDu * bDir);

                norm = rec.NormalAtIntersect + D;
                norm.Normalize();
            }

            return norm;
        }
    }
}