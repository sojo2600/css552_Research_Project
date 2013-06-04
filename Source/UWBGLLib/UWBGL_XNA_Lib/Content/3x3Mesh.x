xof 0303txt 0032
template Vector {
 <3d82ab5e-62da-11cf-ab39-0020af71e433>
 FLOAT x;
 FLOAT y;
 FLOAT z;
}

template MeshFace {
 <3d82ab5f-62da-11cf-ab39-0020af71e433>
 DWORD nFaceVertexIndices;
 array DWORD faceVertexIndices[nFaceVertexIndices];
}

template Mesh {
 <3d82ab44-62da-11cf-ab39-0020af71e433>
 DWORD nVertices;
 array Vector vertices[nVertices];
 DWORD nFaces;
 array MeshFace faces[nFaces];
 [...]
}

template MeshNormals {
 <f6f23f43-7686-11cf-8f52-0040333594a3>
 DWORD nNormals;
 array Vector normals[nNormals];
 DWORD nFaceNormals;
 array MeshFace faceNormals[nFaceNormals];
}

template Coords2d {
 <f6f23f44-7686-11cf-8f52-0040333594a3>
 FLOAT u;
 FLOAT v;
}

template MeshTextureCoords {
 <f6f23f40-7686-11cf-8f52-0040333594a3>
 DWORD nTextureCoords;
 array Coords2d textureCoords[nTextureCoords];
}

template ColorRGBA {
 <35ff44e0-6c7c-11cf-8f52-0040333594a3>
 FLOAT red;
 FLOAT green;
 FLOAT blue;
 FLOAT alpha;
}

template IndexedColor {
 <1630b820-7842-11cf-8f52-0040333594a3>
 DWORD index;
 ColorRGBA indexColor;
}

template MeshVertexColors {
 <1630b821-7842-11cf-8f52-0040333594a3>
 DWORD nVertexColors;
 array IndexedColor vertexColors[nVertexColors];
}

template FVFData {
 <b6e70a0e-8ef9-4e83-94ad-ecc8b0c04897>
 DWORD dwFVF;
 DWORD nDWords;
 array DWORD data[nDWords];
}


Mesh {
 9;
 -1.000000;0.000000;1.000000;,
 0.000000;0.000000;1.000000;,
 1.000000;0.000000;1.000000;,
 -1.000000;0.000000;0.000000;,
 0.000000;0.000000;0.000000;,
 1.000000;0.000000;0.000000;,
 -1.000000;0.000000;-1.000000;,
 0.000000;0.000000;-1.000000;,
 1.000000;0.000000;-1.000000;;
 8;
 3;0,1,3;,
 3;3,1,4;,
 3;1,2,4;,
 3;4,2,5;,
 3;3,4,6;,
 3;6,4,7;,
 3;4,5,7;,
 3;7,5,8;;

 MeshNormals {
  9;
  0.000000;1.000000;0.000000;,
  0.000000;1.000000;0.000000;,
  0.000000;1.000000;0.000000;,
  0.000000;1.000000;0.000000;,
  0.000000;1.000000;0.000000;,
  0.000000;1.000000;0.000000;,
  0.000000;1.000000;0.000000;,
  0.000000;1.000000;0.000000;,
  0.000000;1.000000;0.000000;;
  8;
  3;0,1,3;,
  3;3,1,4;,
  3;1,2,4;,
  3;4,2,5;,
  3;3,4,6;,
  3;6,4,7;,
  3;4,5,7;,
  3;7,5,8;;
 }

 MeshTextureCoords {
  9;
  0.000000;1.000000;,
  0.500000;1.000000;,
  1.000000;1.000000;,
  0.000000;0.500000;,
  0.500000;0.500000;,
  1.000000;0.500000;,
  0.000000;0.000000;,
  0.500000;0.000000;,
  1.000000;0.000000;;
 }

 MeshVertexColors {
  9;
  0;0.000000;0.000000;0.000000;0.000000;;,
  1;0.000000;0.000000;0.000000;0.000000;;,
  2;0.000000;0.000000;0.000000;0.000000;;,
  3;0.000000;0.000000;0.000000;0.000000;;,
  4;0.000000;0.000000;0.000000;0.000000;;,
  5;0.000000;0.000000;0.000000;0.000000;;,
  6;0.000000;0.000000;0.000000;0.000000;;,
  7;0.000000;0.000000;0.000000;0.000000;;,
  8;0.000000;0.000000;0.000000;0.000000;;;
 }

 FVFData {
  258;
  18;
  0,
  1065353216,
  1056964608,
  1065353216,
  1065353216,
  1065353216,
  0,
  1056964608,
  1056964608,
  1056964608,
  1065353216,
  1056964608,
  0,
  0,
  1056964608,
  0,
  1065353216,
  0;
 }
}