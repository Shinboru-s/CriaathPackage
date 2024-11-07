using System;
using System.Collections.Generic;
using UnityEngine;

namespace Criaath
{
    public class ObjectPool<T> where T : Component
    {
        private readonly GameObject _poolItemPref;
        private readonly Transform _poolParent;

        private readonly int _poolDefaultSize;

        private readonly List<T> _pool = new List<T>();
        private readonly List<T> _poolItemsInUse = new List<T>();

        private int _poolSize;
        private int _poolCount = 0;
        public Action<T> OnNewItemSpawn;
        public ObjectPool(GameObject itemPref, Transform poolParent, int defaultSize, bool autoGeneratePool = true)
        {
            _poolItemPref = itemPref;
            _poolParent = poolParent;
            _poolDefaultSize = defaultSize;
            _poolSize = 0;
            _pool = new List<T>();
            _poolItemsInUse = new List<T>();

            if (autoGeneratePool)
                FillRemainingItems();
        }
        public void GeneratePool() => FillRemainingItems();

        private void FillRemainingItems()
        {
            if (_poolSize >= _poolDefaultSize) return;

            int step = _poolDefaultSize - _poolSize;
            for (int i = 0; i < step; i++)
            {
                SpawnNewItem();
            }
        }

        private void SpawnNewItem()
        {
            var item = UnityEngine.Object.Instantiate(_poolItemPref, _poolParent);
            _poolCount++;
            item.name = _poolItemPref.name + _poolCount;
            T itemAsT = item.GetComponent<T>();
            PushItem(itemAsT, false);
            OnNewItemSpawn?.Invoke(itemAsT);
        }

        public void PushItem(T item, bool setParent = true)
        {
            _poolSize++;
            _pool.Add(item);
            item.gameObject.SetActive(false);
            if (setParent) item.transform.SetParent(_poolParent, false);
            if (_poolItemsInUse.Contains(item))
                _poolItemsInUse.Remove(item);
        }

        public T Pull()
        {
            if (_poolSize <= 0) SpawnNewItem();

            var item = _pool[0];
            _poolSize--;
            _pool.RemoveAt(0);
            _poolItemsInUse.Add(item);
            return item;
        }

        public void PushAllItems()
        {
            if (_poolItemsInUse.Count == 0) { Debug.Log("There are no items to push!"); return; }

            T[] itemsToPush = _poolItemsInUse.ToArray();

            for (int i = 0; i < itemsToPush.Length; i++)
            {
                PushItem(itemsToPush[i]);
            }
            _poolItemsInUse.Clear();
            Debug.Log("All items pushed to pool.");
        }
        public List<T> GetItemsInUse() { return _poolItemsInUse; }
        public List<T> GetAllItems() { return _pool; }
    }
}
