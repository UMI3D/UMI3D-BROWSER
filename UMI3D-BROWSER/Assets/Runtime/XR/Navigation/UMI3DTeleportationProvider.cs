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

using inetum.unityUtils;
using umi3d.browserRuntime.player;
using umi3dVRBrowsersBase.interactions;
using umi3dVRBrowsersBase.navigation;
using UnityEngine;


namespace umi3d.browserRuntime.navigation
{
    /// <summary>
    /// Teleports users where they want. 
    /// </summary>
    public class UMI3DTeleportationProvider : MonoBehaviour
    {
        /// <summary>
        /// Left teleportation arc preview.
        /// </summary>
        [SerializeField] TeleportArc leftArc;
        /// <summary>
        /// Right teleportation arc preview.
        /// </summary>
        [SerializeField] TeleportArc rightArc;

        /// <summary>
        /// The amount of time that UMI3D waits before performing another action.
        /// </summary>
        [SerializeField] float debounceTime = .3f;

        UMI3DVRPlayer player;

        ActionMovementTiming actionMovementTiming;

        AbstractControllerInputManager input => AbstractControllerInputManager.Instance;

        void Awake()
        {
            Linker
                .Get<UMI3DVRPlayer>(nameof(UMI3DVRPlayer))
                .linked += (player, isSet) =>
                {
                    this.player = isSet ? player : null;
                };

            actionMovementTiming = new();
            actionMovementTiming.DebounceTime = () => debounceTime;
        }

        void Update()
        {
            if (player == null)
            {
                return;
            }

            if (actionMovementTiming.CanPerformAction())
            {
                if (input.GetTeleportDown(ControllerType.LeftHandController))
                {
                    leftArc.Display();
                }
                else if (input.GetTeleportUp(ControllerType.LeftHandController))
                {
                    Teleport(leftArc.GetPointedPoint());

                    leftArc.Hide();
                }

                if (input.GetTeleportDown(ControllerType.RightHandController))
                {
                    rightArc.Display();
                }
                else if (input.GetTeleportUp(ControllerType.RightHandController))
                {
                    Teleport(rightArc.GetPointedPoint());

                    rightArc.Hide();
                }
            } else
            {
                if (input.GetTeleportUp(ControllerType.LeftHandController))
                {
                    leftArc.Hide();
                }
                if (input.GetTeleportUp(ControllerType.RightHandController))
                {
                    rightArc.Hide();
                }
            }
        }

        /// <summary>
        /// Teleports player.
        /// </summary>
        void Teleport(Vector3? position)
        {
            if (!position.HasValue || player == null)
            {
                return;
            }

            PlayerTransformUtils.MovePlayerAndCenterCamera(player.transform, player.mainCamera.transform, position.Value);

            actionMovementTiming.ResetDebounceTime();
        }
    }
}
