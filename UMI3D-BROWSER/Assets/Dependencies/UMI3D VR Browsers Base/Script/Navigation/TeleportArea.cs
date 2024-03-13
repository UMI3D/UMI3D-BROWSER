﻿/*
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

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace umi3dVRBrowsersBase.navigation
{
    /// <summary>
    /// Makes an object targetable by users to teleport on it.
    /// </summary>
    public class TeleportArea : MonoBehaviour
    {
        public TeleportationArea tpArea;

        private void Start()
        {
            tpArea = GetComponent<TeleportationArea>();
            if (tpArea == null )
            {
                tpArea = gameObject.AddComponent<TeleportationArea>();
            }

            //tpArea.colliders.Add(GetComponent<Collider>());
        }
    }
}

