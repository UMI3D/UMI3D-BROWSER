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

namespace inetum.unityUtils.versioning
{
    public abstract class VersioningManager : IVersioningDataDelegate
    {
        #region Initialization

        // Copy past on concrete VersioningManager.
        //public static XVersioningManager @default => _default.Value;
        //static readonly Lazy<XVersioningManager> _default = new(() => new XVersioningManager());

        VersioningManager() { }

        #endregion

        public IVersioningDataDelegate dataDelegate;

        public Version GetVersion()
        {
            return dataDelegate.GetVersion();
        }

        public ReleaseCycle GetReleaseCycle()
        {
            return dataDelegate.GetReleaseCycle();
        }

        //public static bool IsMajor(int major)
        //{
        //    return 
        //}
    }
}