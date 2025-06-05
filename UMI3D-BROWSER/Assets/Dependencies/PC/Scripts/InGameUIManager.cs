/*
Copyright 2019 - 2024 Inetum

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

using inetum.unityUtils.observation;
using umi3d.browserRuntime.cursor;
using umi3d.browserRuntime.ui.inGame;
using UnityEngine;
using umi3d.browserRuntime.ui.tablet;

namespace umi3dBrowsers.ingame_ui
{
    public class InGameUIManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private TabletView TabletPanel;

        [Header("Debug")]
        [SerializeField] private bool debugMode;

        private void Awake()
        {
            //if (TabletPanel)
                //KeyboardShortcut.AddDownListener(ShortcutEnum.DisplayHideGameMenu, ToggleInGamePanel);

            NotificationHub.Default.Subscribe(
                this, 
                InGameNotificationKeys.EnableInGameUi, 
                (Callback)SetActive
            );

            //BaseCursor.SetMovement(this, CursorMovement.Free);
        }

        private void Start()
        {
            gameObject.SetActive(debugMode);
        }

        private void OnEnable()
        {
            //KeyboardShortcut.AddUpListener(ShortcutEnum.FreeCursor, FreeCursor);
            //BaseCursor.SetMovement(this, CursorMovement.Center);
        }

        private void OnDisable()
        {
            //KeyboardShortcut.RemoveUpListener(ShortcutEnum.FreeCursor, FreeCursor);
            //BaseCursor.UnSetMovement(this);
        }

        private void FreeCursor()
        {
            //if (KeyboardShortcut.IsEditingTextField)
                //return;
            if (TabletPanel && TabletPanel.gameObject.activeSelf)
                return;

            //if (BaseCursor.Movement == CursorMovement.Center)
            //    BaseCursor.SetMovement(this, CursorMovement.Free);
            //else
            //    BaseCursor.SetMovement(this, CursorMovement.Center);
        }

        private void ToggleInGamePanel()
        {
            //if (KeyboardShortcut.IsEditingTextField)
                //return;
            if (!gameObject.activeSelf)
                return;

            if (TabletPanel.gameObject.activeSelf)
                NotificationHub.Default.Notify(this, ID.FromType<TabletNotificationKeys.Close>());
            else
                NotificationHub.Default.Notify(this, ID.FromType<TabletNotificationKeys.Open>());
        }

        private void SetActive(Notification notification)
        {
            if (notification.TryGetInfoT<bool>(InGameNotificationKeys.IsInGameUiActive, out var active))
                gameObject.SetActive(active);
        }
    }
}
