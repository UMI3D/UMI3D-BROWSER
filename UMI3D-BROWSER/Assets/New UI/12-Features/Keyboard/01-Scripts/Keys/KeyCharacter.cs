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
    public class KeyCharacter : MonoBehaviour
    {
        Key key;
        TMPro.TMP_Text text;

        static Dictionary<string, object> info = new()
        {
            { KeyboardNotificationKeys.Info.IsAddingCharacters, true },
            { KeyboardNotificationKeys.Info.Characters, 'a' }
        };

        void Awake()
        {
            key = GetComponent<Key>();
            text = GetComponentInChildren<TMPro.TMP_Text>();

            key.PointerUp += PointerUp;
        }

        private void PointerUp()
        {
            info[KeyboardNotificationKeys.Info.Characters] = text.text;
            NotificationHub.Default.Notify(
                this,
                KeyboardNotificationKeys.AddOrRemoveCharacters,
                info
            );
        }
    }
}