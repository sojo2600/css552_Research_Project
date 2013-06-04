using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;
#if XBOX360
#else
using System.Drawing;
using System.Windows.Forms;
#endif

namespace XNALib
{
    public class RGraphicsDeviceService : IGraphicsDeviceService
    {
        private GraphicsDevice m_graphicsDevice;

        public event EventHandler DeviceCreated;
        public event EventHandler DeviceDisposing;
        public event EventHandler DeviceReset;
        public event EventHandler DeviceResetting;

        public GraphicsDevice GraphicsDevice
        {
            get { return this.m_graphicsDevice; }
            set { m_graphicsDevice = value; }
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
	//This is our custome vertex structure XNA likes to recieve 
	//arrays of structs with vertex information. For simple, unlit
	//drawing; a vertex and color at the vertex will suffice.
	public struct DeviceVertexFormat
	{
		private Vector3 m_Position;
		private Color m_Color;
        private Vector2 m_UV;

		//Format in which this type of vertex will be passed to the graphics hardware
		public static VertexElement[] VertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, 
                              VertexElementUsage.Position, 0),
            new VertexElement(0, sizeof(float)*3, VertexElementFormat.Color, VertexElementMethod.Default, 
                              VertexElementUsage.Color, 0),
            new VertexElement(0, (sizeof(float)*2)+sizeof(uint), VertexElementFormat.Vector2, VertexElementMethod.Default,
                              VertexElementUsage.TextureCoordinate, 0)
        };

		//m_Position accessor and modifier
		public Vector3 Position
		{
			get
			{
				return m_Position;
			}
			set
			{
				m_Position = value;
			}
		}

		//m_Color accessor and modifier
		public Color Color
		{
			get
			{
				return m_Color;
			}
			set
			{
				m_Color = value;
			}
		}
        
        public float U
        {
            get { return m_UV.X; }
            set { m_UV.X = value; }
        }


        public float V
        {
            get { return m_UV.Y; }
            set { m_UV.Y = value; }
        }
	};

	public class UWB_XNAGraphicsSystem
	{
		private EffectPool m_EffectPool;
		public BasicEffect m_BasicEffect;
		private GraphicsDevice m_XNA_GraphicsDevice;
        public ContentManager resources;
		public static UWB_XNAGraphicsSystem m_TheAPI = new UWB_XNAGraphicsSystem();
        RGameView serv;

        public BasicEffect MyEffect
        {
            get
            {
                return m_BasicEffect;
            }
            set
            {
                m_BasicEffect = value;
            }
        }

        public void SetService(RGameView SP)
        {
            serv = SP;
            resources = new ContentManager(serv);
            resources.RootDirectory = "Content";
        }
		private UWB_XNAGraphicsSystem()
		{
            
		}

#if XBOX360
        public void CreateGraphicsContext(int deviceWidth, int deviceHeight)
        {
            GraphicsDeviceManager m_Manager = new GraphicsDeviceManager(game);
            m_Manager.PreferredBackBufferWidth = deviceWidth;
            m_Manager.PreferredBackBufferHeight = deviceHeight;

            if (m_XNA_GraphicsDevice != null)
                return false;
            //create an XNA graphics device
            m_XNA_GraphicsDevice = m_Manager.GraphicsDevice();

            //check if graphics device was created
            Debug.Assert(m_XNA_GraphicsDevice != null, "XNA Graphics Device did not Initialize");

            m_EffectPool = new EffectPool();
            m_BasicEffect = new BasicEffect(m_XNA_GraphicsDevice, m_EffectPool);

            //set the model matrix to identity
            m_BasicEffect.World = Matrix.Identity;
            m_BasicEffect.View = Matrix.Identity;
            m_BasicEffect.Projection = Matrix.Identity;
            m_XNA_GraphicsDevice.RenderState.DepthBufferEnable = true;
            m_XNA_GraphicsDevice.RenderState.DepthBufferWriteEnable = true;

            return true;
        }
#else

