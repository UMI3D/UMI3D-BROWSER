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

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class NotificationElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text description;
        [Header("Badge")]
        [SerializeField] private Image badge;
        [SerializeField] private Color badgeColorNew;
        [SerializeField] private Color badgeColorRead;

        public bool IsRead { 
            get => IsRead; 
            set {
                isRead = value;
                badge.color = value ? badgeColorRead : badgeColorNew;
            } 
        }

        private bool isRead;

        public void Init(string descriptionText)
        {
            IsRead = false;
            description.text = descriptionText;
        }
    }
}