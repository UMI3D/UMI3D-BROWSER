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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public sealed class Controller 
    {
        internal Controller(string id)
        {
            this.id = id;
        }

        IControllerDataDelegate _delegate;
        public IControllerDataDelegate @delegate
        {
            get => _delegate;
            set
            {
                if (value == null)
                {
                    UnityEngine.Debug.LogError($"[Controller] Error: you are trying to set a null delegate.");
                    return;
                }
                _delegate = value;
            }
        }

        /// <summary>
        /// The unique identifier of this controller.<br/>
        /// <br/>
        /// You can set the name of the controller here but it has to be unique.
        /// </summary>
        public readonly string id;

        /// <summary>
        /// Whether a tool can be projected on this controller.
        /// </summary>
        public bool isActive { get; private set; } = true;
        /// <summary>
        /// Set the active status.<br/>
        /// <br/>
        /// If true then a tool can be projected on this controller.
        /// </summary>
        /// <param name="active"></param>
        public void SetActive(bool active)
        {
            this.isActive = active;
            ControllerManager.@default.delegates.ForEach(@delegate =>
            {
                @delegate.OnChangeOfIsActive(active, this);
                return inetum.unityUtils.observation.Flow.Continue;
            });
        }

        List<Input> _inputs = new List<Input>();
        public ReadOnlyCollection<Input> inputs => _inputs.AsReadOnly();
        public bool Add(Input input)
        {
            if (input.controller != null) { return false; }

            input.Associate(this);
            _inputs.Add(input);
            return true;
        }
        public bool Remove(Input input)
        {
            if (input.controller != this) { return false; }

            input.DissociateFromController();
            _inputs.Remove(input);
            return true;
        }
    }
}