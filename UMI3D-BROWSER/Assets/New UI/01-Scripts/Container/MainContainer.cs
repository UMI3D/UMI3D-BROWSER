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
using umi3dBrowsers.data.ui;
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
        [SerializeField] private Transform popupTransform;
        [SerializeField] private Transform skyBoxTransform;

        [Header("Light")]
        [SerializeField] private Light directionalLight;

        [Header("Navigation-navbar")]
        [SerializeField] private GameObject navBar;
        [Space]
        [SerializeField] private ColorBlock navBarButtonsColors = new ColorBlock();
        [Space]
        [SerializeField] private GameObject Top;


        [Header("Navigation")]
        [SerializeField] private SimpleButton backButton;
        [SerializeField] private SimpleButton cancelConnectionButton;

        [Header("Title")]
        [SerializeField] private TitleManager title;

        [Header("Version")]
        [SerializeField] private TextMeshProUGUI versionText;

        [Header("Services")]
        [SerializeField] private ConnectionProcessor connectionProcessorService;
        [SerializeField] private UITweens tween;
        [SerializeField] private PlayerSpawner spawner;
        [SerializeField] private SceneLoader sceneLoader;

        [Header("Linker")]
        [SerializeField] private ConnectionToImmersiveLinker connectionToImmersiveLinker;
        [SerializeField] private ConnectionServiceLinker connectionServiceLinker;
        [SerializeField] private MenuNavigationLinker m_menuNavigationLinker;
        [SerializeField] private PanelData m_mainMenuPanel;
        [SerializeField] private PanelData m_formPanel;
        [SerializeField] private PopupLinker m_popupLinker;
        [SerializeField] private PopupData m_tryConnectPopup;
        [SerializeField] private PopupData m_connectionErrorPopup;

        private void Awake()
        {
            navBarButtonsColors.colorMultiplier = 1.0f;

            var local = PlayerPrefsManager.GetLocalisationLocal();
            LocalizationSettings.SelectedLocale = local ?? LocalizationSettings.ProjectLocale;

            m_menuNavigationLinker.Initialize(contentTransform);

            m_menuNavigationLinker.OnSetTopActive += (active) => Top.SetActive(active);
            m_menuNavigationLinker.OnSetTitle += (type, prefix, suffix) => title.SetTitle(type, prefix, suffix);
            m_menuNavigationLinker.OnSetNavBarActive += (active) => navBar.SetActive(active);
            m_menuNavigationLinker.OnSetBackButtonActive += (active) => backButton.gameObject.SetActive(active);
            m_menuNavigationLinker.OnSetCancelButtonActive += (active) => cancelConnectionButton.gameObject.SetActive(active);

            m_popupLinker.Initialize(popupTransform);

            connectionToImmersiveLinker.OnLeave += () =>
            {
                ShowUI();
                m_menuNavigationLinker.ShowPanel(m_mainMenuPanel);
                connectionProcessorService.Disconnect();
                sceneLoader.ReloadScene();
            };
        }

        private void Start()
        {
            BindNavigationButtons();

            m_popupLinker.OnPopupOpen += () => tween.TweenTo();
            m_popupLinker.OnPopupClose += () => tween.Rewind();

            SetVersion(Application.version);
            BindConnectionService();

            m_menuNavigationLinker.OnReplacePlayerAndShowPanel += () => {
                spawner.RepositionPlayer();
                ShowUI();
            };

            connectionServiceLinker.OnTryToConnect += (url) => {
                m_popupLinker.SetArguments(m_tryConnectPopup, new Dictionary<string, object>() { { "url", url } });
                m_popupLinker.Show(m_tryConnectPopup, "popup_connection_server", "popup_trying_connect");
            };
            connectionServiceLinker.OnConnectionFailure += (message) => {
                m_popupLinker.SetArguments(m_connectionErrorPopup, new Dictionary<string, object>() { { "error", message } });
                m_popupLinker.Show(m_connectionErrorPopup, "popup_fail_connect", "error_msg",
                    ("popup_close", () => { m_popupLinker.CloseAll(); }
                ));
            };
            connectionServiceLinker.OnMediaServerPingSuccess += (virtualWorldData) => {
                m_popupLinker.CloseAll();
            };
            connectionServiceLinker.OnAnswerFailed += () => {
                m_popupLinker.Show(m_connectionErrorPopup, "popup_answer_failed_title", "popup_answer_failed_description",
                    ("popup_close", () => { m_popupLinker.CloseAll(); }
                ));
            };
            connectionServiceLinker.OnAsksToLoadLibrairies += (ids, action) => action?.Invoke(true);

            m_menuNavigationLinker.ShowStartPanel();
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
            cancelConnectionButton?.OnClick.AddListener(() => {
                UMI3DCollaborationClientServer.Logout();
            });

            connectionToImmersiveLinker.OnSkeletonStandUp += () =>
                parentTransform.position = new Vector3(parentTransform.position.x, Camera.main.transform.position.y, parentTransform.position.z);
        }

        private void BindConnectionService()
        {
            connectionServiceLinker.OnParamFormDtoReceived += (connectionFormDto) =>
            {
                if (parentTransform.gameObject.activeSelf == false)
                {
                    ShowUI();
                    spawner.RepositionPlayer();
                }
                m_menuNavigationLinker.ShowPanel(m_formPanel);
            };
            connectionServiceLinker.OnDivFormDtoReceived += (connectionFormDto) => {
                if (parentTransform.gameObject.activeSelf == false)
                {
                    ShowUI();
                    spawner.RepositionPlayer();
                }
                m_menuNavigationLinker.ShowPanel(m_formPanel);
            };

            connectionServiceLinker.OnConnectionSuccess += () => HideUI();
        }

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