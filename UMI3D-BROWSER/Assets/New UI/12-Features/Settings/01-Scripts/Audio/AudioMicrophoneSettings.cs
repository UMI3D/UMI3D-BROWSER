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
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(SettingsDropdownControl))]
    internal class AudioMicrophoneSettings : MonoBehaviour
    {
        SettingsDropdownControl dropdownControl;

        AudioSettings audioSettings;

        List<string> microphones;

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
            microphones = MicrophoneListener.GetMicrophonesNames().ToList();

            if (string.IsNullOrEmpty(audioSettings.model.microphone) || !microphones.Contains(audioSettings.model.microphone))
            {
                dropdownControl.selectedIndex = 0;
            }
            else
            {
                dropdownControl.selectedIndex = microphones.IndexOf(audioSettings.model.microphone);
            }

            MicrophoneListener.Instance.SetCurrentMicrophoneName(microphones[dropdownControl.selectedIndex])
                .Start(TaskScheduler.FromCurrentSynchronizationContext());

            dropdownControl.optionsCount = microphones.Count;
            dropdownControl.SetOptions();
        }

        string ToUpper(string mic)
        {
            return Unity.VisualScripting.StringUtility.FirstCharacterToUpper(mic);
        }

        void ValueChanged(int index)
        {
            MicrophoneListener.Instance.SetCurrentMicrophoneName(microphones[index])
                .Start(TaskScheduler.FromCurrentSynchronizationContext());
            audioSettings.model.microphone = microphones[index];
        }
    }
}