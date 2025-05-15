/*
Copyright 2019 - 2025 Inetum

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
using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.notificationKeys;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d
{
    internal class ContextualMenuOpenInputXR : MonoBehaviour
    {
        [SerializeField] InputActionReference _openInputAction;

        List<AbstractParameterDto> _parameters;

        Notifier displayParameterNotifier;

        private void Awake()
        {
            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<InteractionNotificationKeys.ParameterInputFound>(),
                (Callback)ParameterInputFound
            );

            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<InteractionNotificationKeys.ToolReleased>(),
                (Callback)ToolReleased
            );

            displayParameterNotifier = NotificationHub.Default.GetNotifier(
                this,
                ID.FromType<InteractionNotificationKeys.DisplayParameters>()
            );

            _parameters = new List<AbstractParameterDto>();
            _openInputAction.action.started += OnClick;
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
            _openInputAction.action.started -= OnClick;
        }

        private void AddParameter(AbstractParameterDto dto)
        {
            _parameters.Add(dto);
        }

        private void Release()
        {
            _parameters.Clear();
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            if (_parameters == null || _parameters.Count == 0)
                return;

            displayParameterNotifier[InteractionNotificationKeys.DisplayParameters.parameters] = _parameters;
            displayParameterNotifier.Notify();
        }

        void ParameterInputFound(Notification notification)
        {
            if (!notification.TryGetInfoT(InteractionNotificationKeys.ParameterInputFound.parameterDto, out AbstractParameterDto dto))
            {
                return;
            }

            AddParameter(dto);
        }

        void ToolReleased(Notification notification)
        {
            Release();
        }
    }
}