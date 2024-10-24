﻿/*
Copyright 2019 - 2023 Inetum
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

using System.Collections.Generic;
using umi3d.cdk;
using UnityEngine;

namespace umi3dVRBrowsersBase.interactions.selection.cursor
{


    class NodeContainer : MonoBehaviour
    {
        /// <summary>
        /// List of all <see cref="umi3d.cdk.interaction.Interactable"/> containers.
        /// </summary>
        public static List<NodeContainer> containers = new List<NodeContainer>();

        public UMI3DNodeInstance instance;

        private void Awake()
        {
            if (!containers.Contains(this))
                containers.Add(this);
        }

        private void OnDestroy()
        {
            containers.Remove(this);
        }
    }
}