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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.thumbnails
{
    [ExecuteInEditMode]
    public class ThumbnailFactory : MonoBehaviour
    {
        [SerializeField] internal Transform _content;
        [SerializeField] private ThumbnailModelContainer _thumbnailPrefab;

        private ThumbnailListModelContainer _thumbnailListModelContainer;

        internal Queue<ThumbnailModelContainer> _pool = new();

        private void Awake()
        {
            _thumbnailListModelContainer = GetComponent<ThumbnailListModelContainer>();

            if (_thumbnailListModelContainer)
            {
                _thumbnailListModelContainer.Model.CreateThumbnail += GetOrCreateThumbnailForList;
                _thumbnailListModelContainer.Model.CreateThumbnailTemp += GetOrCreateThumbnailTempForList;
                _thumbnailListModelContainer.Model.RemoveThumbnail += ReturnThumbnailForList;
            }
        }

        private void OnDestroy()
        {
            if (_thumbnailListModelContainer)
            {
                _thumbnailListModelContainer.Model.CreateThumbnail -= GetOrCreateThumbnailForList;
                _thumbnailListModelContainer.Model.CreateThumbnailTemp -= GetOrCreateThumbnailTempForList;
                _thumbnailListModelContainer.Model.RemoveThumbnail -= ReturnThumbnailForList;
            }
        }

        public ThumbnailModelContainer GetOrCreateThumbnail(string name = "", Sprite image = null, Action callback = null, Color? normalColor = null, Color? hoverColor = null)
        {
            if (!_content)
                _content = transform;

            ThumbnailModelContainer thumbnail;
            if (!_pool.TryDequeue(out thumbnail))
                thumbnail = Instantiate(_thumbnailPrefab);

            thumbnail.gameObject.SetActive(true);
            thumbnail.transform.SetParent(_content, false);

            thumbnail.Model.SetName(name);
            thumbnail.Model.SetImage(image);
            thumbnail.Model.SetCallback(callback);
            thumbnail.Model.SetColors(normalColor, hoverColor);

            return thumbnail;
        }

        public void ReturnThumbnail(ThumbnailModelContainer modelContainer)
        {
            if (!modelContainer)
                return;

            modelContainer.gameObject.SetActive(false);
            modelContainer.transform.SetParent(transform, false);
            modelContainer.Model.SetCallback(null);

            _pool.Enqueue(modelContainer);
        }

        private void GetOrCreateThumbnailForList(string name, Sprite image, Action callback, Color? normalColor = null, Color? hoverColor = null)
        {
            var modelContainer = GetOrCreateThumbnail(name, image, callback, normalColor, hoverColor);
            _thumbnailListModelContainer.Model.Thumbnails.Add(modelContainer.Model);
            _thumbnailListModelContainer.Model.ThumbnailContainers.Add(modelContainer);
        }

        private void GetOrCreateThumbnailTempForList()
        {
            var modelContainer = GetOrCreateThumbnail(null, null, null, null);
            _thumbnailListModelContainer.Model._thumbnailContainersTemp.Add(modelContainer);
        }

        private void ReturnThumbnailForList(ThumbnailModelContainer container)
        {
            _thumbnailListModelContainer.Model.ThumbnailContainers.Remove(container);
            _thumbnailListModelContainer.Model._thumbnailContainersTemp.Remove(container);
            ReturnThumbnail(container);
        }

#if UNITY_EDITOR
        [ContextMenu("Get or create thumbnail test")]
        public void GetOrCreateThumbnailTest()
        {
            GetOrCreateThumbnail("Test", null, () => Debug.Log("Thumbnail clicked!"), Color.gray, Color.white);
        }

        [ContextMenu("Return thumbnail test")]
        public void ReturnThumbnailTest()
        {
            if (transform.childCount > 0)
                ReturnThumbnail(transform.GetChild(0).GetComponent<ThumbnailModelContainer>());
        }

        [ContextMenu("[List] Get or create thumbnail test")]
        public void GetOrCreateThumbnailForListTest()
        {
            _thumbnailListModelContainer.Model.AddThumbnail("Test", null, () => Debug.Log("Thumbnail clicked!"), Color.gray, Color.white);
        }

        [ContextMenu("[List] Clear thumbnail test")]
        public void ClearThumbnailForListTest()
        {
            _thumbnailListModelContainer.Model.ClearThumbnails();
        }
#endif
    }
}