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
using umi3d.cdk;
using umi3d.cdk.navigation;
using umi3d.common;
using UnityEngine;

namespace umi3d.browserRuntime.navigation
{
    public class CommonFrameController : FrameController
    {
        Transform personalSkeletonContainer;

        public CommonFrameController(Transform personalSkeletonContainer)
        {
            this.personalSkeletonContainer = personalSkeletonContainer;
        }

        public override void UpdateFrame(ulong environmentId, FrameRequestDto data)
        {
            if (data.FrameId == 0)
            {
                // bind the personalSkeletonContainer to the Scene.
                SetParentToScene();
                globalFrame.Delete -= SetParentToScene;
                globalFrame = null;
            }
            else
            {
                // bind the personalSkeletonContainer to the Frame.
                UMI3DNodeInstance Frame = UMI3DEnvironmentLoader.GetNode(environmentId, data.FrameId);
                if (Frame != null)
                {
                    globalFrame = Frame;
                    personalSkeletonContainer.SetParent(
                        Frame.transform,
                        true
                    );
                    globalFrame.Delete += SetParentToScene;
                }
            }
        }

        void SetParentToScene()
        {
            personalSkeletonContainer.SetParent(
                UMI3DLoadingHandler.Instance.transform,
                true
            );
        }
    }
}