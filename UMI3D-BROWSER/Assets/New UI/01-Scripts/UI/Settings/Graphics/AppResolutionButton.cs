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

using inetum.unityUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings.graphics
{
    [RequireComponent(typeof(Button))]
    public class AppResolutionButton : MonoBehaviour
    {
        enum ResolutionEnum
        {
            Low, Medium, High, Custom
        }


        [SerializeField] private ResolutionEnum resolution;
        [SerializeField] private List<GameObject> gameObjects;
        [SerializeField] private string toggleGroup;
        [SerializeField] private static int maxFps;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => {
                QualitySettings.SetQualityLevel((int)resolution, false);
            });
            NotificationHub.Default.Subscribe(this, SettingsNotificationKeys.NewToggleCustomSelected + toggleGroup, Selected);
        }

        private void Start()
        {
            var myResolution = QualitySettings.GetQualityLevel();
            if (myResolution == (int)resolution)
                button.onClick?.Invoke();
        }

        private void Selected(Notification notification)
        {
            var active = (notification.Publisher as MonoBehaviour).gameObject == gameObject;
            foreach (var item in gameObjects)
                item.SetActive(active);
        }

        private void FullScreenResolutionValueChanged(Resolution value)
        {
            Screen.SetResolution(value.width, value.height, Screen.fullScreen);
        }
    }
}