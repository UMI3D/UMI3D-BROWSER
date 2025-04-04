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
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public interface IControllerDataDelegate 
    {
        /// <summary>
        /// How many tools can be projected at the same time on this controller.
        /// </summary>
        int ToolCountLimitation { get; }

        /// <summary>
        /// Whether a tool can be projected on this controller if it has been selected by a selector.
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        bool CanProjectToolWhenSelected(Tool tool, Selector selector);
    }
}