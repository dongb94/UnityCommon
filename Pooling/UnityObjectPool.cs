using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using k514;
using UI2020;
using UnityEngine;


namespace BDG
{
    /// 유니티 기본 컴포넌트들를 풀링하는데 쓰는 클레스
    public abstract class UnityObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        private Stack<T> _pool;
        
        protected abstract void OnCreate(T obj);
        protected abstract void OnActive(T obj);
        protected abstract void OnPooled(T obj);

        private void Awake()
        {
            _pool = new Stack<T>();
        }

        public UnityObjectPool(int minSize)
        {
            for (var i = 0; i < minSize; i++)
            {
                CreateObject();
            }
        }

        ~UnityObjectPool()
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
            OnActive(obj);
            return obj;
        }

        public void PoolObject(T obj)
        {
            OnPooled(obj);
            _pool.Push(obj);
        }

        private void CreateObject()
        {
            T poolingObject = new GameObject(typeof(T).ToString()).AddComponent<T>();
            poolingObject.transform.parent = transform;
            OnCreate(poolingObject);
            OnPooled(poolingObject);
            _pool.Push(poolingObject);
        }
    }

    public abstract class UnityPrefabObjectPool : MonoBehaviour
    {
        public string prefabName;

        private Stack<GameObject> _pool;

        /// <summary>
        /// 초기화시 프리펩 이름 반드시 정해줘야함
        /// </summary>
        protected abstract void Initialize();
        protected abstract void OnCreate(GameObject obj);
        protected abstract void OnActive(GameObject obj);
        protected abstract void OnPooled(GameObject obj);


        protected virtual void Awake()
        {
            _pool = new Stack<GameObject>();
            Initialize();
        }

        protected virtual void OnDestroy()
        {
            _pool = null;
        }

        public GameObject GetObject()
        {
            if (_pool.Count == 0)
            {
                CreateObject();
            }

            var obj = _pool.Pop();
            OnActive(obj);
            return obj;
        }

        public void PoolObject(GameObject obj)
        {
            OnPooled(obj);
            _pool.Push(obj);
        }

        private async Task CreateObject()
        {
            var prefabObj = await LoadAssetManager.GetInstance.LoadAssetAsync<GameObject>(ResourceType.GameObjectPrefab, ResourceLifeCycleType.WholeGame, prefabName);
            var obj = Instantiate(prefabObj, transform, true);
            OnCreate(obj);
            OnPooled(obj);
            _pool.Push(obj);
        }
    }
    
    public abstract class UnityPrefabObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        public string prefabName;

        private Stack<T> _pool;

        protected abstract void Initialize();
        protected abstract void OnCreate(T obj);
        protected abstract void OnActive(T obj);
        protected abstract void OnPooled(T obj);

        protected virtual void Awake()
        {
            _pool = new Stack<T>();
            Initialize();
        }

        protected virtual void OnDestroy()
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
            OnActive(obj);
            return obj;
        }

        public void PoolObject(T obj)
        {
            OnPooled(obj);
            _pool.Push(obj);
        }

        private async Task CreateObject()
        {
            var prefabObj = await LoadAssetManager.GetInstance.LoadAssetAsync<GameObject>(ResourceType.GameObjectPrefab, ResourceLifeCycleType.WholeGame, prefabName);
            var obj = Instantiate(prefabObj, transform).AddComponent<T>();
            //var obj = Instantiate(Resources.Load<GameObject>(prefabName),transform).AddComponent<T>(); // 임시
            OnCreate(obj);
            OnPooled(obj);
            _pool.Push(obj);
        }
    }
}