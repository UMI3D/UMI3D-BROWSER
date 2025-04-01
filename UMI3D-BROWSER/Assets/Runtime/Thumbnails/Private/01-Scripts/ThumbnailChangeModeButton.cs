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

namespace umi3d.browserRuntime.thumbnails
{
    [RequireComponent(typeof(Button)), ExecuteInEditMode]

    internal class ThumbnailChangeModeButton : MonoBehaviour
    {
        [SerializeField] private List<ThumbnailMode> _modes = new List<ThumbnailMode>();
        private int _currentModeIndex = 0;

        private ThumbnailListModelContainer _modelContainer;
        private Button _button;

        private void Awake()
        {
            _modelContainer = GetComponentInParent<ThumbnailListModelContainer>();
            _button = GetComponent<Button>();

            if (_modes.Count > 0)
                _modelContainer.Model.ChangeModeTo(_modes[0]);

            _button.onClick.AddListener(CycleMode);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(CycleMode);
        }

        private void CycleMode()
        {
            if (_modes.Count == 0)
                return;

            _currentModeIndex = (_currentModeIndex + 1) % _modes.Count;
            _modelContainer.Model.ChangeModeTo(_modes[_currentModeIndex]);
        }
    }
}