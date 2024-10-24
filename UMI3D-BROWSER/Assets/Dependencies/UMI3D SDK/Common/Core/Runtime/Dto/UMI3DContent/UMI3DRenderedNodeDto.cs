﻿/*
Copyright 2019 - 2021 Inetum

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

namespace umi3d.common
{
    /// <summary>
    /// DTO describing a node that has a rendered part in the scene.
    /// </summary>
    public class UMI3DRenderedNodeDto : UMI3DNodeDto
    {
        /// <summary>
        /// Should some material be overrided
        /// </summary>
        public List<MaterialOverrideDto> overridedMaterials { get; set; } = null;

        /// <summary>
        /// Should apply the overriders list
        /// </summary>
        public bool applyCustomMaterial { get; set; } = false;

        /// <summary>
        /// State if object will be opaque to light.
        /// </summary>
        public bool castShadow { get; set; }
        /// <summary>
        /// State if object display shadow of other object.
        /// </summary>
        public bool receiveShadow { get; set; }

        /// <summary>
        /// Specify the values of blendshapes in SkinMesh. On Model, it will be applied on each SkinnedMeshRenderer that has blendShapes, On SubModel, it will be applied on the current entity.
        /// </summary>
        public List<float> blendShapesValues { get; set; } = null;

        /// <summary>
        /// If true, the mesh will be used for navmesh generation on the browser.
        /// </summary>
        public bool isPartOfNavmesh { get; set; } = false;

        /// <summary>
        /// Indicate whether or not the user is allowed to navigate through this object.
        /// </summary>
        public bool isTraversable { get; set; } = true;

        /// <summary>
        /// Indicate whether or not the user is allowed to navigate through this object.
        /// </summary>
        public bool isBlockingInteraction { get; set; } = true;
    }

}