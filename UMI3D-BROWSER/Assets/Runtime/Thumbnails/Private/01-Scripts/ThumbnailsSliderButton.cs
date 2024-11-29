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
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.thumbnails
{
    [RequireComponent(typeof(Button))]
    internal class ThumbnailsSliderButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        enum Direction
        {
            Left,
            Right
        }

        [SerializeField] Direction direction;
        [SerializeField] Color arrowDefaultColor;
        [SerializeField] Color arrowHoverColor;
        [SerializeField] Color arrowPressColor;

        Button button;
        Image arrow;

        ThumbnailsModelContainer model;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);

            arrow = GetComponentsInChildren<Image>()[2];

            model = GetComponentInParent<ThumbnailsModelContainer>();

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.SliderButtonVisibilityWillChange>(
                this,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == model.model),
                SliderButtonVisibilityChanged
            );

            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void OnClick()
        {
            switch (direction)
            {
                case Direction.Left:
                    model.model.SlideTowardLeft();
                    break;
                case Direction.Right:
                    model.model.SlideTowardRight();
                    break;
            }
        }

        void SliderButtonVisibilityChanged(Notification notification)
        {
            if (!notification.TryGetInfoT(ThumbnailsNotificationKeys.SliderButtonVisibilityWillChange.IsVisible, out bool isVisible))
            {
                return;
            }

            gameObject.SetActive(isVisible);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            arrow.color = arrowPressColor;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            arrow.color = arrowHoverColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            arrow.color = arrowHoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            arrow.color = arrowDefaultColor;
        }
    }
}