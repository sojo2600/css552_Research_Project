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
		/// size of the world
		private UWB_BoundingBox m_WorldBounds = new UWB_BoundingBox();

		private UWB_SceneNode m_RootNode;
        private UWB_SceneNode m_SceneDatabase;

        private bool mDrawDB = true;

		private UWB_XNADrawHelper m_DrawHelper = new UWB_XNADrawHelper();


        public void SetDrawDB(bool on) { mDrawDB = on; }

		internal RTModelViewer()
		{
			m_DrawHelper.initializeModelTransform();

			m_RootNode = new UWB_SceneNode("Scene");
            NewSceneDatabase();
            mCamera = new UWB_SceneNode();

            UWB_SceneNode pAxisNode = new UWB_SceneNode("axis frame");
            UWB_XFormInfo xf = pAxisNode.getXFormInfo();
            xf.SetScale(new Vector3(3.0f, 3.0f, 3.0f));
            pAxisNode.setXFormInfo(xf);
            UWB_XNAPrimitiveMeshAxis pMeshAxis = new UWB_XNAPrimitiveMeshAxis();
            //pMeshAxis.EnableBlending(true);
            pAxisNode.setPrimitive(pMeshAxis);
            m_RootNode.insertChildNode(pAxisNode);

			m_WorldBounds.setCorners( new Vector3(-100,-100,-100), new Vector3(100,100,100) );
		}

		///
		/// Draw all of the geometry that is part of this model.
		internal void DrawModel(bool drawCamera)
		{
			eLevelofDetail lod = m_DrawHelper.getLod();

			m_DrawHelper.initializeModelTransform();
	
			m_DrawHelper.pushModelTransform();
			    m_RootNode.Draw(lod, m_DrawHelper);

                if (!drawCamera) // this is the preview window
                    m_SceneDatabase.Draw(lod, m_DrawHelper);
                else 
                    if (mDrawDB) m_SceneDatabase.Draw(lod, m_DrawHelper);

                if (drawCamera)
                {
                    mCamera.Draw(lod, m_DrawHelper);
                    if (null != mDebugInfo)
                       mDebugInfo.Draw(lod, m_DrawHelper);
                }
			m_DrawHelper.popModelTransform();

		}

        ///
        /// Get the bounds that this model defines
        internal UWB_BoundingBox GetWorldBounds()
        {
            return m_WorldBounds;
        }

        internal Rectangle WorldBounds
        {
            get { return m_WorldBounds.Rectangle; }
        }
	}
}