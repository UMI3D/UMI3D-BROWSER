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
    public class HandFilter : MonoBehaviour, IXRHandProcessor
    {
        [SerializeField]
        private Handedness handedness;

        public enum FilterType
        {
            AVERAGE_MOVING,
            ONE_EURO
        }

        [Header("Filtering technique")]
        [SerializeField]
        private FilterType filterType;

        private AverageMovingFilterPosition m_HandPositionFilter;
        private AverageMovingFilterRotation m_HandRotationFilter;

        /// <inheritdoc />
        public int callbackOrder => 0;

        private XRHandSubsystem m_HandSubsystem;
        private static readonly List<XRHandSubsystem> s_AllHandSubsystems = new List<XRHandSubsystem>();

        private bool m_WasTrackedLastFrame;

        /// <inheritdoc />
        private void OnDisable()
        {
            if (m_HandSubsystem != null)
            {
                m_HandSubsystem.UnregisterProcessor(this);
                m_HandSubsystem = null;
            }
        }

        private void Start()
        {
            if (filterType is FilterType.AVERAGE_MOVING)
            {
                m_HandPositionFilter = new AverageMovingFilterPosition();
                m_HandRotationFilter = new AverageMovingFilterRotation();

            }
            else if (filterType is FilterType.ONE_EURO)
            {
                //m_HandPositionFilter = new OneEuroFilterPosition();
                //m_HandRotationFilter = new OneEuroFilterRotation();
            }
        }

        /// <inheritdoc />
        private void Update()
        {
            if (m_HandSubsystem != null && m_HandSubsystem.running)
                return;

            SubsystemManager.GetSubsystems(s_AllHandSubsystems);
            bool foundRunningHandSubsystem = false;
            foreach (XRHandSubsystem handSubsystem in s_AllHandSubsystems)
            {
                if (handSubsystem.running)
                {
                    m_HandSubsystem?.UnregisterProcessor(this); // unregister old susbsystem
                    m_HandSubsystem = handSubsystem;
                    foundRunningHandSubsystem = true;
                    break;
                }
            }

            if (!foundRunningHandSubsystem)
                return;

            m_WasTrackedLastFrame = false;
            m_HandSubsystem.RegisterProcessor(this);
        }

        /// <inheritdoc />
        public void ProcessJoints(XRHandSubsystem subsystem, XRHandSubsystem.UpdateSuccessFlags successFlags, XRHandSubsystem.UpdateType updateType)
        {
            XRHand hand = handedness == Handedness.Left ? subsystem.leftHand : subsystem.rightHand;
            if (hand.isTracked)
            {
                Pose handPose = hand.rootPose;
                if (!m_WasTrackedLastFrame)
                {
                    m_HandPositionFilter.Initialize(handPose.position);
                    m_HandRotationFilter.Initialize((handPose.forward, handPose.up));
                }
                else
                {
                    Vector3 newPosition = m_HandPositionFilter.Filter(handPose.position, Time.deltaTime);
                    (Vector3 forward, Vector3 up) = m_HandRotationFilter.Filter((handPose.forward, handPose.up), Time.deltaTime);
                    Quaternion newLeftQuaternion = Quaternion.LookRotation(forward, up);
                    Pose newHandPose = new(newPosition, newLeftQuaternion);

                    hand.SetRootPose(newHandPose);
                    subsystem.SetCorrespondingHand(hand);
                }
            }

            m_WasTrackedLastFrame = hand.isTracked;
        }
    }
}