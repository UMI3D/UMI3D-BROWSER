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
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using umi3d.baseBrowser.cursor;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3dBrowsers.data.ui;
using umi3dBrowsers.linker;
using umi3dBrowsers.linker.ingameui;
using umi3dBrowsers.linker.ui;
using umi3dBrowsers.sceneManagement;
using umi3dBrowsers.services.connection;
using umi3dBrowsers.services.title;
using UnityEngine;
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

        [Header("Dependencies")]
        [SerializeField] private MainContainerLinker mainContainerLinker;

        [Header("Navigation-navbar")]
        [SerializeField] private GameObject navBar;
        [Space]
        [SerializeField] private ColorBlock navBarButtonsColors = new ColorBlock();
        [Space]
        [SerializeField] private GameObject Logo;
        [SerializeField] private GameObject Title;
        [SerializeField] private Button PageTipButton;


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
        [SerializeField] private PanelTutoDisplayer PageTipDisplayer;

        [Header("Linker")]
        [SerializeField] private ConnectionToImmersiveLinker connectionToImmersiveLinker;
        [SerializeField] private ConnectionServiceLinker connectionServiceLinker;
        [SerializeField] private MenuNavigationLinker m_menuNavigationLinker;
        [SerializeField] private InGameLinker inGameLinker;
        [SerializeField] private PanelData m_mainMenuPanel;
        [SerializeField] private PanelData m_formPanel;
        [SerializeField] private PopupLinker m_popupLinker;
        [SerializeField] private PopupData m_tryConnectPopup;
        [SerializeField] private PopupData m_connectionErrorPopup;
        [SerializeField] private PopupData m_quittingPopup;

        private Notifier m_quittingNotifier;

        private void Awake()
        {
            m_quittingNotifier = NotificationHub.Default.GetNotifier(this, QuittingManagerNotificationKey.QuittingConfirmation);
            NotificationHub.Default.Subscribe(this, QuittingManagerNotificationKey.RequestToQuit, PopupQuit);

            navBarButtonsColors.colorMultiplier = 1.0f;

            var local = PlayerPrefsManager.GetLocalisationLocal();
            LocalizationSettings.SelectedLocale = local ?? LocalizationSettings.ProjectLocale;

            m_menuNavigationLinker.Initialize(contentTransform);
            m_menuNavigationLinker.OnSetCancelButtonActive += (active) => cancelConnectionButton.gameObject.SetActive(active);

            m_menuNavigationLinker.OnPanelChanged += (panel, panelTutoManager) => {
                Logo.SetActive(panel.DisplayTop);
                Title.SetActive(panel.DisplayTop);
                title.SetTitle(panel.TitleType, panel.TitlePrefix, panel.TitleSuffix);
                navBar.SetActive(panel.DisplayNavbar);
                backButton.gameObject.SetActive(panel.DisplayBack);

                var hasTuto = panelTutoManager != null && panelTutoManager.Count > 0;   

                try
                {
                    PageTipButton.transform.parent.gameObject.SetActive(hasTuto);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }

                if (hasTuto)
                {
                    PageTipButton.onClick.RemoveAllListeners();
                    PageTipButton.onClick.AddListener(() => PageTipDisplayer.Show(panelTutoManager));
                }
            };

            m_popupLinker.Initialize(popupTransform);

            connectionToImmersiveLinker.OnLeave += () =>
            {
                // Reset the set up skeleton to compute the size of the player.
                connectionToImmersiveLinker.SetSetUpSkeleton(null);
                new Task(async () =>
                {
                    while (connectionToImmersiveLinker.SetUpSkeleton == null)
                    {
                        await Task.Yield();
                    }
                    connectionToImmersiveLinker.StandUp();
                }).Start(TaskScheduler.FromCurrentSynchronizationContext());

                ShowUI();
                m_menuNavigationLinker.ShowPanel(m_mainMenuPanel);
                inGameLinker.EnableDisableInGameUI(false);
                connectionProcessorService.Disconnect();
                mainContainerLinker.Loader.ReloadScene();
            };

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded?.AddListener(() => {
                inGameLinker.EnableDisableInGameUI(true);
            } );

            UMI3DCollaborationClientServer.Instance.OnRedirectionStarted?.AddListener(() => {
                inGameLinker.EnableDisableInGameUI(false);
            });
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this, QuittingManagerNotificationKey.RequestToQuit);
        }

        private void PopupQuit()
        {
            Debug.Log("PopupQuit");
            m_popupLinker.Show(m_quittingPopup, "Quit", "empty",
                ("Quit", () => {
                    m_quittingNotifier[QuittingManagerNotificationKey.QuittingConfirmationInfo.Confirmation] = true;
                    m_quittingNotifier.Notify();
                }),
                ("Cancel", () => {
                    m_quittingNotifier[QuittingManagerNotificationKey.QuittingConfirmationInfo.Confirmation] = false;
                    m_quittingNotifier.Notify();
                    m_popupLinker.CloseAll();
                }
            ));
        }

        private void Start()
        {
            BindNavigationButtons();

#if UMI3D_XR
            m_popupLinker.OnPopupOpen += () => tween.TweenTo();
            m_popupLinker.OnPopupClose += () => tween.Rewind();
#endif

            SetVersion(Application.version);
            BindConnectionService();

            m_menuNavigationLinker.OnReplacePlayerAndShowPanel += () => {
                mainContainerLinker.Spawner.RepositionPlayer();
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
            inGameLinker.EnableDisableInGameUI(false);
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
                connectionToImmersiveLinker.Leave();
            });
#if UMI3D_XR
            connectionToImmersiveLinker.OnSkeletonStandUp += () =>
                parentTransform.position = new Vector3(parentTransform.position.x, Camera.main.transform.position.y, parentTransform.position.z);
#endif
        }

        private void BindConnectionService()
        {
            connectionServiceLinker.OnParamFormDtoReceived += (connectionFormDto) =>
            {
                if (parentTransform.gameObject.activeSelf == false)
                {
                    ShowUI();
                    mainContainerLinker.Spawner.RepositionPlayer();
                }
                m_menuNavigationLinker.ShowPanel(m_formPanel);
            };
            connectionServiceLinker.OnDivFormDtoReceived += (connectionFormDto) => {
                if (parentTransform.gameObject.activeSelf == false)
                {
                    ShowUI();
                    mainContainerLinker.Spawner.RepositionPlayer();
                }
                m_menuNavigationLinker.ShowPanel(m_formPanel);
            };

            connectionServiceLinker.OnConnectionSuccess += () => HideUI();
        }

        private void HideUI()
        {
            parentTransform.gameObject.SetActive(false);
            mainContainerLinker.Skybox.gameObject.SetActive(false);
            mainContainerLinker.DirectionalLight.gameObject.SetActive(false);
        }

        private void ShowUI()
        {
            parentTransform.gameObject.SetActive(true);
            mainContainerLinker.Skybox.gameObject.SetActive(true);
            mainContainerLinker.DirectionalLight.gameObject.SetActive(true);
        }
    }
}