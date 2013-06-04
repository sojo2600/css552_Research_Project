using System;
using System.Collections.Generic;
using System.Text;
using UWBGL_XNA_Lib;
using Microsoft.Xna.Framework;


using RayTracer_552;

namespace RTViewer
{
    public partial class RTModelViewer
    {
        private UWB_SceneNode mCamera = null;
        private Vector3 mCameraPosition = new Vector3();
        private UWB_PrimitiveList mCameraPrimitives = null;

        internal void AddRTScene(RayTracer_552.RTCamera c, RayTracer_552.SceneDatabase rtScene)
        {
            UWB_Primitive prim;
            
            NewSceneDatabase();
            SceneResource<RTGeometry> allGeom = rtScene.GetAllGeom();
            for (int i = 0; i < allGeom.Count; i++)
            {
                RTGeometry g = (RTGeometry)allGeom.ResourceLookup(i);
                switch (g.GeomType()) {
                    case RTGeometry.RTGeometryType.Sphere:
                        RTSphere s = (RTSphere)g;
                        prim = CreateSphereMesh();
                        SetMeshMaterial(prim, rtScene.GetMaterial(s.GetMaterialIndex()));
                        float scale =s.Radius/2f;
                        CreateNode(s.Center, scale, scale, scale, prim);
                        break;
                    case RTGeometry.RTGeometryType.Rectangle:
                        RTRectangle r = (RTRectangle) g;
                        prim = CreateRectangle(r);
                        SetMeshMaterial(prim, rtScene.GetMaterial(r.GetMaterialIndex()));
                        UWB_SceneNode node = CreateNode(r.GetCenter(), r.GetUSize()/2f, 1f, r.GetVSize()/2f, prim);
                        // now rotate the y-vector of node to point towards r.Normal;
                        float dot = (float)Math.Abs(Vector3.Dot(Vector3.UnitY, r.GetNormal()));                        
                        if (dot < 0.9999f)
                        {
                            float angle = (float)Math.Acos(dot);                                                
                            Vector3 axis = Vector3.Cross(Vector3.UnitY, r.GetNormal());
                            axis.Normalize();
                            Quaternion q = Quaternion.CreateFromAxisAngle(axis, angle);
                            UWB_XFormInfo xf = node.getXFormInfo();
                            xf.SetRotationQuat(q);
                            node.setXFormInfo(xf);
                        }
                        break;
                    case RTGeometry.RTGeometryType.Triangle:
                        RTTriangle t = (RTTriangle)g;
                        Vector3[] v = t.GetVertices();
                        prim = new UWB_PrimitiveTriangle(v[0], v[1], v[2]);
                        prim.EnableLighting(true);
                        prim.EnableTexturing(false);
                        SetMeshMaterial(prim, rtScene.GetMaterial(t.GetMaterialIndex()));
                        CreateNode(Vector3.Zero, 1f, 1f, 1f, prim);
                        break;
                    }
            }
            AddCamera(c);
            AddLights(rtScene);

            // to show ray list
            mShownRayX = mShownRayY = 0;
            mRaysToShow = new UWB_PrimitiveList();

            mDebugInfo = new UWB_SceneNode();
            mDebugInfo.setPrimitive(mRaysToShow);
            mDebugInfo.insertChildNode(mPixelsToShow.GetAllPixels());
            mDebugInfo.insertChildNode(mPixelInWorld.GetAllPixels());
        }


        private UWB_Primitive CreateSphereMesh()
        {
            UWB_XNAPrimitiveMesh m = new UWB_XNAPrimitiveMesh("sphere");
            m.EnableTexturing(false);
            m.EnableLighting(true);
            return m;
        }

        private UWB_Primitive CreateRectangle(RTRectangle r)
        {
            UWB_XNAPrimitiveMesh m = new UWB_XNAPrimitiveMesh("HiResFloor");
            m.EnableTexturing(false);
            m.EnableLighting(true);
            return m;
        }

        private void SetMeshMaterial(UWB_Primitive prim, RayTracer_552.RTMaterial mat)
        {
            prim.EnableLighting(true);
            prim.setShadeMode(eShadeMode.smGouraud);
            prim.Material.Emissive = new Vector4(Vector3.Zero, 1f);
            prim.Material.Ambient = new Vector4(mat.GetAmbientColor, 1f);
            prim.Material.Diffuse = new Vector4(mat.GetDiffuseColor, 1f);
            prim.Material.Specular = new Vector4(mat.GetSpecularColor, 1f);
            prim.Material.Power = mat.GetN;
        }

        private UWB_SceneNode CreateNode(Vector3 at, float sx, float sy, float sz, UWB_Primitive prim)
        {
            UWB_SceneNode pNode = new UWB_SceneNode();
            pNode.setPrimitive(prim);
            UWB_XFormInfo xf = pNode.getXFormInfo();
            xf.SetTranslation(at);
            xf.SetScale(new Vector3(sx, sy, sz));
            pNode.setXFormInfo(xf);
            m_SceneDatabase.insertChildNode(pNode);
            return pNode;
        }

