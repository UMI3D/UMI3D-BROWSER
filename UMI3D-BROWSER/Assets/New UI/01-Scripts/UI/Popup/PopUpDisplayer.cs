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
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;

namespace umi3d.browserRuntime.ui.popup
{
    public class PopupDisplayer : MonoBehaviour
    {
        [SerializeField] private LocalizeStringEvent title;
        [SerializeField] private LocalizeStringEvent description;
        [SerializeField] private GameObject buttonGroup;
        [SerializeField] private PopupType type;

        [Header("Prefabs")]
        [SerializeField] private GameObject buttonPrefab;

        private void Awake()
        {
            NotificationHub.Default.Subscribe<PopupNotificationKeys.Show>(this, Show);
            NotificationHub.Default.Subscribe<PopupNotificationKeys.CloseAll>(this, Close);

            gameObject.SetActive(false);
        }

        private void Show(Notification notification)
        {
            // Verify type
            PopupType popupType;
            if (!notification.TryGetInfoT<PopupType>(PopupNotificationKeys.Show.Type, out popupType))
                return;

            if (popupType != type)
            {
                gameObject.SetActive(false);
                return;
            }

            // Set Arguments
            if (notification.TryGetInfoT<Dictionary<string, object>>(PopupNotificationKeys.Show.Arguments, out var arguments))
            {
                title.StringReference.Arguments = new object[] { arguments };
                description.StringReference.Arguments = new object[] { arguments };
            }

            // Set Texts
            string mTitle, mDescription;
            if (!notification.TryGetInfoT<string>(PopupNotificationKeys.Show.Title, out mTitle))
                mTitle = "empty";
            if (!notification.TryGetInfoT<string>(PopupNotificationKeys.Show.Description, out mDescription))
                mDescription = "empty";
            title.SetEntry(mTitle);
            description.SetEntry(mDescription);

            // Set buttons
            if (notification.TryGetInfoT<List<(string, Action)>>(PopupNotificationKeys.Show.Buttons, out var buttons))
            {
                ClearButtons();

                foreach (var button in buttons)
                    CreateButton(button);
            }
            gameObject.SetActive(true);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }

        private void ClearButtons()
        {
            for (var i = buttonGroup.transform.childCount - 1; i >= 0; i--)
                Destroy(buttonGroup.transform.GetChild(i).gameObject);
        }

        private GameObject CreateButton((string, Action) button)
        {
            var buttonObject = Instantiate(buttonPrefab, buttonGroup.transform).GetComponent<SimpleButton>();
            buttonObject.GetComponentInChildren<LocalizeStringEvent>().SetEntry(button.Item1);
            buttonObject.OnClick.AddListener(new UnityAction(button.Item2));

            return buttonObject.gameObject;
        }
    }

}
