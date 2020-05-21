using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI2020.UIComponent
{
    public class UIScroll<T> where T:MonoBehaviour
    {
        private float _xInterval, _yInterval;
        private int _nOfContent;

        private AbstractPoolingManager<T> _scrollObjectPool;
        private List<T> _contentList;

        private RectTransform _content;

        public UIScroll()
        {
            _nOfContent = 0;
            _contentList = new List<T>();
        }
        
        public UIScroll(AbstractPoolingManager<T> scrollObjectPool, RectTransform content) : this()
        {
            _scrollObjectPool = scrollObjectPool;
            _content = content;
        }
        
        public UIScroll(AbstractPoolingManager<T> scrollObjectPool, RectTransform content, int xInterval, int yInterval) : this(scrollObjectPool,content)
        {
            _xInterval = xInterval;
            _yInterval = yInterval;
        }

        public T AddContent()
        {
            var content = _scrollObjectPool.GetObject();
            content.transform.SetParent(_content);
            content.transform.localPosition = _nOfContent * new Vector3(_xInterval, -_yInterval, 0);
            _contentList.Add(content);
            _nOfContent++;
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _nOfContent * _xInterval);
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _nOfContent * _yInterval);
            return content;
        }

        public void DeleteContent(T content)
        {
            var index = _contentList.IndexOf(content);
            if (index == -1) return; // 해당 개체가 없는 경우
            _contentList.Remove(content);
            _scrollObjectPool.PoolObject(content);

            while (_contentList.Count > index)
            {
                _contentList[index].transform.localPosition -= new Vector3(_xInterval, -_yInterval);
                index++;
            }
            
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _nOfContent * _xInterval);
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _nOfContent * _yInterval);
            
            _nOfContent--;
        }

        public void DeleteContent(int index)
        {
            if (index >= _contentList.Count) return; // 해당 인덱스가 없는 경우
            var content = _contentList[index];
            _contentList.RemoveAt(index);
            _scrollObjectPool.PoolObject(content);

            while (_contentList.Count > index)
            {
                _contentList[index].transform.localPosition -= new Vector3(_xInterval, -_yInterval);
                index++;
            }
            
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _nOfContent * _xInterval);
            _content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _nOfContent * _yInterval);
            
            _nOfContent--;
        }
    }
}