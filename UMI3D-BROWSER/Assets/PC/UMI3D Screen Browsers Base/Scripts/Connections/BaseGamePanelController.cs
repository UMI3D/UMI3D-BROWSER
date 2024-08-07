/*
Copyright 2019 - 2022 Inetum
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
using umi3d.baseBrowser.Controller;
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.commonScreen;
using umi3d.commonScreen.Displayer;
using umi3d.commonScreen.game;
using umi3dBrowsers.linker;
using UnityEngine;
using UnityEngine.UIElements;
using static umi3d.baseBrowser.cursor.BaseCursor;
using static umi3d.commonScreen.game.GamePanel_C;
using NotificationLoader = umi3d.baseBrowser.notification.NotificationLoader;

namespace umi3d.baseBrowser.connection
{
    public abstract partial class BaseGamePanelController : inetum.unityUtils.SingleBehaviour<BaseGamePanelController>
    {
        #region Field
        [Header("Linker")]
        [SerializeField] private ConnectionToImmersiveLinker connectionToImmersiveLinker;

        public UIDocument document;
        [HideInInspector]
        public PanelSettings PanelSettings;

        [HideInInspector]
        public NotificationLoader NotificationLoader;

        public GamePanel_C GamePanel;

        protected VisualElement root => document.rootVisualElement;
        protected VisualElement logo;

        protected DateTime m_time_Start;

        protected System.Action m_next;

        #endregion

        #region Initialization of the Connection Process

        protected void Init()
        {
            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(OnEnvironmentLoaded);
            UMI3DCollaborationClientServer.Instance.OnRedirectionAborted.AddListener(OnRedirectionAborted);
            UMI3DCollaborationClientServer.Instance.OnConnectionLost?.AddListener(OnConnectionLost);
            UMI3DCollaborationClientServer.Instance.OnForceLogoutMessage.AddListener(OnForceLogoutMessage);
            Debug.Log("TODO: SETUP ASK FOR LIBS");
            /*BaseConnectionProcess.Instance.AskForDownloadingLibraries += (count, callback) =>
            {
                var dialoguebox = new Dialoguebox_C();
                dialoguebox.Type = DialogueboxType.Confirmation;
                dialoguebox.Title = new LocalisationAttribute
                (
                    (count == 1) ? $"One assets library is required" : $"{count} assets libraries are required",
                    "ErrorStrings", 
                    (count == 1) ? "AssetsLibRequired1" : "AssetsLibRequired", 
                    (count == 1) ? null : new string[1] { count.ToString() }
                );
                dialoguebox.Message = new LocalisationAttribute("Download libraries and connect to the server ?", "ErrorStrings", "DownloadAndConnect");
                dialoguebox.ChoiceA.Type = ButtonType.Default;
                dialoguebox.ChoiceAText = new LocalisationAttribute("Accept", "GenericStrings", "Accept");
                dialoguebox.ChoiceB.Type = ButtonType.Default;
                dialoguebox.ChoiceBText = new LocalisationAttribute("Deny", "GenericStrings", "Deny");
                dialoguebox.Callback = (index) => callback?.Invoke(index == 0);
                dialoguebox.Enqueue(root);
            };*/
            UMI3DEnvironmentClient.EnvironementLoaded.AddListener(EnvironmentLoaded);
            UMI3DCollaborationEnvironmentLoader.Instance.OnUpdateJoinnedUserList += OnUpdateJoinnedUserList;
        }

        private void OnUpdateJoinnedUserList()
        {
            var count = UMI3DCollaborationEnvironmentLoader.Instance.JoinnedUserList.Count;
            Game.NotifAndUserArea.UserList.RefreshList();
            Game.NotifAndUserArea.OnUserCountUpdated(count);
            Menu.GameData.ParticipantCount = count;
        }

        private void EnvironmentLoaded()
        {
            Menu.Libraries.InitLibraries();
            Menu.Tips.InitTips();
            EnvironmentSettings.Instance.AudioSetting.GeneralVolume = ((int)Menu.Settings.Audio.Data.GeneralVolume) / 10f;
        }

        private void OnForceLogoutMessage(string message)
        {
            var dialoguebox = new Dialoguebox_C();
            dialoguebox.Type = DialogueboxType.Default;
            dialoguebox.Title = new LocalisationAttribute("Forced Deconnection", "ErrorStrings", "ForcedDeco");
            dialoguebox.Message = message;
            dialoguebox.ChoiceAText = new LocalisationAttribute("Leave", "GenericStrings", "Leave");
            dialoguebox.Callback = (index) => connectionToImmersiveLinker.Leave();
            dialoguebox.Enqueue(root);
        }

        private void OnConnectionLost()
        {
            BaseController.CanProcess = false;

            var dialoguebox = new Dialoguebox_C();
            dialoguebox.Type = DialogueboxType.Confirmation;
            dialoguebox.Title = new LocalisationAttribute("Connection to the server lost", "ErrorStrings", "ConnectionLost");
            dialoguebox.Message = new LocalisationAttribute
            (
                "Leave the environment or try to reconnect ?",
                "ErrorStrings", "LeaveOrTry"
            );
            dialoguebox.ChoiceAText = new LocalisationAttribute("Reconnect", "GenericStrings", "Reconnect");
            dialoguebox.ChoiceA.Type = ButtonType.Default;
            dialoguebox.ChoiceBText = new LocalisationAttribute("Leave", "GenericStrings", "Leave");
            dialoguebox.Callback = (index) => {
                BaseController.CanProcess = true;
                if (index == 0)
                    UMI3DCollaborationClientServer.Reconnect();
                else
                    connectionToImmersiveLinker.Leave();
            };
            dialoguebox.Enqueue(root);
        }

        private void OnRedirectionAborted()
        {
            GamePanel.AddScreenToStack = GameViews.Game;
        }

        private void OnEnvironmentLoaded()
        {
            Debug.Log("OnEnvironmentLoaded");
            GamePanel.AddScreenToStack = GameViews.Game;
            m_isContextualMenuDown = false;
            BaseController.Instance.CurrentController.ResetInputsWhenEnvironmentLaunch();
            OnMenuObjectContentChange();
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(document != null);
            Debug.Assert(FormMenu != null);
            Debug.Assert(formMenuDisplay != null);

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            PanelSettings = Resources.Load<PanelSettings>("PanelSettings");
            NotificationLoader = Resources.Load<NotificationLoader>("Scriptables/GamePanel/NotificationLoader");
            m_time_Start = DateTime.Now;
        }

        protected virtual void Start()
        {
            root.Add(TooltipsLayer_C.Instance);
            GamePanel = root.Q<GamePanel_C>();

            Init();

            InitLoader();
            InitMenu();
            InitGame();

            InitControls();

            GamePanel.CurrentView = GameViews.Loader;

            connectionToImmersiveLinker.OnLeave += () =>
            {
                UnityEngine.Debug.Log($"leave");
                var clhGameObject = UMI3DCollaborationLoadingHandler.Instance.gameObject;
                UMI3DCollaborationLoadingHandler.Destroy();
                Destroy(clhGameObject);
            };
        }

        #region OnDestroy

        protected virtual void ConcludeGame_UserList()
        {
            UMI3DEnvironmentClient.EnvironementJoinned.RemoveListener(Game.NotifAndUserArea.UserList.OnEnvironmentChanged);
            UMI3DUser.OnUserMicrophoneStatusUpdated.RemoveListener(Game.NotifAndUserArea.UserList.UpdateUser);
            UMI3DUser.OnUserAvatarStatusUpdated.RemoveListener(Game.NotifAndUserArea.UserList.UpdateUser);
            UMI3DUser.OnUserAttentionStatusUpdated.RemoveListener(Game.NotifAndUserArea.UserList.UpdateUser);
            UMI3DUser.OnRemoveUser.RemoveListener(Game.NotifAndUserArea.UserList.RemoveUser);

            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.RemoveListener(OnEnvironmentLoaded);
            UMI3DCollaborationClientServer.Instance.OnRedirectionAborted.RemoveListener(OnRedirectionAborted);
            UMI3DCollaborationClientServer.Instance.OnConnectionLost?.RemoveListener(OnConnectionLost);
            UMI3DCollaborationClientServer.Instance.OnForceLogoutMessage.RemoveListener(OnForceLogoutMessage);
            Debug.Log("TODO: SETUP ASK FOR LIBS");
            UMI3DEnvironmentClient.EnvironementLoaded.RemoveListener(EnvironmentLoaded);
            UMI3DCollaborationEnvironmentLoader.Instance.OnUpdateJoinnedUserList -= OnUpdateJoinnedUserList;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ConcludeGame_UserList();
        }

        #endregion

        float fps = 30;
        private void Update()
        {
            var time = DateTime.Now - m_time_Start;
            Menu.GameData.Time = time.ToString("hh") + ":" + time.ToString("mm") + ":" + time.ToString("ss");

            //FrameManagement();
        }

        protected void FrameManagement()
        {
            float newFPS = 1.0f / Time.deltaTime;
            fps = Mathf.Lerp(fps, newFPS, Time.deltaTime);

            UnityEngine.Debug.Log($"fps = {fps}, {Menu.Settings.Resolution.RenderScaleSlider.value}, {QualitySettings.names[QualitySettings.GetQualityLevel()]}");

            if (GamePanel.CurrentView != GameViews.Game || Menu.Settings.Resolution.GameResolutionSegmentedPicker.ValueEnum == preferences.SettingsPreferences.ResolutionEnum.Custom) return;

            var renderScaleValue = Menu.Settings.Resolution.RenderScaleSlider.value;

            if (fps < Menu.Settings.Resolution.TargetFPS - 20 && renderScaleValue > 0.01) Menu.Settings.Resolution.RenderScaleValueChanged(renderScaleValue * 0.95f);
            else if (fps > Menu.Settings.Resolution.TargetFPS - 5) Menu.Settings.Resolution.RenderScaleValueChanged(renderScaleValue * 1.05f);
        }

        public void UpdateCursor(CursorState state)
        {
            switch (state)
            {
                case CursorState.Default:
                    Game.Cursor.State = ElementPseudoState.Enabled;
                    break;
                case CursorState.Hover:
                    Game.Cursor.State = ElementPseudoState.Hover;
                    break;
                case CursorState.Clicked:
                    Game.Cursor.State = ElementPseudoState.Hover;
                    break;
                case CursorState.FollowCursor:
                    Game.Cursor.State = ElementPseudoState.Enabled;
                    break;
                default:
                    break;
            }
        }


    }
}