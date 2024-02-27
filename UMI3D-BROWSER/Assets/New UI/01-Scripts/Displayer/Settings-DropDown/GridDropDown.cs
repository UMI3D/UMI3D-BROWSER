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
using System.Collections;
using System.Collections.Generic;
using TMPro;
using umi3dBrowsers.utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    [RequireComponent(typeof(UIColliderScaller), typeof(UMI3DUI_Button))]
    public class GridDropDown : MonoBehaviour, ISubDisplayer
    {
        [Header("MainDisplayer")]
        [SerializeField] private TextMeshProUGUI text;

        [Header("Colors")]
        [SerializeField] private Image thisImage;
        [SerializeField] private Color thisColor;
        [Space]
        [SerializeField] private Color lightGridColor;
        [SerializeField] private Color darkGridColor;
        [SerializeField] private bool isLightToBeginWith;
        [Space]
        [SerializeField] private Color hightLight;

        [Header("Prefabs")]
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private List<GridDropDownItemCell> cells = new();
        [SerializeField] private RectTransform content;

        public event Action OnClick;
        public event Action OnDisabled;
        public event Action OnHover;

        private GameObject currentlySelectedCellGo;

        public string GetValue()
        {
            return text.text;
        }

        public void Init(List<GridDropDownItemCell> cells)
        {
            this.cells = cells;
            thisImage.color = thisColor;

            text.text = cells[0].ItemText;
            InitAllCells();
        }

        private void InitAllCells()
        {
            foreach (GridDropDownItemCell cell in cells)
            {
                Button button = Instantiate(itemPrefab, content).GetComponent<Button>();
                Image image = button.GetComponent<Image>();
                TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

                cell.SetImage(image);
                cell.SetCorrespondingButton(button);
                text.text = cell.ItemText;

                cell.OnSelected += (selected) =>
                {
                    this.text.text = selected.ItemText;
                    content.gameObject.SetActive(false);
                    if (currentlySelectedCellGo != null)
                    {
                        currentlySelectedCellGo.SetActive(true);
                    }

                    OnClick?.Invoke();
                };
            }

            RefreshStyle();
        }

        private void RefreshStyle()
        {
            bool light = isLightToBeginWith;
            foreach (GridDropDownItemCell cell in cells)
            {
                if (!cell.Image.gameObject.activeSelf) continue;
                if (light)
                {
                    cell.Image.color = darkGridColor;
                    light = false;
                }
                else
                {
                    cell.Image.color = lightGridColor;
                    light = true;
                }
            }
        }

        public void Click()
        {
            if (content.gameObject.activeSelf)
            {
                content.gameObject.SetActive(false);
                if (currentlySelectedCellGo != null)
                {
                    currentlySelectedCellGo.SetActive(true);
                }
            }
            else
            {
                content.gameObject.SetActive(true);
                currentlySelectedCellGo = cells.Find(c =>  c.ItemText == text.text).Button.gameObject;
                currentlySelectedCellGo.SetActive(false);
                RefreshStyle();
            }
        }

        public void Disable()
        {

        }

        public void HoverEnter(PointerEventData eventData)
        {
            thisImage.color = hightLight;
        }

        public void HoverExit(PointerEventData eventData)
        {
            thisImage.color = thisColor;
        }
    }
}