        private void AddLights(RayTracer_552.SceneDatabase rtScene)
        {
            UWB_XNAGraphicsDevice.m_TheAPI.LightManager.ResetAllLights();
            for (int l = 0; l < rtScene.GetNumLights(); l++)
            {
                RTLight lgt = rtScene.GetLight(l);
                Vector4 useColor = new Vector4(lgt.GetColor(new Vector3(0,0,0)),1f);
                UWB_XNALight theLight = null;

                if (lgt.GetLightSourceType() == RTLightType.RTLightSourceType.RTLightSourceTypeDirection)
                {
                    theLight = UWB_XNAGraphicsDevice.m_TheAPI.LightManager.CreateDirectionalLight();
                }
                else if (lgt.GetLightSourceType() == RTLightType.RTLightSourceType.RTLightSourceTypeSpot)
                {
                    theLight = UWB_XNAGraphicsDevice.m_TheAPI.LightManager.CreateSpotLight();
                }
                else
                {
                    theLight = UWB_XNAGraphicsDevice.m_TheAPI.LightManager.CreatePointLight();
                }
                
                theLight.Ambient = Vector4.Zero;
                theLight.Diffuse = useColor;
                theLight.Specular = useColor;
                theLight.Position = lgt.GetLightPosition();
                theLight.Direction = -lgt.GetNormalizedDirection(Vector3.Zero);
                theLight.Color = useColor;
                theLight.Attenuation = new Vector3(1f, 0.0f, 0.0f);
                theLight.Range = 10000f;
                theLight.SwitchOnLight();

                UWB_Primitive prim = CreateSphereMesh();
                SetMeshMaterial(prim, rtScene.GetMaterial(0));
                float scale = 0.25f;
                CreateNode(lgt.GetLightPosition(), scale, scale, scale, prim);
            }
        }


        private void AddCamera(RayTracer_552.RTCamera c)
        {
            // Look at position
            UWB_SceneNode atN = new UWB_SceneNode();
            UWB_XNAPrimitiveMesh at = new UWB_XNAPrimitiveMesh("sphere");
            at.Material.Diffuse = new Vector4(0.8f, 0.1f, 0.1f, 1.0f);
            atN.setPrimitive(at);
            UWB_XFormInfo atxf = atN.getXFormInfo();
            atxf.SetTranslation(c.AtPosition);
            atxf.SetScale(new Vector3(0.3f, 0.3f, 0.3f));
            atN.setXFormInfo(atxf);

            // Eye position
            UWB_SceneNode eyeN = new UWB_SceneNode();
            UWB_XNAPrimitiveMesh eye = new UWB_XNAPrimitiveMesh("cone");
            eyeN.setPrimitive(eye);
            UWB_XFormInfo eyexf = eyeN.getXFormInfo();
            eyexf.SetTranslation(c.EyePosition);
            mCameraPosition = c.EyePosition;
            Vector3 init = new Vector3(0, 0, 1); // initial cone orientation
            Vector3 final = c.AtPosition - c.EyePosition;
            final = Vector3.Normalize(final);
            float dot = Vector3.Dot(init, final);
            if (Math.Abs(dot) < 0.9999)
            {
                float angle = (float)Math.Acos(dot);
                Vector3 axis = Vector3.Cross(init, final);
                axis = Vector3.Normalize(axis);
                Quaternion q = Quaternion.CreateFromAxisAngle(axis, angle);
                eyexf.SetRotationQuat(q);
            }
            eyeN.setXFormInfo(eyexf);

            // Lines ...
            UWB_SceneNode lineN = new UWB_SceneNode();
            mCameraPrimitives = new UWB_PrimitiveList();
            lineN.setPrimitive(mCameraPrimitives);
            UWB_PrimitiveLine l = new UWB_PrimitiveLine();
            l.setStartPoint(c.EyePosition.X, c.EyePosition.Y, c.EyePosition.Z);
            l.setEndPoint(c.AtPosition.X, c.AtPosition.Y, c.AtPosition.Z);
            mCameraPrimitives.append(l);

            mCamera.insertChildNode(lineN);
            mCamera.insertChildNode(atN);
            mCamera.insertChildNode(eyeN);
        }

