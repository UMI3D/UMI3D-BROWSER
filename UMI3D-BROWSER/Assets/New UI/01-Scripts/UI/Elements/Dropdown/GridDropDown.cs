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
    public class GridDropDown : MonoBehaviour, ISubDisplayer, IDisplayer
    {
        [Header("MainDisplayer")]
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI title;

        [Header("Colors")]
        [SerializeField] private Button dropdownButton;
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

        public object GetValue(bool trim)
        {
            if (trim)
                return GetValue().Trim();
            else
                return GetValue();
        }

        public void Init(List<GridDropDownItemCell> cells, int indexCurrent = 0)
        {
            this.cells = cells;

            var colors = dropdownButton.colors;
            colors.normalColor = thisColor;
            colors.highlightedColor = hightLight;
            colors.pressedColor = colors.normalColor;
            colors.selectedColor = colors.normalColor;
            dropdownButton.colors = colors;

            text.text = cells[indexCurrent].ItemText;
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
            bool isLastElementLight = isLightToBeginWith;
            ColorBlock colors;
            foreach (GridDropDownItemCell cell in cells)
            {
                if (!cell.Button.gameObject.activeSelf) 
                    continue;

                colors = cell.Button.colors;
                colors.normalColor = isLastElementLight ? darkGridColor : lightGridColor;
                colors.highlightedColor = hightLight;
                colors.pressedColor = colors.normalColor;
                colors.selectedColor = colors.normalColor;
                cell.Button.colors = colors;

                isLastElementLight = !isLastElementLight;
            }
        }

        public void Click()
        {
            if (content.gameObject.activeSelf)
            {
                content.gameObject.SetActive(false);
                if (currentlySelectedCellGo != null)
                    currentlySelectedCellGo.SetActive(true);
            }
            else
            {
                content.gameObject.SetActive(true);
                currentlySelectedCellGo = cells.Find(c =>  c.ItemText == text.text).Button.gameObject;
                currentlySelectedCellGo.SetActive(false);
                RefreshStyle();
            }
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void Disable()
        {

        }

        public void Init(Color normalColor, Color hoverColor, Color selectedColor)
        {
            throw new NotImplementedException();
        }

        public void SetTitle(string title)
        {
            if (this.title == null) return;
            this.title.text = title + " :";
        }

        public void SetPlaceHolder(List<string> placeHolder)
        {
            List<GridDropDownItemCell> possibleValues = new();
            placeHolder.ForEach(language =>
            {
                GridDropDownItemCell cell = new(language);
                possibleValues.Add(cell);
            });

            Init(possibleValues);
        }

        public void SetColor(Color color)
        {

        }

        public void SetResource(object resource)
        {

        }

        public void HoverEnter(PointerEventData eventData)
        {
        }

        public void HoverExit(PointerEventData eventData)
        {
        }
    }
}

