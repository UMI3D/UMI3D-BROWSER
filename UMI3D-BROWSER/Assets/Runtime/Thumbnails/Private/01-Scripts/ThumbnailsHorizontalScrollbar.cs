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
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.thumbnails
{
    [RequireComponent(typeof(Scrollbar))]
    internal class ThumbnailsHorizontalScrollbar : MonoBehaviour
    {
        Scrollbar scrollbar;

        ThumbnailsModelContainer model;

        void Awake()
        {
            scrollbar = GetComponent<Scrollbar>();
            scrollbar.onValueChanged.AddListener(SliderValueChanged);

            model = GetComponentInParent<ThumbnailsModelContainer>();

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.SliderValueSet>(
                this,
                SliderValueSet
            );

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.Added>(
                this,
                ThumbnailAdded
            );

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.Deleted>(
                this,
                ThumbnailDeleted
            );
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void SliderValueChanged(float value)
        {
            model.model.horizontalSliderValue = value;
        }

        void SliderValueSet(Notification notification)
        {
            if (!notification.TryGetInfoT(ThumbnailsNotificationKeys.SliderValueSet.Value, out float value))
            {
                return;
            }

            scrollbar.value = value;
        }

        void ThumbnailAdded(Notification notification)
        {
            new Task(async () =>
            {
                await Task.Yield();

                model.model.ResetSlider();
            }).Start(TaskScheduler.FromCurrentSynchronizationContext());
        }

        void ThumbnailDeleted(Notification notification)
        {
            model.model.ComputeScrolling();
        }
    }
}