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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;

namespace LottieUWP.Manager
{
    internal class ImageAssetManager : IDisposable
    {
        private readonly string _imagesFolder;
        private IImageAssetDelegate _delegate;
        private readonly Dictionary<string, LottieImageAsset> _imageAssets;

        internal ImageAssetManager(string imagesFolder, IImageAssetDelegate @delegate, Dictionary<string, LottieImageAsset> imageAssets)
        {
            _imagesFolder = imagesFolder;
            if (!string.IsNullOrEmpty(imagesFolder) && _imagesFolder[_imagesFolder.Length - 1] != '/')
            {
                _imagesFolder += '/';
            }

            //if (!(callback is UIElement)) // TODO: Makes sense on UWP?
            //{
            //    Debug.WriteLine("LottieDrawable must be inside of a view for images to work.", L.TAG);
            //    this.imageAssets = new Dictionary<string, LottieImageAsset>();
            //    return;
            //}

            _imageAssets = imageAssets;
            Delegate = @delegate;
        }

        internal IImageAssetDelegate Delegate
        {
            set
            {
                lock (this)
                {
                    _delegate = value;
                }
            }
        }

        /// <summary>
        /// Returns the previously set bitmap or null.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        internal SKBitmap UpdateBitmap(string id, SKBitmap bitmap)
        {
            lock (this)
            {
                if (bitmap == null)
                {
                    if (_imageAssets.TryGetValue(id, out var asset))
                    {
                        var ret = asset.Bitmap;
                        asset.Bitmap = null;
                        return ret;
                    }
                    return null;
                }

                SKBitmap prevBitmap = null;
                if (_imageAssets.TryGetValue(id, out var prevAsset))
                {
                    prevBitmap = prevAsset.Bitmap;
                }
                PutBitmap(id, bitmap);
                return bitmap;
            }
        }

        internal SKBitmap BitmapForId(string id)
        {
            lock (this)
            {
                if (!_imageAssets.TryGetValue(id, out var asset))
                {
                    return null;
                }
                var bitmap = asset.Bitmap;
                if (bitmap != null)
                {
                    return bitmap;
                }

                if (_delegate != null)
                {
                    bitmap = _delegate.FetchBitmap(asset);
                    if (bitmap != null)
                    {
                        PutBitmap(id, bitmap);
                    }
                    return bitmap;
                }

                var filename = asset.FileName;
                Task<SKBitmap> task = null;
                Stream @is;

                if (filename.StartsWith("data:") && filename.IndexOf("base64,") > 0)
                {
                    // Contents look like a base64 data URI, with the format data:image/png;base64,<data>.
                    byte[] data;
                    try
                    {
                        data = Convert.FromBase64String(filename.Substring(filename.IndexOf(',') + 1));
                        @is = new MemoryStream(data);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"data URL did not have correct base64 format. {e}", LottieLog.Tag);
                        return null;
                    }
                    task = Task.FromResult( SKBitmap.Decode(@is));
                    bitmap = task.Result;

                    @is.Dispose();

                    PutBitmap(id, bitmap);
                    return bitmap;
                }

                try
                {
                    if (string.IsNullOrEmpty(_imagesFolder))
                    {
                        throw new InvalidOperationException("You must set an images folder before loading an image. Set it with LottieDrawable.ImageAssetsFolder");
                    }
                    @is = File.OpenRead(_imagesFolder + asset.FileName);
                }
                catch (IOException e)
                {
                    Debug.WriteLine($"Unable to open asset. {e}", LottieLog.Tag);
                    return null;
                }
                task = Task.FromResult( SKBitmap.Decode(@is));
                bitmap = task.Result;

                @is.Dispose();

                PutBitmap(id, bitmap);

                return bitmap;
            }
        }

        internal void RecycleBitmaps()
        {
            lock (this)
            {
                foreach (var entry in _imageAssets)
                {
                    var asset = entry.Value;
                    var bitmap = asset.Bitmap;
                    if (bitmap != null)
                    {
                        bitmap.Dispose();
                        asset.Bitmap = null;
                    }
                }
            }
        }


        private SKBitmap PutBitmap(string key, SKBitmap bitmap)
        {
            lock (this)
            {
                _imageAssets[key].Bitmap = bitmap;
                return bitmap;
            }
        }

        private void Dispose(bool disposing)
        {
            RecycleBitmaps();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ImageAssetManager()
        {
            Dispose(false);
        }
    }
}