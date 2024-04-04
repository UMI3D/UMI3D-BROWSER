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
using System.Collections.Generic;
using umi3dVRBrowsersBase.connection;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace umi3d.runtimeBrowser.handTracking
{
    /// <summary>
    /// Resize Skeleton hand to be the same size as the HandTracking hand
    /// </summary>
    public class HandsResize : MonoBehaviour
    {
        [Serializable]
        private struct Data
        {
            public Transform WristTransform;
            public Transform MiddleTipTransform;
            public List<Transform> MiddleFingerTransforms;
        }

        [SerializeField] private SetUpSkeleton m_SetUpSkeleton;

        [Header("Left")]
        [SerializeField] private Data m_LeftHand;
        [SerializeField] private Data m_LeftSkeletonHand;

        [Header("Right")]
        [SerializeField] private Data m_RightHand;
        [SerializeField] private Data m_RightSkeletonHand;

        private XRHandSubsystem m_HandSubsystem;

        protected void Start()
        {
            var handSubsystems = new List<XRHandSubsystem>();
            SubsystemManager.GetSubsystems(handSubsystems);

            for (var i = 0; i < handSubsystems.Count; ++i)
            {
                var handSubsystem = handSubsystems[i];
                if (handSubsystem.running)
                {
                    m_HandSubsystem = handSubsystem;
                    break;
                }
            }

            if (m_HandSubsystem != null)
                m_HandSubsystem.trackingAcquired += HandsResize_TrackingAcquired;
        }

        protected void OnEnable()
        {
            m_SetUpSkeleton.SkeletonResized += HandsResize_SkeletonResized;
        }

        protected void OnDisable()
        {
            m_SetUpSkeleton.SkeletonResized -= HandsResize_SkeletonResized;
        }

        public void ResetSkeletonHandScale()
        {
            m_LeftSkeletonHand.WristTransform.localScale = Vector3.one;
            m_RightSkeletonHand.WristTransform.localScale = Vector3.one;
        }

        private void HandsResize_SkeletonResized()
        {
            if (m_HandSubsystem.leftHand.isTracked)
                ResizeHand(m_LeftHand, m_LeftSkeletonHand);
            if (m_HandSubsystem.rightHand.isTracked)
                ResizeHand(m_RightHand, m_RightSkeletonHand);
        }

        private void HandsResize_TrackingAcquired(XRHand hand)
        {
            if (hand.handedness == Handedness.Left)
                ResizeHand(m_LeftHand, m_LeftSkeletonHand);
            else
                ResizeHand(m_RightHand, m_RightSkeletonHand);
        }

        private static void ResizeHand(Data hand, Data skeletonHand)
        {
            foreach (var item in hand.MiddleFingerTransforms)
                item.localRotation = Quaternion.identity;

            skeletonHand.WristTransform.localScale = Vector3.one;
            foreach (var item in skeletonHand.MiddleFingerTransforms)
                item.localRotation = Quaternion.identity;

            var distanceHand = Vector3.Distance(hand.WristTransform.position, hand.MiddleTipTransform.position);
            var distanceSkeletonHand = Vector3.Distance(skeletonHand.WristTransform.position, skeletonHand.MiddleTipTransform.position);

            var scale = distanceHand / distanceSkeletonHand;
            hand.WristTransform.localScale = new Vector3(scale, scale, scale);
            skeletonHand.WristTransform.localScale = new Vector3(scale, scale, scale);
        }
    }
}