using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MVZ2.UI
{
    public class ElementList : MonoBehaviour
    {
        public void updateList(
            int count,
            Action<int, GameObject> onUpdate = null,
            Action<GameObject> onCreateOrEnable = null,
            Action<GameObject> onDestroyOrDisable = null,
            bool dontDestroy = false,
            bool rebuild = false)
        {
            int maxNum = Math.Max(itemList.Count, count);

            for (int i = 0; i < maxNum; i++)
            {
                if (i < count) // 应当出现在列表中
                {
                    GameObject item;
                    if (i >= itemList.Count) // 目前没有这个项
                    {
                        //创建列表项
                        item = CreateItem();
                        Add(item);
                        onCreateOrEnable?.Invoke(item);
                    }
                    else // 目前有这个项
                    {
                        item = itemList[i];
                        if (!item.activeSelf)
                        {
                            //激活
                            item.SetActive(true);
                            onCreateOrEnable?.Invoke(item);
                        }
                    }
                    //更新
                    onUpdate?.Invoke(i, item);
                }
                else // 不应出现在列表中
                {
                    GameObject item;
                    if (!dontDestroy) // 可以销毁
                    {
                        if (count < itemList.Count) // 目前有这个项
                        {
                            // 销毁列表项
                            item = itemList[count];
                            DestroyItem(item);
                            onDestroyOrDisable?.Invoke(item);
                        }
                    }
                    else // 不可以销毁
                    {
                        if (i < itemList.Count) // 目前有这个项
                        {
                            item = itemList[i];
                            if (item.activeSelf)
                            {
                                // 禁用
                                item.SetActive(false);
                                onDestroyOrDisable?.Invoke(item);
                            }
                        }
                    }
                }
            }
        }
        public void Add(GameObject item)
        {
            Insert(Count, item);
        }
        public void Insert(int index, GameObject item)
        {
            item.transform.SetParent(_listRoot, true);
            itemList.Insert(index, item);
            SortItems();
        }
        public GameObject CreateItem()
        {
            var item = Object.Instantiate(_template, _listRoot);
            //激活
            item.SetActive(true);
            return item;
        }
        public bool Remove(GameObject item)
        {
            return itemList.Remove(item);
        }
        public void RemoveAt(int index)
        {
            itemList.RemoveAt(index);
        }
        public bool DestroyItem(GameObject item)
        {
            if (Remove(item))
            {
                item.transform.SetParent(null);
                Object.Destroy(item);
                return true;
            }
            return false;
        }
        public int indexOf(GameObject go)
        {
            return itemList.IndexOf(go);
        }
        public int indexOf(Component comp)
        {
            return indexOf(comp.gameObject);
        }
        public GameObject getElement(int index)
        {
            if (index < 0 || index >= itemList.Count)
                return null;
            return itemList[index];
        }
        public T getElement<T>(int index) where T : Component
        {
            return getElement(index)?.GetComponent<T>();
        }
        public GameObject getTemplate()
        {
            return _template;
        }
        public T getTemplate<T>() where T : Component
        {
            return getTemplate()?.GetComponent<T>();
        }
        public IEnumerable<GameObject> getElements()
        {
            return itemList;
        }
        public IEnumerable<T> getElements<T>() where T : Component
        {
            foreach (var item in itemList)
            {
                yield return item.GetComponent<T>();
            }
        }
        private void Awake()
        {
            if (_template.transform.parent == _listRoot)
            {
                _template.SetActive(false);
            }
        }
        private void SortItems()
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                var item = itemList[i];
                item.transform.SetAsLastSibling();
            }
        }
        public int Count => itemList.Count;
        public Transform ListRoot => _listRoot;
        [SerializeField]
        private Transform _listRoot;
        [SerializeField]
        private GameObject _template;
        [SerializeField]
        private List<GameObject> itemList = new List<GameObject>();
    }
}
