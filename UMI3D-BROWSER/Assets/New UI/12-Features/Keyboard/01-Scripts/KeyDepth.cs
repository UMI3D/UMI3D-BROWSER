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

using inetum.unityUtils;
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.NotificationKeys;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.keyboard
{
    public class KeyDepth : MonoBehaviour
    {
        [Tooltip("Speed of the key movement.")]
        [SerializeField] float speed = 10f;
        [Tooltip("Depth of the key when pressed.")]
        [SerializeField] float depth = 10f;
        [Tooltip("Limit of the difference between two floats.")]
        [SerializeField] float epsilon = .001f;

        Key key;
        PointerDownBehaviour pointerDown;
        Button button;
        Coroutine coroutine;

        Vector3 upPosition;
        Vector3 downPosition;
        RectTransform rectTransform;

        bool withAnimation;

        void Awake()
        {
            key = GetComponent<Key>();
            pointerDown = GetComponent<PointerDownBehaviour>();
            button = GetComponent<Button>();

            rectTransform = transform.GetChild(0).GetComponent<RectTransform>();
            upPosition = rectTransform.anchoredPosition3D;
            downPosition = upPosition + new Vector3(0, 0, depth);

            NotificationHub.Default.Subscribe(
                this,
                KeyboardNotificationKeys.AnimationSettings,
                EnableOrDisableAnimation
            );
        }

        void OnEnable()
        {
            pointerDown.pointerClickedSimple += PointerDown;
        }

        void OnDisable()
        {
            pointerDown.pointerClickedSimple -= PointerDown;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            rectTransform.anchoredPosition3D = upPosition;
        }

        void PointerDown()
        {
            if (!withAnimation)
            {
                return;
            }

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(MoveKey());
        }

        IEnumerator MoveKey()
        {
            while (true)
            {
                if (key.buttonPressed && button.interactable && rectTransform.anchoredPosition3D.z - downPosition.z < -epsilon)
                {
                    rectTransform.anchoredPosition3D = Vector3.Lerp(rectTransform.anchoredPosition3D, downPosition, Time.deltaTime * speed);
                }
                else if ((!key.buttonPressed || !button.interactable) && rectTransform.anchoredPosition3D.z - upPosition.z > epsilon)
                {
                    rectTransform.anchoredPosition3D = Vector3.Lerp(rectTransform.anchoredPosition3D, upPosition, Time.deltaTime * speed);
                }
                else if (rectTransform.anchoredPosition3D.z - upPosition.z < epsilon)
                {
                    StopCoroutine(coroutine);
                    coroutine = null;
                }

                yield return null;
            }
        }

        void EnableOrDisableAnimation(Notification notification)
        {
            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.AnimationType, out KeyboardAnimationType animationType))
            {
                return;
            }

            if (animationType != KeyboardAnimationType.KeyPress)
            {
                return;
            }

            if (!notification.TryGetInfoT(KeyboardNotificationKeys.Info.WithAnimation, out bool withAnimation))
            {
                return;
            }

            this.withAnimation = withAnimation;
        }
    }
}