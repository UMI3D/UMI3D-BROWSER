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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.button
{
    public class ButtonFactory : MonoBehaviour
    {
        public class Settings
        {
            public class TransformSettings
            {
                public Vector3 Position = Vector2.zero;
                public Vector3 Size = Vector2.one;
            }
            public TransformSettings Transform = null;

            public class ImageSettings
            {
                public Sprite Sprite = null;
                public ColorBlock ColorBlock = ColorBlock.defaultColorBlock;
            }
            public ImageSettings Image = null;

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

        [SerializeField] internal ButtonModelContainer _buttonPrefab;

        internal Queue<ButtonModelContainer> _pool = new();

        public GameObject GetOrCreateButton(Transform parent, string label = "", Action callback = null, Settings settings = null)
        {
            if (settings == null)
                settings = new();

            ButtonModelContainer button;
            if (!_pool.TryDequeue(out button))
                button = Instantiate(_buttonPrefab);
            button.gameObject.SetActive(true);
            button.transform.SetParent(parent, false);

            button.Model.SetLabel(label);
            button.Model.SetCallback(callback);
            if (settings.Transform != null)
            {
                button.Model.SetPosition(settings.Transform.Position);
                button.Model.SetSize(settings.Transform.Size);
            }
            if (settings.Image != null)
            {
                button.Model.SetImage(settings.Image.ColorBlock, 
                    settings.Image.Sprite);
            }
            if (settings.Anchor != null)
            {
                button.Model.SetAnchor(settings.Anchor.AnchorMin, 
                    settings.Anchor.AnchorMax, 
                    settings.Anchor.Pivot);
            }
            if (settings.TextStyle != null)
            {
                button.Model.SetTextStyle(settings.TextStyle.FontSize, 
                    settings.TextStyle.Color, 
                    settings.TextStyle.FontStyles,
                    settings.TextStyle.TextAlignementOptions);
            }

            return button.gameObject;
        }

        public void ReturnButton(GameObject gameObject)
        {
            if (!gameObject)
                return;
            var modelContainer = gameObject.GetComponent<ButtonModelContainer>();
            if (!modelContainer)
                return;

            modelContainer.gameObject.SetActive(false);
            modelContainer.transform.SetParent(transform, false);
            modelContainer.Model.SetCallback(null);


            _pool.Enqueue(modelContainer);
        }
    }
}