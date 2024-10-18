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
using utils.tweens;

namespace umi3d.browserRuntime.ui.settings.comfort
{
    [RequireComponent(typeof(Button))]
    public class UiAnimationButton : MonoBehaviour
    {
        [SerializeField] private bool isOn;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => {
                UITweens.ToggleAnimation(isOn);
                PlayerPrefs.SetInt(SettingsPlayerPrefsKeys.UiAnimation, isOn ? 1 : 0);
            });
        }

        private void Start()
        {
            if (isOn == PlayerPrefs.GetInt(SettingsPlayerPrefsKeys.UiAnimation, 1) > 0)
                button.onClick?.Invoke();
        }
    }
}