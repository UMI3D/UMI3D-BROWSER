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

using inetum.unityUtils;
using umi3d.baseBrowser.cursor;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.browserRuntime.ui;
using UnityEngine;

namespace umi3d.browserRuntime.shortcuts
{
    public class UiVisibilityShortcut : MonoBehaviour
    {
        private bool internalVisibilityState = true;

        private void OnEnable()
        {
            KeyboardShortcut.AddDownListener(ShortcutEnum.ToggleUiVisibility, ToggleUiVisibility);
        }

        private void OnDisable()
        {
            KeyboardShortcut.RemoveDownListener(ShortcutEnum.ToggleUiVisibility, ToggleUiVisibility);
        }

        private void ToggleUiVisibility()
        {
            internalVisibilityState = !internalVisibilityState;

            if (internalVisibilityState)
            {
                NotificationHub.Default.Notify(this, UiNotificationKeys.Show);
                BaseCursor.UnSetMovement(this);
            } else
            {
                var cursorMode = BaseCursor.Movement;
                NotificationHub.Default.Notify(this, UiNotificationKeys.Hide);
                BaseCursor.SetMovement(this, cursorMode);
            }
        }
    }
}
