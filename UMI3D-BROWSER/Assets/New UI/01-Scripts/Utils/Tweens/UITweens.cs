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
using System.Collections.Generic;
using UnityEngine;

namespace utils.tweens
{
    public class UITweens : MonoBehaviour
    {
        [SerializeField] private AnimationCurve EaseInOutCurve = AnimationCurve.EaseInOut(0,0,1,1);
        public event Action OnTweenComplete;
        [SerializeField] private Transform tweenTarget;
        [SerializeField]
        private float time = 1;
        private Vector3 tweenOrigine;

        private void Awake()
        {
            tweenOrigine = transform.position;
        }

        public void TweenTo()
        {
            StopAllCoroutines();
            StartCoroutine(CoTweenTo());
        }

        public void Rewind()
        {
            StopAllCoroutines();
            StartCoroutine(CoRewind());
        }

        private IEnumerator CoTweenTo() 
        {
            float totalTime = 0;
            float eval = 0;

            while (totalTime < time)
            {
                totalTime += Time.deltaTime;
                eval = EaseInOutCurve.Evaluate(totalTime / time);
                transform.position = Vector3.Lerp(transform.position, tweenTarget.position, eval);

                yield return null;
            }

            OnTweenComplete?.Invoke();
        }

        private IEnumerator CoRewind()
        {
            float totalTime = 0;
            float eval = 0;

            while (totalTime < time)
            {
                totalTime += Time.deltaTime;
                eval = EaseInOutCurve.Evaluate(totalTime / time);
                transform.position = Vector3.Lerp(transform.position, tweenOrigine, eval);

                yield return null;
            }

            OnTweenComplete?.Invoke();
        }
    }
}

