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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;
using LottieUWP.Model;
using LottieUWP.Model.Layer;

namespace LottieUWP
{
    /// <summary>
    /// After Effects/Bodymovin composition model. This is the serialized model from which the
    /// animation will be created.
    /// 
    /// To create one, use <see cref="LottieCompositionFactory"/>.
    /// 
    /// It can be used with a <seealso cref="LottieAnimationView"/> or
    /// <seealso cref="LottieDrawable"/>.
    /// </summary>
    public class LottieComposition
    {
        private readonly PerformanceTracker _performanceTracker = new PerformanceTracker();
        private readonly HashSet<string> _warnings = new HashSet<string>();
        private Dictionary<string, List<Layer>> _precomps;
        private Dictionary<string, LottieImageAsset> _images;
        /** Map of font names to fonts */
        public Dictionary<string, Font> Fonts { get; private set; }
        public Dictionary<int, FontCharacter> Characters { get; private set; }
        private Dictionary<long, Layer> _layerMap;
        public List<Layer> Layers { get; private set; }

        // This is stored as a set to avoid duplicates.
        public SKRect Bounds { get; private set; }
        public float StartFrame { get; private set; }
        public float EndFrame { get; private set; }
        public float FrameRate { get; private set; }

        internal void AddWarning(string warning)
        {
            Debug.WriteLine(warning, LottieLog.Tag);
            _warnings.Add(warning);
        }

        public List<string> Warnings => _warnings.ToList();

        public bool PerformanceTrackingEnabled
        {
            set => _performanceTracker.Enabled = value;
        }

        public PerformanceTracker PerformanceTracker => _performanceTracker;

        internal Layer LayerModelForId(long id)
        {
            _layerMap.TryGetValue(id, out Layer layer);
            return layer;
        }

        public float Duration
        {
            get
            {
                return (long)(DurationFrames / FrameRate * 1000);
            }
        }

        public void Init(SKRect bounds, float startFrame, float endFrame, float frameRate, List<Layer> layers, Dictionary<long, Layer> layerMap, Dictionary<string, List<Layer>> precomps, Dictionary<string, LottieImageAsset> images, Dictionary<int, FontCharacter> characters, Dictionary<string, Font> fonts)
        {
            Bounds = bounds;
            StartFrame = startFrame;
            EndFrame = endFrame;
            FrameRate = frameRate;
            Layers = layers;
            _layerMap = layerMap;
            _precomps = precomps;
            _images = images;
            Characters = characters;
            Fonts = fonts;
        }

        internal List<Layer> GetPrecomps(string id)
        {
            return _precomps[id];
        }

        public bool HasImages => _images.Count > 0;

        public Dictionary<string, LottieImageAsset> Images => _images;

        internal float DurationFrames => EndFrame - StartFrame;

        public override string ToString()
        {
            var sb = new StringBuilder("LottieComposition:\n");
            foreach (var layer in Layers)
            {
                sb.Append(layer.ToString("\t"));
            }
            return sb.ToString();
        }

    }
}