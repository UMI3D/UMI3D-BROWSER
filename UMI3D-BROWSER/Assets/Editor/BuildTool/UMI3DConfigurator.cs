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
using umi3d.cdk;
using umi3d.cdk.collaboration;
using umi3d.common;
using UnityEngine;

namespace BuildTool
{
    public class UMI3DConfigurator : IBuilToolComponent
    {
        UMI3DCollabLoadingParameters loadingParameters;

        public UMI3DConfigurator(UMI3DCollabLoadingParameters loadingParameters)
        {
            this.loadingParameters = loadingParameters;
        }

        public void HandleTarget(E_Target target)
        {
            switch (target)
            {
                case E_Target.Quest:
                    loadingParameters.supportedformats.Remove(UMI3DAssetFormat.unity_standalone_urp);
                    loadingParameters.supportedformats.Remove(UMI3DAssetFormat.unity_android_urp);
                    loadingParameters.supportedformats.Add(UMI3DAssetFormat.unity_android_urp);
                    break;
                case E_Target.Vive:
                    loadingParameters.supportedformats.Remove(UMI3DAssetFormat.unity_standalone_urp);
                    loadingParameters.supportedformats.Remove(UMI3DAssetFormat.unity_android_urp);
                    loadingParameters.supportedformats.Add(UMI3DAssetFormat.unity_standalone_urp);
                    break;
                case E_Target.Focus:
                    loadingParameters.supportedformats.Remove(UMI3DAssetFormat.unity_standalone_urp);
                    loadingParameters.supportedformats.Remove(UMI3DAssetFormat.unity_android_urp);
                    loadingParameters.supportedformats.Add(UMI3DAssetFormat.unity_android_urp);
                    break;
                case E_Target.Pico:
                    loadingParameters.supportedformats.Remove(UMI3DAssetFormat.unity_standalone_urp);
                    loadingParameters.supportedformats.Remove(UMI3DAssetFormat.unity_android_urp);
                    loadingParameters.supportedformats.Add(UMI3DAssetFormat.unity_android_urp);
                    break;
            }
        }
    }
}

