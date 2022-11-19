using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ShadedTechnology.GrassPhysics
{
    /// <summary>
    /// Specialization of <see cref="LimitedSizeArray{T}"/> for <see cref="GrassActor"/>
    /// </summary>
    [System.Serializable]
    public class LimitedSizeArray_GrassActor : LimitedSizeArray<GrassActor>
    {
        public LimitedSizeArray_GrassActor() : base(GlobalConstants.MAX_GRASS_ACTORS, 0) { }
        public LimitedSizeArray_GrassActor(int currentSize) : base(GlobalConstants.MAX_GRASS_ACTORS, currentSize) { }
    }

    /// <summary>
    /// Container used to store array with maximum size limit
    /// </summary>
    /// <typeparam name="T">Type of class to store</typeparam>
    [System.Serializable]
    public class LimitedSizeArray<T> where T : class
    {
        protected readonly int maxSize;
        [SerializeField]
        protected T[] elements;
        public delegate void ArraySizeChangeEvent();
        public event ArraySizeChangeEvent onArraySizeChange;

        public LimitedSizeArray(int maxSize, int currentSize)
        {
            this.maxSize = maxSize;
            this.elements = new T[currentSize];
        }

        public int MaxLength
        {
            get
            {
                return maxSize;
            }
        }

        public int Length
        {
            get
            {
                if (elements == null) return 0;
                return elements.Length;
            }
        }

        public T this[int i]
        {
            get
            {
                return elements[i];
            }
            set
            {
                elements[i] = value;
            }
        }

        /// <summary>
        /// Removes element at index from array
        /// </summary>
        /// <param name="index">Index of element that you want to remove</param>
        public void RemoveTargetAtIndex(int index)
        {
            var foos = new List<T>(elements);
            foos.RemoveAt(index);
            elements = foos.ToArray();
            if (onArraySizeChange != null) onArraySizeChange.Invoke(); 
        }

        /// <summary>
        /// Removes element from array
        /// </summary>
        /// <param name="element">Element that you want to remove</param>
        public void RemoveTargetFromArray(T element)
        {
            var foos = new List<T>(elements);
            foos.Remove(element);
            elements = foos.ToArray();
            if (onArraySizeChange != null) onArraySizeChange.Invoke();
        }

        /// <summary>
        /// Adds element to array
        /// </summary>
        /// <param name="target">Element object to add</param>
        public void AddTargetToArray(T element)
        {
            if (elements.Length >= maxSize) return;
            var foos = new List<T>(elements);
            foos.Add(element);
            elements = foos.ToArray();
            if (onArraySizeChange != null) onArraySizeChange.Invoke();
        }
    }
}