		public bool CreateGraphicsContext(IntPtr hwnd, ref PresentationParameters pp)
		{
			//define presentation parameters
			pp = new PresentationParameters();
			pp.BackBufferCount = 1;
			pp.BackBufferFormat = SurfaceFormat.Unknown;
			pp.DeviceWindowHandle = hwnd;
			pp.IsFullScreen = false;
			pp.SwapEffect = SwapEffect.Discard;
			pp.BackBufferHeight = Control.FromHandle(hwnd).Height;
			pp.BackBufferWidth = Control.FromHandle(hwnd).Width;
            //Initialize Z-buffer - need this for winform
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.Depth24;

			if (m_XNA_GraphicsDevice != null)
				return false;
			//create an XNA graphics device
			m_XNA_GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, DeviceType.Hardware,
				hwnd, pp);

			//check if graphics device was created
			Debug.Assert(m_XNA_GraphicsDevice != null, "XNA Graphics Device did not Initialize");

			m_EffectPool = new EffectPool();
			m_BasicEffect = new BasicEffect(m_XNA_GraphicsDevice, m_EffectPool);

			//set the model matrix to identity
			m_BasicEffect.World = Matrix.Identity;
			m_BasicEffect.View = Matrix.Identity;
			m_BasicEffect.Projection = Matrix.Identity;
            m_XNA_GraphicsDevice.RenderState.DepthBufferEnable = true;
            m_XNA_GraphicsDevice.RenderState.DepthBufferWriteEnable = true;

			return true;
		}

#endif

        public GraphicsDevice GetActiveDevice()
		{
			return m_XNA_GraphicsDevice;
		}

		public bool BeginScene(PresentationParameters pp)
		{
			if (m_XNA_GraphicsDevice == null)
				return false;
            
			m_XNA_GraphicsDevice.Reset(pp);
           
            m_XNA_GraphicsDevice.VertexDeclaration = new VertexDeclaration(m_XNA_GraphicsDevice, DeviceVertexFormat.VertexElements);
			m_XNA_GraphicsDevice.RenderState.CullMode = CullMode.None;
			m_XNA_GraphicsDevice.RenderState.FillMode = FillMode.Solid;
            m_XNA_GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.LightGray, 1.0f, 0);  

			m_BasicEffect.Begin();
			m_BasicEffect.CurrentTechnique.Passes[0].Begin();

			return true;
		}

		public bool EndSceneAndShow(PresentationParameters pp)
		{
			if (m_XNA_GraphicsDevice == null)
				return false;

			m_BasicEffect.CurrentTechnique.Passes[0].End();
			m_BasicEffect.End();
			m_XNA_GraphicsDevice.Present();

			return true;
		}

		public void setViewMatrix(Matrix matrix)
		{
			m_BasicEffect.View = matrix;
			m_BasicEffect.CommitChanges();
		}

		public void setWorldMatrix(Matrix matrix)
		{
			m_BasicEffect.World = matrix;
			m_BasicEffect.CommitChanges();
		}
        public void setProjectionMatrix(Matrix matrix)
        {
            m_BasicEffect.Projection = matrix;
            m_BasicEffect.CommitChanges();
        }
        public void ShutDown()
        {
            //release all of the textures/meshes
            resources.Dispose();
        }

        public bool ActivateTexture(string texName)
        {
            Texture2D t = null;
            try
            {
                t = resources.Load<Texture2D>(texName);
                m_BasicEffect.Texture = t;
                m_BasicEffect.TextureEnabled = true;
                m_BasicEffect.VertexColorEnabled = false;
            }
            catch (Exception e)
            {
                //texture load failed - disable texturing
                m_BasicEffect.Texture = null;
                m_BasicEffect.TextureEnabled = false;
                m_BasicEffect.VertexColorEnabled = true;
                XNALib.UWB_Utility.echoToStatusArea((e.Message));
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
	}
}