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
using umi3d.browserRuntime.player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d.browserRuntime.navigation
{
    public class UMI3DLocomotionTarget : MonoBehaviour
    {
        [Tooltip("Whether this gameObject is the target for snap turn.")]
        [SerializeField] bool isSnapTurnTarget = true;
        [Tooltip("Whether this gameObject is the target for teleportation.")]
        [SerializeField] bool teleportationTarget = true;

        [Space]
        [Tooltip("The camera of the player. If null will user Camera.main")]
        [SerializeField] Camera mainCamera;

        void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        void OnEnable()
        {
            if (isSnapTurnTarget)
            {
                NotificationHub.Default.Subscribe(
                    this,
                    LocomotionNotificationKeys.SnapTurn,
                    null,
                    SnapTurn
                );
            }

            if (teleportationTarget)
            {
                NotificationHub.Default.Subscribe(
                    this,
                    LocomotionNotificationKeys.Teleportation,
                    null,
                    Teleport
                );
            }
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this, LocomotionNotificationKeys.SnapTurn);
            NotificationHub.Default.Unsubscribe(this, LocomotionNotificationKeys.Teleportation);
        }

        void SnapTurn(Notification notification)
        {
            if (!notification.TryGetInfoT(LocomotionNotificationKeys.Info.Direction, out int direction))
            {
                notification.LogError(nameof(UMI3DVRPlayer), LocomotionNotificationKeys.Info.Direction);
                return;
            }

            if (!notification.TryGetInfoT(LocomotionNotificationKeys.Info.TurnAmount, out float turnAmount))
            {
                notification.LogError(nameof(UMI3DVRPlayer), LocomotionNotificationKeys.Info.TurnAmount);
                return;
            }

            float angle = 0f;
            if (direction == 0)
            {
                angle = 180f;
            }
            else if (direction == 1)
            {
                angle = -turnAmount;
            }
            else
            {
                angle = turnAmount;
            }

            PlayerTransformUtils.SnapTurn(transform, mainCamera.transform, angle);
        }

        void Teleport(Notification notification)
        {
            if (!notification.TryGetInfoT(LocomotionNotificationKeys.Info.ActionPhase, out InputActionPhase phase))
            {
                return;
            }

            if (phase != InputActionPhase.Waiting)
            {
                // The teleportation happen when 'phase' equals 'InputActionPhase.Waiting'.
                return;
            }

            if (!notification.TryGetInfoT(LocomotionNotificationKeys.Info.Position, out Vector3 position))
            {
                return;
            }

            PlayerTransformUtils.MovePlayerAndCenterCamera(transform, mainCamera.transform, position);
        }
    }
}