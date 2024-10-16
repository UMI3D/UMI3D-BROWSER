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

using System;
using umi3dVRBrowsersBase.interactions;

namespace umi3dVRBrowsersBase.ui
{
    /// <summary>
    /// Makes any entity triggerable/clickable by <see cref="selection.VRClickableElementSelector"/>.
    /// </summary>
    public interface ITriggerableElement : IClientElement
    {
        /// <summary>
        /// Event raised when <see cref="Trigger"/> is called.
        /// </summary>
        event Action triggerHandler;

        /// <summary>
        /// Raises <see cref="triggerHandler"/> when this element is triggered/clicked.
        /// </summary>
        /// <param name="controller">Controller used to click</param>
        void Trigger(ControllerType controller);
    }

}
