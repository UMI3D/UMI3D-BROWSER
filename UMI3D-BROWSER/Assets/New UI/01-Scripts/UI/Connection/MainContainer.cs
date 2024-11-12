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
using umi3d.browserRuntime.ui.popup;
using umi3d.browserRuntime.ui.inGame;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3dBrowsers.data.ui;
using umi3dBrowsers.linker;
using umi3dBrowsers.linker.ui;
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
        [SerializeField] private PanelData m_mainMenuPanel;
        [SerializeField] private PanelData m_formPanel;

        private Notifier m_quitPopupNotifier;
        private Notifier m_popupTryToConnectNotifier;
        private Notifier m_popupConnectionFailedNotifier;
        private Notifier m_popupConnectionLostNotifier;
        private Notifier m_popupConnectionForceLogoutNotifier;
        private Notifier m_popupAnswerFailedNotifier;
        private Notifier m_quittingNotifier;
        private Notifier m_enableInGameUiNotifier;

        private void Awake()
        {
            SetupQuitPopupNotifier();
            SetupTryToConnectPopupNotifier();
            SetupConnectionFailedPopupNotifier();
            SetupConnectionLostPopupNotifier();
            SetupAnswerFailedPopupNotifier();
            SetupConnectionForceLogoutPopupNotifier();
            m_quittingNotifier = NotificationHub.Default.GetNotifier(this, QuittingManagerNotificationKey.QuittingConfirmation);
            m_enableInGameUiNotifier = NotificationHub.Default.GetNotifier(this, InGameNotificationKeys.EnableInGameUi);
             NotificationHub.Default.Subscribe(this, QuittingManagerNotificationKey.RequestToQuit, () => {
                m_quitPopupNotifier.Notify();
            });

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

                m_enableInGameUiNotifier[InGameNotificationKeys.IsInGameUiActive] = false;
                m_enableInGameUiNotifier.Notify();

                connectionProcessorService.Disconnect();
                mainContainerLinker.Loader.ReloadScene();
            };

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded?.AddListener(() => {
                m_enableInGameUiNotifier[InGameNotificationKeys.IsInGameUiActive] = true;
                m_enableInGameUiNotifier.Notify();
            } );

            UMI3DCollaborationClientServer.Instance.OnRedirectionStarted?.AddListener(() => {
                m_enableInGameUiNotifier[InGameNotificationKeys.IsInGameUiActive] = false;
                m_enableInGameUiNotifier.Notify();
            });
        }

        private void SetupQuitPopupNotifier()
        {
            m_quitPopupNotifier = NotificationHub.Default.GetNotifier<PopupNotificationKeys.Show>(this);
            m_quitPopupNotifier[PopupNotificationKeys.Show.Type] = PopupType.Information;
            m_quitPopupNotifier[PopupNotificationKeys.Show.Title] = "Quit";
            m_quitPopupNotifier[PopupNotificationKeys.Show.Buttons] = new List<(string, Action)>() {
                ("Quit", () => {
                    m_quittingNotifier[QuittingManagerNotificationKey.QuittingConfirmationInfo.Confirmation] = true;
                    m_quittingNotifier.Notify();
                }),
                ("Cancel", () => {
                    m_quittingNotifier[QuittingManagerNotificationKey.QuittingConfirmationInfo.Confirmation] = false;
                    m_quittingNotifier.Notify();
                    NotificationHub.Default.Notify<PopupNotificationKeys.CloseAll>(this);
                }
            )};
        }

        private void SetupTryToConnectPopupNotifier()
        {
            m_popupTryToConnectNotifier = NotificationHub.Default.GetNotifier<PopupNotificationKeys.Show>(this);
            m_popupTryToConnectNotifier[PopupNotificationKeys.Show.Type] = PopupType.Information;
            m_popupTryToConnectNotifier[PopupNotificationKeys.Show.Title] = "popup_connection_server";
            m_popupTryToConnectNotifier[PopupNotificationKeys.Show.Description] = "popup_trying_connect";
        }

        private void SetupConnectionFailedPopupNotifier()
        {
            m_popupConnectionFailedNotifier = NotificationHub.Default.GetNotifier<PopupNotificationKeys.Show>(this);
            m_popupConnectionFailedNotifier[PopupNotificationKeys.Show.Type] = PopupType.Error;
            m_popupConnectionFailedNotifier[PopupNotificationKeys.Show.Title] = "popup_fail_connect";
            m_popupConnectionFailedNotifier[PopupNotificationKeys.Show.Description] = "error_msg";
            m_popupConnectionFailedNotifier[PopupNotificationKeys.Show.Buttons] = new List<(string, Action)>() {
                ("popup_close", () => {
                    NotificationHub.Default.Notify<PopupNotificationKeys.CloseAll>(this);
                })
            };
        }

        private void SetupConnectionLostPopupNotifier()
        {
            m_popupConnectionLostNotifier = NotificationHub.Default.GetNotifier<PopupNotificationKeys.Show>(this);
            m_popupConnectionLostNotifier[PopupNotificationKeys.Show.Type] = PopupType.Error;
            m_popupConnectionLostNotifier[PopupNotificationKeys.Show.Title] = "popup_forced_leave";
            m_popupConnectionLostNotifier[PopupNotificationKeys.Show.Description] = "popup_connection_lost_msg";
            m_popupConnectionLostNotifier[PopupNotificationKeys.Show.Buttons] = new List<(string, Action)>() {
                ("popup_connection_lost_leave", ()=>{
                    connectionToImmersiveLinker.Leave();
                    NotificationHub.Default.Notify<PopupNotificationKeys.CloseAll>(this);
                }),
                ("popup_connection_lost_retry", () =>{
                    UMI3DCollaborationClientServer.Reconnect();
                    NotificationHub.Default.Notify<PopupNotificationKeys.CloseAll>(this); 
                })
            };
        }

        private void SetupConnectionForceLogoutPopupNotifier()
        {
            m_popupConnectionForceLogoutNotifier = NotificationHub.Default.GetNotifier<PopupNotificationKeys.Show>(this);
            m_popupConnectionForceLogoutNotifier[PopupNotificationKeys.Show.Type] = PopupType.Error;
            m_popupConnectionForceLogoutNotifier[PopupNotificationKeys.Show.Title] = "popup_forced_leave";
            m_popupConnectionForceLogoutNotifier[PopupNotificationKeys.Show.Description] = "popup_forced_leave_msg";
            m_popupConnectionForceLogoutNotifier[PopupNotificationKeys.Show.Buttons] = new List<(string, Action)>() {
                ("popup_connection_lost_leave", ()=>{
                    connectionToImmersiveLinker.Leave(); 
                    NotificationHub.Default.Notify<PopupNotificationKeys.CloseAll>(this);
                }),
            };
        }

        private void SetupAnswerFailedPopupNotifier()
        {
            m_popupAnswerFailedNotifier = NotificationHub.Default.GetNotifier<PopupNotificationKeys.Show>(this);
            m_popupAnswerFailedNotifier[PopupNotificationKeys.Show.Type] = PopupType.Error;
            m_popupAnswerFailedNotifier[PopupNotificationKeys.Show.Title] = "popup_answer_failed_title";
            m_popupAnswerFailedNotifier[PopupNotificationKeys.Show.Description] = "popup_answer_failed_description";
            m_popupAnswerFailedNotifier[PopupNotificationKeys.Show.Buttons] = new List<(string, Action)>() {
                ("popup_close", () => {
                    NotificationHub.Default.Notify<PopupNotificationKeys.CloseAll>(this);
                })
            };
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this, QuittingManagerNotificationKey.RequestToQuit);
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
                m_popupTryToConnectNotifier[PopupNotificationKeys.Show.Arguments] = 
                    new Dictionary<string, object>() { { "url", url } };
                m_popupTryToConnectNotifier.Notify();
            };
            connectionServiceLinker.OnConnectionFailure += (message) => {
                m_popupConnectionFailedNotifier[PopupNotificationKeys.Show.Arguments] = 
                    new Dictionary<string, object>() { { "error", message } };
                m_popupConnectionFailedNotifier.Notify();
            };
            UMI3DClientServer.Instance.OnConnectionLost.AddListener(() => {
                m_popupConnectionLostNotifier.Notify();
            });
            UMI3DCollaborationClientServer.Instance.OnForceLogoutMessage.AddListener((message) => {
                m_popupConnectionForceLogoutNotifier[PopupNotificationKeys.Show.Arguments] =
    new Dictionary<string, object>() { { "message", message } };
                m_popupConnectionForceLogoutNotifier.Notify();
            });
            connectionServiceLinker.OnMediaServerPingSuccess += (virtualWorldData) => {
                NotificationHub.Default.Notify<PopupNotificationKeys.CloseAll>(this);
            };
            connectionServiceLinker.OnAnswerFailed += () => {
                m_popupAnswerFailedNotifier.Notify();
            };
            connectionServiceLinker.OnAsksToLoadLibrairies += (ids, action) => action?.Invoke(true);

            m_menuNavigationLinker.ShowStartPanel();
            m_enableInGameUiNotifier[InGameNotificationKeys.IsInGameUiActive] = false;
            m_enableInGameUiNotifier.Notify();
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
            void Show()
            {
                if (parentTransform.gameObject.activeSelf == false)
                {
                    ShowUI();
                    mainContainerLinker.Spawner.RepositionPlayer();
                    connectionToImmersiveLinker.DisplayEnvironmentHandler();
                }
                m_menuNavigationLinker.ShowPanel(m_formPanel);
            };

            connectionServiceLinker.OnParamFormDtoReceived += (connectionFormDto) => Show();
            connectionServiceLinker.OnDivFormDtoReceived += (connectionFormDto) => Show();
            connectionServiceLinker.OnWaitReceived += (connectionFormDto) => Show();

            connectionServiceLinker.OnConnectionSuccess += () => {
                HideUI();
                connectionToImmersiveLinker.StopDisplayEnvironmentHandler();
            };
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