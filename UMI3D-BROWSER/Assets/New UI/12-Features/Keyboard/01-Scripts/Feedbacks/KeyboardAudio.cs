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

using inetum.unityUtils.observation;
using UnityEngine;

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

        void Awake()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        void OnEnable()
        {
            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.KeyHovered,
                (Callback)KeyHovered
            );

            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.KeyClicked,
                (Callback)KeyClicked
            );
        }

        void OnDisable()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void KeyHovered(Notification notification)
        {
            // Play the hover sound.
            audioSource.PlayOneShot(hoverSound);
        }

        void KeyClicked(Notification notification)
        {
            // Play the click sound.
            audioSource.PlayOneShot(clickSound);
        }
    }
}