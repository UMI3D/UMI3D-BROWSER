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

using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    [RequireComponent(typeof(Button))]
    public class ContextualMenuCloseButton : MonoBehaviour
    {
        Button button;

        ContextualMenuController _controller;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);

            _controller = GetComponentInParent<ContextualMenuController>();
        }

        void OnDestroy()
        {
            button.onClick.RemoveListener(OnClick);
        }

        void OnClick()
        {
            _controller.CloseButtonClicked();
        }
    }
}
