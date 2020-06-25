using BDG;
using UnityEngine;

namespace UI2020
{
    /// <summary>
    /// UI 프리펩 오브젝트 풀을 싱글톤으로 사용하기 위한 추상클레스
    /// </summary>
    public abstract class AbstractPrefabPoolingManager<T> : UnityPrefabObjectPool<T> where T : MonoBehaviour
    {
        private static AbstractPrefabPoolingManager<T> _instance;

        public static AbstractPrefabPoolingManager<T> Instance
        {
            get
            {
                if (_instance == null)
                {
#if UNITY_EDITOR
                    // error - 게임오브젝트에 붙을때 Initialize 함수 호출 통해 초기화 시켜줘야 한다. 그 전에 호출되면 에러 (불완전 싱글톤)
                    Debug.LogError($"Wrong Access");
#endif
                }
                return _instance;
            }
        }

        public override UnityPrefabObjectPool<T> Initialize()
        {
            base.Initialize();
            _instance = this;
            return this;
        }
    }
    
    /// <summary>
    /// UI 컴포넌트 풀을 싱글톤으로 사용하기 위한 추상클레스
    /// </summary>
    public abstract class AbstractComponentPoolingManager<T> : UnityObjectPool<T> where T : MonoBehaviour
    {
        private static AbstractComponentPoolingManager<T> _instance;

        public static AbstractComponentPoolingManager<T> Instance
        {
            get
            {
                if (_instance == null)
                {
#if UNITY_EDITOR
                    // error - 게임오브젝트에 붙을때 Initialize 함수 호출 통해 초기화 시켜줘야 한다. 그 전에 호출되면 에러 (불완전 싱글톤)
                    Debug.LogError($"Wrong Access");
#endif
                }
                return _instance;
            }
        }

        public override UnityObjectPool<T> Initialize()
        {
            base.Initialize();
            _instance = this;
            return this;
        }
    }
}