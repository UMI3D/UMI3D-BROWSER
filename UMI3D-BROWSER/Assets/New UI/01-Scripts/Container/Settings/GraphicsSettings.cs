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

using System;
using System.Collections;
using System.Collections.Generic;
using umi3dBrowsers.displayer;
using UnityEngine;

namespace umi3dBrowsers.container
{
    public class GraphicsSettings : MonoBehaviour
    {
        [SerializeField] private QualityLevel quality;
        [SerializeField] private RadioButtonGroup radioButtonGroup;

        public event Action<QualityLevel> OnQualityLevelChange;

        private void Awake()
        {
            radioButtonGroup.OnSelectedButtonChanged += (sender, id) =>
            {
                switch (id)
                {
                    case 0:
                        quality = QualityLevel.Fast;
                        break;
                    case 1:
                        quality = QualityLevel.Good;
                        break;
                    case 2:
                        quality = QualityLevel.Fantastic;
                        break;
                }

                OnQualityLevelChange(quality);
            };
        }
    }
}

