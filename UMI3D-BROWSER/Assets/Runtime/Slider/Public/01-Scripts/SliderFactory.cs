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

using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.ui.slider
{
    public class SliderFactory : MonoBehaviour
    {
        [SerializeField] SliderModelContainer _intPrefab;
        [SerializeField] SliderModelContainer _floatPrefab;

        Queue<SliderModelContainer> _lstIntSlidersAvailable = new();
        Queue<SliderModelContainer> _lstFloatSlidersAvailable = new();

        public int AvailableIntSlider => _lstIntSlidersAvailable.Count;
        public int AvailableFloatSlider => _lstFloatSlidersAvailable.Count;

        /// <summary>
        /// This method retrieves or creates a slider GameObject with specified properties.<br/>
        /// <br/>
        /// <example>
        /// Given a parent transform, label, value, minValue, maxValue, and isInteger flag, when calling GetOrCreateSlider, then a slider GameObject is created or retrieved with the specified properties.
        /// <code>
        /// Transform parentTransform = new GameObject().transform;
        /// GameObject slider = GetOrCreateSlider(parentTransform, "Volume", 5, 0, 10, true);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="parent">The parent transform to which the slider will be attached.</param>
        /// <param name="label">The label for the slider. Default is an empty string.</param>
        /// <param name="value">The initial value of the slider. Default is 0.</param>
        /// <param name="minValue">The minimum value of the slider. Default is 0.</param>
        /// <param name="maxValue">The maximum value of the slider. Default is 10.</param>
        /// <param name="isInteger">Indicates whether the slider is for integer values. Default is false.</param>
        /// <returns>The created or retrieved slider GameObject.</returns>
        public GameObject GetOrCreateSlider(Transform parent, string label = "", float value = 0, float minValue = 0, float maxValue = 10, bool isInteger = false)
        {
            SliderModelContainer sliderModelContainer;
            if (isInteger)
            {
                if (!_lstIntSlidersAvailable.TryDequeue(out sliderModelContainer))
                    sliderModelContainer = GameObject.Instantiate(_intPrefab);
            }
            else
            {
                if (!_lstIntSlidersAvailable.TryDequeue(out sliderModelContainer))
                    sliderModelContainer = GameObject.Instantiate(_floatPrefab);
            }

            sliderModelContainer.gameObject.SetActive(true);
            sliderModelContainer.transform.SetParent(parent.transform, false);

            if (!string.IsNullOrEmpty(label))
                sliderModelContainer.model.SetLabel(label);
            sliderModelContainer.model.SetMinValue(minValue);
            sliderModelContainer.model.SetMaxValue(maxValue);
            sliderModelContainer.model.SetValue(value);
            sliderModelContainer.model.SetIsInteger(isInteger);

            return sliderModelContainer.gameObject;
        }

        /// <summary>
        /// This method returns a slider GameObject to the pool, deactivates it, and reassigns its parent.<br/>
        /// <br/>
        /// <example>
        /// Given a slider GameObject, when calling Return, then the slider is added to the appropriate queue and deactivated.
        /// <code>
        /// GameObject slider = GetOrCreateSlider(parentTransform, "Volume", 5, 0, 10, true);
        /// Return(slider);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="sliderGameobject">The slider GameObject to be returned to the pool.</param>
        public void Return(GameObject sliderGameobject)
        {
            var sliderModelContainer = sliderGameobject.GetComponent<SliderModelContainer>();
            if (!sliderModelContainer)
                return;

            if (sliderModelContainer.model.isInteger)
                _lstIntSlidersAvailable.Enqueue(sliderModelContainer);
            else
                _lstFloatSlidersAvailable.Enqueue(sliderModelContainer);

            sliderModelContainer.gameObject.SetActive(false);
            sliderModelContainer.transform.SetParent(transform, false);
        }
    }
}