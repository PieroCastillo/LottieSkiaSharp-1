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
using LottieUWP.Model.Content;
using SkiaSharp;
using LottieUWP.Expansion;

namespace LottieUWP.Animation.Content
{
    internal class MergePathsContent : IPathContent, IGreedyContent
    {
        private SKPath _firstPath = new SKPath();
        private readonly SKPath _remainderPath = new SKPath();
        private readonly SKPath _path = new SKPath();

        private readonly List<IPathContent> _pathContents = new List<IPathContent>();
        private readonly MergePaths _mergePaths;

        internal MergePathsContent(MergePaths mergePaths)
        {
            Name = mergePaths.Name;
            _mergePaths = mergePaths;
        }

        public void AbsorbContent(List<IContent> contents)
        {
            var index = contents.Count;
            // Fast forward the iterator until after this content.
            while (index > 0)
            {
                index--;
                if (contents[index] == this)
                    break;
            }
            while (index > 0)
            {
                index--;
                var content = contents[index];
                if (content is IPathContent pathContent)
                {
                    _pathContents.Add(pathContent);
                    contents.RemoveAt(index);
                }
            }
        }

        public void SetContents(List<IContent> contentsBefore, List<IContent> contentsAfter)
        {
            for (var i = 0; i < _pathContents.Count; i++)
            {
                _pathContents[i].SetContents(contentsBefore, contentsAfter);
            }
        }

        public SKPath Path
        {
            get
            {
                _path.Reset();

                if (_mergePaths.IsHidden)
                {
                    return _path;
                }

                switch (_mergePaths.Mode)
                {
                    case MergePaths.MergePathsMode.Merge:
                        AddPaths();
                        break;
                    case MergePaths.MergePathsMode.Add:
                        OpFirstPathWithRest(SKPathOp.Union);
                        break;
                    case MergePaths.MergePathsMode.Subtract:
                        OpFirstPathWithRest(SKPathOp.Difference);
                        break;
                    case MergePaths.MergePathsMode.Intersect:
                        OpFirstPathWithRest(SKPathOp.Intersect);
                        break;
                    case MergePaths.MergePathsMode.ExcludeIntersections:
                        OpFirstPathWithRest(SKPathOp.Xor);
                        break;
                }

                return _path;
            }
        }

        public string Name { get; }

        private void AddPaths()
        {
            for (var i = 0; i < _pathContents.Count; i++)
            {
                _path.AddPath(_pathContents[i].Path);
            }
        }

        private void OpFirstPathWithRest(SKPathOp op)
        {
            _remainderPath.Reset();
            _firstPath.Reset();

            for (var i = _pathContents.Count - 1; i >= 1; i--)
            {
                var content = _pathContents[i];

                if (content is ContentGroup contentGroup)
                {
                    var pathList = contentGroup.PathList;
                    for (var j = pathList.Count - 1; j >= 0; j--)
                    {
                        var path = pathList[j].Path;
                        path.Transform(contentGroup.TransformationMatrix.ToSKMatrix());
                        _remainderPath.AddPath(path);
                    }
                }
                else
                {
                    _remainderPath.AddPath(content.Path);
                }
            }

            var lastContent = _pathContents[0];
            if (lastContent is ContentGroup group)
            {
                var pathList = group.PathList;
                for (var j = 0; j < pathList.Count; j++)
                {
                    var path = pathList[j].Path;
                    path.Transform(group.TransformationMatrix.ToSKMatrix());
                    _firstPath.AddPath(path);
                }
            }
            else
            {
                _firstPath = lastContent.Path;
            }

            _firstPath.Op( _remainderPath, op,_firstPath);
        }
    }
}