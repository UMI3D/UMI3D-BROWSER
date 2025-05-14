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

namespace umi3d.browserRuntime.ui
{
    public class SliderModel : ILabelSubject, IValueSubject<float>, ISliderRangeSubject, ISliderWholeNumbersSubject
    {
        public bool isLabelVisible { get; private set; } = false;
        public string label { get; private set; }
        public float value { get; private set; }
        public float maxValue { get; private set; }
        public float minValue { get; private set; }
        public bool isInteger { get; private set; } = false;

        #region Subject

        LabelSubject labelSubject = new();

        public void Subscribe(ILabelObserver observer)
        {
            labelSubject.Subscribe(observer);
        }

        public void Unsubscribe(ILabelObserver observer)
        {
            labelSubject.Unsubscribe(observer);
        }

        void NotifyLabelObserver()
        {
            labelSubject.NotifyLabelObserver(label, isLabelVisible);
        }

        ValueSubject<float> valueSubject = new();

        public void Subscribe(IValueObserver<float> observer)
        {
            valueSubject.Subscribe(observer);
        }

        public void Unsubscribe(IValueObserver<float> observer)
        {
            valueSubject.Unsubscribe(observer);
        }

        void NotifyValueObserver()
        {
            valueSubject.NotifyValueObserver(value);
        }

        List<ISliderRangeObserver> _rangeObservers = new();

        public void Subscribe(ISliderRangeObserver observer)
        {
            if (!_rangeObservers.Contains(observer))
            {
                _rangeObservers.Add(observer);
            }
        }

        public void Unsubscribe(ISliderRangeObserver observer)
        {
            _rangeObservers.Remove(observer);
        }

        void NotifyRangeObserver()
        {
            foreach (var observer in _rangeObservers)
            {
                try
                {
                    observer.UpdateRange(minValue, maxValue);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        List<ISliderWholeNumbersObserver> _wholeNumbersObservers = new();

        public void Subscribe(ISliderWholeNumbersObserver observer)
        {
            if (!_wholeNumbersObservers.Contains(observer))
            {
                _wholeNumbersObservers.Add(observer);
            }
        }

        public void Unsubscribe(ISliderWholeNumbersObserver observer)
        {
            _wholeNumbersObservers.Remove(observer);
        }

        void NotifyWholeNumbersObserver()
        {
            foreach (var observer in _wholeNumbersObservers)
            {
                try
                {
                    observer.UpdateWholeNumbers(isInteger);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        #endregion

        /// <summary>
        /// This method sets the label for the slider and updates its visibility status.<br/>
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
            NotifyLabelObserver();
        }

        /// <summary>
        /// This method sets the value for the slider, clamping it within the specified min and max range.<br/>
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
            NotifyValueObserver();
        }

        /// <summary>
        /// This method sets the maximum value for the slider, clamps the current value within the new range, and notifies observers of the changes.<br/>
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
            NotifyRangeObserver();
        }

        /// <summary>
        /// This method sets the minimum value for the slider, clamps the current value within the new range, and notifies observers of the changes.<br/>
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
            NotifyRangeObserver();
        }

        /// <summary>
        /// This method sets whether the slider represents integer values and notifies observers of the change.<br/>
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
            NotifyWholeNumbersObserver();
        }
    }
}