using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RayTracer_552
{
    /// <summary>
    /// A Rectangle
    /// </summary>
    public partial class RTTriangle
    {
        private Vector3[] mNormalAtVertices = null;     // may not be there (only present for triangles created from a mesh)

        /// <summary>
        /// Constructs from given positions, normal, and uv then intialize for intersection computation.
        /// </summary>
        /// <param name="parser"></param>
        protected RTTriangle(Vector3[] vertices, Vector3[] normalAtVertex, Vector2[] uvAtVertex, int material)
        {
            mType = RTGeometryType.Triangle;
            mMaterialIndex = material;
            mVertices = (Vector3[])vertices.Clone();
            if (null != normalAtVertex)
                mNormalAtVertices = (Vector3[])normalAtVertex.Clone();
            mVertexUV = (Vector2[])uvAtVertex.Clone();

            InitializeTriangle();
        }

        static public void ParseMeshForTriangles(CommandFileParser parser, ContentManager meshLoader, SceneDatabase sceneDatabase)
        {
            String meshFileName = null;
            int material = 0;

            // has xform?
            bool hasTransform = false;
            Matrix xform = Matrix.Identity;
            Matrix invT = Matrix.Identity;

            #region Parse the command file
            parser.ParserRead();
            while (!parser.IsEndElement("mesh"))
            {
                if (parser.IsElement() && (!parser.IsElement("mesh")))
                {
                    if (parser.IsElement("filename"))
                    {
                        meshFileName = parser.ReadString();
                    }
                    else if (parser.IsElement("xform"))
                    {
                        hasTransform = true;
                        xform = ParseTransform(parser);
                    }
                    else if (parser.IsElement("material"))
                        material = parser.ReadInt();
                    else
                        parser.ParserError("mesh");
                }
                else
                    parser.ParserRead();
            }
            #endregion 

            if (null == meshFileName)
            {
                parser.ParserError("No Mesh filename!");
                return;
            }

            if (hasTransform)
                invT = Matrix.Transpose(Matrix.Invert(xform));

            Model model = meshLoader.Load<Model>(meshFileName);
            Byte[] localVertexBuffer = null;
            Int32[] localIndexBuffer = null;
            Vector3[] vertexPosition = null;
            Vector3[] normalAtVertex = null;
            Vector2[] uvAtVertex = null;
            bool hasPosition = false;
            bool hasNormal = false;
            bool hasUV = false;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    int numIndices = part.IndexBuffer.IndexCount;  // total number of localIndexBuffer in the buffer
                    int numVertices = part.VertexBuffer.VertexCount; // total number of vertices in the buffer

                    #region Only need to load/translate these buffers once!
                    // *** REALLY? ***
                    if (null == localIndexBuffer)
                    {
                        vertexPosition = new Vector3[numVertices];
                        normalAtVertex = new Vector3[numVertices];
                        uvAtVertex = new Vector2[numVertices];

                        localIndexBuffer = LoadIndexBuffer(part);

                        int numBytes = numVertices * part.VertexBuffer.VertexDeclaration.VertexStride;
                        localVertexBuffer = new Byte[numBytes];
                        part.VertexBuffer.GetData<Byte>(localVertexBuffer);

                        VertexElement[] vertexElements = part.VertexBuffer.VertexDeclaration.GetVertexElements();

                        int bufferOffset = 0;
                        #region parse each vertex
                        // now get all the vertices
                        for (int vertCount = 0; vertCount < numVertices; vertCount++)
                        {   // parse through the elements
                            bufferOffset = vertCount * part.VertexBuffer.VertexDeclaration.VertexStride;

                            hasUV = false; // do this for each vertex
                            for (int e = 0; e < vertexElements.Length; e++)
                            {
                                switch (vertexElements[e].VertexElementUsage)
                                {
                                    case VertexElementUsage.Position:
                                        hasPosition = true;
                                        // Vertex position
                                        int vertexByteOffset = bufferOffset + vertexElements[e].Offset;
                                        vertexPosition[vertCount].X = BitConverter.ToSingle(localVertexBuffer, vertexByteOffset); vertexByteOffset += 4;
                                        vertexPosition[vertCount].Y = BitConverter.ToSingle(localVertexBuffer, vertexByteOffset); vertexByteOffset += 4;
                                        vertexPosition[vertCount].Z = BitConverter.ToSingle(localVertexBuffer, vertexByteOffset);
                                        break;
                                    case VertexElementUsage.Normal:
                                        hasNormal = true;
                                        int normalByteOffset = bufferOffset + vertexElements[e].Offset;
                                        normalAtVertex[vertCount].X = BitConverter.ToSingle(localVertexBuffer, normalByteOffset); normalByteOffset += 4;
                                        normalAtVertex[vertCount].Y = BitConverter.ToSingle(localVertexBuffer, normalByteOffset); normalByteOffset += 4;
                                        normalAtVertex[vertCount].Z = BitConverter.ToSingle(localVertexBuffer, normalByteOffset);
                                        break;
                                    case VertexElementUsage.TextureCoordinate:
                                        if (!hasUV)
                                        {  // if more than one defined, will only use the first one
                                            hasUV = true;
                                            int uvByteOffset = bufferOffset + vertexElements[e].Offset;
                                            uvAtVertex[vertCount].X = BitConverter.ToSingle(localVertexBuffer, uvByteOffset); uvByteOffset += 4;
                                            uvAtVertex[vertCount].Y = BitConverter.ToSingle(localVertexBuffer, uvByteOffset);
                                        }
                                        break;
                                    // ignore all other cases
                                } // the switch on usage for parsing
                            } // for of vertexElements
                        } // for each vertex
                        #endregion


                        #region compute the transform for each vertex
                        // now do the transform
                        if (hasTransform)
                        {
                            for (int v = 0; v < numVertices; v++)
                                vertexPosition[v] = Vector3.Transform(vertexPosition[v], xform);
                            if (hasNormal)
                                for (int v = 0; v < numVertices; v++)
                                {
                                    normalAtVertex[v] = Vector3.Transform(normalAtVertex[v], invT);
                                    normalAtVertex[v] = Vector3.Normalize(normalAtVertex[v]);
                                }
                        }
                        #endregion
                    }
                    #endregion 

                    // now start working on this particular part ...
                    int startIndex = part.StartIndex;

                    int vertexOffset = part.VertexOffset;

                    // start from part.StartIndex
                    int numTriangles = part.PrimitiveCount;

                    #region create the triangles
                    // now all vertice are stored.
                    // let's create the Triangles
                    for (int nthT = 0; nthT < numTriangles; nthT++)
                    {
                        Vector3[] vertices = new Vector3[3];
                        Vector2[] uv = new Vector2[3];
                        Vector3[] normals = null;
                        if (hasNormal)
                            normals = new Vector3[3];

                        #region copy vertex info
                        int indexOffset = startIndex + (nthT * 3);
                        for (int i = 0; i < 3; i++)
                        {
                            int index = vertexOffset + localIndexBuffer[indexOffset + i];
                            vertices[i] = vertexPosition[index];
                            uv[i] = uvAtVertex[index];
                            if (hasNormal)
                                normals[i] = normalAtVertex[index];
                        }
                        #endregion 

                        // now create the new triangle
                        // watch out for bad triangles!!
                        if (hasPosition)
                        {
                            if (!hasNormal)
                                normalAtVertex = null;
                            Vector3 aVec = vertices[1] - vertices[0];
                            if (aVec.LengthSquared() > float.Epsilon)
                            {
                                Vector3 bVec = vertices[2] - vertices[0];
                                if (bVec.LengthSquared() > float.Epsilon)
                                {
                                    RTTriangle t = new RTTriangle(vertices, normals, uv, material);
                                    sceneDatabase.AddGeom(t);
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
        }
    
    static private Int32[] LoadIndexBuffer(ModelMeshPart thisPart) {
            int numIndices = thisPart.IndexBuffer.IndexCount;       
            Int32[] localIndexBuffer = new Int32[numIndices];
            if (thisPart.IndexBuffer.IndexElementSize == IndexElementSize.SixteenBits)
            {
                        Int16[] tmpI = new Int16[numIndices];
                        thisPart.IndexBuffer.GetData<Int16>(tmpI);
                        for (int i = 0; i < numIndices; i++)
                            localIndexBuffer[i] = (Int32)tmpI[i];
            }
            else
            {
                        thisPart.IndexBuffer.GetData<Int32>(localIndexBuffer);
            }
        return localIndexBuffer;
    }


        }
    
}