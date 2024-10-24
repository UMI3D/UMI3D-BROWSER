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
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardHaptic : MonoBehaviour
    {
        [SerializeField] float amplitude = .1f;
        [SerializeField] float duration = .1f;

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.KeyHovered,
                null,
                Haptic
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, KeyboardNotificationKeys.KeyHovered);
        }

        void Haptic(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.PointerEventData, out PointerEventData pointerEventData))
            {
                return;
            }

            if (pointerEventData is TrackedDeviceEventData trackedDeviceEventData
                && trackedDeviceEventData.interactor is XRBaseControllerInteractor xrInteractor)
            {
                xrInteractor.SendHapticImpulse(amplitude, duration);
            }
        }
    }
}