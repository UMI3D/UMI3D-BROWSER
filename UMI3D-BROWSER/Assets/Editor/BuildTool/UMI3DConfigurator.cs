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

using System.Reflection;
using umi3d.cdk.collaboration;
using umi3d.common;
using UnityEngine.Rendering.Universal;

namespace umi3d.browserEditor.BuildTool
{
    public class UMI3DConfigurator
    {
        UMI3DCollabLoadingParameters loadingParameters;
        //UniversalRenderPipelineAsset urpData

        public UMI3DConfigurator(UMI3DCollabLoadingParameters loadingParameters)
        {
            this.loadingParameters = loadingParameters;
            //this.urpData = urpData;
        }

        public void HandleTarget(E_Target target)
        {
            loadingParameters.supportedformats.Remove(UMI3DAssetFormat.unity_standalone_urp);
            loadingParameters.supportedformats.Remove(UMI3DAssetFormat.unity_android_urp);
            switch (target)
            {
                case E_Target.Quest:
                case E_Target.Focus:
                case E_Target.Pico:
                    loadingParameters.supportedformats.Add(UMI3DAssetFormat.unity_android_urp);
                    loadingParameters.HasHeadMountedDisplay = true;
                    loadingParameters.CollaborationUserCaptureActivated = true;
                    //urpData.GetRenderer(1);
                    break;

                case E_Target.SteamVR:
                    loadingParameters.supportedformats.Add(UMI3DAssetFormat.unity_standalone_urp);
                    loadingParameters.HasHeadMountedDisplay = true;
                    loadingParameters.CollaborationUserCaptureActivated = true;
                    break;

                case E_Target.Windows:
                    loadingParameters.supportedformats.Add(UMI3DAssetFormat.unity_standalone_urp);
                    loadingParameters.HasHeadMountedDisplay = false;
                    loadingParameters.CollaborationUserCaptureActivated = true;
                    break;
            }
        }

        public static void SetDefaultPipelineRendererData(UniversalRenderPipelineAsset urpAsset, int newRendererIndex)
        {
            FieldInfo field = typeof(UniversalRenderPipelineAsset).GetField(
                "m_DefaultRendererIndex", 
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            field.SetValue(urpAsset, newRendererIndex);
        }
    }
}

