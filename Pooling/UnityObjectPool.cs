using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using k514;
using k514.Extra;
using UI2020;
using UnityEngine;


namespace BDG
{
    /// 유니티 기본 컴포넌트들를 풀링하는데 쓰는 클레스
    /// <para>풀링할 오브젝트들의 부모가 될 오브젝트에 <c>AddComponent</c>를 통해 붙이고, 수동으로 <see cref="Initialize()"/>함수를 호출해 초기화 해야한다.</para>
    public abstract class UnityObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        private Stack<T> _pool;
        
        protected abstract void OnCreate(T obj);
        protected abstract void OnActive(T obj);
        protected abstract void OnPooled(T obj);
        
        public virtual UnityObjectPool<T> Initialize()
        {
            _pool = new Stack<T>();
            return this;
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
            if (obj == null)
            {
                Debug.LogError("Pool null reference");
                return;
            }
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

    /// <summary>
    /// 스크립트가 없는 게임 오브젝트 프리펩을 관리한다.
    /// </summary>
    /// <remarks>
    /// <para>풀링할 오브젝트들의 부모가 될 오브젝트에 <c>AddComponent</c>를 통해 붙이고, 수동으로 <see cref="Initialize()"/>함수를 호출해 초기화 해야한다.</para>
    /// <para>초기화시 <c>prefabName</c>을 반드시 지정해 줘야 한다.</para>
    /// </remarks>
    public abstract class UnityPrefabObjectPool : MonoBehaviour
    {
        /// <summary>
        /// 초기화시 프리펩 이름 반드시 정해줘야함
        /// </summary>
        public string prefabName;

        private Stack<GameObject> _pool;

        protected abstract void OnCreate(GameObject obj);
        protected abstract void OnActive(GameObject obj);
        protected abstract void OnPooled(GameObject obj);

        public virtual UnityPrefabObjectPool Initialize()
        {
            _pool = new Stack<GameObject>();
            return this;
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
            if (obj == null)
            {
                Debug.LogError("Pool null reference");
                return;
            }
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
    
    /// <summary>
    /// 프리펩 리소스에 <c>T</c>컴포넌트를 붙이고 관리한다.
    /// </summary>
    /// <remarks>
    /// <para>풀링할 오브젝트들의 부모가 될 오브젝트에 <c>AddComponent</c>를 통해 붙이고, 수동으로 <see cref="Initialize()"/>함수를 호출해 초기화 해야한다.</para>
    /// <para>초기화시 <c>prefabName</c>을 반드시 지정해 줘야 한다.</para>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public abstract class UnityPrefabObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// 초기화시 프리펩 이름 반드시 정해줘야함
        /// </summary>
        public string prefabName;

        private Stack<T> _pool;

        protected abstract void OnCreate(T obj);
        protected abstract void OnActive(T obj);
        protected abstract void OnPooled(T obj);

        public virtual UnityPrefabObjectPool<T> Initialize()
        {
            _pool = new Stack<T>();
            return this;
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
            if (obj == null)
            {
                Debug.LogError("Pool null reference");
                return;
            }
            OnPooled(obj);
            _pool.Push(obj);
        }

        private void CreateObject()
        {
            // AssetCacheMgr.GetResource(prefabName, preset => {});
            var prefabObj = LoadAssetManager.GetInstance.LoadAsset<GameObject>(ResourceType.GameObjectPrefab, ResourceLifeCycleType.WholeGame, prefabName);
            var obj = Instantiate(prefabObj, transform).AddComponent<T>();
            if(prefabObj == null)
                Debug.LogError($"Resource Load Error : {prefabName}");
            //var obj = Instantiate(Resources.Load<GameObject>(prefabName),transform).AddComponent<T>(); // 임시
            OnCreate(obj);
            OnPooled(obj);
            _pool.Push(obj);
        }
    }
}