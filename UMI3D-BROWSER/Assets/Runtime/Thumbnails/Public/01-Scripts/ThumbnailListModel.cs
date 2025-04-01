/*
Copyright 2019 - 2025 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using inetum.unityUtils.observation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace umi3d.browserRuntime.thumbnails
{
    public class ThumbnailListModel
    {
        public List<ThumbnailModel> Thumbnails { get; private set; } = new();
        public ThumbnailMode Mode { get; private set; } = new ThumbnailMode();

        internal List<ThumbnailModelContainer> _thumbnailContainers = new();
        internal List<ThumbnailModelContainer> _thumbnailContainersTemp = new List<ThumbnailModelContainer>();

        internal Action<string, Sprite, Action, ThumbnailFactory.Settings> CreateThumbnail;
        internal Action CreateThumbnailTemp;
        internal Action<ThumbnailModelContainer> RemoveThumbnail;

        private readonly Notifier _changeModeNotifier;

        public ThumbnailListModel()
        {
            _changeModeNotifier = NotificationHub.Default.GetNotifier(this, ID.FromType<ThumbnailNotificationKeys.ChangeMode>());

            FillWithTempThumbnails();
        }

        public void AddThumbnail(string name = "", Sprite image = null, Action callback = null, ThumbnailFactory.Settings settings = null)
        {
            for (int i = _thumbnailContainersTemp.Count - 1; i >= 0; i--)
                RemoveThumbnail?.Invoke(_thumbnailContainersTemp[i]);
            CreateThumbnail?.Invoke(name, image, callback, settings);

            FillWithTempThumbnails();
        }

        public void ClearThumbnails()
        {
            for (int i = _thumbnailContainersTemp.Count - 1; i >= 0; i--)
                RemoveThumbnail?.Invoke(_thumbnailContainersTemp[i]);
            for (int i = _thumbnailContainers.Count - 1; i >= 0; i--)
                RemoveThumbnail?.Invoke(_thumbnailContainers[i]);

            Thumbnails.Clear();

            FillWithTempThumbnails();
        }

        public void ChangeModeTo(ThumbnailMode mode)
        {
            Mode = mode;

            _changeModeNotifier[ThumbnailNotificationKeys.ChangeMode.Mode] = Mode;
            _changeModeNotifier.Notify();

            for (int i = _thumbnailContainersTemp.Count - 1; i >= 0; i--)
                RemoveThumbnail?.Invoke(_thumbnailContainersTemp[i]);
            FillWithTempThumbnails();
        }

        private void FillWithTempThumbnails()
        {
            var nbrTemp = Mode.NbrRow * Mode.NbrColumn - Thumbnails.Count;
            for (int i = 0; i < nbrTemp; i++)
                CreateThumbnailTemp?.Invoke();
        }
    }
}