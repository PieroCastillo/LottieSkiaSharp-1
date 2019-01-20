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
using SkiaSharp;
using LottieUWP.Animation.Content;
using LottieUWP.Animation.Keyframe;
using LottieUWP.Model.Content;
using LottieUWP.Value;
using LottieUWP.Expansion;

namespace LottieUWP.Model.Layer
{
    public abstract class BaseLayer : IDrawingContent, IKeyPathElement
    {
        //private static readonly int SaveFlags = BitmapCanvas.ClipSaveFlag | BitmapCanvas.ClipToLayerSaveFlag | BitmapCanvas.MatrixSaveFlag;

        internal static BaseLayer ForModel(Layer layerModel, ILottieDrawable drawable, LottieComposition composition)
        {
            switch (layerModel.GetLayerType())
            {
                case Layer.LayerType.Shape:
                    return new ShapeLayer(drawable, layerModel);
                case Layer.LayerType.PreComp:
                    return new CompositionLayer(drawable, layerModel, composition.GetPrecomps(layerModel.RefId), composition);
                case Layer.LayerType.Solid:
                    return new SolidLayer(drawable, layerModel);
                case Layer.LayerType.Image:
                    return new ImageLayer(drawable, layerModel);
                case Layer.LayerType.Null:
                    return new NullLayer(drawable, layerModel);
                case Layer.LayerType.Text:
                    return new TextLayer(drawable, layerModel);
                case Layer.LayerType.Unknown:
                default:
                    // Do nothing
                    LottieLog.Warn("Unknown layer type " + layerModel.GetLayerType());
                    return null;
            }
        }

        private SKPath _path = new SKPath();
        internal Matrix3X3 Matrix = Matrix3X3.CreateIdentity();
        private readonly SKPaint _contentPaint = SkRectExpansion.CreateSkPaint();
        private readonly SKPaint _addMaskPaint = SkRectExpansion.CreateSkPaint();
        private readonly SKPaint _subtractMaskPaint = SkRectExpansion.CreateSkPaint();
        private readonly SKPaint _mattePaint = SkRectExpansion.CreateSkPaint();
        private readonly SKPaint _clearPaint = SkRectExpansion.CreateSkPaintWithoutAntialias();
        protected SKRect Rect;
        private SKRect _maskBoundsRect;
        private SKRect _matteBoundsRect;
        private SKRect _tempMaskBoundsRect;
        private readonly string _drawTraceName;
        internal Matrix3X3 BoundsMatrix = Matrix3X3.CreateIdentity();
        internal readonly ILottieDrawable LottieDrawable;
        internal Layer LayerModel;
        private readonly MaskKeyframeAnimation _mask;
        private BaseLayer _matteLayer;
        private BaseLayer _parentLayer;
        private List<BaseLayer> _parentLayers;

        private readonly List<IBaseKeyframeAnimation> _animations = new List<IBaseKeyframeAnimation>();
        internal readonly TransformKeyframeAnimation Transform;
        private bool _visible = true;

        internal BaseLayer(ILottieDrawable lottieDrawable, Layer layerModel)
        {
            LottieDrawable = lottieDrawable;
            LayerModel = layerModel;
            _drawTraceName = layerModel.Name + ".Draw";
            _contentPaint.SetAlpha(255);
            _clearPaint.BlendMode = SKBlendMode.Clear;
            _addMaskPaint.BlendMode = SKBlendMode.DstIn;
            _addMaskPaint.Color = SKColors.Black;
            _subtractMaskPaint.BlendMode = SKBlendMode.DstOut;
            _subtractMaskPaint.Color = SKColors.Black;
            if (layerModel.GetMatteType() == Layer.MatteType.Invert)
            {
                _mattePaint.BlendMode = SKBlendMode.DstOut;
            }
            else
            {
                _mattePaint.BlendMode = SKBlendMode.DstIn;
            }
            _mattePaint.Color = SKColors.Black;

            Transform = layerModel.Transform.CreateAnimation();
            Transform.ValueChanged += OnValueChanged;

            if (layerModel.Masks != null && layerModel.Masks.Count > 0)
            {
                _mask = new MaskKeyframeAnimation(layerModel.Masks);
                foreach (var animation in _mask.MaskAnimations)
                {
                    // Don't call AddAnimation() because progress gets set manually in setProgress to 
                    // properly handle time scale.
                    animation.ValueChanged += OnValueChanged;
                }
                foreach (var animation in _mask.OpacityAnimations)
                {
                    AddAnimation(animation);
                    animation.ValueChanged += OnValueChanged;
                }
            }
            SetupInOutAnimations();
        }

        public void OnValueChanged(object sender, EventArgs eventArgs)
        {
            InvalidateSelf();
        }

