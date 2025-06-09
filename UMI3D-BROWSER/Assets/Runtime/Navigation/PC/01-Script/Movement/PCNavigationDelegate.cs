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

using System;
using umi3d.baseBrowser.Navigation;
using umi3d.cdk;
using umi3d.cdk.navigation;
using umi3d.common;

using UnityEngine;

public class PCNavigationDelegate 
{
    #region Dependencies

    public Transform personalSkeletonContainer;
    public Transform cameraTransform;
    public BaseFPSData data;

    /// <summary>
    /// Is player active ?
    /// </summary>
    public bool isActive = false;

    #endregion

    public NavigationData GetNavigationData()
    {
        //var translation = data.playerTranslationSpeed * Time.deltaTime;
        //Func<float> yVelocity = () =>
        //{
        //    return collisionManager.IsCloseToGround 
        //    ? 0f 
        //    : translation.y / Time.deltaTime;
        //};

        //return new NavigationData()
        //{
        //    speed = new Vector3Dto()
        //    {
        //        X = translation.x / Time.deltaTime,
        //        Y = yVelocity(),
        //        Z = translation.z / Time.deltaTime
        //    },
        //    crouching = data.IsCrouching,
        //    jumping = data.IsJumping,
        //    grounded = !isActive ? true : collisionManager.IsGrounded,
        //};
        throw new System.NotImplementedException();
    }
}
