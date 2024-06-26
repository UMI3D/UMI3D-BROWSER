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
using UnityEngine;

namespace umi3d.browserRuntime.navigation
{
    public class UMI3DSnapTurnProvider : MonoBehaviour
    {
        [SerializeField] float turnAmount = 45;

        /// <summary>
        /// The amount of time that UMI3D waits before performing another action.
        /// </summary>
        [SerializeField] float debounceTime = .3f;

        UMI3DVRPlayer player;

        ActionMovementTiming actionMovementTiming;

        AbstractControllerInputManager input => AbstractControllerInputManager.Instance;

        bool turnLeft => input.GetLeftSnapTurn(ControllerType.LeftHandController)
            || input.GetLeftSnapTurn(ControllerType.RightHandController);

        bool turnRight => input.GetRightSnapTurn(ControllerType.LeftHandController)
            || input.GetRightSnapTurn(ControllerType.RightHandController);

        private void Awake()
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

        private void Update()
        {
            if (player == null)
            {
                return;
            }

            if (actionMovementTiming.CanPerformAction())
            {
                if (turnLeft)
                {
                    PlayerTransformUtils.SnapTurn(player.transform, player.mainCamera.transform, -turnAmount);

                    actionMovementTiming.ResetDebounceTime();
                }
                else if (turnRight)
                {
                    PlayerTransformUtils.SnapTurn(player.transform, player.mainCamera.transform, turnAmount);

                    actionMovementTiming.ResetDebounceTime();
                }
            }
        }
    }
}