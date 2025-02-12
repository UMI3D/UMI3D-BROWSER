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
using System.Diagnostics;
using System.Linq;
using umi3d.common.core.target;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

        [SerializeField, HideInInspector] Platform platform;

        [SerializeField, HideInInspector] Controller[] controllers;
        [SerializeField, HideInInspector] Controller[] currentControllers;

        [SerializeField, HideInInspector] Plugin[] plugins;
        [SerializeField, HideInInspector] Feature[] features;

        [SerializeField, HideInInspector] ImmersiveType[] immersiveTypes;
        [SerializeField, HideInInspector] ImmersiveType currentImmersiveType;

        public OperatingSystem GetOperatingSystem()
        {
            return operatingSystem;
        }

        public Platform GetPlatform()
        {
            return platform;
        }

        public IReadOnlyList<Controller> GetAuthorizedControllers()
        {
            return controllers;
        }
        public IReadOnlyList<Controller> GetCurrentControllers()
        {
            return currentControllers;
        }

        public IReadOnlyList<Plugin> GetActivePlugins()
        {
            return plugins;
        }
        public IReadOnlyList<Feature> GetActiveFeatures()
        {
            return features;
        }

        public IReadOnlyList<ImmersiveType> GetAuthorizedImmersiveTypes()
        {
            return immersiveTypes;
        }
        public ImmersiveType GetCurrentImmersiveType()
        {
            return currentImmersiveType;
        }

        public bool TrySetCurrentControllers(IEnumerable<Controller> controllers)
        {
            List<Controller> unauthorizedControllers = new();
            foreach (var controller in controllers)
            {
                if (!controllers.Contains(controller))
                {
                    unauthorizedControllers.Add(controller);
                }
            }

            if (unauthorizedControllers.Count > 0)
            {
                UnityEngine.Debug.LogError($"[TargetSO] Error: Try to set controllers that are not authorized: [{string.Join(',', unauthorizedControllers)}].");
                return false;
            }

            currentControllers = unauthorizedControllers.ToArray();
            return true;
        }

        public bool TrySetCurrentImmersiveType(ImmersiveType immersiveType)
        {
            if (!immersiveTypes.Contains(immersiveType))
            {
                UnityEngine.Debug.LogError($"[TargetSO] Error: Try to set the immersive type that is not authorized: [{immersiveType}].");
                return false;
            }

            currentImmersiveType = immersiveType;
            return true;
        }

        #region Set methods that can only be called in the editor.

        [Conditional("UNITY_EDITOR")]
        public void UpdateOperatingSystem(OperatingSystem operatingSystem)
        {
            this.operatingSystem = operatingSystem;
            Save();
        }

        [Conditional("UNITY_EDITOR")]
        public void UpdatePlatform(Platform platform)
        {
            this.platform = platform;
            Save();
        }

        [Conditional("UNITY_EDITOR")]
        public void UpdateControllers(Controller[] controllers)
        {
            this.controllers = controllers;
            Save();
        }

        [Conditional("UNITY_EDITOR")]
        public void UpdatePlugins(Plugin[] plugins)
        {
            this.plugins = plugins;
            Save();
        }

        [Conditional("UNITY_EDITOR")]
        public void UpdateFeatures(Feature[] features)
        {
            this.features = features;
            Save();
        }

        [Conditional("UNITY_EDITOR")]
        public void UpdateImmersiveTypes(ImmersiveType[] immersiveTypes)
        {
            this.immersiveTypes = immersiveTypes;
            Save();
        }

        [Conditional("UNITY_EDITOR")]
        void Save()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        #endregion
    }
}