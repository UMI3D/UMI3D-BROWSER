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
using umi3d.browserRuntime.cursor;
using umi3d.cdk.navigation;
using umi3d.browserRuntime.player;
using UnityEngine;

namespace umi3d.browserRuntime.pc
{
    public class UMI3DPCManager
    {
        #region Singleton

        static Lazy<UMI3DPCManager> _default = new(() => new());

        public static UMI3DPCManager @default => _default.Value;

        private UMI3DPCManager()
        {
        }

        #endregion

        public CursorModel cursorModel { get; set; }

        public UMI3DPlayer player { get; set; }
        public Transform personalSkeletonContainer { get; set; }
        public Transform personalSkeleton {  get; set; }
        public Transform viewpointPivot { get; set; }
        public Transform neckPivot { get; set; }
        public Transform head { get; set; }

        public UMI3DCamera umi3dCamera { get; set; }
        public Camera camera { get; set; }

        public cdk.navigation.UMI3DNavigation navigation { get; set; }
    }
}