using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using System.Windows.Forms;

namespace UWBGL_XNA_Lib
{
    public class RGraphicsDeviceService : IGraphicsDeviceService
    {
        private GraphicsDevice m_GraphicsDevice;

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        public GraphicsDevice GraphicsDevice
        {
            get { return this.m_GraphicsDevice; }
            set { m_GraphicsDevice = value; }
        }
    }

    public class RGameView : IServiceProvider
    {
        public RGraphicsDeviceService m_graphicsDeviceService;

        public void SetService(RGraphicsDeviceService s)
        {
            m_graphicsDeviceService = s;
        }

        public object GetService(System.Type serviceType)
        {
            return this.m_graphicsDeviceService;
        }
    }

    public class UWB_XNAGraphicsDevice
    {
        private GraphicsDevice m_GraphicsDevice;
        private RasterizerState m_DefaultState;

        public static UWB_XNAGraphicsDevice m_TheAPI = new UWB_XNAGraphicsDevice();
        public ContentManager mResources;
        private RGameView mService;
        
        public UWB_XNAEffect m_LightingEffect;
        private UWB_XNALightManager m_LightManager;

        private UWB_XNAGraphicsDevice()
        {
        }

        public UWB_XNAEffect LightingEffect
        {
            get
            {
                return m_LightingEffect;
            }
        }

        public UWB_XNALightManager LightManager
        {
            get
            {
                return m_LightManager;
            }
        }


        public bool CreateGraphicsContext(IntPtr hwnd, ref PresentationParameters pp)
        {
            //define presentation parameters
            pp = new PresentationParameters();
            pp.BackBufferFormat = SurfaceFormat.Color;
            pp.DeviceWindowHandle = hwnd;
            pp.IsFullScreen = false;
            pp.BackBufferHeight = Control.FromHandle(hwnd).Height;
            pp.BackBufferWidth = Control.FromHandle(hwnd).Width;
            //Initialize Z-buffer - need this for winform
            pp.DepthStencilFormat = DepthFormat.Depth24;

            if (m_GraphicsDevice != null)
                return false;
            //create an XNA graphics device
            m_GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, 
                    GraphicsProfile.HiDef,
                    pp);

            m_DefaultState = new RasterizerState();
            m_DefaultState.CullMode = CullMode.None;
            m_DefaultState.FillMode = FillMode.Solid;
            m_GraphicsDevice.RasterizerState = m_DefaultState;

            //check if graphics device was created
            Debug.Assert(m_GraphicsDevice != null, "XNA Graphics Device did not Initialize");

            m_GraphicsDevice.Reset();
            m_GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            RGraphicsDeviceService gS = new RGraphicsDeviceService();
            gS.GraphicsDevice = m_GraphicsDevice;
            mService = new RGameView();
            mService.SetService(gS);
            mResources = new ContentManager(mService);
            mResources.RootDirectory = "./Content/";

            Initialize();

            return true;
        }

        public void Initialize()
        {
            m_LightingEffect = new UWB_XNAEffect(mResources.Load<Effect>("Effects/UWB_LightingEffect"));

            m_LightManager = new UWB_XNALightManager(m_LightingEffect);


            m_GraphicsDevice.RasterizerState = m_DefaultState;
            m_GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            m_LightingEffect.World = Matrix.Identity;
            m_LightingEffect.View = Matrix.Identity;
            m_LightingEffect.Projection = Matrix.Identity;
        }

        public void Clear(Color clearColor)
        {
            m_GraphicsDevice.Clear(clearColor);
        }

        public void Clear()
        {
            Clear(new Color(new Vector4(.8f, .8f, .8f, 1f)));
        }

        public bool BeginScene(PresentationParameters pp)
        {
            if (m_GraphicsDevice == null)
                return false;
            m_GraphicsDevice.Reset(pp);
            m_GraphicsDevice.RasterizerState = m_DefaultState;
            m_GraphicsDevice.Clear(new Color(new Vector4(.8f, .8f, .8f, 0)));

            return true;
        }

        public bool EndSceneAndShow(PresentationParameters pp)
        {
            if (m_GraphicsDevice == null)
                return false;

            m_GraphicsDevice.Present();

            return true;
        }


        public Matrix WorldMatrix
        {
            set
            {
                m_LightingEffect.World = value;
            }
        }

        public Matrix ViewMatrix
        {
            set
            {
                m_LightingEffect.View = value;
            }
        }

        public Matrix ProjectionMatrix
        {
            set
            {
                m_LightingEffect.Projection = value;
            }
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return m_GraphicsDevice; }
        }

        public Viewport Viewport
        {
            set { m_GraphicsDevice.Viewport = value; }
            get { return m_GraphicsDevice.Viewport; }
        }

        

        public void ShutDown()
        {
            //release all of the textures/meshes
            mResources.Dispose();
        }

        /*
        public bool ActivateTexture(string texName)
        {
            Texture2D t = RetrieveTexture(texName);

            if (t != null) 
            {
                m_BasicEffect.Texture = t;
                m_BasicEffect.TextureEnabled = true;
                m_BasicEffect.VertexColorEnabled = false;
            }
            else
            {
                //texture load failed - disable texturing
                m_BasicEffect.Texture = null;
                m_BasicEffect.TextureEnabled = false;
                m_BasicEffect.VertexColorEnabled = true;
            }
            
            m_BasicEffect.CommitChanges();
            return true;
        }
        

        public void DeactivateTexture()
        {
            m_BasicEffect.TextureEnabled = false;
            m_BasicEffect.Texture = null;
            m_BasicEffect.VertexColorEnabled = true;
            m_BasicEffect.CommitChanges();
        }
        */

        public Texture2D RetrieveTexture(string texName)
        {
            Texture2D t = null;

            if (texName != null && texName != "")
            {
                try
                {
                    t = mResources.Load<Texture2D>(texName);

                }
                catch (Exception e)
                {
                    //texture load failed
                    UWBGL_XNA_Lib.UWB_Utility.echoToStatusArea((e.Message));
                }
            }
            return t;
        }

    }
}
