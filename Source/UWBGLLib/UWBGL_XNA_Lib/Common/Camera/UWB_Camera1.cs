using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;

namespace UWBGL_XNA_Lib
{
    public class UWB_Camera
    {
        private Vector3 m_CameraPos;
        private Vector3 m_LookAt;
        private Vector3 m_UpVector;
        private float m_ViewFoV;
        private float m_ViewNearPlane, m_ViewFarPlane;
        private int m_TmpInitXPos, m_TmpInitYPos;
        private float kMinDeltaRotate = MathHelper.ToRadians(0.1f);

        public UWB_Camera()
        {
            m_CameraPos = Vector3.Forward;
            m_LookAt = Vector3.Zero;
            m_UpVector = new Vector3(0f, 1f, 0f);
            m_ViewFoV = 45.0f;
            m_ViewNearPlane = .1f;
            m_ViewFarPlane = 100f;
            m_TmpInitXPos = 0;
            m_TmpInitYPos = 0;
        }


        public void SetViewFrustumInDegree(float v, float n, float f)
        {
            m_ViewFoV = v;
            m_ViewNearPlane = n;
            m_ViewFarPlane = f;
        }

        public void GetViewFrustumInDegree(ref float v, ref float n, ref float f)
        {
            v = m_ViewFoV;
            n = m_ViewNearPlane;
            f = m_ViewFarPlane;
        }
     
        public Vector3 CameraPosition
        {
            get { return m_CameraPos; }
            set { m_CameraPos = value; }
        }

        public Vector3 CameraLookAt
        {
            get { return m_LookAt; }
            set { m_LookAt = value; }
        }

        public Vector3 CameraUp
        {
            get { return m_UpVector; }
            set { m_UpVector = value; }
        }

        public float FoV
        {
            get { return m_ViewFoV; }
            set { m_ViewFoV = value; }
        }

        public float FoVRadian
        {
            get { return MathHelper.ToRadians(m_ViewFoV); }
            set { m_ViewFoV = MathHelper.ToDegrees(value); }
        }

        public float NearClipping
        {
            get { return m_ViewNearPlane; }
            set { m_ViewNearPlane = value; }
        }

        public float FarClipping
        {
            get { return m_ViewFarPlane; }
            set { m_ViewFarPlane = value; }
        }

        private void ComputeCameraFrame(ref float d, ref Vector3 v, ref Vector3 u, ref Vector3 w)
        {
            v = m_LookAt - m_CameraPos;

            d = v.Length();
            v = v * (1.0f / d);

            w = Vector3.Cross(m_UpVector, v);
            v.Normalize();

            u = Vector3.Cross(v, w);
            u.Normalize();
        }

        private void RotateCameraY(float rot)
        {
            float dist = 0f;
            Vector3 u = Vector3.UnitX, v = Vector3.UnitY, w = Vector3.UnitZ;
            ComputeCameraFrame(ref dist, ref v, ref u, ref w);
            // 1. rotate in the direction of fW by certain -dx degree ...
            //     in the fW direction, this is rotation wrt to fU vector
            // 
            // we DO NOT want to rotate wrt to u, but then, what are we rotating wrt? (the UP vector!)
            if (v.Y != 0.0)
            {
                Matrix m;

                float sign = 1.0f;

                if (Vector3.Dot(Vector3.UnitY, u) < 0)
                    sign = -1.0f;

                Vector3 axis = Vector3.UnitY * sign;
                m = Matrix.CreateFromAxisAngle(axis, rot);
                v = Vector3.Transform(v, m);
                Vector3 tv = new Vector3(v.X, 0f, v.Z);
                w = Vector3.Cross(axis, tv);
                w.Normalize();
                m_UpVector = Vector3.Cross(v, w);
                m_UpVector.Normalize();
                m_CameraPos = m_LookAt - (dist * v);
            }
        }

        private void RotateCameraX(float rot)
        {
            float dist = 0f;
            Vector3 u = Vector3.UnitX, v = Vector3.UnitY, w = Vector3.UnitZ;
            ComputeCameraFrame(ref dist, ref v, ref u, ref w);

            // 2. now rotate in the direction of fU by certain dy degree
            //			fU direction rotation is wrt to fW
            Matrix m;
            m = Matrix.CreateFromAxisAngle(w, rot);
            v = Vector3.Transform(v, m);
            u = Vector3.Transform(u, m);

            w = Vector3.Cross(u, v);
            w.Normalize();
            m_UpVector = u;

            m_CameraPos = m_LookAt - (dist * v);
        }

        public void BeginMouseRotate(int x, int y)
        {
          m_TmpInitXPos = x;
          m_TmpInitYPos = y;
        }

        public void MouseRotate(int x, int y)
        {
            float xRot = -MathHelper.ToRadians(0.4f * (y - m_TmpInitYPos));
            float yRot = -MathHelper.ToRadians(0.4f * (x - m_TmpInitXPos));


            if (Math.Abs(yRot) > kMinDeltaRotate)
            {
                RotateCameraY(yRot);
                m_TmpInitXPos = x;
            }

            if (Math.Abs(xRot) > kMinDeltaRotate)
            {
                RotateCameraX(xRot);
                m_TmpInitYPos = y;
            }
        }


        public void BeginMouseTrack(int x, int y)
        {
          m_TmpInitXPos = x;
          m_TmpInitYPos = y;
        }

        
        private static float TRACK_FACTOR  = 0.1f;
        
        public void MouseTrack(int x, int y)
        {
          float dx = (float)(x-m_TmpInitXPos) * TRACK_FACTOR;     // each pixel tracks 0.2 WC unit
          float dy = (float)(y-m_TmpInitYPos) * TRACK_FACTOR;

          Vector3 view_direction, cross_direction;
          Vector3 up_direction = m_UpVector;
          view_direction = m_LookAt - m_CameraPos;

          cross_direction = Vector3.Cross(up_direction, view_direction);
          cross_direction.Normalize();

          up_direction = Vector3.Cross(view_direction, cross_direction);
          up_direction.Normalize();

          Vector3 delta = (-dx * cross_direction) + (dy * up_direction);
          m_CameraPos = m_CameraPos - delta;
          m_LookAt = m_LookAt - delta;

          // continue to track
          BeginMouseTrack(x, y);
        }

        public void BeginMouseZoom(int x)
        {
          m_TmpInitXPos = x;
        }

        public void MouseZoom(int x)
        {
          float dx = (float)(x - m_TmpInitXPos) * TRACK_FACTOR;
          Vector3 view_vec = m_LookAt - m_CameraPos;
          float view_distance = view_vec.Length();
          view_vec.Normalize();
          m_CameraPos = m_LookAt - ( (view_distance + dx) * view_vec);
          BeginMouseZoom(x);
        }


    }
}


