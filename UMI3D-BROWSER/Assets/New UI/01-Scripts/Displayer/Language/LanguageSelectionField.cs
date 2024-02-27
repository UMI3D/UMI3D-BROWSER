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
using umi3dBrowsers.displayer;
using umi3dBrowsers.utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    [RequireComponent(typeof(UMI3DUI_Button), typeof(UIColliderScaller))]
    public class LanguageSelectionField : MonoBehaviour, ISubDisplayer
    {
        [SerializeField] private LanguageParams _params;
        public LanguageParams Params => _params;

        [Serializable]
        public struct LanguageParams
        {
            [SerializeField] public SupportedLanguages SupportedLanguages;
            [SerializeField] public string languageDual;
            [SerializeField] public string language;
            [SerializeField] public Sprite languageFlag;
        }

        [Header("UI")]
        [SerializeField] private Image flagImage;
        [SerializeField] private TextMeshProUGUI dualText;
        [SerializeField] private TextMeshProUGUI languageText;
        [SerializeField] private Image BackgroundImage;
        [Space]
        [SerializeField, ColorUsage(showAlpha: true, hdr: true)] private Color normalBGColor;
        [SerializeField, ColorUsage(showAlpha: true, hdr: true)] private Color hoverBG;
        [SerializeField, ColorUsage(showAlpha: true, hdr: true)] private Color selectedBG;


        public event Action OnClick;
        public event Action OnDisabled;
        public event Action OnHover;

        private bool _isSelected;

        public void Init(LanguageParams param) 
        { 
            this._params = param;
            BackgroundImage.color = normalBGColor;
            flagImage.sprite = _params.languageFlag;
            dualText.text = _params.languageDual;
            languageText.text = _params.language;
        }

        public void Click()
        {
            if (_isSelected)
            {
                Disable();
            }
            else
            {
                BackgroundImage.color = selectedBG;
                _isSelected = true;
                OnClick?.Invoke();
            }
        }

        public void Disable()
        {
            BackgroundImage.color = normalBGColor;
            _isSelected = false;
            OnDisabled?.Invoke();
        }

        public void HoverEnter(PointerEventData eventData)
        {
            BackgroundImage.color = hoverBG;
            OnHover?.Invoke();
        }

        public void HoverExit(PointerEventData eventData)
        {
            if(!_isSelected)
                BackgroundImage.color = normalBGColor;
        }
    }
}

