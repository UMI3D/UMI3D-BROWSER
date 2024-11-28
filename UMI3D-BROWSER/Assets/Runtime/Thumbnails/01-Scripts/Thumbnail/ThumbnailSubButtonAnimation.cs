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
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.thumbnails
{
    [RequireComponent(typeof(CanvasGroup), typeof(Button))]
    internal class ThumbnailSubButtonAnimation : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Button button;

        [SerializeField, Range(0f, 1f)] float animationDuration = 0.5f;

        new inetum.unityUtils.Animation animation = new();
        Coroutine coroutine;

        ThumbnailModelContainer model;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            button = GetComponent<Button>();

            animation
               .SetApplyValue<float>(x => canvasGroup.alpha = x)
               .SetLerp<float>(Easings.Lerp);

            model = GetComponentInParent<ThumbnailModelContainer>();

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.SubInputsVisibilityWillChange>(
                this,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == model.model),
                SubButtonsVisibilityWillChange
            );
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void SubButtonsVisibilityWillChange(Notification notification)
        {
            if (!notification.TryGetInfoT(ThumbnailsNotificationKeys.SubInputsVisibilityWillChange.IsVisible, out bool isVisible))
            {
                return;
            }

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            coroutine = StartCoroutine(
                isVisible
                ? Displaying()
                : Hiding()
            );
        }

        IEnumerator Displaying()
        {
            yield return animation
                .SetInitAndFinalValue(0f, 1f)
                .SetAnimationTime(animationDuration)
                .SetEasing(Easings.EaseInQuad)
                .Start();

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            button.interactable = true;
        }

        IEnumerator Hiding()
        {
            yield return animation
                .SetInitAndFinalValue(1f, 0f)
                .SetAnimationTime(animationDuration)
                .SetEasing(Easings.EaseInQuad)
                .Start();

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            button.interactable = false;
        }
    }
}