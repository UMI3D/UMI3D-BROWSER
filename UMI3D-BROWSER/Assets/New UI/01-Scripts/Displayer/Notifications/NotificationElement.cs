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

using System;
using TMPro;
using umi3d.cdk;
using umi3d.common;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class NotificationElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text descriptionComplete;
        [Header("Badge")]
        [SerializeField] private Image badge;
        [SerializeField] private Color badgeColorNew;
        [SerializeField] private Color badgeColorRead;
        [Header("Buttons")]
        [SerializeField] private Transform buttonsContainer;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private int buttonPadding;

        public bool IsRead { 
            get => isRead; 
            set {
                isRead = value;
                badge.color = value ? badgeColorRead : badgeColorNew;
            } 
        }

        private bool isRead;

        public void Init(NotificationDto notificationDto)
        {
            IsRead = false;
            description.text = notificationDto.content;
            descriptionComplete.text = notificationDto.content;

            RectTransform buttonTransform = buttonPrefab.transform as RectTransform;

            if (notificationDto.callback == null)
                return;

            descriptionComplete.margin = notificationDto.callback.Length > 0 ? new Vector4(0, 0, 0, buttonTransform.sizeDelta.y + 2 * buttonPadding) : Vector4.zero;
            for (int i = 0; i < notificationDto.callback.Length; i++)
            {
                var buttonGameObject = Instantiate(buttonPrefab, buttonsContainer);
                buttonGameObject.GetComponentInChildren<SimpleButton>().OnClick.AddListener(() => {
                    var callbackDto = new NotificationCallbackDto() {
                        id = notificationDto.id,
                        callback = i == 0
                    };
                    UMI3DClientServer.SendRequest(callbackDto, true);
                });
                buttonGameObject.GetComponentInChildren<TMP_Text>().text = notificationDto.callback[i];
            }
        }

        private void RemoveButtons()
        {
            foreach (Transform child in buttonsContainer)
                Destroy(child);
        }
    }
}