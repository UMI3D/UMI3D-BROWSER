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
using System.Collections.Generic;
using TMPro;
using umi3d;
using umi3d.cdk.collaboration;
using umi3d.common.interaction;
using umi3dBrowsers.connection;
using umi3dBrowsers.container;
using umi3dBrowsers.container.formrenderer;
using umi3dBrowsers.displayer;
using umi3dBrowsers.linker;
using umi3dBrowsers.services.connection;
using umi3dBrowsers.services.title;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using utils.tweens;

namespace umi3dBrowsers
{
    public class MainContainer : MonoBehaviour
    {
        [Header("Parent")]
        [SerializeField] private Transform parentTransform;

        [Header("Light")]
        [SerializeField] private Light directionalLight;

        [Header("Navigation-navbar")]
        [SerializeField] private Button parameterButton;
        [SerializeField] private Button storageButton;
        [SerializeField] private Button hintButton;
        [SerializeField] private Button bugButton;
        [SerializeField] private GameObject navBar;
        [Space]
        [SerializeField] private ColorBlock navBarButtonsColors = new ColorBlock();
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
        [SerializeField] private SimpleButton cancelConnectionButton;
        [SerializeField] private SimpleButton standUpButton;
        [SerializeField] private SimpleButton flagButton;

        [Header("Title")]
        [SerializeField] private TitleManager title;

        [Header("Version")]
        [SerializeField] private TextMeshProUGUI versionText;

        [Header("Connection")]
        [SerializeField] private URLDisplayer urlDisplayer;
        [SerializeField] private FormRendererContainer dynamicServerContainer;

        [Header("Services")]
        [SerializeField] private ConnectionProcessor connectionProcessorService;
        [SerializeField] private PopupManager popupManager;
        [SerializeField] private LoadingContainer loadingContainer;
        [SerializeField] private umi3dBrowsers.services.librairies.LibraryManager libraryManager;
        [SerializeField] private UITweens tween;
        [SerializeField] private PlayerSpawner spawner;

        [Header("Options")]
        [SerializeField] private bool forceFlagContent;

        [Header("Linker")]
        [SerializeField] private ConnectionToImmersiveLinker connectionToImmersiveLinker;

        private ContentState _returnState;

        private void Awake()
        {
            navBarButtonsColors.colorMultiplier = 1.0f;

            var local = services.connection.PlayerPrefsManager.GetLocalisationLocal();
            if (contentState == ContentState.flagContent && local != null && !forceFlagContent)
                contentState = ContentState.standUpContent;
            LocalizationSettings.SelectedLocale = local ?? LocalizationSettings.ProjectLocale;

            connectionToImmersiveLinker.OnLeave += () =>
            {
                ShowUI();
                HandleContentState(ContentState.mainContent);
                spawner.RepositionPlayer();
                connectionProcessorService.Disconnect();
            };
        }

