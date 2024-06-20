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
using umi3d.cdk.userCapture.tracking;
using umi3dBrowsers.input;
using umi3dBrowsers.linker;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace umi3dBrowsers.player.navigation
{
    public class SnapTurn : MonoBehaviour
    {
        [SerializeField] float snapTurnAngle = 45;

        UMI3DVRPlayer player;

        [Header("Linker")]
        [SerializeField] private PlayerLinker playerLinker;

        private void Awake()
        {
            playerLinker.OnPlayerReady += player =>
            {
                this.player = player;

                if (Umi3dVRInputManager.ActionMap.TryGetValue(ControllerType.RightHandController, 
                    out var controllerAction))
                {
                    if (controllerAction.TryGetValue(ActionType.RightSnapTurn, out var inputAction))
                    {
                        inputAction.action.performed += i =>
                        {
                            Vector2 axis = i.ReadValue<Vector2>();
                            if (axis.x > 0)
                                transform.RotateAround(player.mainCamera.transform.position, Vector3.up, snapTurnAngle);
                            else if (axis.x < 0)
                                transform.RotateAround(player.mainCamera.transform.position, Vector3.up, -snapTurnAngle);
                        };
                    }
                }

            };
        }
    }
}