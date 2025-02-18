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
using umi3d.cdk.collaboration;
using UnityEngine;
using System.Collections.Generic;
using umi3d.common;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(SettingsDropdownControl))]
    internal class AudioMicrophoneSettings : MonoBehaviour
    {
        SettingsDropdownControl dropdownControl;

        AudioSettings audioSettings;

        List<string> microphones;

        bool noMicrophoneFound = false;

        void Awake()
        {
            dropdownControl = GetComponent<SettingsDropdownControl>();

            audioSettings = GetComponentInParent<AudioSettings>();

            dropdownControl.indexToItem = index => ToUpper(microphones[index]);
            dropdownControl.valueChanged += ValueChanged;
        }

        void OnEnable()
        {
            try
            {
                SetMicrophoneFromPreferences();
            }
            catch (System.Exception ex)
            {
                UMI3DLogger.LogException(ex, DebugScope.Collaboration);
            }
        }

        private void SetMicrophoneFromPreferences()
        {
            if (string.IsNullOrEmpty(audioSettings.model.microphone))
                return;

            RefreshMicOptions();

            string[] micNames = Microphone.devices;

            for (int i = 0; i < micNames.Length; i++)
            {
                string micName = micNames[i];

                if (micName == audioSettings.model.microphone)
                {
                    ValueChanged(i);
                    return;
                }
            }
        }

        void Update()
        {
            RefreshMicOptions();
        }

        void RefreshMicOptions()
        {
            if (!MicrophoneListener.Exists)
            {
                if (this.noMicrophoneFound)
                    return;

                this.noMicrophoneFound = true;
                microphones = new List<string> { "No Microphone Listener" };

                dropdownControl.optionsCount = microphones.Count;
                dropdownControl.SetOptions();

                return;
            }

            var tmp = MicrophoneListener.GetMicrophonesNames();

            if (tmp.Length <= 0)
            {
                UnityEngine.AudioSettings.Reset(UnityEngine.AudioSettings.GetConfiguration());

                if (this.noMicrophoneFound)
                    return;

                this.noMicrophoneFound = true;
                microphones = new List<string> { "No Microphone Found" };

                dropdownControl.optionsCount = microphones.Count;
                dropdownControl.SetOptions();

                return;
            }

            if (!this.noMicrophoneFound && microphones is not null && tmp.Length == microphones.Count && tmp.Zip(microphones, (a, b) => a == b).All(c => c))
                return;

            this.noMicrophoneFound = false;

            microphones = tmp.ToList();

            var current = audioSettings.model.microphone;
            if (string.IsNullOrEmpty(current) || !microphones.Contains(current))
                dropdownControl.selectedIndex = 0;
            else
                dropdownControl.selectedIndex = microphones.IndexOf(current);

            MicrophoneListener.Instance.SetCurrentMicrophoneName(microphones[dropdownControl.selectedIndex]);

            dropdownControl.optionsCount = microphones.Count;
            dropdownControl.SetOptions();
        }

        string ToUpper(string mic)
        {
            return Unity.VisualScripting.StringUtility.FirstCharacterToUpper(mic);
        }

        void ValueChanged(int index)
        {
            if (MicrophoneListener.Exists)
            {
                MicrophoneListener.Instance.SetCurrentMicrophoneName(microphones[index]);
                audioSettings.model.microphone = microphones[index];
            }
        }
    }
}