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

namespace umi3d.browserRuntime.thumbnails
{
    public class ThumbnailFactory : MonoBehaviour
    {
        public class Settings
        {
            public Color? NormalColor;
            public Color? HoverColor;
        }

        [SerializeField] private Transform _content;
        [SerializeField] private ThumbnailModelContainer _thumbnailPrefab;

        internal Queue<ThumbnailModelContainer> _pool = new();

        public GameObject GetOrCreateThumbnail(string name = "", Sprite image = null, Action callback = null, Settings settings = null)
        {
            if (!_content)
                _content = transform;

            ThumbnailModelContainer thumbnail;
            if (!_pool.TryDequeue(out thumbnail))
                thumbnail = Instantiate(_thumbnailPrefab);

            thumbnail.gameObject.SetActive(true);
            thumbnail.transform.SetParent(_content, false);

            if (settings != null)
            {
                thumbnail.Model.SetName(name);
                thumbnail.Model.SetImage(image);
                thumbnail.Model.SetCallback(callback);
            }

            return thumbnail.gameObject;

        }

        public void ReturnThumbnail(GameObject gameObject)
        {
            if (!gameObject)
                return;
            var modelContainer = gameObject.GetComponent<ThumbnailModelContainer>();
            if (!modelContainer)
                return;

            modelContainer.gameObject.SetActive(false);
            modelContainer.transform.SetParent(transform, false);
            modelContainer.Model.SetCallback(null);

            _pool.Enqueue(modelContainer);
        }

#if UNITY_EDITOR
        [ContextMenu("Get or create thumbnail test")]
        public void GetOrCreateThumbnailTest()
        {
            var settings = new Settings() {
                NormalColor = Color.gray,
                HoverColor = Color.white,
            };
            GetOrCreateThumbnail("Test", null, () => Debug.Log("Thumbnail clicked!"), settings);
        }

        [ContextMenu("Return thumbnail test")]
        public void ReturnThumbnailTest()
        {
            if (transform.childCount > 0)
                ReturnThumbnail(transform.GetChild(0).gameObject);
        }
#endif
    }
}