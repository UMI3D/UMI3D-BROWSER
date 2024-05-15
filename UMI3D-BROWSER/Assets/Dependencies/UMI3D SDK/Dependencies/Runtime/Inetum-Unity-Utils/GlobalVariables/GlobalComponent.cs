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
using UnityEngine;

namespace inetum.unityUtils
{
    public sealed class GlobalComponent : MonoBehaviour
    {
        umi3d.debug.UMI3DLogger logger = new();

        [Tooltip("Sub key. If not null or empty the key will be [subKey]/[Type of the variable]")]
        [SerializeField] string subKey;
        [SerializeField] bool addRemoveWhenActivationChange;
        [SerializeField] MonoBehaviour[] variables;

        public string SubKey => subKey;

        private void Awake()
        {
            logger.MainContext = this;
            logger.MainTag = nameof(GlobalComponent);
            AddAll();
        }

        private void OnEnable()
        {
            if (addRemoveWhenActivationChange)
            {
                AddAll();
            }
        }

        private void OnDisable()
        {
            if (addRemoveWhenActivationChange)
            {
                RemoveAll();
            }
        }

        private void OnDestroy()
        {
            RemoveAll();
        }

        string GetKey<T>(T variable)
        {
            string key = "";
            if (!string.IsNullOrEmpty(subKey))
            {
                key += $"{subKey}/";
            }
            key += variable.GetType().FullName;
            return key;
        }

        void AddAll()
        {
            for (int i = 0; i < (variables?.Length ?? 0); i++)
            {
                MonoBehaviour variable = variables[i];
                if (variable == null)
                {
                    logger.Error(nameof(RemoveAll), $"Variable {i} is null");
                    continue;
                }
                Global.Add(GetKey(variable), variable);
            }
        }

        void RemoveAll()
        {
            for (int i = 0; i < (variables?.Length ?? 0); i++)
            {
                MonoBehaviour variable = variables[i];
                if (variable == null)
                {
                    logger.Error(nameof(RemoveAll), $"Variable {i} is null");
                    continue;
                }
                Global.Remove(GetKey(variable));
            }
        }
    }
}