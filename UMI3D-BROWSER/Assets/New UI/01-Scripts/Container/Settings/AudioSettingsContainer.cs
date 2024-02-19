/*
Copyright 2019 - 2022 Inetum

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

using System;
using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.displayer;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dBrowsers.container
{
    public class AudioSettingsContainer : MonoBehaviour
    {
        [Header("AudioSettings params")]
        [SerializeField] private float environmentVolume;
        [SerializeField] private float conversationVolume;
        [SerializeField] private bool isMicOn;
        public event Action<bool> OnIsMicOnChanged;
        public event Action<float> OnEnvironmentVolumeChanged;
        public event Action<float> OnConversationVolumenChanged;

        [Header("Components")]
        [SerializeField] private Slider environmentSlider;
        [SerializeField] private Slider conversationSlider;
        [SerializeField] private RadioButtonGroup radioButtonGroup;

        private void Awake()
        {
            radioButtonGroup.OnSelectedButtonChanged += (sender, value) =>
            {
                if (value == 0)
                {
                    isMicOn = false;
                }
                else if (value == 1)
                {
                    isMicOn = true;
                }
                OnIsMicOnChanged?.Invoke(isMicOn);
            };

            environmentSlider.onValueChanged.AddListener((value) =>
            { 
                environmentVolume = value; 
                OnEnvironmentVolumeChanged?.Invoke(environmentVolume);
            });
            conversationSlider.onValueChanged.AddListener((value) =>
            { 
                conversationVolume = value; 
                OnConversationVolumenChanged?.Invoke(conversationVolume);
            });
        }
    }
}