        internal BaseLayer MatteLayer
        {
            set => _matteLayer = value;
        }

        internal bool HasMatteOnThisLayer()
        {
            return _matteLayer != null;
        }

        internal BaseLayer ParentLayer
        {
            set => _parentLayer = value;
        }

        private void SetupInOutAnimations()
        {
            if (LayerModel.InOutKeyframes.Count > 0)
            {
                var inOutAnimation = new FloatKeyframeAnimation(LayerModel.InOutKeyframes);
                inOutAnimation.SetIsDiscrete();
                inOutAnimation.ValueChanged += (sender, args) =>
                {
                    Visible = inOutAnimation.Value == 1f;
                };
                Visible = inOutAnimation.Value == 1f;
                AddAnimation(inOutAnimation);
            }
            else
            {
                Visible = true;
            }
        }

        private void InvalidateSelf()
        {
            LottieDrawable.InvalidateSelf();
        }


        internal void AddAnimation(IBaseKeyframeAnimation newAnimation)
        {
            _animations.Add(newAnimation);
        }

        public virtual void GetBounds(out SKRect outBounds, Matrix3X3 parentMatrix)
        {
            outBounds = SKRect.Empty;
            BoundsMatrix.Set(parentMatrix);
            BoundsMatrix = MatrixExt.PreConcat(BoundsMatrix, Transform.Matrix);
        }

        public void Draw(SKCanvas canvas, Matrix3X3 parentMatrix, byte parentAlpha)
        {
            LottieLog.BeginSection(_drawTraceName);
            if (!_visible || LayerModel.IsHidden)
            {
                LottieLog.EndSection(_drawTraceName);
                return;
            }
            BuildParentLayerListIfNeeded();
            LottieLog.BeginSection("Layer.ParentMatrix");
            Matrix.Reset();
            Matrix.Set(parentMatrix);
            for (var i = _parentLayers.Count - 1; i >= 0; i--)
            {
                Matrix = MatrixExt.PreConcat(Matrix, _parentLayers[i].Transform.Matrix);
            }
            LottieLog.EndSection("Layer.ParentMatrix");
            var alpha = (byte)(parentAlpha / 255f * (float)Transform.Opacity.Value / 100f * 255);
            if (!HasMatteOnThisLayer() && !HasMasksOnThisLayer())
            {
                Matrix = MatrixExt.PreConcat(Matrix, Transform.Matrix);
                LottieLog.BeginSection("Layer.DrawLayer");
                DrawLayer(canvas, Matrix, alpha);
                LottieLog.EndSection("Layer.DrawLayer");
                RecordRenderTime(LottieLog.EndSection(_drawTraceName));
                return;
            }

            LottieLog.BeginSection("Layer.ComputeBounds");
            RectExt.Set(ref Rect, 0, 0, 0, 0);
            GetBounds(out Rect, Matrix);
            IntersectBoundsWithMatte(ref Rect, Matrix);

            Matrix = MatrixExt.PreConcat(Matrix, Transform.Matrix);
            IntersectBoundsWithMask(ref Rect, Matrix);

            RectExt.Set(ref Rect, 0, 0, canvas.LocalClipBounds.Width, canvas.LocalClipBounds.Height);
            LottieLog.EndSection("Layer.ComputeBounds");

            LottieLog.BeginSection("Layer.SaveLayer");
            //canvas.SaveLayer(Rect, _contentPaint, BitmapCanvas.AllSaveFlag);
            canvas.SaveLayer(Rect, _contentPaint);

            LottieLog.EndSection("Layer.SaveLayer");

            // Clear the off screen buffer. This is necessary for some phones.
            ClearCanvas(canvas);
            LottieLog.BeginSection("Layer.DrawLayer");
            DrawLayer(canvas, Matrix, alpha);
            LottieLog.EndSection("Layer.DrawLayer");

            if (HasMasksOnThisLayer())
            {
                ApplyMasks(canvas, Matrix);
            }

            if (HasMatteOnThisLayer())
            {
                LottieLog.BeginSection("Layer.DrawMatte");
                LottieLog.BeginSection("Layer.SaveLayer");
                //canvas.SaveLayer(Rect, _mattePaint, SaveFlags);
                canvas.SaveLayer(Rect, _mattePaint);

                LottieLog.EndSection("Layer.SaveLayer");
                ClearCanvas(canvas);

                _matteLayer.Draw(canvas, parentMatrix, alpha);
                LottieLog.BeginSection("Layer.RestoreLayer");
                canvas.Restore();
                LottieLog.EndSection("Layer.RestoreLayer");
                LottieLog.EndSection("Layer.DrawMatte");
            }

            LottieLog.BeginSection("Layer.RestoreLayer");
            canvas.Restore();
            LottieLog.EndSection("Layer.RestoreLayer");
            RecordRenderTime(LottieLog.EndSection(_drawTraceName));
        }

