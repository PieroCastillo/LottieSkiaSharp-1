//   Copyright 2018 yinyue200.com

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using System.Collections.Generic;

namespace LottieUWP.Model
{
    public class LruCache<K, V>
    {
        private readonly int _capacity;
        private Dictionary<K, LinkedListNode<LruCacheItem<K, V>>> _cacheMap = new Dictionary<K, LinkedListNode<LruCacheItem<K, V>>>();
        private LinkedList<LruCacheItem<K, V>> _lruList = new LinkedList<LruCacheItem<K, V>>();

        public LruCache(int capacity)
        {
            _capacity = capacity;
        }

        public V Get(K key)
        {
            lock (this)
            {
                if (_cacheMap.TryGetValue(key, out LinkedListNode<LruCacheItem<K, V>> node))
                {
                    V value = node.Value.Value;
                    _lruList.Remove(node);
                    _lruList.AddLast(node);
                    return value;
                }
                return default(V);
            }
        }

        public void Put(K key, V val)
        {
            lock (this)
            {
                if (_cacheMap.Count >= _capacity)
                {
                    RemoveFirst();
                }

                var cacheItem = new LruCacheItem<K, V>(key, val);
                var node = new LinkedListNode<LruCacheItem<K, V>>(cacheItem);
                _lruList.AddLast(node);
                _cacheMap[key] = node;
            }
        }

        private void RemoveFirst()
        {
            // Remove from LRUPriority
            var node = _lruList.First;
            _lruList.RemoveFirst();

            // Remove from cache
            _cacheMap.Remove(node.Value.Key);
        }
    }

    class LruCacheItem<K, V>
    {
        public LruCacheItem(K k, V v)
        {
            Key = k;
            Value = v;
        }
        public K Key;
        public V Value;
    }
}
