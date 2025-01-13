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

using inetum.unityUtils.saveSystem;
using umi3d.cdk.collaboration;

namespace umi3d.browserRuntime.ui.settings
{
    internal class AudioSettingsPSM : PersistentScriptableModel
    {
        /// <summary>
        /// Volume of the environment.<br/>
        /// <br/>
        /// Value: [0, 100]
        /// </summary>
        public float environmentVolume = 100f;
        /// <summary>
        /// Volume of the conversations.<br/>
        /// <br/>
        /// Value: [0, 100]
        /// </summary>
        public float conversationVolume = 100f;
        /// <summary>
        /// Noise threshold.<br/>
        /// <br/>
        /// Value: [0, 100]
        /// </summary>
        public float noiseThreshold = 0f;
        /// <summary>
        /// The active microphone.
        /// </summary>
        public string microphone = null;
        /// <summary>
        /// The sending mode of the microphone.
        /// </summary>
        public MicrophoneMode mode = MicrophoneMode.AlwaysSend;
        /// <summary>
        /// Whether the noise reduction is enabled.
        /// </summary>
        public bool isNoiseReductionEnabled = true;
    }
}