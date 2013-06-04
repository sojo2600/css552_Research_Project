using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UWBGL_XNA_Lib
{
	public class UWB_XFormInfo
	{
        Vector3 mTranslation;
        Vector3 mScale;
        Quaternion mRotation;
        Vector3 mPivot;

		public UWB_XFormInfo()
		{
			mPivot = Vector3.Zero;
			toIdentity();
		}

		public void setupModelStack(UWB_DrawHelper drawHelper)
		{
			 drawHelper.accumulateModelTransform(mTranslation, mScale, GetRotation(), mPivot);
		}

		public void toIdentity()
		{
			 mTranslation = Vector3.Zero;
			 mScale = Vector3.One;
            InitializeRotation();
		}

		public Vector3 GetTranslation()
		{
			return mTranslation;
		}

		public void SetTranslation(Vector3 translation)
		{
			mTranslation = translation;
		}

		public Vector3 GetScale()
		{
			return mScale;
		}

		public void SetScale(Vector3 scale)
		{
			mScale = scale;
		}


		public Vector3 GetPivot()
		{
			return mPivot;
		}

		public void SetPivot(Vector3 pivot)
		{
			mPivot = pivot;
		}

		public void drawPivot(UWB_DrawHelper drawHelper, float size)
		{
			drawHelper.resetAttributes();
			drawHelper.setColor1(new Color(new Vector3(1f, 1f, 1f)));
			drawHelper.setColor2(new Color(new Vector3(0, 0, 1f)));
			drawHelper.setFillMode(eFillMode.fmSolid);
			drawHelper.setShadeMode(eShadeMode.smGouraud);
			drawHelper.drawCircle(mPivot, size);
		}

        
        // Rotation support
       
        public Quaternion GetRotationQuat()
        {
            return mRotation;
        }

        public void InitializeRotation() {
            mRotation = new Quaternion();
            mRotation = Quaternion.Identity; // Identity
        }
        public void SetRotation(Matrix m)
        {
            mRotation = Quaternion.CreateFromRotationMatrix(m);
        }
        public Matrix GetRotation()
        {
            return Matrix.CreateFromQuaternion(mRotation);
        }
        public void SetRotationQuat(Quaternion q)
        {
            mRotation = q;
        }
        public void UpdateRotationByQuat(Quaternion q)
        {
            mRotation = q * mRotation;
        }
        public void UpdateRotationXByDegree(float dx)
        {
            UpdateRotationXByRadian(MathHelper.ToRadians(dx));
        }
        public void UpdateRotationYByDegree(float dy)
        {
            UpdateRotationYByRadian(MathHelper.ToRadians(dy));
        }
        public void UpdateRotationZByDegree(float dz)
        {
            UpdateRotationZByRadian(MathHelper.ToRadians(dz));
        }
        public void UpdateRotationXByRadian(float dx)
        {
            Quaternion q = Quaternion.CreateFromAxisAngle(new Vector3(1.0f, 0.0f, 0.0f), -dx);
            mRotation = mRotation * q;
        }
        public void UpdateRotationYByRadian(float dy)
        {
            Quaternion q = Quaternion.CreateFromAxisAngle(new Vector3(0.0f, 1.0f, 0.0f), -dy);
            mRotation = mRotation * q;
        }
        public void UpdateRotationZByRadian(float dz)
        {
            Quaternion q = Quaternion.CreateFromAxisAngle(new Vector3(0.0f, 0.0f, 1.0f), -dz);
            mRotation = mRotation * q;
        }
	}
}
