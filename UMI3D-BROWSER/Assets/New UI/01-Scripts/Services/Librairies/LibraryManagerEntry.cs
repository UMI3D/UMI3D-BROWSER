/*
Copyright 2019 - 2022 Inetum

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
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.services.librairies
{
    public class LibraryManagerEntry : MonoBehaviour
    {
        [SerializeField] private Button deleteButton;
        [SerializeField] private Image deleteButtonImage;
        [SerializeField] private Sprite deleteButtonSprite;
        [SerializeField] private Sprite deleteButtonSpriteHover;
        [SerializeField] private Color deleteButtonImageColor;
        [SerializeField] private Color deleteButtonImageColorHover;
        [SerializeField] private TextMeshProUGUI libLabel;

        public event Action DeleteLib;

        public Button DeleteButton => deleteButton;
        public TextMeshProUGUI LibLabel => libLabel;

        private void OnEnable()
        {
            deleteButton.gameObject.SetActive(UMI3DCollaborationClientServer.Environement == null);
        }

        public void Delete()
        {
            DeleteLib?.Invoke();
        }

        public void DeleteButtonHover(bool isHover)
        {
            deleteButtonImage.sprite = isHover ? deleteButtonSpriteHover : deleteButtonSprite;
            deleteButtonImage.color = isHover ? deleteButtonImageColorHover : deleteButtonImageColor;
        }
    }
}