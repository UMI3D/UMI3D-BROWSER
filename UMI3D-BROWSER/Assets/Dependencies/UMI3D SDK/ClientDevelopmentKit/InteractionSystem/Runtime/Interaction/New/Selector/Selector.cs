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
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public class Selector 
    {
        internal Selector(string id) 
        {
            this.id = id;
        }

        /// <summary>
        /// The unique identifier of this selector.<br/>
        /// <br/>
        /// You can set the name of the selector here but it has to be unique.
        /// </summary>
        public readonly string id;

        List<Controller> _controllers = new List<Controller>();
        ReadOnlyCollection<Controller> controllers => _controllers.AsReadOnly();
        public void Add(Controller controller)
        {
            if (controller  == null || _controllers.Contains(controller))
            {
                return;
            }

            _controllers.Add(controller);
        }
        public void Remove(Controller controller)
        {
            _controllers.Remove(controller);
        }

        public void Select(Tool tool)
        {
            foreach (Controller controller in _controllers)
            {
                //if (controller.@delegate.CanProjectTool(tool))
                //{
                //    controller.TryToProject(tool, selector);
                //}
            }

            SelectorManager.@default.delegates.ForEach(@delegate =>
            {
                @delegate.ToolSelected(tool, this);
                return Flow.Continue;
            });
        }
    }
}