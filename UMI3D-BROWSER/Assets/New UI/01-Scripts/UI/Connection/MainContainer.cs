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
using System.Security.Policy;
using System.Threading.Tasks;
using TMPro;
using umi3d.browserRuntime.notificationKeys;
using umi3d.browserRuntime.ui.inGame;
using umi3d.browserRuntime.ui.popup;
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

        private Notifier m_quittingNotifier;
        private Notifier m_enableInGameUiNotifier;

        const string LOCALIZATION_TABLE = "UMI3D_inetum";
        PopupNotifier popupNotifier;

        private void Awake()
        {
            Notifier _popupNotifier = NotificationHub.Default.GetNotifier<PopupNotificationKeys.EnqueuePopup>(this);
            popupNotifier = new(_popupNotifier);

            m_quittingNotifier = NotificationHub.Default.GetNotifier(this, QuittingManagerNotificationKey.QuittingConfirmation);
            m_enableInGameUiNotifier = NotificationHub.Default.GetNotifier(this, InGameNotificationKeys.EnableInGameUi);
            
            NotificationHub.Default.Subscribe(
                this, 
                QuittingManagerNotificationKey.RequestToQuit, 
                TryToQuit
            );

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

        void TryToQuit()
        {
            popupNotifier
                .SetType(PopupType.Information)
                .SetTitle(LOCALIZATION_TABLE, "Quit")
                .SetButtons((LOCALIZATION_TABLE, "Quit"), (LOCALIZATION_TABLE, "Cancel"))
                .SetButtonsAction(index =>
                {
                    m_quittingNotifier[QuittingManagerNotificationKey.QuittingConfirmationInfo.Confirmation] = index == 0;
                    m_quittingNotifier.Notify();
                })
                .Notify();
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this, QuittingManagerNotificationKey.RequestToQuit);

            connectionServiceLinker.OnTryToConnect -= OnTryToConnect;
            connectionServiceLinker.OnConnectionFailure -= OnConnectionFailure;
            UMI3DClientServer.Instance.OnConnectionLost.RemoveListener(OnConnectionLost);
            UMI3DCollaborationClientServer.Instance.OnForceLogoutMessage.RemoveListener(OnForceLogoutMessage);
            connectionServiceLinker.OnAnswerFailed -= OnAnswerFailed;
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

            connectionServiceLinker.OnTryToConnect += OnTryToConnect;
            connectionServiceLinker.OnConnectionFailure += OnConnectionFailure;
            UMI3DClientServer.Instance.OnConnectionLost.AddListener(OnConnectionLost);
            UMI3DCollaborationClientServer.Instance.OnForceLogoutMessage.AddListener(OnForceLogoutMessage);
            connectionServiceLinker.OnMediaServerPingSuccess += (virtualWorldData) => {
                NotificationHub.Default.Notify<PopupNotificationKeys.CloseAll>(this);
            };
            connectionServiceLinker.OnAnswerFailed += OnAnswerFailed;
            connectionServiceLinker.OnAsksToLoadLibrairies += (ids, action) => action?.Invoke(true);

            m_menuNavigationLinker.ShowStartPanel();
            m_enableInGameUiNotifier[InGameNotificationKeys.IsInGameUiActive] = false;
            m_enableInGameUiNotifier.Notify();
        }

        void OnTryToConnect(string url)
        {
            popupNotifier
                .SetType(PopupType.Information)
                .SetArguments(("url", url))
                .SetTitle(LOCALIZATION_TABLE, "popup_connection_server")
                .SetDescription(LOCALIZATION_TABLE, "popup_trying_connect")
                .Notify();
        }

        void OnConnectionFailure(string message)
        {
            popupNotifier
                 .SetType(PopupType.Error)
                 .SetArguments(("error", message))
                 .SetTitle(LOCALIZATION_TABLE, "popup_fail_connect")
                 .SetDescription(LOCALIZATION_TABLE, "error_msg")
                 .SetButtons((LOCALIZATION_TABLE, "popup_close"))
                 .Notify();
        }

        void OnConnectionLost()
        {
            popupNotifier
                 .SetType(PopupType.Error)
                 .SetTitle(LOCALIZATION_TABLE, "popup_forced_leave")
                 .SetDescription(LOCALIZATION_TABLE, "popup_connection_lost_msg")
                 .SetButtons((LOCALIZATION_TABLE, "popup_connection_lost_leave"), (LOCALIZATION_TABLE, "popup_connection_lost_retry"))
                 .SetButtonsAction(index =>
                 {
                     if (index == -1 || index == 0)
                     {
                         connectionToImmersiveLinker.Leave();
                     }
                     else
                     {
                         UMI3DCollaborationClientServer.Reconnect();
                     }
                 })
                 .Notify();
        }

        void OnForceLogoutMessage(string message)
        {
            popupNotifier
                 .SetType(PopupType.Error)
                 .SetArguments(("message", message))
                 .SetTitle(LOCALIZATION_TABLE, "popup_forced_leave")
                 .SetDescription(LOCALIZATION_TABLE, "popup_forced_leave_msg")
                 .SetButtons((LOCALIZATION_TABLE, "popup_connection_lost_leave"))
                 .SetButtonsAction(index =>
                 {
                     connectionToImmersiveLinker.Leave();
                 })
                 .Notify();
        }

        void OnAnswerFailed()
        {
            popupNotifier
                 .SetType(PopupType.Error)
                 .SetTitle(LOCALIZATION_TABLE, "popup_answer_failed_title")
                 .SetDescription(LOCALIZATION_TABLE, "popup_answer_failed_description")
                 .SetButtons((LOCALIZATION_TABLE, "popup_close"))
                 .Notify();
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