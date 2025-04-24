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

using inetum.unityUtils.observation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace umi3d.cdk.interaction
{
    public sealed class ControllerManager 
    {
        #region Initialize

        static Lazy<ControllerManager> _default = new(() => new());
        public static ControllerManager @default => _default.Value;

        ControllerManager()
        {

        }

        #endregion

        List<Controller> _controllers = new();
        public ReadOnlyCollection<Controller> controllers => _controllers.AsReadOnly();

        public bool TryToInstantiateController(out Controller controller, string id)
        {
            controller = _controllers.Find(x => x.id == id);
            if (controller != null)
            {
                UnityEngine.Debug.LogWarning($"[ControllerManager] Warning: Cannot instantiate controller for '{id}' because this controller already exist.");
                return false;
            }

            controller = new(id);
            _controllers.Add(controller);
            return true;
        }

        public bool TryToGetController(out Controller controller, string id)
        {
            controller = _controllers.Find(x => x.id == id);
            if (controller == null)
            {
                UnityEngine.Debug.LogWarning($"[ControllerManager] Warning: controller for '{id}' has not been instantiated yet.");
                return false;
            }

            return true;
        }
    }
}