        private void Start()
        {
            spawner.Init(connectionToImmersiveLinker);

            BindNavigationButtons();
            BindURL();
            BindFormContainer();

            popupManager.OnPopUpOpen += () => tween.TweenTo();
            popupManager.OnPopUpClose += () => tween.Rewind();

            HandleContentState(contentState);
            SetVersion(UMI3DVersion.version);
            BindConnectionService();
            BindLoaderDisplayer();

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
                HandleContentState(ContentState.parametersContent);
            });
            storageButton?.onClick.AddListener(() => {
                HandleContentState(ContentState.storageContent);
            });
            hintButton?.onClick.AddListener(() => {

            });
            bugButton?.onClick.AddListener(() => {
                popupManager.ShowPopup(PopupManager.PopupType.ReportBug, "", "");
            });
            backButton?.OnClick.AddListener(() =>
            {
                if (contentState == ContentState.parametersContent || contentState == ContentState.storageContent)
                    HandleContentState(_returnState);
            });
            cancelConnectionButton?.OnClick.AddListener(() => {
                UMI3DCollaborationClientServer.Logout();
            });
            flagButton?.OnClick.AddListener(() =>
            {
                HandleContentState(ContentState.standUpContent);
            });
            standUpButton?.OnClick.AddListener(() =>
            {
                SetUpSkeleton setup =  connectionToImmersiveLinker.SetUpSkeleton;
                StartCoroutine(setup.SetupSkeleton());
                HandleContentState(ContentState.mainContent);
                parentTransform.position = new Vector3(parentTransform.position.x, Camera.main.transform.position.y, parentTransform.position.z);
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
            connectionProcessorService.OnTryToConnect += (url) => {
                popupManager.SetArguments(PopupManager.PopupType.Info, new Dictionary<string, object>() { { "url", url } });
                popupManager.ShowPopup(PopupManager.PopupType.Info, "popup_connection_server", "popup_trying_connect");
            };
            connectionProcessorService.OnConnectionFailure += (message) => { 
                Debug.LogError("Failed to connect");
                popupManager.SetArguments(PopupManager.PopupType.Error, new Dictionary<string, object>() { { "error", message } });
                popupManager.ShowPopup(PopupManager.PopupType.Error, "popup_fail_connect", "error_msg", 
                    ("popup_close", () => { popupManager.ClosePopUp(); }) );
            };
            connectionProcessorService.OnMediaServerPingSuccess += (virtualWorldData) => 
            {
                title.SetTitle(TitleType.connectionTitle,"Connected to", virtualWorldData.worldName, true , true);

                popupManager.ClosePopUp();
                services.connection.PlayerPrefsManager.GetVirtualWorlds().AddWorld(virtualWorldData);
            };
            connectionProcessorService.OnParamFormReceived += (connectionFormDto) =>
            {
                HandleContentState(ContentState.dynamicServerContent);
                ProcessForm(connectionFormDto);
                title.SetTitle(TitleType.connectionTitle,"", connectionFormDto?.name, true, true);
            };
            connectionProcessorService.OnDivFormReceived += (connectionFormDto) =>
            {
                HandleContentState(ContentState.dynamicServerContent);
                ProcessForm(connectionFormDto);
            };
            connectionProcessorService.OnAsksToLoadLibrairies += (ids) => connectionProcessorService.SendAnswerToLibrariesDownloadAsk(true);
            connectionProcessorService.OnConnectionSuccess += () => HideUI();
            connectionProcessorService.OnAnswerFailed += () => {
                popupManager.ShowPopup(PopupManager.PopupType.Error, "popup_answer_failed_title", "popup_answer_failed_description",
                    ("popup_close", () => { popupManager.ClosePopUp(); }
                ));
            };
            UMI3DCollaborationClientServer.Instance.OnLeavingEnvironment.AddListener(() => {
                UMI3DCollaborationClientServer.Instance.Identifier.Reset();
                HandleContentState(ContentState.mainContent);
            });
        }

        private void BindLoaderDisplayer()
        {
            loadingContainer.Init(connectionToImmersiveLinker);
            loadingContainer.OnLoadingInProgress += () =>
            {
                HandleContentState(ContentState.loadingContent);
                title.SetTitle(TitleType.mainTitle,"Loading ... ", "");
                spawner.RepositionPlayer();
                ShowUI();
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
            if (contentState is not (ContentState.storageContent or ContentState.parametersContent))
                _returnState = contentState;
            contentState = state;
            switch (state)
            {
                case ContentState.mainContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    mainContent.gameObject.SetActive(true);
                    title.SetTitle(TitleType.mainTitle,mainContent.PrefixTitleKey, mainContent.SuffixTitleKey);
                    break;
                case ContentState.storageContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    storageContent.gameObject.SetActive(true);
                    backButton?.gameObject.SetActive(true);
                    title.SetTitle(TitleType.subTitle, storageContent.PrefixTitleKey, storageContent.SuffixTitleKey);
                    libraryManager.UpdateContent();
                    break;
                case ContentState.parametersContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    parametersContent.gameObject.SetActive(true);
                    backButton?.gameObject.SetActive(true);
                    title.SetTitle(TitleType.subTitle, parametersContent.PrefixTitleKey, parametersContent.SuffixTitleKey);
                    break;
                case ContentState.flagContent:
                    CloseAllPanels();
                    flagContent.gameObject.SetActive(true);
                    Top.SetActive(true);
                    navBar.SetActive(false);
                    title.SetTitle(TitleType.mainTitle, flagContent.PrefixTitleKey, flagContent.SuffixTitleKey);
                    break;
                case ContentState.standUpContent:
                    CloseAllPanels();
                    standUpContent.gameObject.SetActive(true);
                    break;
                case ContentState.dynamicServerContent:
                    CloseAllPanels();
                    Top.SetActive(true);
                    cancelConnectionButton?.gameObject.SetActive(true);
                    dynamicServerContent.gameObject.SetActive(true);
                    break;
                case ContentState.loadingContent:
                    CloseAllPanels();
                    //Top.SetActive(true);
                    loadingContent.gameObject.SetActive(true);
                    title.SetTitle(TitleType.mainTitle, loadingContent.PrefixTitleKey, loadingContent.SuffixTitleKey);
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
            cancelConnectionButton?.gameObject.SetActive(false);
            flagContent.gameObject.SetActive(false);
            standUpContent.gameObject.SetActive(false);
            loadingContent.gameObject.SetActive(false);
            dynamicServerContent.gameObject.SetActive(false);
            navBar.SetActive(true);
        }
        private void HideUI()
        {
            parentTransform.gameObject.SetActive(false);
            directionalLight.gameObject.SetActive(false);
        }

        private void ShowUI()
        {
            parentTransform.gameObject.SetActive(true);
            directionalLight.gameObject.SetActive(true);
        }
    }
}