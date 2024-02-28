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

using inetum.unityUtils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace umi3d
{
    public class MainContainer : MonoBehaviour
    {
        [Header("Navigation-navbar")]
        [SerializeField] private Button parameterButton;
        [SerializeField] private Button storageButton;
        [SerializeField] private Button hintButton;
        [SerializeField] private Button bugButton;
        [Space]
        [SerializeField] private ColorBlock navBarButtonsColors = new ColorBlock();
        [Space]
        [SerializeField] private UnityEvent parametersClicked;
        [SerializeField] private UnityEvent storageClicked;
        [SerializeField] private UnityEvent hintClicked;
        [SerializeField] private UnityEvent bugClicked;
        [Space]
        [SerializeField] private GameObject parametersContent;
        [SerializeField] private GameObject storageContent;
        [SerializeField] private GameObject mainContent;
        [SerializeField] private GameObject standUpContent;
        [SerializeField] private GameObject flagContent;
        [Space]
        [SerializeField] private GameObject Top;
        // todo : call something like an hint manager hints
        // todo : call something like a pop up manager to open a bug popup

        private enum ContentState { mainContent, storageContent, parametersContent, flagContent, standUpContent };
        [SerializeField, Tooltip("start content")] private ContentState contentState;   


        [Header("Navigation")]
        [SerializeField] private SimpleButton backButton;
        [SerializeField] private SimpleButton standUpButton;
        [SerializeField] private SimpleButton flagButton;
        [Space]
        [SerializeField] private UnityEvent backButtonClicked;
        [SerializeField] private UnityEvent standUpButtonClicked;
        [SerializeField] private UnityEvent flagButtonClicked;

        [Header("Title")]
        [SerializeField] private TextMeshProUGUI prefixText;
        [SerializeField] private TextMeshProUGUI suffixText;

        [Header("Version")]
        [SerializeField] private TextMeshProUGUI versionText;

        private void Awake()
        {
            navBarButtonsColors.colorMultiplier = 1.0f;
            InitializeButtons();
            HandleContentState(contentState);
        }

        public void SetVersion(string version)
        {
            versionText.text = version;
        }

        public void SetTitle(string prefix, string suffix)
        {
            float suffitLength = suffix.Length;
            float prefixLength = prefix.Length;
            Rect suffitRectTransform = suffixText.rectTransform.rect;
            Rect prefixRectTransform = prefixText.rectTransform.rect;

            prefixText.text = prefix;
            suffixText.text = suffix;
            suffitRectTransform.width = suffitLength * 12.5f;
            prefixRectTransform.width = prefixLength * 10f;
            suffixText.rectTransform.sizeDelta = new Vector2(suffitRectTransform.width, suffitRectTransform.height);
            prefixText.rectTransform.sizeDelta = new Vector2(prefixRectTransform.width, prefixRectTransform.height);
        }

        private void InitializeButtons()
        {
            parameterButton.colors = navBarButtonsColors;
            storageButton.colors = navBarButtonsColors;
            hintButton.colors = navBarButtonsColors;
            bugButton.colors = navBarButtonsColors;

            parameterButton?.onClick.AddListener(() =>
            {
                parametersClicked?.Invoke();
                HandleContentState(ContentState.parametersContent);
            });
            storageButton?.onClick.AddListener(() => {
                storageClicked?.Invoke();
                HandleContentState(ContentState.storageContent);
            });
            hintButton?.onClick.AddListener(() => {
                hintClicked?.Invoke();

            });
            bugButton?.onClick.AddListener(() => {
                bugClicked?.Invoke();

            });
            backButton?.OnClick.AddListener(() =>
            {
                backButtonClicked?.Invoke();
                if (contentState == ContentState.parametersContent || contentState == ContentState.storageContent)
                    HandleContentState(ContentState.mainContent);
            });
            flagButton?.OnClick.AddListener(() =>
            {
                flagButtonClicked?.Invoke();
                HandleContentState(ContentState.standUpContent);
            });
            standUpButton?.OnClick.AddListener(() =>
            {
                standUpButtonClicked?.Invoke();
                HandleContentState(ContentState.mainContent);
            });
        }

        private void HandleContentState(ContentState state)
        {
            contentState = state;
            switch (state)
            {
                case ContentState.mainContent:
                    Top.SetActive(true);
                    parametersContent.SetActive(false);
                    storageContent.SetActive(false);
                    mainContent.SetActive(true);
                    backButton?.gameObject.SetActive(false);
                    standUpContent.SetActive(false);
                    SetTitle("Connect to an", "Intraverse Portal");
                    break;
                case ContentState.storageContent:
                    parametersContent.SetActive(false);
                    storageContent.SetActive(true);
                    mainContent.SetActive(false);
                    backButton?.gameObject.SetActive(true);
                    break;
                case ContentState.parametersContent:
                    parametersContent.SetActive(true);
                    storageContent.SetActive(false);
                    mainContent.SetActive(false);
                    backButton?.gameObject.SetActive(true);
                    SetTitle("", "Settings");
                    break;
                case ContentState.flagContent:
                    Top.SetActive(false);
                    parametersContent.SetActive(false);
                    storageContent.SetActive(false);
                    mainContent.SetActive(false);
                    backButton?.gameObject.SetActive(false);
                    standUpContent.SetActive(false);
                    flagContent.SetActive(true);
                    SetTitle("Choose your", "language");
                    break;
                case ContentState.standUpContent:
                    Top.SetActive(false);
                    parametersContent.SetActive(false);
                    storageContent.SetActive(false);
                    mainContent.SetActive(false);
                    backButton?.gameObject.SetActive(false);
                    flagContent.SetActive(false);
                    standUpContent.SetActive(true);
                    break;
            }
        }
    }
}