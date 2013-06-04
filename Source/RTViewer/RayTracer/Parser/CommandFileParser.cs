using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RayTracer_552.RTSupport;

namespace RayTracer_552
{

    public class CommandFileParser
    {
        private String mFullPath;
        private bool mHasError = false;

        private XmlTextReader mParser = null;
        private System.Windows.Forms.TextBox mStatusArea;
        
        public CommandFileParser(String cmdFile, ContentManager meshLoader, System.Windows.Forms.TextBox statusArea, RTCore rt, SceneDatabase scene)
        {
            mStatusArea = statusArea;

            mFullPath = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(cmdFile));

            mParser = new XmlTextReader(cmdFile);
            mParser.WhitespaceHandling = WhitespaceHandling.None;

            ParserRead();
            while (!IsEndElement("RayTracer_552"))
            {
                if (IsElement() && (!IsElement("RayTracer_552")) ) 
                {
                    if (IsElement("camera"))
                    {
                        RTCamera c = new RTCamera(this);
                        rt.SetCamera(c);
                        ParserRead();
                    }
                    else if (IsElement("sphere"))
                    {
                        RTSphere s = new RTSphere(this);
                        scene.AddGeom(s);
                        ParserRead();
                    }
                    else if (IsElement("rectangle"))
                    {
                        RTRectangle r = new RTRectangle(this);
                        scene.AddGeom(r);
                        ParserRead();
                    }
                    else if (IsElement("triangle"))
                    {
                        RTTriangle t = new RTTriangle(this);
                        scene.AddGeom(t);
                        ParserRead();
                    }
                    else if (IsElement("mesh"))
                    {
                        RTTriangle.ParseMeshForTriangles(this, meshLoader, scene);
                        ParserRead();
                    }
                    else if (IsElement("imagespec"))
                    {
                        ImageSpec s = new ImageSpec(this);
                        rt.SetImageSpec(s);
                        ParserRead();
                    }
                    else if (IsElement("rtspec"))
                    {
                        rt.Parse(this);
                        ParserRead();
                    }
                    else if (IsElement("material"))
                    {
                        RTMaterial m = new RTMaterial(this);
                        scene.AddMaterial(m);
                        ParserRead();
                    }
                    else if (IsElement("light"))
                    {
                        RTLight l = new RTLight(this);
                        scene.AddLight(l);
                        ParserRead();
                    }
                    else if (IsElement("texture"))
                    {
                        RTTexture t = new RTTexture(this);
                        scene.AddTexture(t);
                        ParserRead();
                    }
                    else
                        ParserError("Main Parser:");
                }
                else
                    ParserRead();
            }
            mParser.Close();

            if (!mHasError)
                mStatusArea.Text = "Parsing Completed!";
        }

        public bool IsElement()
        {
            return (mParser.NodeType == XmlNodeType.Element);
        }

        public bool IsElement(String elm)
        {
            return (mParser.NodeType == XmlNodeType.Element && mParser.Name.Equals(elm));
        }


        public bool IsEndElement()
        {
            return (mParser.NodeType == XmlNodeType.EndElement);
        }
        public bool IsEndElement(String tag)
        {
            return ((mParser.NodeType == XmlNodeType.EndElement) && (mParser.Name.Equals(tag)));
        }

        public bool ParserRead()
        {
            bool hasMore = mParser.Read();
            while (hasMore && (!IsElement()) && (!IsEndElement()))
                hasMore = mParser.Read();
            return hasMore;
        }

        public Vector3 ReadVector3()
        {
            float[] a = new float[3];
            a = (float[]) mParser.ReadElementContentAs(typeof(float[]), null);
            return new Vector3(a[0], a[1], a[2]);
        }

        public Vector2 ReadVector2()
        {
            float[] a = new float[2];
            a = (float[])mParser.ReadElementContentAs(typeof(float[]), null);
            return new Vector2(a[0], a[1]);
        }

        public float ReadFloat()
        {
            return mParser.ReadElementContentAsFloat();
        }

        public int ReadInt()
        {
            return mParser.ReadElementContentAsInt();
        }

        public bool ReadBool()
        {
            return (mParser.ReadElementContentAsInt() == 1);
        }

        public String ReadString()
        {
            String s = mParser.ReadElementContentAsString();
            return s.Trim();
        }
       

        public void ParserError(String msg)
        {
            mHasError = true;

            if (mParser.NodeType == XmlNodeType.Element)
                mStatusArea.Text = "Unknown element: " + msg + " NodeName(" + mParser.Name + ") Value=(" + mParser.Value + ")";
            else
                mStatusArea.Text = "Parser error: " + msg;
        }

        public String FullPath() { return mFullPath; }
    }
}