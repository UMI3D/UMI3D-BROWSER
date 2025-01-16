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

using umi3d.browserRuntime.ui.slider;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    [RequireComponent(typeof(SliderFactory))]
    public class ContextualMenuSliderFactory : MonoBehaviour
    {
        SliderFactory _sliderFactory;

        private void Awake()
        {
            _sliderFactory = GetComponent<SliderFactory>();
        }

        public GameObject GetOrCreate(Transform parent, IntegerRangeParameterDto dto)
        {
            var sliderGameobject = _sliderFactory.GetOrCreateSlider(parent, isInteger: true);
            sliderGameobject.GetComponent<SliderIntParameterModelContainer>().parameterModel.SetDto(dto);

            return sliderGameobject;
        }

        public GameObject GetOrCreate(Transform parent, FloatRangeParameterDto dto)
        {
            var sliderGameobject = _sliderFactory.GetOrCreateSlider(parent, isInteger: false);
            sliderGameobject.GetComponent<SliderFloatParameterModelContainer>().parameterModel.SetDto(dto);

            return sliderGameobject;
        }

        public void Return(GameObject inputFieldModelContainer)
        {
            _sliderFactory.Return(inputFieldModelContainer);
        }
    }
}