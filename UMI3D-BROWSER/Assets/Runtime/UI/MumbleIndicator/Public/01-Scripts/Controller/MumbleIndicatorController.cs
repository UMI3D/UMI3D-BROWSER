/*
Copyright 2019 - 2025 Inetum

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

using umi3d.cdk.collaboration;
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    public class MumbleIndicatorController : MonoBehaviour
    {
        MumbleIndicatorModel _model;

        public MumbleIndicatorModel Model => _model;

        private void Awake()
        {
            _model = new MumbleIndicatorModel();
        }

        private void Start()
        {
            MicrophoneListener.Instance.OnChanelUpdate += OnChanelUpdate;
            MicrophoneListener.Instance.OnConnectionClose += OnConnectionClose;
        }

        private void OnDestroy()
        {
            MicrophoneListener.Instance.OnChanelUpdate -= OnChanelUpdate;
            MicrophoneListener.Instance.OnConnectionClose -= OnConnectionClose;
        }

        private void OnChanelUpdate(string obj)
        {
            _model.SetIsConnected(true);
        }

        private void OnConnectionClose()
        {
            _model.SetIsConnected(false);
        }
    }
}