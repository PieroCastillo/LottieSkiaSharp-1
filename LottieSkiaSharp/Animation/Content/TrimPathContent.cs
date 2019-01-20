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
using LottieUWP.Animation.Keyframe;
using LottieUWP.Model.Content;
using LottieUWP.Model.Layer;

namespace LottieUWP.Animation.Content
{
    internal class TrimPathContent : IContent
    {
        public event EventHandler ValueChanged;

        internal TrimPathContent(BaseLayer layer, ShapeTrimPath trimPath)
        {
            Name = trimPath.Name;
            Type = trimPath.GetType();
            Start = trimPath.Start.CreateAnimation();
            End = trimPath.End.CreateAnimation();
            Offset = trimPath.Offset.CreateAnimation();

            layer.AddAnimation(Start);
            layer.AddAnimation(End);
            layer.AddAnimation(Offset);

            Start.ValueChanged += OnValueChanged;
            End.ValueChanged += OnValueChanged;
            Offset.ValueChanged += OnValueChanged;
        }

        private void OnValueChanged(object sender, EventArgs eventArgs)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetContents(List<IContent> contentsBefore, List<IContent> contentsAfter)
        {
            // Do nothing.
        }

        public string Name { get; }

        internal ShapeTrimPath.Type Type { get; }

        public IBaseKeyframeAnimation<float?, float?> Start { get; }

        public IBaseKeyframeAnimation<float?, float?> End { get; }

        public IBaseKeyframeAnimation<float?, float?> Offset { get; }

        public bool IsHidden { get; }
    }
}