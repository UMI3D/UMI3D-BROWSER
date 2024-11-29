/*
Copyright 2019 - 2024 Inetum

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.ui.thumbnails
{
    internal class ThumbnailsTests : MonoBehaviour
    {
        ThumbnailsModelContainer model;

        void Awake()
        {
            model = GetComponent<ThumbnailsModelContainer>();
        }

        void Start()
        {
            AddThumbnails();
        }

        void OnDisable()
        {
        }

        void OnDestroy()
        {
        }

        [ContextMenu("Add thumbnails")]
        void AddThumbnails()
        {
            ThumbnailModel thumbnails1 = new()
            {
                name = "Thumbnail 1",
            };
            model.model.Add(thumbnails1);

            ThumbnailModel thumbnails2 = new()
            {
                name = "Thumbnail 2",
                isFavorite = true,
            };
            model.model.Add(thumbnails2);

            ThumbnailModel thumbnails3 = new()
            {
                name = "Thumbnail 3",
                isFavorite = true,
            };
            model.model.Add(thumbnails3);
        }
    }
}