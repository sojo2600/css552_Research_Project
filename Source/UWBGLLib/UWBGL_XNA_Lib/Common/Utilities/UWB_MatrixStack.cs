// University of Washington Bothell Graphics Library
// Author: Timothy Chuang
// Based on Kelvin Sung and Steve Baer's D3D Library 
// and Robert Stone's XNA Library version 5
// The accompanying library supports CSS Graphics courses taught at UW-Bothell
// See: http://courses.washington.edu/css450/
//      http://courses.washington.edu/css451/
///////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace UWBGL_XNA_Lib
{
    public class UWB_MatrixStack
    {
        private Stack<Matrix> m_xna_stack;
        
        public UWB_MatrixStack()
        {
            // initialize the stack to hold 5 elements
            m_xna_stack = new Stack<Matrix>(5);
            //LoadIdentity();
        }

        public Matrix Top()
        {
            return m_xna_stack.Peek();
        }

        public void Push()
        {
            // Simulating D3D stack by always making a duplicate of the
            // top level item
            if (m_xna_stack.Count != 0)
            {
                Matrix temp = m_xna_stack.Peek();
                m_xna_stack.Push(temp);
            }
        }

        public void Pop()
        {
            if (m_xna_stack.Count != 0 )
                m_xna_stack.Pop();
        }

        public void LoadIdentity()
        {
            Matrix identity = new Matrix();
            identity = Matrix.Identity;
			if (m_xna_stack.Count == 0)
			{
				m_xna_stack.Push(identity);
			}
			else
			{
				m_xna_stack.Pop();
				m_xna_stack.Push(identity);
			}
        }

        public bool LoadMatrix(Matrix m)
        {
            if (m_xna_stack.Count == 0)
                return false;
            m_xna_stack.Pop();
            m_xna_stack.Push(m);
            return true;
        }

        public bool MultMatrix(Matrix m)
        {
            // Right-multiplies matrices
            if (m_xna_stack.Count == 0)
                return false;
            Matrix temp = m_xna_stack.Pop();
            m_xna_stack.Push(temp * m);
            return true;
        }

        public bool MultMatrixLocal(Matrix m)
        {
            // Left-multiplies matrices
            if (m_xna_stack.Count == 0)
                return false;
            Matrix temp = m_xna_stack.Pop();
            m_xna_stack.Push(m * temp);
            return true;
        }

        public bool RotateAxis(Vector3 pV, float radians)
        {
            pV.Normalize();
            Matrix rotation = Matrix.CreateFromAxisAngle(pV, radians);
            if (m_xna_stack.Count == 0)
                return false;
            Matrix temp = m_xna_stack.Pop();
            m_xna_stack.Push(temp * rotation);
            return true;
        }

        public bool RotateAxisLocal(Vector3 pV, float radians)
        {
            pV.Normalize();
            Matrix rotation = Matrix.CreateFromAxisAngle(pV, radians);
            if (m_xna_stack.Count == 0)
                return false;
            Matrix temp = m_xna_stack.Pop();
            m_xna_stack.Push(rotation * temp);
            return true;
        }

        public bool Scale(Vector3 scale)
        {
            Matrix mscale = Matrix.CreateScale(scale);
            if (m_xna_stack.Count == 0)
                return false;
            Matrix temp = m_xna_stack.Pop();
            m_xna_stack.Push(temp * mscale);
            return true;
        }

        public bool ScaleLocal(Vector3 scale)
        {
            Matrix mscale = Matrix.CreateScale(scale);
            if (m_xna_stack.Count == 0)
                return false;
            Matrix temp = m_xna_stack.Pop();
            m_xna_stack.Push(mscale * temp);
            return true;
        }

        public bool Translate(Vector3 trans)
        {
            Matrix translation = Matrix.CreateTranslation(trans);
            if (m_xna_stack.Count == 0)
                return false;
            Matrix temp = m_xna_stack.Pop();
            m_xna_stack.Push(temp * translation);
            return true;
        }

        public bool TranslateLocal(Vector3 trans)
        {
            Matrix translation = Matrix.CreateTranslation(trans);
            if (m_xna_stack.Count == 0)
                return false;
            Matrix temp = m_xna_stack.Pop();
            m_xna_stack.Push(translation * temp);
            return true;
        }
    }
}
