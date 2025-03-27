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
using UnityEngine.UI;

namespace umi3d.browserRuntime.text
{
    public class TextFactory : MonoBehaviour
    {
        public class Settings
        {
            public class TransformSettings
            {
                public Vector3 Position = Vector2.zero;
                public Vector3 Size = Vector2.one;
            }
            public TransformSettings Transform = null;

            public class AnchorSettings
            {
                public Vector2 AnchorMin = new Vector2(.5f, .5f);
                public Vector2 AnchorMax = new Vector2(.5f, .5f);
                public Vector2 Pivot = new Vector2(.5f, .5f);
            }
            public AnchorSettings Anchor = null;

            public class TextStyleSettings
            {
                public int FontSize = 12;
                public Color Color = Color.white;
                public FontStyles FontStyles = FontStyles.Normal;
                public TextAlignmentOptions TextAlignementOptions = TextAlignmentOptions.MidlineLeft;
            }
            public TextStyleSettings TextStyle = null;
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
            if (settings.Transform != null)
            {
                modelContainer.Model.SetPosition(settings.Transform.Position);
                modelContainer.Model.SetSize(settings.Transform.Size);
            }
            if (settings.Anchor != null)
            {
                modelContainer.Model.SetAnchor(settings.Anchor.AnchorMin, settings.Anchor.AnchorMax, settings.Anchor.Pivot);
            }
            if (settings.TextStyle != null)
            {
                modelContainer.Model.SetTextStyle(settings.TextStyle.FontSize, settings.TextStyle.Color, settings.TextStyle.FontStyles, settings.TextStyle.TextAlignementOptions);
            }

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