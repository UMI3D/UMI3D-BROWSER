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

using System;
using umi3d.browserRuntime.navigation;
using umi3d.cdk;
using umi3d.cdk.collaboration.userCapture;
using umi3d.cdk.navigation;
using umi3dVRBrowsersBase.interactions;
using umi3dVRBrowsersBase.navigation;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace umi3d.browserRuntime.player
{
    public class UMI3DVRPlayer : MonoBehaviour
    {
        public umi3d.debug.UMI3DLogger logger;

        public GameObject personalSkeletonContainer;
        public XROrigin xrOrigin;

        [HideInInspector] public Camera mainCamera;
        [HideInInspector] public Camera uiCamera;
        [HideInInspector] public LocomotionSystem locomotionSystem;
        [HideInInspector] public TeleportationProvider teleportationProvider;
        [HideInInspector] public DynamicMoveProvider dynamicMoveProvider;

        [HideInInspector] public UMI3DNavigation navigation = new();
        [HideInInspector] public SnapTurn snapTurn;
        public VRInputObserver teleportingLeft;
        public VRInputObserver teleportingRight;

        private void Awake()
        {
            logger.MainContext = this;
            logger.MainTag = nameof(UMI3DVRPlayer);

            mainCamera = xrOrigin?.Camera ?? Camera.main;
            locomotionSystem = xrOrigin?.GetComponentInChildren<LocomotionSystem>();
            teleportationProvider = locomotionSystem?.GetComponentInChildren<TeleportationProvider>();
            dynamicMoveProvider = locomotionSystem?.GetComponentInChildren<DynamicMoveProvider>();

            snapTurn = this?.GetComponent<SnapTurn>();

            var navigationDelegate = new VRNavigationDelegate();
            navigationDelegate.Init(
                this,
                mainCamera.transform,
                xrOrigin.transform,
                personalSkeletonContainer.transform,
                snapTurn,
                teleportingLeft,
                teleportingRight
            );
            navigation.Init(navigationDelegate);

            // SKELETON SERVICE
            CollaborationSkeletonsManager.Instance.navigation = navigationDelegate; //also use to init manager via Instance call
        }

        private void Start()
        {
            personalSkeletonContainer.transform.SetParent(
                UMI3DLoadingHandler.Instance.transform,
                true
            );
        }
    }
}