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
using inetum.unityUtils.async;
using System;
using System.Threading.Tasks;
using umi3d.browserRuntime.player;
using umi3dVRBrowsersBase.interactions;
using UnityEngine;
using UnityEngine.Events;

namespace umi3dVRBrowsersBase.navigation
{
    public class SnapTurn : MonoBehaviour
    {
        [SerializeField] float snapTurnAngle = 45;

        Task<UMI3DVRPlayer> player;

        #region Methods

        private void Awake()
        {
            player = Global.GetAsync<UMI3DVRPlayer>();
        }

        private void Update()
        {
            var input = AbstractControllerInputManager.Instance;
            Func<bool> turnLeft = () =>
            {
                return input.GetLeftSnapTurn(ControllerType.LeftHandController)
                || input.GetLeftSnapTurn(ControllerType.RightHandController);
            };
            Func<bool> turnRight = () =>
            {
                return input.GetRightSnapTurn(ControllerType.LeftHandController)
                || input.GetRightSnapTurn(ControllerType.RightHandController);
            };

            player.IfCompleted(p =>
            {
                if (turnLeft())
                {
                    transform.RotateAround(p.mainCamera.transform.position, Vector3.up, -snapTurnAngle);
                }
                else if (turnRight())
                {
                    transform.RotateAround(p.mainCamera.transform.position, Vector3.up, snapTurnAngle);
                }
            });
        }

        #endregion
    }
}