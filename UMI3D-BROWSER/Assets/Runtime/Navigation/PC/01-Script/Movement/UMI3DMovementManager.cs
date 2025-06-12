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
using umi3d.browserRuntime.cursor;
using umi3d.baseBrowser.Navigation;
using UnityEngine;

public sealed class UMI3DMovementManager
{
    #region Dependencies

    public Transform playerTransform;
    public Transform skeleton;
    public BaseFPSData data;

    #endregion

    public event Action<Vector3> playerWillMoveDelegate;
    public event Action<Vector3> playerMovedDelegate;

    public void ComputeMovement()
    {
        data.playerTranslationSpeed = Vector3.zero;
        data.playerTranslation = Vector3.zero;

        UpdatePlayerPosition();

        Func<bool> isNearDestination 
            = () => Vector3.Distance(
                playerTransform.position,
                data.continuousDestination.Value
            ) < .5f;
        if (data.continuousDestination.HasValue && isNearDestination())
        {
            data.continuousDestination = null;
        }
    }

    /// <summary>
    /// Update the skeleton position to reflect the crouching or standing up position.
    /// </summary>
    void UpdateSkeletonHeight()
    {
        skeleton.localPosition = new Vector3
        (
            0,
            Mathf.Lerp
            (
                skeleton.localPosition.y,
                (data.IsCrouching) ? data.crouchYAxis : 0f,
                data.crouchSpeed == 0 ? 1000000 : Time.deltaTime / data.crouchSpeed
            ),
            0
        );
    }


    void UpdatePlayerPosition()
    {
        playerWillMoveDelegate?.Invoke(data.playerTranslation);
        playerTransform.position += data.playerTranslation;
        UpdateSkeletonHeight();
        playerMovedDelegate?.Invoke(data.playerTranslation);
    }
}
