/*
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

using System.Collections.Generic;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3dVRBrowsersBase.interactions.input
{
    /// <summary>
    /// Input for UMI3D Manipulation.
    /// </summary>
    public class ManipulationInput : AbstractCursorBasedManipulationInput
    {
        [SerializeField]
        private List<DofGroupEnum> implementedDofs = new List<DofGroupEnum>();

        #region Methods

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="dofs"></param>
        /// <returns></returns>
        public override bool IsCompatibleWith(DofGroupEnum dofs)
        {
            return implementedDofs.Contains(dofs);
        }

        #endregion
    }
}