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
using umi3d.browserRuntime.navigation;
using umi3d.cdk;
using umi3d.cdk.collaboration.userCapture;
using umi3d.cdk.navigation;
using umi3d.cdk.notification;
using umi3dBrowsers.linker;
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
        [HideInInspector] public UMI3DSnapTurnProvider snapTurn;
        [HideInInspector] public UMI3DTeleportationProvider umi3dTeleportationProvider;

        /// <summary>
        /// The <see cref="UMI3DVRPlayer"/> linker.
        /// </summary>
        Linker<UMI3DVRPlayer> linker;
        [SerializeField] private ConnectionToImmersiveLinker connectionLinker;

        Request playerRequest;

        private void Awake()
        {
            logger.MainContext = this;
            logger.MainTag = nameof(UMI3DVRPlayer);

            mainCamera = xrOrigin?.Camera ?? Camera.main;
            locomotionSystem = xrOrigin?.GetComponentInChildren<LocomotionSystem>();
            teleportationProvider = locomotionSystem?.GetComponentInChildren<TeleportationProvider>();
            dynamicMoveProvider = locomotionSystem?.GetComponentInChildren<DynamicMoveProvider>();

            snapTurn = GetComponentInChildren<UMI3DSnapTurnProvider>();
            umi3dTeleportationProvider = GetComponentInChildren<UMI3DTeleportationProvider>();

            var navigationDelegate = new VRNavigationDelegate();
            navigationDelegate.Init(
                this,
                mainCamera.transform,
                xrOrigin.transform,
                personalSkeletonContainer.transform
            );
            navigation.Init(navigationDelegate);

            // SKELETON SERVICE
            CollaborationSkeletonsManager.Instance.navigation = navigationDelegate; //also use to init manager via Instance call

            linker = Linker.Get<UMI3DVRPlayer>(nameof(UMI3DVRPlayer));
        }

        private void Start()
        {
            personalSkeletonContainer.transform.SetParent(
                UMI3DLoadingHandler.Instance.transform,
                true
            );

            StartCoroutine(CenterCamera());
        }

        void OnEnable()
        {
            playerRequest = RequestHub.Default.
                SubscribeAsSupplier<UMI3DClientRequestKeys.PlayerRequest>(this);

            playerRequest[this, UMI3DClientRequestKeys.PlayerRequest.Transform] = () => transform;
            playerRequest[this, UMI3DClientRequestKeys.PlayerRequest.Camera] = () => mainCamera;
            playerRequest.NotifyClientsThatSupplierChanged();

            // Link is made at the end of the OnEnable method so that all the set up has been made.
            linker.Link(this);
        }

        void OnDisable()
        {
            RequestHub.Default.
                UnsubscribeAsSupplier<UMI3DClientRequestKeys.PlayerRequest>(this);

            // Unlink when disabled.
            linker.Link(null, false);
        }

        IEnumerator CenterCamera()
        {
            // Wait one frame so that the VR trackers update the position of gameObjects.
            yield return null;

            // Center the camera at the position of the player.
            PlayerTransformUtils.CenterCamera(mainCamera.transform.parent, mainCamera.transform);
        }



        [ContextMenu("Leave")]
        void DebugLeave()
        {
            connectionLinker.Leave();
        }
    }
}