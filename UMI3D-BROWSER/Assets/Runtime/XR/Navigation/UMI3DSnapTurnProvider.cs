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

using inetum.unityUtils.math;
using inetum.unityUtils.observation;
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace umi3d.browserRuntime.navigation
{
    public class UMI3DSnapTurnProvider : MonoBehaviour
    {
        [Header("Controller Actions")]
        [Tooltip("The reference to the action of snap turning the XR Origin with this controller.")]
        [SerializeField] InputActionReference leftHandSnapTurn;
        [SerializeField] XRRayInteractor leftIndicator;
        [Tooltip("The reference to the action of snap turning the XR Origin with this controller.")]
        [SerializeField] InputActionReference rightHandSnapTurn;
        [SerializeField] XRRayInteractor rightIndicator;

        [Header("Settings")]
        [Tooltip("The amount of degree that locomotion turn.")]
        [SerializeField] float turnAmount = 45;
        [Tooltip("The amount of time that locomotion waits before performing another action.")]
        [SerializeField] float debounceTime = .3f;
        [Tooltip("Whether the player can turn around by snap turning.")]
        [SerializeField] bool enableTurnAround = true;

        Coroutine coroutine;

        ActionMovementTiming actionMovementTiming;

        Dictionary<string, System.Object> info = new();

        [SerializeField] LayerMask uiLayerMask;

        void Awake()
        {
            actionMovementTiming = new();
            actionMovementTiming.DebounceTime = () => debounceTime;
        }

        void OnEnable()
        {
            leftHandSnapTurn.action.performed += Left_SnapTurn;
            rightHandSnapTurn.action.performed += Right_SnapTurn;
        }

        void OnDisable()
        {
            leftHandSnapTurn.action.performed -= Left_SnapTurn;
            rightHandSnapTurn.action.performed -= Right_SnapTurn;
        }

        void Left_SnapTurn(InputAction.CallbackContext context)
        {
            if (coroutine != null)
            {
                return;
            }

            coroutine = StartCoroutine(SnapTurnCoroutine(false));
        }
        void Right_SnapTurn(InputAction.CallbackContext context)
        {
            if (coroutine != null)
            {
                return;
            }

            coroutine = StartCoroutine(SnapTurnCoroutine(true));
        }

        IEnumerator SnapTurnCoroutine(bool isRight)
        {
            if (!IsRotating(out float angle))
            {
                coroutine = null;
                yield break;
            }

            var direction = RotationDirection(angle);

            if (rightIndicator.IsOverUIGameObject() || leftIndicator.IsOverUIGameObject())
            {
                var ob = isRight ? rightIndicator.rayEndTransform.gameObject : leftIndicator.rayEndTransform.gameObject;
                Dictionary<string, object> uiInfo = new Dictionary<string, object>();
                if (direction == 0)
                    uiInfo[ActionNotifictionKeys.UiJoystic.Vector] = Vector2.down;
                else if (direction == 1)
                    uiInfo[ActionNotifictionKeys.UiJoystic.Vector] = Vector2.left;
                else if (direction == 2)
                    uiInfo[ActionNotifictionKeys.UiJoystic.Vector] = Vector2.right;
                uiInfo[ActionNotifictionKeys.UiJoystic.Object] = ob;
                NotificationHub.Default.Notify(
                        this,
                        ID.FromType<ActionNotifictionKeys.UiJoystic>(),
                        uiInfo
                    );
                coroutine = null;
                yield break;
            }

            info[LocomotionNotificationKeys.Info.Direction] = direction;
            info[LocomotionNotificationKeys.Info.TurnAmount] = turnAmount;
            NotificationHub.Default.Notify(
                this,
                LocomotionNotificationKeys.SnapTurn,
                info
            );

            actionMovementTiming.ResetDebounceTime();

            while (!actionMovementTiming.CanPerformAction())
            {
                yield return null;
            }

            coroutine = null;
        }

        Vector2 ReadInput()
        {
            var leftHandValue = leftHandSnapTurn.action?.ReadValue<Vector2>() ?? Vector2.zero;
            var rightHandValue = rightHandSnapTurn.action?.ReadValue<Vector2>() ?? Vector2.zero;

            return leftHandValue + rightHandValue;
        }

        bool IsRotating(out float angle)
        {
            Vector2 axis = ReadInput();

            if (axis.x == 0 && axis.y == 0)
            {
                // No rotation. It should not happen.
                angle = float.NaN;
                return false;
            }

            axis = axis.normalized;
            angle = axis.Vector2Degree();
            // Subtract 90° so that 0 is below (0, -1).
            angle -= 90f;
            RotationUtils.ZeroTo360(ref angle);

            return true;
        }

        int RotationDirection(float angle)
        {
            if (angle < 45f || 315f < angle)
            {
                // Turn around.
                return 0;
            }
            else if (angle < 180)
            {
                // Turn left.
                return 1;
            }
            else
            {
                // Turn right.
                return 2;
            }
        }
    }
}