using System.Collections.Generic;
using LottieUWP.Model.Content;
using LottieUWP.Utils;
using LottieUWP.Value;
using SkiaSharp;

namespace LottieUWP.Animation.Keyframe
{
    internal class ShapeKeyframeAnimation : BaseKeyframeAnimation<ShapeData, SKPath>
    {
        private readonly ShapeData _tempShapeData = new ShapeData();
        private readonly SKPath _tempPath = new SKPath();

        internal ShapeKeyframeAnimation(List<Keyframe<ShapeData>> keyframes) : base(keyframes)
        {
        }

        public override SKPath GetValue(Keyframe<ShapeData> keyframe, float keyframeProgress)
        {
            var startShapeData = keyframe.StartValue;
            var endShapeData = keyframe.EndValue;

            _tempShapeData.InterpolateBetween(startShapeData, endShapeData, keyframeProgress);
            MiscUtils.GetPathFromData(_tempShapeData, _tempPath);
            return _tempPath;
        }
    }
}