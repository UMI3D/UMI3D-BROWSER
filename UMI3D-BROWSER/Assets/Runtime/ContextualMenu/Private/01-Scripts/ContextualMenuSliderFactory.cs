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
using umi3d.browserRuntime.ui.slider;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    [RequireComponent(typeof(SliderFactory))]
    internal class ContextualMenuSliderFactory : MonoBehaviour
    {
        SliderFactory _sliderFactory;

        private void Awake()
        {
            _sliderFactory = GetComponent<SliderFactory>();
        }

        public GameObject GetOrCreate(Transform parent, IntegerRangeParameterDto dto)
        {
            var sliderGameobject = _sliderFactory.GetOrCreateSlider(parent, isInteger: true);
            var model = sliderGameobject.GetComponent<SliderIntParameterModelContainer>().parameterModel;
            model.SetDto(dto);

            NotificationHub.Default.Subscribe(sliderGameobject,
                ID.FromType<ContextualMenuNotificationKeys.Submit>(),
                (Callback)model.Submit);

            return sliderGameobject;
        }

        public GameObject GetOrCreate(Transform parent, FloatRangeParameterDto dto)
        {
            var sliderGameobject = _sliderFactory.GetOrCreateSlider(parent, isInteger: false);
            var model = sliderGameobject.GetComponent<SliderFloatParameterModelContainer>().parameterModel;
            model.SetDto(dto);

            NotificationHub.Default.Subscribe(sliderGameobject,
                ID.FromType<ContextualMenuNotificationKeys.Submit>(),
                (Callback)model.Submit);


            return sliderGameobject;
        }

        public void Return(GameObject sliderModelContainer)
        {
            NotificationHub.Default.Unsubscribe(sliderModelContainer);
            var modelFloat = sliderModelContainer.GetComponent<SliderFloatParameterModelContainer>();
            if (modelFloat != null)
                modelFloat.parameterModel.ReleaseDto();
            var modelInt = sliderModelContainer.GetComponent<SliderIntParameterModelContainer>();
            if (modelInt != null)
                modelInt.parameterModel.ReleaseDto();
            _sliderFactory.Return(sliderModelContainer);
        }
    }
}