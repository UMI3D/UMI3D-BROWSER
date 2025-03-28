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
using TMPro;
using UnityEngine;

namespace umi3d.browserRuntime.text
{
    public class TextFactory : MonoBehaviour
    {
        public class Settings
        {
            public Vector3? Position;
            public Vector2? Size;

            public Vector2? AnchorMin;
            public Vector2? AnchorMax;
            public Vector2? Pivot;

            public int? FontSize;
            public Color? TextColor;
            public FontStyles? FontStyles;
            public TextAlignmentOptions? TextAlignmentOptions;
        }

        [SerializeField] internal TextModelContainer _textPrefab;

        internal Queue<TextModelContainer> _pool = new();

        public GameObject GetOrCreateText(Transform parent, string text = "", Settings settings = null)
        {
            if (settings == null)
                settings = new();

            TextModelContainer modelContainer;
            if (!_pool.TryDequeue(out modelContainer))
                modelContainer = Instantiate(_textPrefab);

            modelContainer.gameObject.SetActive(true);
            modelContainer.transform.SetParent(parent, false);

            modelContainer.Model.SetText(text);
            modelContainer.Model.SetPosition(settings.Position);
            modelContainer.Model.SetSize(settings.Size);
            modelContainer.Model.SetAnchor(settings.AnchorMin, settings.AnchorMax, settings.Pivot);
            modelContainer.Model.SetTextStyle(settings.FontSize, settings.TextColor, settings.FontStyles, settings.TextAlignmentOptions);

            return modelContainer.gameObject;
        }

        public void ReturnText(GameObject gameObject)
        {
            if (!gameObject)
                return;
            var modelContainer = gameObject.GetComponent<TextModelContainer>();
            if (!modelContainer)
                return;

            modelContainer.gameObject.SetActive(false);
            modelContainer.transform.SetParent(transform, false);


            _pool.Enqueue(modelContainer);
        }
    }
}