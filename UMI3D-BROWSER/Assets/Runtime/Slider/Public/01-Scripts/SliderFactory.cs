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

            if (!string.IsNullOrEmpty(label))
                sliderModelContainer.model.SetLabel(label);
            sliderModelContainer.model.SetMinValue(minValue);
            sliderModelContainer.model.SetMaxValue(maxValue);
            sliderModelContainer.model.SetValue(value);
            sliderModelContainer.model.SetIsInteger(isInteger);

            return sliderModelContainer.gameObject;
        }

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