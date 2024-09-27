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
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.player;
using umi3d.cdk.userCapture.tracking;
using umi3d.common.userCapture;
using umi3dBrowsers.linker;
using umi3dVRBrowsersBase.ikManagement;
using Unity.XR.CoreUtils;
using UnityEngine;
using umi3d.cdk.userCapture.tracking.constraint;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace umi3dBrowsers.connection
{
    /// <summary>
    /// Sets up the avatar when users set their height and manages its display.
    /// </summary>
    public class SetUpSkeleton : MonoBehaviour
    {
        [Header("Linker")]
        [SerializeField] private ConnectionToImmersiveLinker linker;
        
        /// <summary>
        /// Avatar skeleton root.
        /// </summary>
        public Transform skeletonContainer;

        public List<Tracker> controllerIKTrackers = new List<Tracker>();

        public List<Tracker> handTrackingIKTrackers = new List<Tracker>();
        public List<FingerTracker> handTrackingFingerTrackers = new List<FingerTracker>();

        public List<Tracker> AdditionnalTrackers = new List<Tracker>();

        List<ITracker> handTrackingTrackers = new();

        public SkinnedMeshRenderer Joint, Surface;
        public GameObject LeftWatch, RightWatch;

        /// <summary>
        /// ?
        /// </summary>
        public umi3dVRBrowsersBase.ikManagement.FootTargetBehavior FootTargetBehavior;

        /// <summary>
        /// Avatar's neck.
        /// </summary>
        public Transform Neck;

        /// <summary>
        /// Factor to smooth body rotation.
        /// </summary>
        public float smoothRotationSpeed = .1f;

        /// <summary>
        /// If users turn their heads more than this angle, the reset of fthe body will turn too.
        /// </summary>
        public float maxAngleBeforeRotating = 50;

        public XRInputModalityManager XRInputModalityManager;

        [SerializeField]
        bool debugJointAndSurface = false;


        public event System.Action SetupDone;
        public event System.Action SkeletonResized;
        
        /// <summary>
        /// Is avatar's height set up ?
        /// </summary>
        bool isSetup = false;
        TrackedSubskeleton trackedSkeleton;
        /// <summary>
        /// Skeleton scale associated to users heights.
        /// </summary>
        static Vector3 sessionScaleFactor = default;
        /// <summary>
        /// Computed neck position
        /// </summary>
        Vector3 startingVirtualNeckPosition;
        /// <summary>
        /// Distance between <see cref="startingVirtualNeckPosition"/> and <see cref="skeletonContainer"/>.
        /// </summary>
        float diffY;
        /// <summary>
        /// Offset between anchor and the real neck position
        /// </summary>
        Vector3 neckOffset;

        bool isPlayerSet;
        Transform playerTransform;
        XROrigin xrOrigin;
        Transform mainCameraTransform;
        TrackedSubskeletonBone viewpointTSB;
        Tracker viewpointTracker;

        private void Awake()
        {
            Joint.enabled = debugJointAndSurface;
            Surface.enabled = debugJointAndSurface;

            LeftWatch.SetActive(false);
            RightWatch.SetActive(false);
            linker.SetSetUpSkeleton(this);

            handTrackingTrackers.AddRange(handTrackingIKTrackers);
            handTrackingTrackers.AddRange(handTrackingFingerTrackers);

            Linker
                .Get<UMI3DVRPlayer>(nameof(UMI3DVRPlayer))
                .linked += (player, isSet) =>
                {
                    isPlayerSet = isSet;
                    if (isSet)
                    {
                        playerTransform = player?.transform ?? null;
                        xrOrigin = player?.xrOrigin ?? null;
                        mainCameraTransform = player?.mainCamera.transform ?? null;
                        viewpointTSB = player?.mainCamera.GetComponent<TrackedSubskeletonBone>() ?? null;
                        viewpointTracker = player?.mainCamera.GetComponent<Tracker>() ?? null;
                        AdditionnalTrackers.Add(viewpointTracker);
                    }
                    else
                    {
                        if (AdditionnalTrackers.Contains(viewpointTracker))
                        {
                            AdditionnalTrackers.Remove(viewpointTracker);
                        }
                        playerTransform = null;
                        xrOrigin = null;
                        mainCameraTransform = null;
                        viewpointTSB = null;
                        viewpointTracker = null;
                    }
                };
        }

        private void Start()
        {
            trackedSkeleton = GetComponentInChildren<TrackedSubskeleton>();

            SpineFlexer.Enabled += () =>
            {
                shouldNotRotateHips = true;
            };

            SpineFlexer.Disabled += () =>
            {
                shouldNotRotateHips = false;
            };

            if (XRInputModalityManager != null && XRInputModalityManager.currentInputMode.Value == XRInputModalityManager.InputMode.TrackedHand)
                SwitchTrackerToHandTracking();
            else
                SwitchTrackerToController();
                //XRInputModalityManager.rightHand.SetActive(false);
        }

        public void SwitchTrackerToController()
        {
            handTrackingIKTrackers.ForEach(x => {
                if (controllerIKTrackers.Contains(x))
                    return;
                trackedSkeleton.RemoveController(x.Controller.boneType);
            });

            handTrackingFingerTrackers.ForEach(x => {
                trackedSkeleton.RemoveController(x.Controller.boneType);
            });
            
            controllerIKTrackers.ForEach(x => trackedSkeleton.ReplaceController(x.Controller));
        }

        public void SwitchTrackerToHandTracking()
        {
            controllerIKTrackers.ForEach(x => {
                if (handTrackingIKTrackers.Contains(x))
                    return;
                trackedSkeleton.RemoveController(x.Controller.boneType);
            });

            handTrackingTrackers.ForEach(x => trackedSkeleton.ReplaceController(x.Controller));
        }

        bool shouldNotRotateHips = false;

        public void SetUp()
        {
            StartCoroutine(SetupSkeleton());
        }
        
        public IEnumerator SetupSkeleton()
        {
            while (!isPlayerSet)
            {
                yield return null;
            }

            // Wait for camera to be positioned
            while (!TryResizeSkeleton())
            {
                yield return null;
            }

            LeftWatch.SetActive(true);
            RightWatch.SetActive(true);

            FootTargetBehavior.SetFootTargets();

            AdditionnalTrackers.ForEach(x => trackedSkeleton.ReplaceController(x));
            trackedSkeleton.bones.Add(BoneType.Viewpoint, viewpointTSB);

            isSetup = true;
            SetupDone?.Invoke();
        }

        /// <summary>
        /// Check user's height to change avatar size.
        /// </summary>
        public bool TryResizeSkeleton()
        {
            if (!isPlayerSet)
            {
                return false;
            }

            if (mainCameraTransform.localPosition.y == 0)
                return false;

            var inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            float distanceToGround = inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position) ? position.y : 0f;

            float skeletonHeight = mainCameraTransform.localPosition.y;
            sessionScaleFactor = 1.05f * skeletonHeight * Vector3.one;

            skeletonContainer.localScale = sessionScaleFactor;

            neckOffset = new Vector3(0, -0.060f * mainCameraTransform.localPosition.y, -0.07f);
            startingVirtualNeckPosition = mainCameraTransform.TransformPoint(neckOffset);
            diffY = startingVirtualNeckPosition.y - skeletonContainer.position.y;

            SkeletonResized?.Invoke();

            return true;
        }

        /// <summary>
        /// Sets the position and rotation of the avatar according to users movements.
        /// </summary>
        private void LateUpdate()
        {
            if (isSetup)
            {
                float diffAngle = Vector3.Angle(Vector3.ProjectOnPlane(mainCameraTransform.forward, Vector3.up), this.transform.forward);

                float rotX = mainCameraTransform.localRotation.eulerAngles.x > 180 ? mainCameraTransform.localRotation.eulerAngles.x - 360 : mainCameraTransform.localRotation.eulerAngles.x;

                Neck.localRotation = Quaternion.Euler(Mathf.Clamp(rotX, -60, 60), 0, 0);

                if (!shouldNotRotateHips)
                {
                    Vector3 virtualNeckPosition = mainCameraTransform.TransformPoint(neckOffset);
                    skeletonContainer.position = new Vector3(virtualNeckPosition.x, virtualNeckPosition.y - diffY, virtualNeckPosition.z);

                    Vector3 anchorForwardProjected = Vector3.Cross(mainCameraTransform.right, Vector3.up).normalized;
                    skeletonContainer.rotation = Quaternion.LookRotation(anchorForwardProjected, Vector3.up); // là
                }
            }
        }
    }
}