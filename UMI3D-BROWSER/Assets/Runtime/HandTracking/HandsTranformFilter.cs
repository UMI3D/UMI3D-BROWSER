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
using System.Collections.Generic;
using umi3d.runtimeBrowser.filter;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Processing;

namespace umi3d.runtimeBrowser.handTracking
{
    /// <summary>
    /// Filter Algorithm apply to a Hand from XR.Hands
    /// </summary>
    public class HandsTranformFilter : MonoBehaviour, IXRHandProcessor
    {
        [Header("Filter Algo")]
        [SerializeField] private FilterAlgoVector3 m_LeftHandPositionFilter;
        [SerializeField] private FilterAlgoVector3 m_RightHandPositionFilter;
        [Space(4)]
        [SerializeField] private FilterAlgoRotation m_LeftHandRotationFilter;
        [SerializeField] private FilterAlgoRotation m_RightHandRotationFilter;

        /// <inheritdoc />
        public int callbackOrder => 0;

        private XRHandSubsystem m_Subsystem;
        private static readonly List<XRHandSubsystem> s_SubsystemsReuse = new List<XRHandSubsystem>();

        private bool m_WasLeftHandTrackedLastFrame;
        private bool m_WasRightHandTrackedLastFrame;

        /// <inheritdoc />
        void OnDisable()
        {
            if (m_Subsystem != null)
            {
                m_Subsystem.UnregisterProcessor(this);
                m_Subsystem = null;
            }
        }

        /// <inheritdoc />
        void Update()
        {
            if (m_Subsystem != null && m_Subsystem.running)
                return;

            SubsystemManager.GetSubsystems(s_SubsystemsReuse);
            var foundRunningHandSubsystem = false;
            for (var i = 0; i < s_SubsystemsReuse.Count; ++i)
            {
                var handSubsystem = s_SubsystemsReuse[i];
                if (handSubsystem.running)
                {
                    m_Subsystem?.UnregisterProcessor(this);
                    m_Subsystem = handSubsystem;
                    foundRunningHandSubsystem = true;
                    break;
                }
            }

            if (!foundRunningHandSubsystem)
                return;

            m_WasLeftHandTrackedLastFrame = false;
            m_WasRightHandTrackedLastFrame = false;
            m_Subsystem.RegisterProcessor(this);
        }

        /// <inheritdoc />
        public void ProcessJoints(XRHandSubsystem subsystem, XRHandSubsystem.UpdateSuccessFlags successFlags, XRHandSubsystem.UpdateType updateType)
        {
            XRHand leftHand = subsystem.leftHand;
            if (leftHand.isTracked)
            {
                Pose leftHandPose = leftHand.rootPose;
                if (!m_WasLeftHandTrackedLastFrame)
                {
                    m_LeftHandPositionFilter.Initialize(leftHandPose.position);
                    m_LeftHandRotationFilter.Initialize((leftHandPose.forward, leftHandPose.up));
                }
                else
                {
                    Vector3 newLeftPosition = m_LeftHandPositionFilter.Filter(leftHandPose.position, Time.deltaTime);
                    (Vector3 leftForward, Vector3 leftUp) = m_LeftHandRotationFilter.Filter((leftHandPose.forward, leftHandPose.up), Time.deltaTime);
                    var newLeftQuaternion = Quaternion.LookRotation(leftForward, leftUp);
                    var newLeftPose = new Pose(newLeftPosition, newLeftQuaternion);

                    leftHand.SetRootPose(newLeftPose);
                    subsystem.SetCorrespondingHand(leftHand);
                }
            }

            m_WasLeftHandTrackedLastFrame = leftHand.isTracked;

            XRHand rightHand = subsystem.rightHand;
            if (rightHand.isTracked)
            {
                Pose rightHandPose = rightHand.rootPose;
                if (!m_WasRightHandTrackedLastFrame)
                {
                    m_RightHandPositionFilter.Initialize(rightHandPose.position);
                    m_RightHandRotationFilter.Initialize((rightHandPose.forward, rightHandPose.up));
                }
                else
                {
                    Vector3 newRightPosition = m_RightHandPositionFilter.Filter(rightHandPose.position, Time.deltaTime);
                    (Vector3 rightForward, Vector3 rightUp) = m_RightHandRotationFilter.Filter((rightHandPose.forward, rightHandPose.up), Time.deltaTime);
                    var newRightQuaternion = Quaternion.LookRotation(rightForward, rightUp);
                    var newRightPose = new Pose(newRightPosition, newRightQuaternion);

                    rightHand.SetRootPose(newRightPose);
                    subsystem.SetCorrespondingHand(rightHand);
                }
            }

            m_WasRightHandTrackedLastFrame = rightHand.isTracked;
        }
    }
}