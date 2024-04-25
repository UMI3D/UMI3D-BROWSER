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
using umi3d;
using umi3d.common.interaction;
using umi3dBrowsers.container;
using umi3dBrowsers.container.formrenderer;
using umi3dBrowsers.displayer;
using umi3dBrowsers.services.connection;
using umi3dVRBrowsersBase.connection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace umi3dBrowsers
{
    public class MainContainer : MonoBehaviour
    {
        [Header("Navigation-navbar")]
        [SerializeField] private Button parameterButton;
        [SerializeField] private Button storageButton;
        [SerializeField] private Button hintButton;
        [SerializeField] private Button bugButton;
        [SerializeField] private GameObject navBar;
        [Space]
        [SerializeField] private ColorBlock navBarButtonsColors = new ColorBlock();
        [Space]
        [SerializeField] private UnityEvent parametersClicked;
        [SerializeField] private UnityEvent storageClicked;
        [SerializeField] private UnityEvent hintClicked;
        [SerializeField] private UnityEvent bugClicked;
        [Space]
        [SerializeField] private ContentContainer parametersContent;
        [SerializeField] private ContentContainer storageContent;
        [SerializeField] private ContentContainer mainContent;
        [SerializeField] private ContentContainer standUpContent;
        [SerializeField] private ContentContainer flagContent;
        [SerializeField] private ContentContainer dynamicServerContent;
        [SerializeField] private ContentContainer loadingContent;
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
        [SerializeField] private MainMessageContainer title;

        [Header("Version")]
        [SerializeField] private TextMeshProUGUI versionText;

        [Header("Connection")]
        [SerializeField] private URLDisplayer urlDisplayer;
        [SerializeField] private FormRendererContainer dynamicServerContainer;

        [Header("Services")]
        [SerializeField] private ConnectionProcessor connectionProcessorService;
        [SerializeField] private PopupManager popupManager;
        [SerializeField] private LoadingContainer loadingContainer;

        [Header("Options")]
        [SerializeField] private bool forceFlagContent;


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

            if (contentState == ContentState.flagContent && services.connection.PlayerPrefsManager.HasLocalisationBeenSet() && !forceFlagContent)
                contentState = ContentState.standUpContent;
            HandleContentState(contentState);
            SetVersion(UMI3DVersion.version);
        }

        /// <summary>
        /// Sets the UMI3D version of the browser
        /// </summary>
        /// <param name="version"></param>
        public void SetVersion(string version)
        {
            versionText.text = version;
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
                title.SetTitle("Connected to", virtualWorldData.worldName, true , true);
            };
            connectionProcessorService.OnParamFormReceived += (connectionFormDto) =>
            {
                HandleContentState(ContentState.dynamicServerContent);
                ProcessForm(connectionFormDto);
                title.SetTitle("", connectionFormDto?.name, true, true);
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
                title.SetTitle("Loading ... ", "");
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
                    mainContent.gameObject.SetActive(true);
                    title.SetTitle(mainContent.PrefixTitleKey, mainContent.SuffixTitleKey);
                    break;
                case ContentState.storageContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    storageContent.gameObject.SetActive(true);
                    backButton?.gameObject.SetActive(true);
                    title.SetTitle(storageContent.PrefixTitleKey, storageContent.SuffixTitleKey);
                    break;
                case ContentState.parametersContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    parametersContent.gameObject.SetActive(true);
                    backButton?.gameObject.SetActive(true);
                    title.SetTitle(parametersContent.PrefixTitleKey, parametersContent.SuffixTitleKey);
                    break;
                case ContentState.flagContent:
                    CloseAllPanels();
                    flagContent.gameObject.SetActive(true);
                    Top.SetActive(true);
                    navBar.SetActive(false);
                    title.SetTitle(flagContent.PrefixTitleKey, flagContent.SuffixTitleKey);
                    break;
                case ContentState.standUpContent:
                    CloseAllPanels();
                    standUpContent.gameObject.SetActive(true);
                    break;
                case ContentState.dynamicServerContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    dynamicServerContent.gameObject.SetActive(true);
                    break;
                case ContentState.loadingContent:
                    CloseAllPanels();
                    //Top.SetActive(true);
                    loadingContent.gameObject.SetActive(true);
                    title.SetTitle(loadingContent.PrefixTitleKey, loadingContent.SuffixTitleKey);
                    break;
            }
        }

        private void CloseAllPanels()
        {
            Top.SetActive(false);
            parametersContent.gameObject.SetActive(false);
            storageContent.gameObject.SetActive(false);
            mainContent.gameObject.SetActive(false);
            backButton?.gameObject.SetActive(false);
            flagContent.gameObject.SetActive(false);
            standUpContent.gameObject.SetActive(false);
            loadingContent.gameObject.SetActive(false);
            dynamicServerContent.gameObject.SetActive(false);
            navBar.SetActive(true);
        }
    }
}