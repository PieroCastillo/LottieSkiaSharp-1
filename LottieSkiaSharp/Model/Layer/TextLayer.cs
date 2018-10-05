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
using SkiaSharp;
using LottieUWP.Animation.Content;
using LottieUWP.Animation.Keyframe;
using LottieUWP.Value;
using LottieUWP.Expansion;

namespace LottieUWP.Model.Layer
{
    internal class TextLayer : BaseLayer
    {
        //private Rect _rectF;
        private readonly SKPaint _fillPaint = new SKPaint() { IsAntialias = true, Style = SKPaintStyle.Fill };//TODO:
        private readonly SKPaint _strokePaint = new SKPaint() { IsAntialias = true, Style = SKPaintStyle.Stroke };//TODO:

        private readonly Dictionary<FontCharacter, List<ContentGroup>> _contentsForCharacter = new Dictionary<FontCharacter, List<ContentGroup>>();
        private readonly TextKeyframeAnimation _textAnimation;
        private readonly ILottieDrawable _lottieDrawable;
        private readonly LottieComposition _composition;
        private readonly IBaseKeyframeAnimation<SKColor?, SKColor?> _colorAnimation;
        private readonly IBaseKeyframeAnimation<SKColor?, SKColor?> _strokeColorAnimation;
        private readonly IBaseKeyframeAnimation<float?, float?> _strokeWidthAnimation;
        private readonly IBaseKeyframeAnimation<float?, float?> _trackingAnimation;

        internal TextLayer(ILottieDrawable lottieDrawable, Layer layerModel) : base(lottieDrawable, layerModel)
        {
            _lottieDrawable = lottieDrawable;
            _composition = layerModel.Composition;
            _textAnimation = (TextKeyframeAnimation)layerModel.Text.CreateAnimation();
            _textAnimation.ValueChanged += OnValueChanged;
            AddAnimation(_textAnimation);

            var textProperties = layerModel.TextProperties;
            if (textProperties?._color != null)
            {
                _colorAnimation = textProperties._color.CreateAnimation();
                _colorAnimation.ValueChanged += OnValueChanged;
                AddAnimation(_colorAnimation);
            }

            if (textProperties?._stroke != null)
            {
                _strokeColorAnimation = textProperties._stroke.CreateAnimation();
                _strokeColorAnimation.ValueChanged += OnValueChanged;
                AddAnimation(_strokeColorAnimation);
            }

            if (textProperties?._strokeWidth != null)
            {
                _strokeWidthAnimation = textProperties._strokeWidth.CreateAnimation();
                _strokeWidthAnimation.ValueChanged += OnValueChanged;
                AddAnimation(_strokeWidthAnimation);
            }

            if (textProperties?._tracking != null)
            {
                _trackingAnimation = textProperties._tracking.CreateAnimation();
                _trackingAnimation.ValueChanged += OnValueChanged;
                AddAnimation(_trackingAnimation);
            }
        }

        public override void DrawLayer(SKCanvas canvas, Matrix3X3 parentMatrix, byte parentAlpha)
        {
            canvas.Save();
            if (!_lottieDrawable.UseTextGlyphs())
            {
                canvas.SetMatrix(parentMatrix.ToSKMatrix());
            }
            var documentData = _textAnimation.Value;
            if (!_composition.Fonts.TryGetValue(documentData.FontName, out var font))
            {
                // Something is wrong. 
                canvas.Restore();
                return;
            }

            _fillPaint.Color = _colorAnimation?.Value ?? documentData.Color;

            _strokePaint.Color = _strokeColorAnimation?.Value ?? documentData.StrokeColor;
            var alpha = (byte)(Transform.Opacity.Value * 255 / 100f);
            _fillPaint.SetAlpha(alpha);
            _strokePaint.SetAlpha(alpha);

            if (_strokeWidthAnimation?.Value != null)
            {
                _strokePaint.StrokeWidth = _strokeWidthAnimation.Value.Value;
            }
            else
            {
                var parentScale = Utils.Utils.GetScale(parentMatrix);
                _strokePaint.StrokeWidth = (float)(documentData.StrokeWidth * Utils.Utils.DpScale() * parentScale);
            }

            if (_lottieDrawable.UseTextGlyphs())
            {
                DrawTextGlyphs(documentData, parentMatrix, font, canvas);
            }
            else
            {
                DrawTextWithFont(documentData, font, parentMatrix, canvas);
            }

            canvas.Restore();
        }

        private void DrawTextGlyphs(DocumentData documentData, Matrix3X3 parentMatrix, Font font, SKCanvas canvas)
        {
            var fontScale = (float)documentData.Size / 100;
            var parentScale = Utils.Utils.GetScale(parentMatrix);
            var text = documentData.Text;

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var characterHash = FontCharacter.HashFor(c, font.Family, font.Style);
                if (!_composition.Characters.TryGetValue(characterHash, out var character))
                {
                    // Something is wrong. Potentially, they didn't export the text as a glyph. 
                    continue;
                }
                DrawCharacterAsGlyph(character, parentMatrix, fontScale, documentData, canvas);
                var tx = (float)character.Width * fontScale * Utils.Utils.DpScale() * parentScale;
                // Add tracking 
                var tracking = documentData.Tracking / 10f;
                if (_trackingAnimation?.Value != null)
                {
                    tracking += _trackingAnimation.Value.Value;
                }
                tx += tracking * parentScale;
                canvas.Translate(tx, 0);
            }
        }

