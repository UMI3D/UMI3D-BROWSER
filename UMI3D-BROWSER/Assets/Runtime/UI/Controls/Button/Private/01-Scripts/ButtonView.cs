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

using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui
{
    [RequireComponent(typeof(Button)), ExecuteInEditMode]
    internal class ButtonView : MonoBehaviour, IButtonImageObserver
    {
        Button _button;

        ButtonController _controller;
        ButtonModel _model;

        void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);

            _controller= GetComponent<ButtonController>();
            _model = _controller.model;
            _model.Subscribe(this as IButtonImageObserver);
        }

        void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
            _model?.Unsubscribe(this as IButtonImageObserver);
        }

        public void UpdateImage(Sprite sprite, ColorBlock colorBlock)
        {
            _button.image.sprite = sprite;

            colorBlock.colorMultiplier = _button.colors.colorMultiplier;
            colorBlock.fadeDuration = _button.colors.fadeDuration;
            _button.colors = colorBlock;
        }

        void OnClick()
        {
            _controller.OnClick();
        }
    }
}