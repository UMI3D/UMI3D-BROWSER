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

using System.Collections.Generic;
using umi3d.common.core.target;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace umi3d.browserRuntime.target
{
    [CreateAssetMenu(fileName = "TargetSO", menuName = "UMI3D/browser/TargetSO")]
    public class TargetSO : ScriptableObject, ITargetDataDelegate
    {
        static TargetSO _default;
        public static TargetSO @default
        {
            get
            {
                if (_default == null)
                {
                    var op = Addressables.LoadAssetAsync<TargetSO>(typeof(TargetSO).Name);

                    _default = op.WaitForCompletion(); //Forces synchronous load so that we can return immediately
                }
                return _default;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            TargetManager.@default.dataDelegate = @default;
        }

        [SerializeField, HideInInspector] OperatingSystem operatingSystem;
        [SerializeField, HideInInspector] List<Controller> controllers;
        [SerializeField, HideInInspector] List<Plugin> plugins;
        [SerializeField, HideInInspector] List<Feature> features;

        public IReadOnlyList<Feature> GetActiveFeatures()
        {
            return features;
        }

        public IReadOnlyList<Plugin> GetActivePlugins()
        {
            return plugins;
        }

        public IReadOnlyList<Controller> GetAuthorizedControllers()
        {
            return controllers;
        }

        public OperatingSystem GetCurrentOperatingSystem()
        {
            return operatingSystem;
        }


    }
}