        private void DrawTextWithFont(DocumentData documentData, Font font, Matrix3X3 parentMatrix, SKCanvas canvas)
        {
            var parentScale = Utils.Utils.GetScale(parentMatrix);
            var typeface = _lottieDrawable.GetTypeface(font.Family, font.Style);
            if (typeface == null)
            {
                return;
            }
            var text = documentData.Text;
            var textDelegate = _lottieDrawable.TextDelegate;
            if (textDelegate != null)
            {
                text = textDelegate.GetTextInternal(text);
            }
            _fillPaint.Typeface = typeface;
            _fillPaint.TextSize = (float)documentData.Size * Utils.Utils.DpScale();
            _strokePaint.Typeface = _fillPaint.Typeface;
            _strokePaint.TextSize = _fillPaint.TextSize;
            for (var i = 0; i < text.Length; i++)
            {
                var character = text[i];
                var size = DrawCharacterFromFont(character, documentData, canvas);

                // Add tracking
                var tracking = documentData.Tracking / 10f;
                if (_trackingAnimation?.Value != null)
                {
                    tracking += _trackingAnimation.Value.Value;
                }
                var tx = (float)(size?.Width ?? 0) + tracking * parentScale;
                canvas.Translate(tx, 0);
            }
        }

        private void DrawCharacterAsGlyph(FontCharacter character, Matrix3X3 parentMatrix, float fontScale, DocumentData documentData, SKCanvas canvas)
        {
            var contentGroups = GetContentsForCharacter(character);
            for (var j = 0; j < contentGroups.Count; j++)
            {
                var path = contentGroups[j].Path;
                //path.ComputeBounds(out _rectF);
                Matrix.Set(parentMatrix);
                Matrix = MatrixExt.PreTranslate(Matrix, 0, (float)-documentData.BaselineShift * Utils.Utils.DpScale());
                Matrix = MatrixExt.PreScale(Matrix, fontScale, fontScale);
                path.Transform(Matrix.ToSKMatrix());
                if (documentData.StrokeOverFill)
                {
                    DrawGlyph(path, _fillPaint, canvas);
                    DrawGlyph(path, _strokePaint, canvas);
                }
                else
                {
                    DrawGlyph(path, _strokePaint, canvas);
                    DrawGlyph(path, _fillPaint, canvas);
                }
            }
        }

        private void DrawGlyph(SKPath path, SKPaint paint, SKCanvas canvas)
        {
            if (paint.Color == SKColors.Transparent)
            {
                return;
            }
            if (paint.Style == SKPaintStyle.Stroke && paint.StrokeWidth == 0)
            {
                return;
            }
            canvas.DrawPath(path, paint);
        }

        private SKRect? DrawCharacterFromFont(char c, DocumentData documentData, SKCanvas canvas)
        {
            SKRect? ret;
            if (documentData.StrokeOverFill)
            {
                ret = DrawCharacter(c, _fillPaint, canvas);
                return DrawCharacter(c, _strokePaint, canvas) ?? ret;
            }

            ret = DrawCharacter(c, _strokePaint, canvas);
            return DrawCharacter(c, _fillPaint, canvas) ?? ret;
        }

        private SKRect? DrawCharacter(char character, SKPaint paint, SKCanvas canvas)
        {
            if (paint.Color == SKColors.Transparent)
            {
                return null;
            }
            if (paint.Style == SKPaintStyle.Stroke && paint.StrokeWidth == 0)
            {
                return null;
            }
            SKRect sKRect = SKRect.Empty;
            paint.MeasureText(character.ToString(), ref sKRect);
            canvas.DrawText(character.ToString(), new SKPoint(0,0),paint);
            return sKRect;
        }

        private List<ContentGroup> GetContentsForCharacter(FontCharacter character)
        {
            if (_contentsForCharacter.ContainsKey(character))
            {
                return _contentsForCharacter[character];
            }
            var shapes = character.Shapes;
            var size = shapes.Count;
            var contents = new List<ContentGroup>(size);
            for (var i = 0; i < size; i++)
            {
                var sg = shapes[i];
                contents.Add(new ContentGroup(_lottieDrawable, this, sg));
            }
            _contentsForCharacter[character] = contents;
            return contents;
        }

        public override void AddValueCallback<T>(LottieProperty property, ILottieValueCallback<T> callback)
        {
            base.AddValueCallback(property, callback);
            if (property == LottieProperty.Color && _colorAnimation != null)
            {
                _colorAnimation.SetValueCallback((ILottieValueCallback<SKColor?>)callback);
            }
            else if (property == LottieProperty.StrokeColor && _strokeColorAnimation != null)
            {
                _strokeColorAnimation.SetValueCallback((ILottieValueCallback<SKColor?>)callback);
            }
            else if (property == LottieProperty.StrokeWidth && _strokeWidthAnimation != null)
            {
                _strokeWidthAnimation.SetValueCallback((ILottieValueCallback<float?>)callback);
            }
            else if (property == LottieProperty.TextTracking)
            {
                _trackingAnimation?.SetValueCallback((ILottieValueCallback<float?>)callback);
            }
        }
    }
}