        private void RecordRenderTime(float ms)
        {
            LottieDrawable.Composition.PerformanceTracker.RecordRenderTime(LayerModel.Name, ms);
        }

        private void ClearCanvas(SKCanvas canvas)
        {
            LottieLog.BeginSection("Layer.ClearLayer");
            // If we don't pad the clear draw, some phones leave a 1px border of the graphics buffer.
            canvas.DrawRect(Rect.Left - 1, Rect.Top - 1, Rect.Right + 1, Rect.Bottom + 1, _clearPaint);
            LottieLog.EndSection("Layer.ClearLayer");
        }

        private void IntersectBoundsWithMask(ref SKRect rect, Matrix3X3 matrix)
        {
            RectExt.Set(ref _maskBoundsRect, 0, 0, 0, 0);
            if (!HasMasksOnThisLayer())
            {
                return;
            }

            var size = _mask.Masks.Count;
            for (var i = 0; i < size; i++)
            {
                var mask = _mask.Masks[i];
                var maskAnimation = _mask.MaskAnimations[i];
                var maskPath = maskAnimation.Value;
                _path.Set(maskPath);
                _path.Transform(matrix.ToSKMatrix());

                switch (mask.GetMaskMode())
                {
                    case Mask.MaskMode.MaskModeSubtract:
                        // If there is a subtract mask, the mask could potentially be the size of the entire
                        // canvas so we can't use the mask bounds.
                        return;
                    case Mask.MaskMode.MaskModeIntersect:
                        // TODO 
                        return;
                    case Mask.MaskMode.MaskModeAdd:
                    default:
                        _path.GetBounds(out _tempMaskBoundsRect);
                        // As we iterate through the masks, we want to calculate the union region of the masks.
                        // We initialize the rect with the first mask. If we don't call set() on the first call,
                        // the rect will always extend to (0,0).
                        if (i == 0)
                        {
                            RectExt.Set(ref _maskBoundsRect, _tempMaskBoundsRect);
                        }
                        else
                        {
                            RectExt.Set(ref _maskBoundsRect, Math.Min(_maskBoundsRect.Left, _tempMaskBoundsRect.Left), Math.Min(_maskBoundsRect.Top, _tempMaskBoundsRect.Top), Math.Max(_maskBoundsRect.Right, _tempMaskBoundsRect.Right), Math.Max(_maskBoundsRect.Bottom, _tempMaskBoundsRect.Bottom));
                        }
                        break;
                }
            }

            RectExt.Set(ref rect, Math.Max(rect.Left, _maskBoundsRect.Left), Math.Max(rect.Top, _maskBoundsRect.Top), Math.Min(rect.Right, _maskBoundsRect.Right), Math.Min(rect.Bottom, _maskBoundsRect.Bottom));
        }

        private void IntersectBoundsWithMatte(ref SKRect rect, Matrix3X3 matrix)
        {
            if (!HasMatteOnThisLayer())
            {
                return;
            }
            if (LayerModel.GetMatteType() == Layer.MatteType.Invert)
            {
                // We can't trim the bounds if the mask is inverted since it extends all the way to the
                // composition bounds.
                return;
            }
            _matteLayer.GetBounds(out _matteBoundsRect, matrix);
            RectExt.Set(ref rect, Math.Max(rect.Left, _matteBoundsRect.Left), Math.Max(rect.Top, _matteBoundsRect.Top), Math.Min(rect.Right, _matteBoundsRect.Right), Math.Min(rect.Bottom, _matteBoundsRect.Bottom));
        }

        public abstract void DrawLayer(SKCanvas canvas, Matrix3X3 parentMatrix, byte parentAlpha);

        private void ApplyMasks(SKCanvas canvas, Matrix3X3 matrix)
        {
            ApplyMasks(canvas, matrix, Mask.MaskMode.MaskModeAdd);
            // Treat intersect masks like add masks. This is not correct but it's closer. 
            ApplyMasks(canvas, matrix, Mask.MaskMode.MaskModeIntersect);
            ApplyMasks(canvas, matrix, Mask.MaskMode.MaskModeSubtract);
        }

