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
using System.Collections.Generic;
using umi3d.baseBrowser.cursor;
using umi3d.browserRuntime.notificationKeys;
using umi3d.browserRuntime.ui.inGame.tablet;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    public class ContextualMenuView : MonoBehaviour
    {
        private void Awake()
        {
            NotificationHub.Default.Subscribe(this,
                ID.FromType<InteractionNotificationKeys.DisplayParameters>(), 
                (Callback)Display);

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ContextualMenuNotificationKeys.Close>(), 
                (Callback)Hide);
            NotificationHub.Default.Subscribe(this, 
                TabletNotificationKeys.Open, 
                (Callback)Hide);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void Display(Notification notification)
        {
            if (!notification.TryGetInfoT(InteractionNotificationKeys.DisplayParameters.parameters, out List<AbstractParameterDto> parameters))
                return;

            if (gameObject.activeSelf || parameters.Count <= 0)
                return;

            gameObject.SetActive(true);
            BaseCursor.SetMovement(this, BaseCursor.CursorMovement.Free);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            BaseCursor.UnSetMovement(this);
            BaseCursor.State = BaseCursor.CursorState.Default;
        }
    }
}