        private void NewSceneDatabase()
        {
            m_SceneDatabase = new UWB_SceneNode();
            mCamera = new UWB_SceneNode();
            mCameraPrimitives = new UWB_PrimitiveList();
        }
        
	}
}
/*
// Setup lights
            UWB_XNAPointLight pointLight = UWB_XNAGraphicsDevice.m_TheAPI.LightManager.CreatePointLight();
            pointLight.Ambient = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            pointLight.Diffuse = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            pointLight.Specular = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            pointLight.Direction = new Vector3(0.0f, -1.0f, 0.0f);
            pointLight.Position = new Vector3(2.0f, 12.0f, 2.0f);
            pointLight.Color = Color.Blue.ToVector4();
            pointLight.Attenuation = new Vector3(1.0f, 0.0f, 0.0f);
            pointLight.SwitchOnLight();

            UWB_XNADirectionalLight dirLight = UWB_XNAGraphicsDevice.m_TheAPI.LightManager.CreateDirectionalLight();
            dirLight.Ambient = new Vector4(0.2f, 0.1f, 0.2f, 1.0f);
            dirLight.Diffuse = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            dirLight.Specular = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            dirLight.Direction = new Vector3(0.0f, -1.0f, 0.0f);
            dirLight.Position = new Vector3(0.0f, 12.0f, 0.0f);
            dirLight.Color = Color.Blue.ToVector4();
            dirLight.SwitchOnLight();

            UWB_XNASpotLight spotLight1 = UWB_XNAGraphicsDevice.m_TheAPI.LightManager.CreateSpotLight();
            spotLight1.Ambient = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            spotLight1.Diffuse = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            spotLight1.Specular = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            spotLight1.Direction = new Vector3(0.0f, -1.0f, 0.0f);
            spotLight1.Position = new Vector3(0.0f, 10.0f, 0.0f);
            spotLight1.Color = Color.Red.ToVector4();
            spotLight1.Attenuation = new Vector3(1.0f, 0.0f, 0.0f);
            spotLight1.Theta = MathHelper.ToRadians(10.0f);
            spotLight1.Phi = MathHelper.ToRadians(20.0f);
            spotLight1.SwitchOnLight();

            UWB_XNASpotLight spotLight2 = UWB_XNAGraphicsDevice.m_TheAPI.LightManager.CreateSpotLight();
            spotLight2.Ambient = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            spotLight2.Diffuse = new Vector4(0.5f, 0.5f, 0.0f, 1.0f);
            spotLight2.Specular = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            spotLight2.Direction = new Vector3(0.0f, -1.0f, 0.0f);
            spotLight2.Position = new Vector3(2.0f, 8.0f, 2.0f);
            spotLight2.Color = Color.Red.ToVector4();
            spotLight2.Attenuation = new Vector3(1.0f, 0.0f, 0.0f);
            spotLight2.Theta = MathHelper.ToRadians(10.0f);
            spotLight2.Phi = MathHelper.ToRadians(20.0f);
            spotLight2.SwitchOnLight();

            UWB_XNASpotLight spotLight3 = UWB_XNAGraphicsDevice.m_TheAPI.LightManager.CreateSpotLight();
            spotLight3.Ambient = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            spotLight3.Diffuse = new Vector4(0.0f, 0.0f, 0.5f, 1.0f);
            spotLight3.Specular = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            spotLight3.Direction = new Vector3(0.0f, -1.0f, 0.0f);
            spotLight3.Position = new Vector3(2.0f, 8.0f, -2.0f);
            spotLight3.Color = Color.Red.ToVector4();
            spotLight3.Attenuation = new Vector3(1.0f, 0.0f, 0.0f);
            spotLight3.Theta = MathHelper.ToRadians(10.0f);
            spotLight3.Phi = MathHelper.ToRadians(20.0f);
            spotLight3.SwitchOnLight();

            UWB_XNASpotLight spotLight4 = UWB_XNAGraphicsDevice.m_TheAPI.LightManager.CreateSpotLight();
            spotLight4.Ambient = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            spotLight4.Diffuse = new Vector4(0.0f, 0.5f, 0.0f, 1.0f);
            spotLight4.Specular = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            spotLight4.Direction = new Vector3(0.0f, -1.0f, 0.0f);
            spotLight4.Position = new Vector3(-2.0f, 8.0f, 2.0f);
            spotLight4.Color = Color.Red.ToVector4();
            spotLight4.Attenuation = new Vector3(1.0f, 0.0f, 0.0f);
            spotLight4.Theta = MathHelper.ToRadians(10.0f);
            spotLight4.Phi = MathHelper.ToRadians(20.0f);
            spotLight4.SwitchOnLight();

            UWB_XNASpotLight spotLight5 = UWB_XNAGraphicsDevice.m_TheAPI.LightManager.CreateSpotLight();
            spotLight5.Ambient = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            spotLight5.Diffuse = new Vector4(0.5f, 0.0f, 0.0f, 1.0f);
            spotLight5.Specular = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            spotLight5.Direction = new Vector3(0.0f, -1.0f, 0.0f);
            spotLight5.Position = new Vector3(-2.0f, 8.0f, -2.0f);
            spotLight5.Color = Color.Red.ToVector4();
            spotLight5.Attenuation = new Vector3(1.0f, 0.0f, 0.0f);
            spotLight5.Theta = MathHelper.ToRadians(10.0f);
            spotLight5.Phi = MathHelper.ToRadians(20.0f);
            spotLight5.SwitchOnLight();
*/