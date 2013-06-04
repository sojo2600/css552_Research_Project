using System;
using System.Collections.Generic;
using System.Text;

namespace UWBGL_XNA_Lib
{
    public class UWB_Array<T>
    {
        public List<T> m_Array;
        
        public UWB_Array()
        {
            m_Array = new List<T>(); 
        }

        public UWB_Array(int capacity)
        {
            m_Array = new List<T>(capacity);
        }

        public UWB_Array(UWB_Array<T> otherArray)
        {
            m_Array = new List<T>(otherArray.m_Array);
        }

        public int count()
        {
            return m_Array.Count;
        }

        public T getItem(int index)
        {
            return m_Array[index];
        }

        public void append(T item)
        {
            m_Array.Add(item);
        }

        public void insert(T item, int index)
        {
            m_Array.Insert(index, item);
        }

        public void deleteItem(int index)
        {
            m_Array.RemoveAt(index);
        }
    }
}
