using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{    
    // This is based on NVidia's GPU shader web-site: Vertex Noise shader:
    //    http://http.download.nvidia.com/developer/SDK/Individual_Samples/samples.html#glsl_vnoise
    //
    // Where, the noice function is more or less based on:
    // Perlin's original code:
    //
    //     http://mrl.nyu.edu/~perlin/doc/oscar.html
    //
    // Implementation copied from:
    //     http://mrl.nyu.edu/~perlin/noise/
    //
    //     Reference paper: http://mrl.nyu.edu/~perlin/paper445.pdf
    //
    // Added: 2D and 1D access to noise
    //
    public class RTTextureType_Noise : RTTextureType 
    {
        
        /// <summary>
        /// This are static: compute once use by all instances
        /// </summary>
        private const int SIZE = 512;
        private static int[] mPermutation = null;

        // this is: 6*t^5 - 15t^4 + 10t^3
        private static float fade(float t) { 
            return t * t * t * (t * (t * 6 - 15) + 10); 
        }

        // Linear interpolation: between from a to b, t is a value between 0 to 1
        // when t==0, value is a, 
        // when t==1, value is b
        private static float lerp(float t, float a, float b) { 
            return a + t * (b - a); 
        }

        // use the "hash" value to simulate dot product 
        private static float grad(int hash, float x, float y, float z) {
          int h = hash & 0x0F;                    // CONVERT LO 4 BITS OF HASH CODE
          float u = h<8 ? x : y,                  // INTO 12 GRADIENT DIRECTIONS.
                v = h<4 ? y : h==12||h==14 ? x : z;
          return ((h&1) == 0 ? u : -u) + ((h&2) == 0 ? v : -v);
        }

        private static void InitPermutation()
        {
            mPermutation = new int[SIZE] {151,160,137,91,90,15,
                   131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
                   190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
                   88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
                   77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
                   102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
                   135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
                   5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
                   223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
                   129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
                   251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
                   49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
                   138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
                   151,160,137,91,90,15,
                   131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
                   190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
                   88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
                   77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
                   102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
                   135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
                   5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
                   223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
                   129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
                   251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
                   49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
                   138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180};
        }

        ///
        /// The following are instance variables for each object
        /// 
        protected Vector3 mColor1, mColor2;
        protected int mFrequency;
        protected float mAmplitude;

        /// <summary>
        /// Constrcuts from the commandfile.
        /// DO NOT change the pasing loop unless you know what you are doing.
        /// </summary>
        /// <param name="parser"></param>
        public RTTextureType_Noise(CommandFileParser parser)
        {
            if (null == mPermutation)
                InitPermutation();

            mColor1 = Vector3.One;
            mColor2 = Vector3.Zero;
            mFrequency = 1;
            mAmplitude = 1f;

            while (!parser.IsEndElement("texture"))
            {
                if (parser.IsElement() && (!parser.IsElement("texture")))
                {
                    if (parser.IsElement("color1"))
                        mColor1 = parser.ReadVector3();
                    else if (parser.IsElement("color2"))
                        mColor2 = parser.ReadVector3();
                    else if (parser.IsElement("frequency"))
                        mFrequency = parser.ReadInt();
                    else if (parser.IsElement("amplitude"))
                        mAmplitude = parser.ReadFloat();
                    else
                        parser.ParserError("TextureType_Noise");
                }
                else
                    parser.ParserRead();
            }
        }

        protected RTTextureType_Noise()
        {
            if (null == mPermutation)
                InitPermutation();
        }

        public override bool NeedUV() { return false; }

        protected float ComputeNoise(float x, float y, float z)
        {
            int I = (int)Math.Floor(x) & 0xFF,                  // FIND UNIT CUBE THAT
                 J = (int)Math.Floor(y) & 0xFF,                  // CONTAINS POINT.
                 K = (int)Math.Floor(z) & 0xFF;
            x -= (float)Math.Floor(x);                                // FIND RELATIVE I,J,K
            y -= (float)Math.Floor(y);                                // OF POINT IN CUBE.
            z -= (float)Math.Floor(z);
            float u = fade(x),                                // COMPUTE FADE CURVES
                   v = fade(y),                                // FOR EACH OF I,J,K.
                   w = fade(z);
            int A = mPermutation[I] + J, AA = mPermutation[A] + K, AB = mPermutation[A + 1] + K,
                B = mPermutation[I + 1] + J, BA = mPermutation[B] + K, BB = mPermutation[B + 1] + K;

            return lerp(w, lerp(v, lerp(u, grad(mPermutation[AA], x, y, z),  // AND ADD
                                         grad(mPermutation[BA], x - 1, y, z)), // BLENDED
                                  lerp(u, grad(mPermutation[AB], x, y - 1, z),  // RESULTS
                                         grad(mPermutation[BB], x - 1, y - 1, z))),// FROM  8
                          lerp(v, lerp(u, grad(mPermutation[AA + 1], x, y, z - 1),  // CORNERS
                                         grad(mPermutation[BA + 1], x - 1, y, z - 1)), // OF CUBE
                                  lerp(u, grad(mPermutation[AB + 1], x, y - 1, z - 1),
                                         grad(mPermutation[BB + 1], x - 1, y - 1, z - 1))));
        }

        protected float ComputeFractalNoise(float x, float y, float z)
        {
            float noise = 0f;
            float f = 1f;

            for (int i = 0; i <= mFrequency; i++)
            {
                noise += (ComputeNoise(x * f, y * f, z * f) / f);
                f *= 2f;
            }
            return noise * mAmplitude;
        }


        protected float ComputeTurbulanceNoise(float x, float y, float z)
        {
            float noise = 0f;
            float f = 1f;

            for (int i = 0; i <= mFrequency; i++)
            {
                noise += (Math.Abs(ComputeNoise(x * f, y * f, z * f)) / f);
                f *= 2f;
            }
            return noise * mAmplitude;
        }

        /// <param name="rec">IntersectionRecord to be texture mapped</param>
        /// <param name="g">The geometry</param>
        /// <returns></returns>
        public override Vector3 GetTexile(IntersectionRecord rec, RTGeometry g)
        {
            float  noise = ComputeFractalNoise(rec.IntersectPosition.X, rec.IntersectPosition.Y, rec.IntersectPosition.Z);
            return (noise * mColor1) + ((1f - noise) * mColor2);
        }
    }
}