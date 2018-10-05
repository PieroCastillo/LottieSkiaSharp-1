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
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;

namespace LottieUWP.Network
{
    public class NetworkFetcher
    {
        private readonly string _url;

        private readonly NetworkCache _networkCache;

        public static Task<LottieResult<LottieComposition>> FetchAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new NetworkFetcher(url).FetchAsync(cancellationToken);
        }

        private NetworkFetcher(string url)
        {
            _url = url;
            _networkCache = new NetworkCache(url);
        }

        private async Task<LottieResult<LottieComposition>> FetchAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            LottieComposition result = await FetchFromCacheAsync(cancellationToken);
            if (result != null)
            {
                return new LottieResult<LottieComposition>(result);
            }

            Debug.WriteLine("Animation for " + _url + " not found in cache. Fetching from network.", LottieLog.Tag);
            return await FetchFromNetworkAsync(cancellationToken);
        }

        /// <summary>
        /// Returns null if the animation doesn't exist in the cache.
        /// </summary>
        /// <returns></returns>
        private async Task<LottieComposition> FetchFromCacheAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var cacheResult = await _networkCache.FetchAsync(cancellationToken);
            if (cacheResult == null)
            {
                return null;
            }

            var extension = cacheResult.Value.Key;
            using (var inputStream = cacheResult.Value.Value)
            {
                LottieResult<LottieComposition> result;
                if (extension == FileExtension.Zip)
                {
                    result = await LottieCompositionFactory.FromZipStreamAsync(new ZipArchive(inputStream), _url);
                }
                else
                {
                    result = await LottieCompositionFactory.FromJsonInputStreamAsync(inputStream, _url, cancellationToken);
                }
                if (result.Value != null)
                {
                    return result.Value;
                }
            }
            return null;
        }

        private async Task<LottieResult<LottieComposition>> FetchFromNetworkAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return await FetchFromNetworkInternalAsync(cancellationToken);
            }
            catch (Exception e)
            {
                return new LottieResult<LottieComposition>(e);
            }
        }

        private async Task<LottieResult<LottieComposition>> FetchFromNetworkInternalAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Debug.WriteLine($"Fetching {_url}", LottieLog.Tag);
            using (var connection = new HttpClient())
            {
                using (var response = await connection.GetAsync(_url, cancellationToken))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return new LottieResult<LottieComposition>(new ArgumentException($"Unable to fetch {_url}. Failed with {response.StatusCode}\n{response.ReasonPhrase}"));
                    }
                    throw new NotSupportedException();
                    //StorageFile file;
                    //FileExtension extension;
                    //LottieResult<LottieComposition> result;
                    //switch (response.Content.Headers.ContentType.MediaType)
                    //{
                    //    case "application/zip":
                    //        Debug.WriteLine("Handling zip response.", LottieLog.Tag);
                    //        extension = FileExtension.Zip;
                    //        file = await _networkCache.WriteTempCacheFileAsync(await response.Content.ReadAsStreamAsync().WithCancellation(cancellationToken), extension, cancellationToken);
                    //        using (var stream = await file.OpenStreamForReadAsync().AsAsyncOperation().AsTask(cancellationToken))
                    //        {
                    //            result = await LottieCompositionFactory.FromZipStreamAsync( new ZipArchive(stream), _url);
                    //        }
                    //        break;
                    //    case "application/json":
                    //    default:
                    //        Debug.WriteLine("Received json response.", LottieLog.Tag);
                    //        extension = FileExtension.Json;
                    //        file = await _networkCache.WriteTempCacheFileAsync(await response.Content.ReadAsStreamAsync().WithCancellation(cancellationToken), extension, cancellationToken);
                    //        using (var stream = await file.OpenStreamForReadAsync().AsAsyncOperation().AsTask(cancellationToken))
                    //        {
                    //            result = await LottieCompositionFactory.FromJsonInputStreamAsync(stream, _url);
                    //        }
                    //        break;
                    //}

                    //if (result.Value != null)
                    //{
                    //    await _networkCache.RenameTempFileAsync(extension, cancellationToken);
                    //}

                    //Debug.WriteLine($"Completed fetch from network. Success: {result.Value != null}", LottieLog.Tag);
                    //return result;
                }
            }
        }
    }
}
