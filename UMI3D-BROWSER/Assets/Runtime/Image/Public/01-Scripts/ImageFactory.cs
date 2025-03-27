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
using UnityEngine.UI;

namespace umi3d.browserRuntime.image
{
    public class ImageFactory : MonoBehaviour
    {
        public class Settings
        {
            public Color Color = Color.white;
            public class TransformSettings
            {
                public Vector3 Position = Vector3.zero;
                public Vector3 Size = Vector3.one;
            }
            public TransformSettings Transform = null;
            public class AnchorSettings
            {
                public Vector2 AnchorMin = new Vector2(0.5f, 0.5f);
                public Vector2 AnchorMax = new Vector2(0.5f, 0.5f);
                public Vector2 Pivot = new Vector2(0.5f, 0.5f);
            }
            public AnchorSettings Anchor = null;
        }

        [SerializeField] ImageModelContainer _imagePrefab;

        internal Queue<ImageModelContainer> _pool = new ();

        public GameObject GetOrCreateImage(Transform parent, Sprite sprite = null, Settings settings = null)
        {
            if (settings == null)
                settings = new();

            ImageModelContainer modelContainer;
            if (!_pool.TryDequeue(out modelContainer))
                modelContainer = Instantiate(_imagePrefab);

            modelContainer.gameObject.SetActive(true);
            modelContainer.transform.SetParent(parent, false);
            modelContainer.Model.SetSprite(sprite);
            modelContainer.Model.SetColor(settings.Color);
            if (settings.Transform != null)
            {
                modelContainer.Model.SetPosition(settings.Transform.Position);
                modelContainer.Model.SetSize(settings.Transform.Size);
            }
            if (settings.Anchor != null)
            {
                modelContainer.Model.SetAnchor(settings.Anchor.AnchorMin, settings.Anchor.AnchorMax, settings.Anchor.Pivot);
            }

            return modelContainer.gameObject;
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