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

namespace umi3d.browserRuntime.ui.slider
{
    public class SliderModel
    {
        public bool isLabelVisible { get; private set; } = false;
        public string label { get; private set; }
        public float value { get; private set; }
        public float maxValue { get; private set; }
        public float minValue { get; private set; }
        public bool isInteger { get; private set; } = false;

        Notifier _setNotifier;
        Notifier _updateNotifier;

        public SliderModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier<SliderNotifiactionKeys.SliderSet>(this);
            _setNotifier[SliderNotifiactionKeys.SliderSet.IsLabelVisible] = isLabelVisible;
            _setNotifier[SliderNotifiactionKeys.SliderSet.Label] = label;
            _setNotifier[SliderNotifiactionKeys.SliderSet.Value] = value;
            _setNotifier[SliderNotifiactionKeys.SliderSet.MaxValue] = maxValue;
            _setNotifier[SliderNotifiactionKeys.SliderSet.MinValue] = minValue;
            _setNotifier[SliderNotifiactionKeys.SliderSet.IsInteger] = isInteger;

            _updateNotifier = NotificationHub.Default.GetNotifier<SliderNotifiactionKeys.SliderUpdated>(this);
        }

        public void SetLabel(string newLabel)
        {
            isLabelVisible = !string.IsNullOrEmpty(newLabel);
            label = newLabel;
            _setNotifier[SliderNotifiactionKeys.SliderSet.IsLabelVisible] = isLabelVisible;
            _setNotifier[SliderNotifiactionKeys.SliderSet.Label] = label;
            _setNotifier.Notify();
        }

        public void SetValue(float newValue)
        {
            value = newValue;
            _setNotifier[SliderNotifiactionKeys.SliderSet.Value] = value;
            _setNotifier.Notify();
        }

        public void UpdateValue(float newValue)
        {
            value = newValue;
            _updateNotifier[SliderNotifiactionKeys.SliderSet.Value] = value;
            _updateNotifier.Notify();
        }

        public void SetMaxValue(float newValue)
        {
            maxValue = newValue;
            _setNotifier[SliderNotifiactionKeys.SliderSet.MaxValue] = value;
            _setNotifier.Notify();
        }

        public void SetMinValue(float newValue)
        {
            minValue = newValue;
            _setNotifier[SliderNotifiactionKeys.SliderSet.MinValue] = value;
            _setNotifier.Notify();
        }

        public void SetPlaceholder(bool newIsInteger)
        {
            isInteger = newIsInteger;
            _setNotifier[SliderNotifiactionKeys.SliderSet.IsInteger] = isInteger;
            _setNotifier.Notify();
        }
    }
}