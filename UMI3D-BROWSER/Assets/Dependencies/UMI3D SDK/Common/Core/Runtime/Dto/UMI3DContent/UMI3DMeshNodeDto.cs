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

namespace umi3d.common
{
    /// <summary>
    /// DTO describing a mesh on the scene graph.
    /// </summary>
    [System.Serializable]
    public class UMI3DMeshNodeDto : UMI3DRenderedNodeDto
    {
        /// <summary>
        /// Model ressource.
        /// </summary>
        public ResourceDto mesh { get; set; } = new ResourceDto();

        /// <summary>
        /// Optional id generator for child objects. Can be used to animate sub objects without to split it in different assets. <br/>
        /// {{pid}} will be replaced by the objects parent id. <br/>
        /// {{name}} will be replaced by the sub-object's name (if there is any).
        /// </summary>
        public string idGenerator { get; set; } = null;

        /// <summary>
        /// If true, add subobjects with id on the loaded mesh to track children (move, rotate, scale, activated, overide material).<br/>
        /// If true, idGenerator must be not null and subobjects must have unique name.<br/>
        /// If false, it is not possible to move the subobjects of this mesh.
        /// </summary>
        public bool areSubobjectsTracked { get; set; }

        /// <summary>
        /// </summary>
        public bool updateSkinnedMeshRendererWhenOffscreen { get; set; }

        /// <summary>
        /// State if the subobject was generated in a rightHanded 
        /// </summary>
        public bool isRightHanded { get; set; } = true;
    }
}
