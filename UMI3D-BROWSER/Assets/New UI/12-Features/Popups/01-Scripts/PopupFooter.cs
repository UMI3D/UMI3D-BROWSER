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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.ui.popup
{
    public class PopupFooter : MonoBehaviour
    {
        [SerializeField] GameObject ButtonPrefab;

        List<PopupButton> activatedButtons = new();
        List<PopupButton> deactivatedButtons = new();

        void Awake()
        {
            UnityEngine.Debug.Log($"message");
            NotificationHub.Default
                .Subscribe<PopupNotificationKeys.Show>(
                this,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher is PopupManager),
                NewPopup
            );
        }

        void OnDestroy()
        {
            NotificationHub.Default
             .Unsubscribe<PopupNotificationKeys.Show>(this);
        }

        void NewPopup(Notification notification)
        {
            DeactivateAllButtons();

            if (!notification.TryGetInfoT(PopupNotificationKeys.Show.Buttons, out List<System.Object> list))
            {
                return;
            }

            if (!notification.TryGetInfoT(PopupNotificationKeys.Show.ButtonActions, out Action<int> action))
            {
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                PopupButton button = ActiveButton();
                button.index = i;
                button.action = action;

                System.Object _text = list[i];
                if (_text is string text)
                {
                    button.UpdateText(text);
                }
                else if (_text is (string table, string entry))
                {
                    button.UpdateLocalizeText(table, entry);
                }
                else
                {
                    UnityEngine.Debug.LogError("The button label is neither string or (string, string)");
                }
            }
        }

        PopupButton ActiveButton()
        {
            PopupButton button = null;
            if (deactivatedButtons.Count == 0)
            {
                GameObject buttonGO = Instantiate(ButtonPrefab);
                buttonGO.transform.SetParent(transform, false);
                button = buttonGO.GetComponent<PopupButton>();
            }
            else
            {
                button = deactivatedButtons[deactivatedButtons.Count - 1];
                deactivatedButtons.RemoveAt(deactivatedButtons.Count - 1);
            }
            activatedButtons.Add(button);
            button.transform.SetAsLastSibling();
            button.gameObject.SetActive(true);

            return button;
        }

        void DeactivateAllButtons()
        {
            for (int i = activatedButtons.Count -1; i >= 0; i--)
            {
                PopupButton button = activatedButtons[i];
                activatedButtons.RemoveAt(i);
                button.gameObject.SetActive(false);
                deactivatedButtons.Add(button);
            }
        }
    }
}