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

using System.Linq;
using System.Collections.Generic;
using umi3d.cdk.collaboration;
using umi3dBrowsers.displayer;
using Unity.VisualScripting;
using UnityEngine;
using inetum.unityUtils;

namespace umi3d.browserRuntime.ui.settings.audio
{
    public class MicrophoneDropdown : MonoBehaviour
    {
        [SerializeField] private GridDropDown microphoneDropdown;

        private string selectedMicrophone;

        private void Start()
        {
            var lstMicrophones = MicrophoneListener.GetMicrophonesNames();

            selectedMicrophone = PlayerPrefs.GetString(SettingsPlayerPrefsKeys.Microphone, null);
            if (selectedMicrophone == null || selectedMicrophone == string.Empty || !lstMicrophones.Contains(selectedMicrophone))
                if (lstMicrophones.Length > 0)
                    Select(lstMicrophones[0]);

            List<GridDropDownItemCell> microphones = new();

            var i = 0;
            var indexCurrent = 0;
            lstMicrophones.ForEach(microphone => {
                GridDropDownItemCell cell = new(microphone.FirstCharacterToUpper());
                microphones.Add(cell);
                if (microphone == selectedMicrophone)
                    indexCurrent = i;
                i++;
            });

            microphoneDropdown.Init(microphones, indexCurrent);

            microphoneDropdown.OnClick += () => {

                Select(microphoneDropdown.GetValue());
            };
        }

        private void Select(string microphone)
        {
            selectedMicrophone = microphone;
            MicrophoneListener.Instance.SetCurrentMicrophoneName(microphone);
            PlayerPrefs.SetString(SettingsPlayerPrefsKeys.Microphone, microphone);
        }
    }
}