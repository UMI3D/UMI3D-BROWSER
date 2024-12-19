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
using umi3d.baseBrowser.inputs.interactions;
using umi3d.browserRuntime.notificationKeys;
using umi3d.common.interaction;
using UnityEngine;

public class LeftClickParametersInteraction : MonoBehaviour
{
    private List<AbstractParameterDto> _parameters;

    Notifier displayParameterNotifier;

    private void Awake()
    {
        NotificationHub.Default.Subscribe<InteractionNotificationKeys.ParameterInputFound>(
            this,
            ParameterInputFound
        );

        NotificationHub.Default.Subscribe<InteractionNotificationKeys.ToolReleased>(
            this,
            ToolReleased
        );

        displayParameterNotifier = NotificationHub.Default
                .GetNotifier<InteractionNotificationKeys.DisplayParameters>(this);

        _parameters = new List<AbstractParameterDto>();
    }

    private void OnEnable()
    {
        KeyboardShortcut.AddDownListener(ShortcutEnum.DisplayHideContextualMenu, OnClick);
    }

    private void OnDisable()
    {
        KeyboardShortcut.RemoveDownListener(ShortcutEnum.DisplayHideContextualMenu, OnClick);
    }

    void OnDestroy()
    {
        NotificationHub.Default.Unsubscribe(this);
    }

    private void AddParameter(AbstractParameterDto dto)
    {
        _parameters.Add(dto);
    }

    private void Release()
    {
        _parameters.Clear();
    }

    private void OnClick()
    {
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
