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
using System.Numerics;
using LottieUWP.Model;
using LottieUWP.Model.Animatable;
using LottieUWP.Model.Layer;
using LottieUWP.Value;

namespace LottieUWP.Animation.Keyframe
{
    public class TransformKeyframeAnimation
    {
        private Matrix3X3 _matrix = Matrix3X3.CreateIdentity();

        private readonly IBaseKeyframeAnimation<Vector2?, Vector2?> _anchorPoint;
        private readonly IBaseKeyframeAnimation<Vector2?, Vector2?> _position;
        private readonly IBaseKeyframeAnimation<ScaleXy, ScaleXy> _scale;
        private readonly IBaseKeyframeAnimation<float?, float?> _rotation;
        private readonly IBaseKeyframeAnimation<int?, int?> _opacity;

        // Used for repeaters 
        private readonly IBaseKeyframeAnimation<float?, float?> _startOpacity;
        private readonly IBaseKeyframeAnimation<float?, float?> _endOpacity;

        internal TransformKeyframeAnimation(AnimatableTransform animatableTransform)
        {
            _anchorPoint = animatableTransform.AnchorPoint.CreateAnimation();
            _position = animatableTransform.Position.CreateAnimation();
            _scale = animatableTransform.Scale.CreateAnimation();
            _rotation = animatableTransform.Rotation.CreateAnimation();
            _opacity = animatableTransform.Opacity.CreateAnimation();
            _startOpacity = animatableTransform.StartOpacity?.CreateAnimation();
            _endOpacity = animatableTransform.EndOpacity?.CreateAnimation();
        }

        internal void AddAnimationsToLayer(BaseLayer layer)
        {
            layer.AddAnimation(_anchorPoint);
            layer.AddAnimation(_position);
            layer.AddAnimation(_scale);
            layer.AddAnimation(_rotation);
            layer.AddAnimation(_opacity);
            if (_startOpacity != null)
            {
                layer.AddAnimation(_startOpacity);
            }
            if (_endOpacity != null)
            {
                layer.AddAnimation(_endOpacity);
            }
        }

        internal event EventHandler ValueChanged
        {
            add
            {
                _anchorPoint.ValueChanged += value;
                _position.ValueChanged += value;
                _scale.ValueChanged += value;
                _rotation.ValueChanged += value;
                _opacity.ValueChanged += value;
                if (_startOpacity != null)
                {
                    _startOpacity.ValueChanged += value;
                }
                if (_endOpacity != null)
                {
                    _endOpacity.ValueChanged += value;
                }
            }
            remove
            {
                _anchorPoint.ValueChanged -= value;
                _position.ValueChanged -= value;
                _scale.ValueChanged -= value;
                _rotation.ValueChanged -= value;
                _opacity.ValueChanged -= value;
                if (_startOpacity != null)
                {
                    _startOpacity.ValueChanged -= value;
                }
                if (_endOpacity != null)
                {
                    _endOpacity.ValueChanged -= value;
                }
            }
        }

        public float Progress
        {
            set
            {
                _anchorPoint.Progress = value;
                _position.Progress = value;
                _scale.Progress = value;
                _rotation.Progress = value;
                _opacity.Progress = value;
                if (_startOpacity != null)
                {
                    _startOpacity.Progress = value;
                }
                if (_endOpacity != null)
                {
                    _endOpacity.Progress = value;
                }
            }
        }

        internal IBaseKeyframeAnimation<int?, int?> Opacity => _opacity;

        internal IBaseKeyframeAnimation<float?, float?> StartOpacity => _startOpacity;

        internal IBaseKeyframeAnimation<float?, float?> EndOpacity => _endOpacity;

        internal Matrix3X3 Matrix
        {
            get
            {
                _matrix.Reset();
                var position = _position.Value;
                if (position != null && (position.Value.X != 0 || position.Value.Y != 0))
                {
                    _matrix = MatrixExt.PreTranslate(_matrix, position.Value.X, position.Value.Y);
                }

                if (_rotation.Value.HasValue && _rotation.Value.Value != 0f)
                {
                    _matrix = MatrixExt.PreRotate(_matrix, _rotation.Value.Value);
                }

                var scaleTransform = _scale.Value;
                if (scaleTransform != null && (scaleTransform.ScaleX != 1f || scaleTransform.ScaleY != 1f))
                {
                    _matrix = MatrixExt.PreScale(_matrix, scaleTransform.ScaleX, scaleTransform.ScaleY);
                }

                var anchorPoint = _anchorPoint.Value;
                if (anchorPoint != null && (anchorPoint.Value.X != 0 || anchorPoint.Value.Y != 0))
                {
                    _matrix = MatrixExt.PreTranslate(_matrix, -anchorPoint.Value.X, -anchorPoint.Value.Y);
                }
                return _matrix;
            }
        }

        /** 
        * TODO: see if we can use this for the main get_Matrix method. 
        */
        internal Matrix3X3 GetMatrixForRepeater(float amount)
        {
            var position = _position.Value;
            var anchorPoint = _anchorPoint.Value;
            var scale = _scale.Value;
            var rotation = _rotation.Value.Value;

            _matrix.Reset();
            _matrix = MatrixExt.PreTranslate(_matrix, position.Value.X * amount, position.Value.Y * amount);
            _matrix = MatrixExt.PreScale(_matrix,
                (float)Math.Pow(scale.ScaleX, amount),
                (float)Math.Pow(scale.ScaleY, amount));
            _matrix = MatrixExt.PreRotate(_matrix, rotation * amount, anchorPoint.Value.X, anchorPoint.Value.Y);

            return _matrix;
        }

        /// <summary>
        /// Returns whether the callback was applied. 
        /// </summary>
        public bool ApplyValueCallback<T>(LottieProperty property, ILottieValueCallback<T> callback)
        {
            if (property == LottieProperty.TransformAnchorPoint)
            {
                _anchorPoint.SetValueCallback((ILottieValueCallback<Vector2?>)callback);
            }
            else if (property == LottieProperty.TransformPosition)
            {
                _position.SetValueCallback((ILottieValueCallback<Vector2?>)callback);
            }
            else if (property == LottieProperty.TransformScale)
            {
                _scale.SetValueCallback((ILottieValueCallback<ScaleXy>)callback);
            }
            else if (property == LottieProperty.TransformRotation)
            {
                _rotation.SetValueCallback((ILottieValueCallback<float?>)callback);
            }
            else if (property == LottieProperty.TransformOpacity)
            {
                _opacity.SetValueCallback((ILottieValueCallback<int?>)callback);
            }
            else if (property == LottieProperty.TransformStartOpacity && _startOpacity != null)
            {
                _startOpacity.SetValueCallback((ILottieValueCallback<float?>)callback);
            }
            else if (property == LottieProperty.TransformEndOpacity && _endOpacity != null)
            {
                _endOpacity.SetValueCallback((ILottieValueCallback<float?>)callback);
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}