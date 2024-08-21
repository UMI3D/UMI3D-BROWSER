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
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyboardAudio : MonoBehaviour
    {
        [Tooltip("The sound a key make when hovered")]
        public AudioClip hoverSound;
        [Tooltip("The sound a key make when clicked")]
        public AudioClip clickSound;

        [Tooltip("The audio source responsible of the sound")]
        AudioSource audioSource;

        private void Awake()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.KeyHovered,
                null,
                KeyHovered
            );

            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.KeyClicked,
                null,
                KeyClicked
            );
        }

        void KeyHovered(Notification notification)
        {
            // Play the hover sound.
            audioSource.PlayOneShot(hoverSound);

            // If the pointer is an XR controller then send haptic.
            if (notification.TryGetInfoT("EventData", out BaseEventData data) 
                && data is TrackedDeviceEventData trackedDeviceEventData
                && trackedDeviceEventData.interactor is XRBaseControllerInteractor xrInteractor) 
            {
                xrInteractor.SendHapticImpulse(0.1f, 0.1f);
            }
        }

        void KeyClicked(Notification notification)
        {
            // Play the click sound.
            audioSource.PlayOneShot(clickSound);
        }
    }
}