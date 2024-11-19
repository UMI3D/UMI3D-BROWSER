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

using TMPro;
using umi3d.baseBrowser.inputs.interactions;
using UnityEngine;

namespace umi3d.browserRuntime.shortcuts
{
    [RequireComponent(typeof(TMP_InputField))]
    public class ShortcutIsEditingTextField : MonoBehaviour
    {
        private TMP_InputField inputField;

        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();

            inputField.onSelect.AddListener(OnSelect);
            inputField.onDeselect.AddListener(OnDeselect);
        }

        private void Destroy()
        {
            inputField.onSelect.RemoveListener(OnSelect);
            inputField.onDeselect.RemoveListener(OnDeselect);
        }

        private void OnSelect(string arg0)
        {
            KeyboardShortcut.IsEditingTextField = true;
            KeyboardEmote.IsEditingTextField = true;
        }

        private void OnDeselect(string arg0)
        {
            KeyboardShortcut.IsEditingTextField = false;
            KeyboardEmote.IsEditingTextField = false;
        }
    }
}