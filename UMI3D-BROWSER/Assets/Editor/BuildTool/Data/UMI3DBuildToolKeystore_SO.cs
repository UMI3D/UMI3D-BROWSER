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

using inetum.unityUtils.saveSystem;
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    /// <summary>
    /// [Warning] do not push this data in a public Git repository !!!<br/>
    /// Create a [Build Tool Keystore] scriptable object in an EXCLUDED folder that is excluded from git.
    /// </summary>
    [CreateAssetMenu(fileName = "UMI3D Build Tool Keystore", menuName = "UMI3D/Build Tools/Build Tool Keystore")]
    public class UMI3DBuildToolKeystore_SO : SerializableScriptableObject
    {
        public string password;
        public string path;
    }
}