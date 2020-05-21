using System;
using System.Collections.Generic;
using UnityEngine;

namespace BDG
{
    public class ObjectPool<T> where T : PoolingObject, new()
    {
        private Stack<T> _pool;

        public ObjectPool(int minSize)
        {
            _pool = new Stack<T>();
            for (var i = 0; i < minSize; i++)
            {
                CreateObject();
            }
        }

        ~ObjectPool()
        {
            _pool = null;
        }

        public T GetObject()
        {
            if (_pool.Count == 0)
            {
                CreateObject();
            }

            var obj = _pool.Pop();
            obj.OnActive();
            return obj;
        }

        public void PoolObject(T obj)
        {
            obj.OnPooling();
            _pool.Push(obj);
        }

        private void CreateObject()
        {
            T poolingObject = new T();
            poolingObject.OnCreate();
            poolingObject.OnPooling();
            _pool.Push(poolingObject);
        }
    }
    
    public interface PoolingObject
    {
        void OnCreate();
        void OnPooling();
        void OnActive();
    }
}