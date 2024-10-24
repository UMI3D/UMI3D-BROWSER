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

namespace umi3d.browserRuntime.ui.keyboard
{
    [RequireComponent(typeof(Key))]
    public class KeyDelete : MonoBehaviour
    {
        Key key;
        PointerDownBehaviour pointerDown;
        Coroutine coroutine;

        [Tooltip("Interval in second between each deletion.")]
        [SerializeField] float deletionInterval = .5f;
        [Tooltip("Duration in second of the phase 0.")]
        [SerializeField] float phase0Duration = 3f;

        Dictionary<string, object> info = new()
        {
            { KeyboardNotificationKeys.Info.TextFieldTextUpdate, TextFieldTextUpdate.RemoveCharacters },
            { KeyboardNotificationKeys.Info.DeletionPhase, 0 }
        };

        void Awake()
        {
            key = GetComponent<Key>();
            pointerDown = GetComponent<PointerDownBehaviour>();

            pointerDown.pointerClickedSimple += PointerDown;
            key.PointerUp += PointerUp;
        }

        void PointerDown()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(Delete());
        }

        void PointerUp()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        IEnumerator Delete()
        {
            float time = 0;
            int phase = 0;

            while (true)
            {
                phase = time < phase0Duration ? 0 : 1;

                info[KeyboardNotificationKeys.Info.DeletionPhase] = phase;
                NotificationHub.Default.Notify(
                    this,
                    KeyboardNotificationKeys.AddOrRemoveCharacters,
                    info
                );

                yield return new WaitForSeconds(deletionInterval);
                time += deletionInterval;
            }
        }
    }
}