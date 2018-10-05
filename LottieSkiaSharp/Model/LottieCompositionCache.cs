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
namespace LottieUWP.Model
{
    internal class LottieCompositionCache
    {
        //private static readonly int _cacheSizeMB = 10;
        private static readonly int _cacheSizeCount = 10;

        public static LottieCompositionCache Instance { get; } = new LottieCompositionCache();

        private readonly LruCache<string, LottieComposition> _cache = new LruCache<string, LottieComposition>(_cacheSizeCount);//1024 * 1024 * _cacheSizeMB);

        internal LottieCompositionCache()
        {
        }

        public LottieComposition Get(string cacheKey)
        {
            if (cacheKey == null)
            {
                return null;
            }
            return _cache.Get(cacheKey);
        }

        public void Put(string cacheKey, LottieComposition composition)
        {
            if (cacheKey == null)
            {
                return;
            }
            _cache.Put(cacheKey, composition);
        }
    }
}
