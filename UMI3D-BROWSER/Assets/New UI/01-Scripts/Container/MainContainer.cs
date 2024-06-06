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
using umi3dBrowsers.linker.ui;
using umi3dBrowsers.sceneManagement;
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
        [SerializeField] private Transform contentTransform;
        [SerializeField] private Transform skyBoxTransform;

        [Header("Light")]
        [SerializeField] private Light directionalLight;

        [Header("Navigation-navbar")]
        [SerializeField] private GameObject navBar;
        [Space]
        [SerializeField] private ColorBlock navBarButtonsColors = new ColorBlock();
        [Space]
        [SerializeField] private GameObject Top;
#if UNITY_EDITOR
        public void ToolAccessProcessForm(ConnectionFormDto connectionFormDto) { ProcessForm(connectionFormDto); }
#endif


        [Header("Navigation")]
        [SerializeField] private SimpleButton backButton;
        [SerializeField] private SimpleButton cancelConnectionButton;
        [SerializeField] private SimpleButton standUpButton;
        [SerializeField] private LanguageWidget languageWidget;

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
        [SerializeField] private SceneLoader sceneLoader;

        [Header("Options")]
        [SerializeField] private bool forceFlagContent;

        [Header("Linker")]
        [SerializeField] private ConnectionToImmersiveLinker connectionToImmersiveLinker;
        [SerializeField] private ConnectionServiceLinker connectionServiceLinker;
        [SerializeField] private MenuNavigationLinker m_menuNavigationLinker;

        private void Awake()
        {
            navBarButtonsColors.colorMultiplier = 1.0f;

            var local = PlayerPrefsManager.GetLocalisationLocal();
            LocalizationSettings.SelectedLocale = local ?? LocalizationSettings.ProjectLocale;

            m_menuNavigationLinker.Initialize(contentTransform, Top, title, navBar);

            connectionToImmersiveLinker.OnLeave += () =>
            {
                ShowUI();
                // TODO: HandleContentState(ContentState.mainContent);
                connectionProcessorService.Disconnect();
                sceneLoader.ReloadScene();
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

            SetVersion(Application.version);
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
           /* TODO : parameterButton.colors = navBarButtonsColors;
            storageButton.colors = navBarButtonsColors;
            hintButton.colors = navBarButtonsColors;
            bugButton.colors = navBarButtonsColors;*/

            backButton?.OnClick.AddListener(() =>
            {
                /* TODO : if (contentState == ContentState.parametersContent || contentState == ContentState.storageContent)
                    HandleContentState(_returnState);*/
            });
            cancelConnectionButton?.OnClick.AddListener(() => {
                UMI3DCollaborationClientServer.Logout();
            });
            languageWidget?.OnSupportedLanguageValidated.AddListener((UnityEngine.Localization.Locale locale) =>
            {
                // TODO : HandleContentState(ContentState.standUpContent);
            });
            standUpButton?.OnClick.AddListener(() =>
            {
                SetUpSkeleton setup =  connectionToImmersiveLinker.SetUpSkeleton;
                StartCoroutine(setup.SetupSkeleton());
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
        }

        private void BindConnectionService()
        {
            connectionProcessorService.OnMediaServerPingSuccess += (virtualWorldData) => 
            {
                title.SetTitle(TitleType.connectionTitle,"Connected to", virtualWorldData.worldName, true , true);

                popupManager.ClosePopUp();
                services.connection.PlayerPrefsManager.GetVirtualWorlds().AddWorld(virtualWorldData);
            };
            connectionProcessorService.OnParamFormReceived += (connectionFormDto) =>
            {
                if (parentTransform.gameObject.activeSelf == false)
                {
                    ShowUI();
                    spawner.RepositionPlayer();
                }
                // TODO: HandleContentState(ContentState.dynamicServerContent);
                ProcessForm(connectionFormDto);
                title.SetTitle(TitleType.connectionTitle,"", connectionFormDto?.name ?? "", true, true);
            };
            connectionProcessorService.OnAsksToLoadLibrairies += (ids) => connectionProcessorService.SendAnswerToLibrariesDownloadAsk(true);
            connectionProcessorService.OnConnectionSuccess += () => HideUI();
            connectionProcessorService.OnAnswerFailed += () => {
                popupManager.ShowPopup(PopupManager.PopupType.Error, "popup_answer_failed_title", "popup_answer_failed_description",
                    ("popup_close", () => { popupManager.ClosePopUp(); }
                ));
            };
        }

        private void BindLoaderDisplayer()
        {
            loadingContainer.Init(connectionToImmersiveLinker);
            loadingContainer.OnLoadingInProgress += () =>
            {
                // TODO: HandleContentState(ContentState.loadingContent);
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

        /* TODO: private void HandleContentState(ContentState state)
        {
            switch (state)
            {
                case ContentState.storageContent:
                    backButton?.gameObject.SetActive(true);
                    libraryManager.UpdateContent();
                    break;
                case ContentState.parametersContent:
                    backButton?.gameObject.SetActive(true);
                    break;
                case ContentState.dynamicServerContent:
                    cancelConnectionButton?.gameObject.SetActive(true);
                    break;
            }
        }*/

        private void HideUI()
        {
            parentTransform.gameObject.SetActive(false);
            skyBoxTransform.gameObject.SetActive(false);
            directionalLight.gameObject.SetActive(false);
        }

        private void ShowUI()
        {
            parentTransform.gameObject.SetActive(true);
            skyBoxTransform.gameObject.SetActive(true);
            directionalLight.gameObject.SetActive(true);
        }
    }
}