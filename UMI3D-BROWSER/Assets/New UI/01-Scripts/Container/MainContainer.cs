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
using TMPro;
using umi3d.common.interaction;
using umi3dBrowsers.container;
using umi3dBrowsers.container.formrenderer;
using umi3dBrowsers.displayer;
using umi3dBrowsers.services.connection;
using umi3dVRBrowsersBase.connection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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
        [SerializeField] private GameObject dynamicServerContent;
        [SerializeField] private GameObject loadingContent;
        [Space]
        [SerializeField] private GameObject Top;
        // todo : call something like an hint manager hints
        // todo : call something like a pop up manager to open a bug popup

        public enum ContentState { mainContent, storageContent, parametersContent, flagContent, standUpContent, dynamicServerContent, loadingContent };
        [SerializeField, Tooltip("start content")] private ContentState contentState;
#if UNITY_EDITOR
        public ContentState _ContentState { get { return contentState; } set { contentState = value; } }
        public void ToolAccessProcessForm(ConnectionFormDto connectionFormDto) { ProcessForm(connectionFormDto); }
        public void ToolAccessProcessForm(umi3d.common.interaction.form.ConnectionFormDto connectionFormDto) { ProcessForm(connectionFormDto); }
#endif


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

        [Header("Connection")]
        [SerializeField] private URLDisplayer urlDisplayer;
        [SerializeField] private FormRendererContainer dynamicServerContainer;

        [Header("Services")]
        [SerializeField] private ConnectionProcessor connectionProcessorService;
        [SerializeField] private PopupManager popupManager;
        [SerializeField] private LoadingContainer loadingContainer;


        private void Awake()
        {
            navBarButtonsColors.colorMultiplier = 1.0f;
        }

        private void Start()
        {
            BindNavigationButtons();
            BindURL();
            BindFormContainer();
            BindConnectionService();
            BindLoaderDisplayer();
            HandleContentState(contentState);
        }

        /// <summary>
        /// Sets the UMI3D version of the browser
        /// </summary>
        /// <param name="version"></param>
        public void SetVersion(string version)
        {
            versionText.text = version;
        }

        /// <summary>
        /// Sets the page title
        /// </summary>
        /// <param name="prefix">The first part of the title</param>
        /// <param name="suffix">The second part of the title</param>
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

        private void BindNavigationButtons()
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
                popupManager.ShowPopup(PopupManager.PopupType.ReportBug);
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
                SetUpSkeleton setUp = PlayerDependenciesAccessor.Instance.SetUpSkeleton;
                StartCoroutine(setUp.SetupSkeleton());
                HandleContentState(ContentState.mainContent);
            });
        }

        private void BindURL()
        {
            urlDisplayer.OnSubmit.AddListener((url) =>
            {
                connectionProcessorService.TryConnectToMediaServer(url);
            });
        }
        private void BindFormContainer()
        {
            dynamicServerContainer.OnFormAnwser += (formAnswer) => connectionProcessorService.SendFormAnswer(formAnswer);
            dynamicServerContainer.OnDivformAnswer += (formAnswer) => connectionProcessorService.SendFormAnswer(formAnswer);
        }

        private void BindConnectionService()
        {
            connectionProcessorService.OnConnectionFailure += (message) => { Debug.LogError("Failled to conenct"); };
            connectionProcessorService.OnMediaServerPingSuccess += (virtualWorldData) => 
            {
                SetTitle("Connected to", virtualWorldData.worldName);
            };
            connectionProcessorService.OnParamFormReceived += (connectionFormDto) =>
            {
                HandleContentState(ContentState.dynamicServerContent);
                ProcessForm(connectionFormDto);
            };
            connectionProcessorService.OnDivFormReceived += (connectionFormDto) =>
            {
                HandleContentState(ContentState.dynamicServerContent);
                ProcessForm(connectionFormDto);
            };
            connectionProcessorService.OnAsksToLoadLibrairies += (ids) => connectionProcessorService.SendAnswerToLibrariesDownloadAsk(true);
            connectionProcessorService.OnConnectionSuccess += () => Debug.Log("____Connection success____");
        }

        private void BindLoaderDisplayer()
        {
            loadingContainer.Init();
            loadingContainer.OnLoadingInProgress += () =>
            {
                HandleContentState(ContentState.loadingContent);
                SetTitle("Loading ... ", "");
            };
            loadingContainer.OnLoadingFinished += () => Debug.Log("TODO : Do something ::: Its loaded");// gameObject.SetActive(false);
        }

        private void ProcessForm(ConnectionFormDto connectionFormDto)
        {
            dynamicServerContainer.HandleParamForm(connectionFormDto);
        }
        private void ProcessForm(umi3d.common.interaction.form.ConnectionFormDto connectionFormDto)
        {
            dynamicServerContainer.HandleDivForm(connectionFormDto);
        }

        private void HandleContentState(ContentState state)
        {
            contentState = state;
            switch (state)
            {
                case ContentState.mainContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    mainContent.SetActive(true);
                    SetTitle("Connect to an", "Intraverse Portal");
                    break;
                case ContentState.storageContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    storageContent.SetActive(true);
                    backButton?.gameObject.SetActive(true);
                    break;
                case ContentState.parametersContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    parametersContent.SetActive(true);
                    backButton?.gameObject.SetActive(true);
                    SetTitle("", "Settings");
                    break;
                case ContentState.flagContent:
                    CloseAllPanels();
                    flagContent.SetActive(true);
                    SetTitle("Choose your", "language");
                    break;
                case ContentState.standUpContent:
                    CloseAllPanels();
                    standUpContent.SetActive(true);
                    break;
                case ContentState.dynamicServerContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    dynamicServerContent.SetActive(true);
                    break;
                case ContentState.loadingContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    loadingContent.SetActive(true);
                    break;
            }
        }

        private void CloseAllPanels()
        {
            Top.SetActive(false);
            parametersContent.SetActive(false);
            storageContent.SetActive(false);
            mainContent.SetActive(false);
            backButton?.gameObject.SetActive(false);
            flagContent.SetActive(false);
            standUpContent.SetActive(false);
            loadingContent.SetActive(false);
            dynamicServerContent.SetActive(false);
        }
    }
}