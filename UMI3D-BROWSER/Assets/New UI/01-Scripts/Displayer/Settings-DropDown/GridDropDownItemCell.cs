/*
Copyright 2019 - 2023 Inetum

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
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    [Serializable]
    public class GridDropDownItemCell
    {
        [SerializeField] private string itemText;
        [SerializeField] private Button button;
        Image image;

        public Action<GridDropDownItemCell> OnSelected;

        public GridDropDownItemCell(string itemText)
        {
            this.itemText = itemText;
        }

        public void SetCorrespondingButton(Button button)
        {
            this.button = button;
            button.onClick.AddListener(() =>
            {
                OnSelected?.Invoke(this);
            });
        }

        public void SetImage(Image image) { this.image = image; }

        public string ItemText => itemText;
        public Button Button => button;
        public Image Image => image;    
    }
}