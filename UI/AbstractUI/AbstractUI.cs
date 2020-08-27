using k514;
using UnityEngine;

namespace UI2020
{
    public abstract class AbstractUI : UIManagerBase
    {
        public bool IsActive => gameObject.activeSelf;

        public virtual void OnActive()
        {
            
        }

        public virtual void OnDisable()
        {
            
        }
        
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        
        protected T GetComponent<T>(string path)
        {
            return transform.Find(path).GetComponent<T>();
        }

        protected T AddComponent<T>(string path) where T : Component
        {
            return Find(path).gameObject.AddComponent<T>();
        }

        protected Transform Find(string path)
        {
            return transform.Find(path);
        }

        public override void OnUpdateUI(float p_DeltaTime)
        {
        }

        public override void DisposeUnManaged()
        {
        }
    }
}