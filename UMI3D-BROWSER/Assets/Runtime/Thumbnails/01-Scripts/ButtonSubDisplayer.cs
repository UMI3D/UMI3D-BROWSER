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
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class ButtonSubDisplayer : MonoBehaviour
    {
        
        [Space]
        [SerializeField] private Button button;

        [Header("Animation")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField, Range(0, 1f)] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);




        private void OnEnable()
        {
            button.enabled = true;

            if (_easeInOutCoroutine != null)
            {
                StopCoroutine(_easeInOutCoroutine);
                _easeInOutCoroutine = null;
            }

            _easeInOutCoroutine = StartCoroutine(EaseInOut(true));
        }

        public event Action OnClick;
        public event Action OnHover;
        public event Action OnDisabled;

        private float _animationValue;
        private Coroutine _easeInOutCoroutine;



        private IEnumerator EaseInOut(bool isEnabeling)
        {
            float startValue = _animationValue;
            float endValue = isEnabeling ? 1 : 0;

            float time = 0;
            if (animationDuration > 0)
            {
                while (time < animationDuration)
                {
                    time += Time.deltaTime;

                    float lerpFactor = slideEase.Evaluate(time / animationDuration);
                    float currentValue = _animationValue = Mathf.Lerp(startValue, endValue, lerpFactor);

                    canvasGroup.alpha = currentValue;

                    yield return null;
                }
            }

            if (!isEnabeling) OnDisabled?.Invoke();
        }

        public void Disable()
        {
            button.enabled = false;

            if (_easeInOutCoroutine != null)
            {
                StopCoroutine(_easeInOutCoroutine);
                _easeInOutCoroutine = null;
            }

            if (gameObject.activeSelf)
            {
                _easeInOutCoroutine = StartCoroutine(EaseInOut(false));
            }
        }
    }
}

