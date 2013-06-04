using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace RayTracer_552
{
    /// <summary>
    /// Camera that views the RT scene. Constrcuts based on command file viewing parameters:
    ///         eye
    ///         lookat
    ///         upvector
    ///         fov
    ///         focus
    /// After the constructor, you should put in initialization computation to compute an
    /// orthogonal frame for this camera (and anything else you deem important) to support RT
    /// run time.
    /// 
    /// In addition, you may want to include functionality to support computation of pixel location 
    /// based on pixel localIndexBuffer (i,j) [You will need ImageSpec information in order to perform this 
    /// computation].
    /// </summary>
    public class RTCamera
    {
        protected Vector3 mEye;       // the eye position
        protected Vector3 mAt;        // the look at position
        protected Vector3 mUp;        // upvector
        protected Vector3 mViewDir;
        protected Vector3 mSideVec;
        protected float mFOV;      // in degree!!
        protected float mFocus;    // place to form the image plane

        protected Vector3 mPixeldX, mPixeldY;
        protected Vector3 mPixelOrigin; // lower left corner

        /// <summary>
        /// Construcs from parsing the command file. 
        /// DO NOT CHANGE the parsing loop unless you know what you are doing.
        /// You can add in initialization computation after the loop
        /// </summary>
        /// <param name="parser"></param>
        public RTCamera(CommandFileParser parser)
        {
            mEye = new Vector3(0f, 0f, 0f);
            mAt = new Vector3(0f, 0f, 1f);
            mUp = new Vector3(0f, 1f, 0f);
            mFOV = 25f;
            mFocus = 1f;

            parser.ParserRead();
            while (!parser.IsEndElement("camera"))
            {
                if (parser.IsElement() && (!parser.IsElement("camera")))
                {
                    if (parser.IsElement("eye"))
                        mEye = parser.ReadVector3();
                    else if (parser.IsElement("lookat"))
                        mAt = parser.ReadVector3();
                    else if (parser.IsElement("upvector"))
                        mUp = parser.ReadVector3();
                    else if (parser.IsElement("fov"))
                        mFOV = parser.ReadFloat();
                    else if (parser.IsElement("focus"))
                        mFocus = parser.ReadFloat();
                    else
                        parser.ParserError("Camera");
                }
                else
                    parser.ParserRead();
            }

            // You can add in your initialization computation after this line

            mViewDir = mAt - mEye;
            
            // now make sure ViewDir and Up are perpendicular
            mSideVec = Vector3.Cross(mUp, mViewDir);
            mUp = Vector3.Cross(mViewDir, mSideVec);
            mSideVec.Normalize();
            mViewDir.Normalize();
            mUp.Normalize();
        }

        // Copy Constructor
        public RTCamera(RTCamera c)
        {
            mEye = c.mEye;
            mAt = c.mAt;
            mUp = c.mUp;
            mViewDir = c.mViewDir;
            mSideVec = c.mSideVec;
            mFOV = c.mFOV;
            mFocus = c.mFocus;
            mPixeldX = c.mPixeldX;
            mPixeldY = c.mPixeldY;
            mPixelOrigin = c.mPixelOrigin;
        }

        /// <summary>
        /// Accessing functions
        /// </summary>
        public Vector3 ViewDirection { get { return mViewDir; } }
        public Vector3 SideDirection { get { return mSideVec; } }
        public Vector3 EyePosition { get { return mEye; } }
        public Vector3 AtPosition { get { return mAt; } }
        public Vector3 UpVector { get { return mUp; } }
        public float FOV { get { return mFOV; } }
        public float Focus { get { return mFocus; } }


        /// <summary>
        /// Initialize pixel location computation based on ImageSpec information.
        /// </summary>
        /// <param name="c"></param>
        public void InitializeImage(ImageSpec im)
        {
            const float kUseViewDist = 10f;
            float halfImageHeight = kUseViewDist * (float)Math.Tan(MathHelper.ToRadians(FOV) / 2.0);
            float halfImageWidth = halfImageHeight * (float)im.XResolution / (float)im.YResolution;
            Vector3 atOnImagePlane = EyePosition + (kUseViewDist * ViewDirection);
            mPixelOrigin = atOnImagePlane + (halfImageHeight * UpVector) + (halfImageWidth * SideDirection);
            mPixeldX = -(halfImageWidth * 2f / (float)im.XResolution) * SideDirection;
            mPixeldY = -(halfImageHeight * 2f / (float)im.YResolution) * UpVector;
        }

        /// <summary>
        /// Gets pixel-localIndexBuffer (x,y) and returns 3D location of the pixel.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 GetPixelPosition(float x, float y)
        {
            return mPixelOrigin + (x * mPixeldX) + (y * mPixeldY);
        }
    }   
}