        private void ApplyMasks(SKCanvas canvas, Matrix3X3 matrix, Mask.MaskMode maskMode)
        {
            SKPaint paint;
            switch (maskMode)
            {
                case Mask.MaskMode.MaskModeSubtract:
                    paint = _subtractMaskPaint;
                    break;
                case Mask.MaskMode.MaskModeIntersect:
                    goto case Mask.MaskMode.MaskModeAdd;
                case Mask.MaskMode.MaskModeAdd:
                default:
                    // As a hack, we treat all non-subtract masks like add masks. This is not correct but it's 
                    // better than nothing.
                    paint = _addMaskPaint;
                    break;
            }

            var size = _mask.Masks.Count;

            var hasMask = false;
            for (int i = 0; i < size; i++)
            {
                if (_mask.Masks[i].GetMaskMode() == maskMode)
                {
                    hasMask = true;
                    break;
                }
            }
            if (!hasMask)
            {
                return;
            }
            LottieLog.BeginSection("Layer.DrawMask");
            LottieLog.BeginSection("Layer.SaveLayer");
            //canvas.SaveLayer(Rect, paint, SaveFlags);
            canvas.SaveLayer(Rect, paint);
            LottieLog.EndSection("Layer.SaveLayer");
            ClearCanvas(canvas);

            for (var i = 0; i < size; i++)
            {
                var mask = _mask.Masks[i];
                if (mask.GetMaskMode() != maskMode)
                {
                    continue;
                }
                var maskAnimation = _mask.MaskAnimations[i];
                var maskPath = maskAnimation.Value;
                _path.Set(maskPath);
                _path.Transform(matrix.ToSKMatrix());

                var opacityAnimation = _mask.OpacityAnimations[i];
                var alpha = _contentPaint.Color.Alpha;
                _contentPaint.SetAlpha((byte)(opacityAnimation.Value.Value * 2.55f));
                canvas.DrawPath(_path, _contentPaint);
                _contentPaint.SetAlpha(alpha);
            }
            LottieLog.BeginSection("Layer.RestoreLayer");
            canvas.Restore();
            LottieLog.EndSection("Layer.RestoreLayer");
            LottieLog.EndSection("Layer.DrawMask");
        }

        internal bool HasMasksOnThisLayer()
        {
            return _mask != null && _mask.MaskAnimations.Count > 0;
        }

        private bool Visible
        {
            set
            {
                if (value != _visible)
                {
                    _visible = value;
                    InvalidateSelf();
                }
            }
        }

        public virtual float Progress
        {
            set
            {
                // Time stretch should not be applied to the layer transform. 
                Transform.Progress = value;
                if (_mask != null)
                {
                    for (int i = 0; i < _mask.MaskAnimations.Count; i++)
                    {
                        _mask.MaskAnimations[i].Progress = value;
                    }
                }
                if (LayerModel.TimeStretch != 0)
                {
                    value /= LayerModel.TimeStretch;
                }
                if (_matteLayer != null)
                {
                    // The matte layer's time stretch is pre-calculated.
                    float matteTimeStretch = _matteLayer.LayerModel.TimeStretch;
                    _matteLayer.Progress = value * matteTimeStretch;
                }
                for (var i = 0; i < _animations.Count; i++)
                {
                    _animations[i].Progress = value;
                }
            }
        }

        private void BuildParentLayerListIfNeeded()
        {
            if (_parentLayers != null)
            {
                return;
            }
            if (_parentLayer == null)
            {
                _parentLayers = new List<BaseLayer>();
                return;
            }

            _parentLayers = new List<BaseLayer>();
            var layer = _parentLayer;
            while (layer != null)
            {
                _parentLayers.Add(layer);
                layer = layer._parentLayer;
            }
        }

        public string Name => LayerModel.Name;

        public void SetContents(List<IContent> contentsBefore, List<IContent> contentsAfter)
        {
            // Do nothing
        }

        public void ResolveKeyPath(KeyPath keyPath, int depth, List<KeyPath> accumulator, KeyPath currentPartialKeyPath)
        {
            if (!keyPath.Matches(Name, depth))
            {
                return;
            }

            if (!"__container".Equals(Name))
            {
                currentPartialKeyPath = currentPartialKeyPath.AddKey(Name);

                if (keyPath.FullyResolvesTo(Name, depth))
                {
                    accumulator.Add(currentPartialKeyPath.Resolve(this));
                }
            }

            if (keyPath.PropagateToChildren(Name, depth))
            {
                int newDepth = depth + keyPath.IncrementDepthBy(Name, depth);
                ResolveChildKeyPath(keyPath, newDepth, accumulator, currentPartialKeyPath);
            }
        }

        internal virtual void ResolveChildKeyPath(KeyPath keyPath, int depth, List<KeyPath> accumulator, KeyPath currentPartialKeyPath)
        {
        }

        public virtual void AddValueCallback<T>(LottieProperty property, ILottieValueCallback<T> callback)
        {
            Transform.ApplyValueCallback(property, callback);
        }
    }
}