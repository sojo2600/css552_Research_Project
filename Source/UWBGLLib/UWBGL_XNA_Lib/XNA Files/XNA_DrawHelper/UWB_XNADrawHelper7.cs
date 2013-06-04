using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace UWBGL_XNA_Lib
{
    public class UWB_XNADrawHelper : UWB_DrawHelper
    {

        public UWB_XNADrawHelper()
        {

        }

        public override bool drawPoint(Vector3 position)
        {
            GraphicsDevice device = UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice;
            Debug.Assert(device != null, "device not valid on DrawHelper drawCircle call");

            // Draw points as rectangles
            VertexPositionColorTexture[] v = new VertexPositionColorTexture[4];
            float offset = mPointSize / 2f;

            for (int i = 0; i < 4; i++)
            {
                v[i] = new VertexPositionColorTexture();
                v[i].Color = mColor1;
            }

            v[0].Position = new Vector3(position.X - offset, position.Y + offset, 0);
            v[0].TextureCoordinate = new Vector2(0f, 0f);

            v[1].Position = new Vector3(position.X - offset, position.Y - offset, 0);
            v[1].TextureCoordinate = new Vector2(0f, 1f);

            v[2].Position = new Vector3(position.X + offset, position.Y + offset, 0);
            v[2].TextureCoordinate = new Vector2(1f, 0f);

            v[3].Position = new Vector3(position.X + offset, position.Y - offset, 0);
            v[3].TextureCoordinate = new Vector2(1f, 1f);

            if (device.RasterizerState.FillMode != FillMode.Solid)
            {
                RasterizerState s = new RasterizerState();
                s.CullMode = CullMode.None;
                s.FillMode = FillMode.Solid;
                device.RasterizerState = s;
            }

            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, v, 0, 2);
            return true;
        }

        public override bool drawLine(Vector3 start, Vector3 end)
        {
            GraphicsDevice device = UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice;
            Debug.Assert(device != null, "device not valid on DrawHelper drawCircle call");
            VertexPositionNormalTexture[] v = new VertexPositionNormalTexture[2];

            Color flatColor = mColor1;

            Vector3 dir = end - start;
            Vector3 normal = Vector3.UnitX;

            if ((dir.X > 0.01f) && (dir.Y > 0.01f))
            {
                normal.X = -dir.Y;
                normal.Y = dir.X;
                normal.Z = 0.0f; // a normal to the line direction
            }

            v[0].Position = start;
            v[0].TextureCoordinate = new Vector2(0f, 0f);
            v[1].Position = end;
            v[1].TextureCoordinate = new Vector2(1f, 1f);
            
            v[0].Normal = normal;
            v[1].Normal = normal;


            UWB_XNAEffect effect = UWB_XNAGraphicsDevice.m_TheAPI.LightingEffect;

            Texture2D texture = null;

            if (m_bTexturingEnabled && m_TexFileName != "")
                texture = UWB_XNAGraphicsDevice.m_TheAPI.RetrieveTexture(m_TexFileName);

            effect.Material = this.m_Material;

            if (texture != null)
            {
                effect.Texture = texture;
                effect.TextureEnabled = true;
            }
            else
            {
                effect.Texture = texture;
                effect.TextureEnabled = false;
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives(PrimitiveType.LineList, v, 0, 1);
            }

            return true;
        }

        public override bool drawCircle(Vector3 center, float radius)
        {
            GraphicsDevice device = UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice;
            Debug.Assert(device != null, "device not valid on DrawHelper drawCircle call");

            if (radius > 0)
            {
                int kNumTriangles = 40;
                if (mLod == eLevelofDetail.lodLow)
                    kNumTriangles = 10;
                else if (mLod == eLevelofDetail.lodMed)
                    kNumTriangles = 20;
                VertexPositionNormalTexture[] v = new VertexPositionNormalTexture[kNumTriangles * 3];

                float theta = (2.0f * (float)Math.PI) / kNumTriangles;

                v[0].Position = center;
                v[0].TextureCoordinate = new Vector2(0.5f, 0.5f);
                v[0].Normal = Vector3.UnitZ;
                float px = center.X + radius;
                float py = center.Y;
                Vector2 puv = new Vector2(1, 0);

                for (int i = 0; i < kNumTriangles; i++)
                {
                    int offset = i * 3;
                    float x = center.X + (radius * (float)Math.Cos((i + 1) * theta));
                    float y = center.Y + (radius * (float)Math.Sin((i + 1) * theta));
                    float tu = 0.5f + (0.5f * (float)Math.Cos((i + 1) * theta));
                    float tv = 0.5f - (0.5f * (float)Math.Sin((i + 1) * theta));
                    v[offset].Position = v[0].Position;
                    v[offset].TextureCoordinate = v[0].TextureCoordinate;
                    v[offset].Normal = Vector3.UnitZ;

                    v[offset + 1].Position = new Vector3(px, py, 0f);
                    v[offset + 1].TextureCoordinate = puv;
                    v[offset + 1].Normal = Vector3.UnitZ;

                    v[offset + 2].Position = new Vector3(x, y, 0f);
                    v[offset + 2].TextureCoordinate = new Vector2(tu, tv);
                    v[offset + 2].Normal = Vector3.UnitZ;

                    px = x;
                    py = y;
                    puv = v[offset + 2].TextureCoordinate;
                }

                UWB_XNAEffect effect = UWB_XNAGraphicsDevice.m_TheAPI.LightingEffect;

                Texture2D texture = null;

                if (m_bTexturingEnabled && m_TexFileName != "")
                    texture = UWB_XNAGraphicsDevice.m_TheAPI.RetrieveTexture(m_TexFileName);

                effect.Material = this.m_Material;

                if (texture != null)
                {
                    effect.Texture = texture;
                    effect.TextureEnabled = true;
                }
                else
                {
                    effect.Texture = texture;
                    effect.TextureEnabled = false;
                }

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserPrimitives(PrimitiveType.TriangleList, v, 0, kNumTriangles);
                }
            }

            return true;

        }

        public override bool drawRectangle(Vector3 min, Vector3 max)
        {
            drawARectangle(min, max, Vector3.UnitZ);
            return true;
        }

        public override bool drawRectangleXZ(Vector3 min, Vector3 max)
        {
            drawARectangle(min, max, Vector3.UnitY);
            return true;
        }

        private void drawARectangle(Vector3 min, Vector3 max, Vector3 n)
        {
            GraphicsDevice device = UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice;
            Debug.Assert(device != null, "device not valid on DrawHelper drawRectangle call");
            VertexPositionNormalTexture[] v = new VertexPositionNormalTexture[12];

            v[0].Position = Vector3.Divide((min + max), 2);
            v[0].TextureCoordinate = new Vector2(0.5f, 0.5f);
            v[0].Normal = n;

            v[1].Position = new Vector3(max.X, min.Y, min.Z);
            v[1].TextureCoordinate = new Vector2(1f, 1f);
            v[1].Normal = n;

            v[2].Position = max;
            v[2].TextureCoordinate = new Vector2(1f, 0f);
            v[2].Normal = n;

            v[3] = v[0];
            v[4] = v[2];

            v[5].Position = new Vector3(min.X, max.Y, max.Z);
            v[5].TextureCoordinate = new Vector2(0f, 0f);
            v[5].Normal = n;

            v[6] = v[0];
            v[7] = v[5];
            v[8].Position = min;
            v[8].TextureCoordinate = new Vector2(0f, 1f);
            v[8].Normal = n;

            v[9] = v[0];
            v[10] = v[8];
            v[11] = v[1];

            UWB_XNAEffect effect = UWB_XNAGraphicsDevice.m_TheAPI.LightingEffect;

            Texture2D texture = null;

            if (m_bTexturingEnabled && m_TexFileName != "")
                texture = UWB_XNAGraphicsDevice.m_TheAPI.RetrieveTexture(m_TexFileName);

            effect.Material = this.m_Material;

            if (texture != null)
            {
                effect.Texture = texture;
                effect.TextureEnabled = true;
            }
            else
            {
                effect.Texture = texture;
                effect.TextureEnabled = false;
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives(PrimitiveType.TriangleList, v, 0, 4);
            }

        }

        public override bool drawTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            GraphicsDevice device = UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice;
            Debug.Assert(device != null, "device not valid on DrawHelper drawTriangle call");
            VertexPositionNormalTexture[] v = new VertexPositionNormalTexture[3];

            Vector3 d1 = v2 - v1;
            Vector3 d2 = v3 - v1;
            Vector3 n = Vector3.Cross(d1, d2);
            n.Normalize();

            v[0].Position = v1;
            v[0].TextureCoordinate = new Vector2(0.5f);
            v[0].Normal = n;

            v[1].Position = v2;
            v[1].TextureCoordinate = new Vector2(0.5f);
            v[1].Normal = n;

            v[2].Position = v3;
            v[2].TextureCoordinate = new Vector2(0.5f); 
            v[2].Normal = n;


            UWB_XNAEffect effect = UWB_XNAGraphicsDevice.m_TheAPI.LightingEffect;

            Texture2D texture = null;

            if (m_bTexturingEnabled && m_TexFileName != "")
                texture = UWB_XNAGraphicsDevice.m_TheAPI.RetrieveTexture(m_TexFileName);

            effect.Material = this.m_Material;

            if (texture != null)
            {
                effect.Texture = texture;
                effect.TextureEnabled = true;
            }
            else
            {
                effect.Texture = texture;
                effect.TextureEnabled = false;
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives(PrimitiveType.TriangleList, v, 0, 1);
            }

            return true;
        }

        public override void setFillMode(eFillMode fillMode)
        {
            GraphicsDevice graphicsDevice = UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice;
            bool needToSetMode = false;
            FillMode newMode = FillMode.Solid;
            switch (fillMode)
            {
                case eFillMode.fmWireframe:
                case eFillMode.fmPoint:
                    needToSetMode = (graphicsDevice.RasterizerState.FillMode != FillMode.WireFrame);
                    newMode = FillMode.WireFrame;
                    break;
                case eFillMode.fmSolid:
                    needToSetMode = (graphicsDevice.RasterizerState.FillMode != FillMode.Solid);
                    break;
            }
            RasterizerState s = new RasterizerState();
            s.CullMode = CullMode.None;
            s.FillMode = newMode;
            graphicsDevice.RasterizerState = s;
        }

        public override bool accumulateModelTransform(
            Vector3 translation,
            Vector3 scale,
            float rotationRadians,
            Vector3 rotation_axis,
            Vector3 pivot)
        {
            if (UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice == null)
                return false;

            Vector3 axis = new Vector3(rotation_axis.X, rotation_axis.Y, rotation_axis.Z);
            m_MatrixStack.TranslateLocal(translation);
            m_MatrixStack.TranslateLocal(pivot);
            m_MatrixStack.RotateAxisLocal(axis, rotationRadians);
            m_MatrixStack.ScaleLocal(scale);
            m_MatrixStack.TranslateLocal(-pivot);

            UWB_XNAGraphicsDevice.m_TheAPI.WorldMatrix = m_MatrixStack.Top();
            return true;

        }

        public override bool accumulateModelTransform(
            Vector3 translation,
            Vector3 scale,
            Vector3 rotationRadians,
            Vector3 pivot)
        {
            if (UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice == null)
                return false;

            Vector3 z_axis = new Vector3(0, 0, 1);
            Vector3 y_axis = new Vector3(0, 1, 0);
            Vector3 x_axis = new Vector3(1, 0, 0);
            m_MatrixStack.TranslateLocal(translation);
            m_MatrixStack.TranslateLocal(pivot);
            m_MatrixStack.RotateAxisLocal(x_axis, rotationRadians.X);
            m_MatrixStack.RotateAxisLocal(y_axis, rotationRadians.Y);
            m_MatrixStack.RotateAxisLocal(z_axis, rotationRadians.Z);
            m_MatrixStack.ScaleLocal(scale);
            m_MatrixStack.TranslateLocal(-pivot);

            UWB_XNAGraphicsDevice.m_TheAPI.WorldMatrix = m_MatrixStack.Top();
            return true;

        }

        public override bool accumulateModelTransform(
           Vector3 translation,
           Vector3 scale,
           Matrix rotation,
           Vector3 pivot)
        {
            if (UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice == null)
                return false;

            m_MatrixStack.TranslateLocal(translation);
            m_MatrixStack.TranslateLocal(pivot);
            m_MatrixStack.MultMatrixLocal(rotation);
            m_MatrixStack.ScaleLocal(scale);
            m_MatrixStack.TranslateLocal(-pivot);

            UWB_XNAGraphicsDevice.m_TheAPI.WorldMatrix = m_MatrixStack.Top();
            return true;
        }

        public override bool pushModelTransform()
        {
            if (UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice == null)
                return false;

            m_MatrixStack.Push();
            UWB_XNAGraphicsDevice.m_TheAPI.WorldMatrix = m_MatrixStack.Top();
            return true;
        }

        public override bool popModelTransform()
        {
            if (UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice == null)
                return false;

            m_MatrixStack.Pop();
            UWB_XNAGraphicsDevice.m_TheAPI.WorldMatrix = m_MatrixStack.Top();
            return true;
        }

        public override bool initializeModelTransform()
        {
            if (UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice == null)
                return false;

            m_MatrixStack.LoadIdentity();
            UWB_XNAGraphicsDevice.m_TheAPI.WorldMatrix = m_MatrixStack.Top();
            return true;
        }
        public bool TransformPoint(ref Vector3 point)
        {
            Vector3 pt = new Vector3(point.X, point.Y, point.Z);

            Matrix m = m_MatrixStack.Top();
            point.X = pt.X * m.M11 + pt.Y * m.M21 + pt.Z * m.M31 + m.M41;
            point.Y = pt.X * m.M12 + pt.Y * m.M22 + pt.Z * m.M32 + m.M42;
            point.Z = pt.X * m.M13 + pt.Y * m.M23 + pt.Z * m.M33 + m.M43;
            return true;
        }

        public override bool EnableBlending(bool on)
        {
            GraphicsDevice d = UWB_XNAGraphicsDevice.m_TheAPI.GraphicsDevice;
            if (d != null)
            {
                m_bBlendingEnabled = on;

                if (on)
                {
                    d.BlendState = BlendState.AlphaBlend;
                    // Same as doing the following ...
                    // d.BlendState.AlphaBlendFunction = BlendFunction.Add;
                    // d.BlendState.AlphaSourceBlend = Blend.SourceAlpha;
                    // d.BlendState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
                }
                else
                {
                    d.BlendState = BlendState.Opaque;
                }
            }
            return true;
        }

        // Pointless funtion at the moment!!
        public override bool EnableTexture(bool on)
        {
            m_bTexturingEnabled = on;
            return !on;
        }


        // Pointless funtion at the moment!!
        public override bool EnableLighting(bool on)
        {
            m_bLightingEnabled = on;
            return !on;
        }

    }
}
