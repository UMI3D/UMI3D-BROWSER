/*
Copyright 2019 - 2024 Inetum

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace umi3d.browserEditor.hierarchyTool
{
    public static class SampleCreateUtility
    {
        [MenuItem("GameObject/UMI3D_UI/Gameobject")]
        public static void CreateRandomGameobject(MenuCommand menuCommand)
        {
            CreateItemInHierarchyUtility.CreateObject("Sample object", typeof(AudioSource));
        }

        [MenuItem("GameObject/UMI3D_UI/Prefab")]
        public static void CreateRandomPrefab(MenuCommand menuCommand)
        {
            CreateItemInHierarchyUtility.CreatePrefab("Prefabs/XR Origin");
        }
    }
}
