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
using inetum.unityUtils;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(SettingsDropdownControl))]
    internal class AudioMicrophoneSettings : MonoBehaviour
    {
        SettingsDropdownControl dropdownControl;

        AudioSettings audioSettings;

        List<string> microphones;
        bool NoMicrophoneFound = false;

        void Awake()
        {
            dropdownControl = GetComponent<SettingsDropdownControl>();

            audioSettings = GetComponentInParent<AudioSettings>();

            dropdownControl.indexToItem = index => ToUpper(microphones[index]);
            dropdownControl.valueChanged += ValueChanged;
        }

        void Update()
        {
            RefreshMicOptions();
        }

        void RefreshMicOptions()
        {
            if (!MicrophoneListener.Exists)
            {
                if (this.NoMicrophoneFound)
                    return;

                this.NoMicrophoneFound = true;
                microphones = new List<string> { "No Microphone Listener" };

                dropdownControl.optionsCount = microphones.Count;
                dropdownControl.SetOptions();

                return;
            }

            var tmp = MicrophoneListener.GetMicrophonesNames();
            
            if (tmp.Length <= 0)
            {
                if (this.NoMicrophoneFound)
                    return;

                this.NoMicrophoneFound = true;
                microphones = new List<string> { "No Microphone Found" };

                dropdownControl.optionsCount = microphones.Count;
                dropdownControl.SetOptions();

                return;
            }

            if(!this.NoMicrophoneFound && microphones is not null && tmp.Length == microphones.Count && tmp.Zip(microphones,(a,b) => a == b).All(c => c))
                return;

            this.NoMicrophoneFound = false;

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
            if (!MicrophoneListener.Exists)
            {
                MicrophoneListener.Instance.SetCurrentMicrophoneName(microphones[index]);
                audioSettings.model.microphone = microphones[index];
            }
        }
    }
}