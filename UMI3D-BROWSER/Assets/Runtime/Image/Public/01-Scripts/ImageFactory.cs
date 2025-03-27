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

using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.image
{
    public class ImageFactory : MonoBehaviour
    {
        [SerializeField] ImageModelContainer _imagePrefab;

        internal Queue<ImageModelContainer> _pool = new ();

        public GameObject GetOrCreateImage(Transform parent, Color color, Sprite sprite = null)
        {
            ImageModelContainer modelContainer;
            if (!_pool.TryDequeue(out modelContainer))
                modelContainer = Instantiate(_imagePrefab);

            modelContainer.gameObject.SetActive(true);
            modelContainer.transform.SetParent(parent, false);
            modelContainer.Model.SetSprite(sprite);
            modelContainer.Model.SetColor(color);

            return modelContainer.gameObject;
        }

        public GameObject GetOrCreateImage(Transform parent, Sprite sprite = null)
        {
            return GetOrCreateImage(parent, Color.white, sprite);
        }

        public GameObject GetOrCreateImage(Transform parent, Color color, Texture2D texture)
        {
            return GetOrCreateImage(
                parent,
                color,
                Sprite.Create(
                    texture, 
                    new Rect(0, 0, texture.width, texture.height), 
                    new Vector2(.5f, .5f)
                )
            );
        }

        public GameObject GetOrCreateImage(Transform parent, Texture2D texture)
        {
            return GetOrCreateImage(parent, Color.white, texture);
        }

        public void ReturnImage(GameObject gameObject)
        {
            if (!gameObject) 
                return;
            var modelContainer = gameObject.GetComponent<ImageModelContainer>();
            if (!modelContainer) 
                return;

            modelContainer.gameObject.SetActive(false);
            modelContainer.transform.SetParent(transform, false);

            _pool.Enqueue(modelContainer);
        }
    }
}