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
using System.Collections.Generic;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    [RequireComponent(typeof(Key))]
    public class KeyDelete : MonoBehaviour
    {
        Key key;
        Coroutine coroutine;

        [Tooltip("Interval in second between each deletion.")]
        [SerializeField] float deletionInterval = .5f;
        [Tooltip("Duration in second of the phase 0.")]
        [SerializeField] float phase0Duration = 3f;

        void Awake()
        {
            key = GetComponent<Key>();

            key.PointerDown += PointerDown;
            key.PointerUp += PointerUp;
        }

        private void PointerDown()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(Delete());
        }

        private void PointerUp()
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        IEnumerator Delete()
        {
            float time = 0;
            int phase = 0;

            while (true)
            {
                if (time < phase0Duration)
                {
                    phase = 0;
                }
                else
                {
                    phase = 1;
                }

                NotificationHub.Default.Notify(
                    this,
                    KeyboardNotificationKeys.RemoveCharacters,
                    new()
                    {
                        { KeyboardNotificationKeys.CharactersInfo.DeletionPhase, phase }
                    }
                );

                yield return new WaitForSeconds(deletionInterval);
                time += deletionInterval;
            }
        }
    }
}