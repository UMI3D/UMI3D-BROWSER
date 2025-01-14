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
using UnityEngine;

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

        /// <summary>
        /// This method sets the label for the slider and updates its visibility status.<br/>
        /// Send a <see cref="SliderNotifiactionKeys.SliderSet"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a new label string, when calling SetLabel, then the label and its visibility status are updated.
        /// <code>
        /// SetLabel("Volume");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newLabel">The new label to set. If the label is null or empty, the label visibility is set to false.</param>
        public void SetLabel(string newLabel)
        {
            isLabelVisible = !string.IsNullOrEmpty(newLabel);
            label = newLabel;
            _setNotifier[SliderNotifiactionKeys.SliderSet.IsLabelVisible] = isLabelVisible;
            _setNotifier[SliderNotifiactionKeys.SliderSet.Label] = label;
            _setNotifier.Notify();
        }

        /// <summary>
        /// This method sets the value for the slider, clamping it within the specified min and max range.<br/>
        /// Send a <see cref="SliderNotifiactionKeys.SliderSet"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a new value, when calling SetValue, then the value is clamped within the min and max range and updated.
        /// <code>
        /// SetValue(5.5f);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newValue">The new value to set. It will be clamped within the min and max range.</param>
        public void SetValue(float newValue)
        {
            value = Mathf.Clamp(newValue, minValue, maxValue);
            _setNotifier[SliderNotifiactionKeys.SliderSet.Value] = value;
            _setNotifier.Notify();
        }

        /// <summary>
        /// This method updates the value for the slider, clamping it within the specified min and max range, and notifies observers of the change.<br/>
        /// Send a <see cref="SliderNotifiactionKeys.SliderUpdated"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a new value, when calling UpdateValue, then the value is clamped within the min and max range and updated.
        /// <code>
        /// UpdateValue(5.5f);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newValue">The new value to update. It will be clamped within the min and max range.</param>
        public void UpdateValue(float newValue)
        {
            value = Mathf.Clamp(newValue, minValue, maxValue);
            _updateNotifier[SliderNotifiactionKeys.SliderUpdated.Value] = value;
            _updateNotifier.Notify();
        }

        /// <summary>
        /// This method sets the maximum value for the slider, clamps the current value within the new range, and notifies observers of the changes.<br/>
        /// Send a <see cref="SliderNotifiactionKeys.SliderSet"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a new maximum value, when calling SetMaxValue, then the maximum value is updated and the current value is clamped within the new range.
        /// <code>
        /// SetMaxValue(20f);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newValue">The new maximum value to set.</param>
        public void SetMaxValue(float newValue)
        {
            maxValue = newValue;
            value = Mathf.Clamp(value, minValue, maxValue);
            _setNotifier[SliderNotifiactionKeys.SliderSet.MaxValue] = maxValue;
            _setNotifier[SliderNotifiactionKeys.SliderSet.Value] = value;
            _setNotifier.Notify();
        }

        /// <summary>
        /// This method sets the minimum value for the slider, clamps the current value within the new range, and notifies observers of the changes.<br/>
        /// Send a <see cref="SliderNotifiactionKeys.SliderSet"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a new minimum value, when calling SetMinValue, then the minimum value is updated and the current value is clamped within the new range.
        /// <code>
        /// SetMinValue(-5f);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newValue">The new minimum value to set.</param>
        public void SetMinValue(float newValue)
        {
            minValue = newValue;
            value = Mathf.Clamp(value, minValue, maxValue);
            _setNotifier[SliderNotifiactionKeys.SliderSet.MinValue] = minValue;
            _setNotifier[SliderNotifiactionKeys.SliderSet.Value] = value;
            _setNotifier.Notify();
        }

        /// <summary>
        /// This method sets whether the slider represents integer values and notifies observers of the change.<br/>
        /// Send a <see cref="SliderNotifiactionKeys.SliderSet"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a boolean value, when calling SetIsInteger, then the isInteger property is updated and observers are notified.
        /// <code>
        /// SetIsInteger(true);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newIsInteger">The new boolean value to set for isInteger.</param>
        public void SetIsInteger(bool newIsInteger)
        {
            isInteger = newIsInteger;
            _setNotifier[SliderNotifiactionKeys.SliderSet.IsInteger] = isInteger;
            _setNotifier.Notify();
        }
    }
}