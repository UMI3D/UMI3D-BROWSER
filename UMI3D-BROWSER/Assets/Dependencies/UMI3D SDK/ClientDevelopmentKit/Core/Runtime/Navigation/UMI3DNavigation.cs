﻿/*
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using umi3d.common;

namespace umi3d.cdk.navigation
{
    /// <summary>
    /// Navigation manager for user displacement in the environment. 
    /// </summary>
    public sealed class UMI3DNavigation
    {
        /// <summary>
        /// Current navigation system.
        /// </summary>
        public static INavigationDelegate currentNav = null;
        /// <summary>
        /// Different available navigation system.
        /// </summary>
        public static List<INavigationDelegate> navigations = new();

        public delegate void OnEmbarkVehicleDelegate(ulong vehicleId);

        public static event OnEmbarkVehicleDelegate onUpdateFrameDelegate;

        public void Init(params INavigationDelegate[] navigationDelegates)
        {
            navigations = new();
            navigations.AddRange(navigationDelegates);
            currentNav = navigations.FirstOrDefault();
            currentNav.Activate();
        }

        public static void SetFrame(ulong environmentId, FrameRequestDto frameRequest)
        {
            UnityEngine.Debug.LogError("Need to handle rescaling");
            if (currentNav != null)
            {
                onUpdateFrameDelegate?.Invoke(frameRequest.FrameId);

                currentNav.UpdateFrame(environmentId, frameRequest);

                var fConfirmation = new FrameConfirmationDto()
                {
                    userId = UMI3DClientServer.Instance.GetUserId()
                };

                UMI3DClientServer.SendRequest(fConfirmation, true);
            }
        }

        /// <summary>
        /// Move the user acording to a <see cref="NavigateDto"/>.
        /// </summary>
        /// <param name="dto"></param>
        public static IEnumerator Navigate(ulong environmentId, NavigateDto dto)
        {
            if (currentNav == null)
            {
                yield break;
            }

            switch (dto)
            {
                case ViewpointTeleportDto viewpointTeleportDto:
                    currentNav.ViewpointTeleport(environmentId, viewpointTeleportDto);
                    break;
                case TeleportDto teleportDto:
                    currentNav.Teleport(environmentId, teleportDto);
                    break;
                default:
                    currentNav.Navigate(environmentId, dto);
                    break;
            }

            yield break;
        }
    }
}