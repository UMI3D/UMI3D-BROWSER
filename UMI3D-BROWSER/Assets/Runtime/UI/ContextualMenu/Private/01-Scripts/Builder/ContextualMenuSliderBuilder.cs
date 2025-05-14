/*
Copyright 2019 - 2025 Inetum

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
using umi3d.cdk.interaction;
using umi3d.common.interaction;
using UnityEngine;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    public class ContextualMenuSliderBuilder<T> : ISliderBuilder where T : IComparable
    {
        public SliderFactory factory { get; private set; }

        GameObject control;
        SliderModel model;

        Projection projection;
        AbstractRangeParameterDto<T> dto;

        public void Build(Transform parent)
        {
            factory.TryToGetOrCreate(out control, out model, parent);
        }

        public void BuildLabel()
        {
            model.SetLabel(dto.name);
        }

        public void BuildRange()
        {
            if (dto.min is int minInt && dto.max is int maxInt)
            {
                model.SetMinValue(minInt);
                model.SetMaxValue(maxInt);
            }
            else if (dto.min is float minFloat && dto.max is float maxFloat)
            {
                model.SetMinValue(minFloat);
                model.SetMaxValue(maxFloat);
            }
        }

        public void BuildValue()
        {
            if (dto.value is int valueInt)
            {
                model.SetValue(valueInt);
                model.SetIsInteger(true);
            }
            else if (dto.value is float valueFloat)
            {
                model.SetValue(valueFloat);
                model.SetIsInteger(false);
            }
        }

        public GameObject GetControl()
        {
            control.SetActive(true);
            return control;
        }
    }
}