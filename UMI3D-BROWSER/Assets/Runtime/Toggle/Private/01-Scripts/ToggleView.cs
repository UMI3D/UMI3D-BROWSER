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

using inetum.unityUtils.observation;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.toggle
{
    [RequireComponent(typeof(Slider))]
    public class ToggleView : MonoBehaviour, IPointerClickHandler
    {
        [Header("Animation")]
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private AnimationCurve _slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Elements")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _handleImage;

        [Header("Colors")]
        [SerializeField] private Color _backgroundColorOff = Color.white;
        [SerializeField] private Color _backgroundColorOn = Color.black;
        [Space]
        [SerializeField] private Color _handleColorOff = Color.black;
        [SerializeField] private Color _handleColorOn = Color.white;

        private ToggleModelContainer _modelContainer;

        private Slider _slider;
        private Coroutine _animationSliderCoroutine;

        private void Awake()
        {
            _modelContainer = GetComponentInParent<ToggleModelContainer>();

            SetUpSliderComponents();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ToggleNotificationKeys.ToggleSet>(),
                (Callback)ToggleSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.model));
        }

        private void OnEnable()
        {
            _slider.Select();
        }

        private void SetUpSliderComponents()
        {
            _slider = GetComponent<Slider>();

            if (_slider == null)
                Debug.Log("No Slider Found !", this);

            _slider.interactable = false;
            var sliderColors = _slider.colors;
            sliderColors.disabledColor = Color.white;
            _slider.colors = sliderColors;
            _slider.transition = Selectable.Transition.None;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _modelContainer.model.ToggleValue();
            Play_SliderGoTo(_modelContainer.model.value ? 1 : 0);
        }

        private void ToggleSet(Notification notification)
        {
            if (!notification.TryGetInfoT(ToggleNotificationKeys.ToggleSet.Value, out bool newValue))
                return;

            Play_SliderGoTo(newValue ? 1 : 0);
        }

        private void Play_SliderGoTo(float newSliderValue)
        {
            if (_animationSliderCoroutine != null)
                StopCoroutine(_animationSliderCoroutine);

            _animationSliderCoroutine = StartCoroutine(SliderGoTo(newSliderValue));
        }

        private IEnumerator SliderGoTo(float newSliderValue)
        {
            float startValue = _slider.value;
            float time = 0;

            while(time < _animationDuration)
            {
                float lerpFactor = _slideEase.Evaluate(time/_animationDuration);
                _slider.value = Mathf.Lerp(startValue, newSliderValue, lerpFactor);

                ChangeColors(_slider.value);

                time += Time.deltaTime;
                yield return null;
            }

            _slider.value = newSliderValue;
        }

        private void ChangeColors(float sliderValue)
        {
            if (_backgroundImage)
                _backgroundImage.color = Color.Lerp(_backgroundColorOff, _backgroundColorOn, sliderValue);

            if (_handleImage)
                _handleImage.color = Color.Lerp(_handleColorOff, _handleColorOn, sliderValue);
        }
    }
}

