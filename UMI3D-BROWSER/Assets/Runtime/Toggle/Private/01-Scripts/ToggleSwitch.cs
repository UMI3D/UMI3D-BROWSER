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
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.toggle
{
    public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] bool _isOn;

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

        public bool CurrentValue { get; private set; }

        public Action onToggleOn;
        public Action onToggleOff;
        public Action onTransitionEffet;

        private float _currentSliderValue;
        private Slider _slider;
        private Coroutine _animationSliderCoroutine;

        private void Awake()
        {
            SetupToggleComponents();

            ChangeColors();
        }

        private void OnValidate()
        {
            SetupToggleComponents();

            _slider.value = _isOn ? 1 : 0;
            _currentSliderValue = _isOn ? 1 : 0;

            ChangeColors();
        }

        protected void OnEnable()
        {
            onTransitionEffet += ChangeColors;
        }

        protected void OnDisable()
        {
            onTransitionEffet -= ChangeColors;
        }

        private void SetupToggleComponents()
        {
            if (_slider != null)
                return;

            SetUpSliderComponents();
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

        private void ChangeColors()
        {
            if (_backgroundImage)
                _backgroundImage.color = Color.Lerp(_backgroundColorOff, _backgroundColorOn, _currentSliderValue);

            if (_handleImage)
                _handleImage.color = Color.Lerp(_handleColorOff, _handleColorOn, _currentSliderValue);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Toggle();
        }

        public void Toggle()
        {
            SetStateAndStartAnimation(!CurrentValue);
        }

        public void SetValue(bool isOn)
        {
            SetStateAndStartAnimation(isOn);
        }

        private void SetStateAndStartAnimation(bool state)
        {
            if (state == CurrentValue)
                return;

            CurrentValue = state;

            if (CurrentValue)
                onToggleOn?.Invoke();
            else
                onToggleOff?.Invoke();

            PlayAnimation();
        }

        private void PlayAnimation()
        {
            if (_animationSliderCoroutine != null)
                StopCoroutine(_animationSliderCoroutine);

            _animationSliderCoroutine = StartCoroutine(AnimateSlider());
        }

        private IEnumerator AnimateSlider()
        {
            float startValue = _slider.value;
            float endValue = CurrentValue ? 1 : 0;

            float time = 0;
            if (_animationDuration > 0)
            {
                while(time < _animationDuration)
                {
                    time += Time.deltaTime;

                    float lerpFactor = _slideEase.Evaluate(time/_animationDuration);
                    _slider.value = _currentSliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);

                    onTransitionEffet?.Invoke();

                    yield return null;
                }
            }

            _slider.value = endValue;
        }
    }
}

