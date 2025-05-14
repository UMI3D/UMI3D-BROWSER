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

namespace umi3d.browserRuntime.ui
{
    /// <summary>
    /// Model of a toggle element
    /// </summary>
    public class ToggleModel : ILabelSubject, IValueSubject<bool>
    {
        public bool isLabelVisible { get; private set; } = false;
        public string label { get; private set; }
        public bool value { get; private set; }

        #region ISubject

        LabelSubject _labelSubject = new();

        public void Subscribe(ILabelObserver observer)
        {
            _labelSubject.Subscribe(observer);
        }

        public void Unsubscribe(ILabelObserver observer)
        {
            _labelSubject.Unsubscribe(observer);
        }

        void NotifyLabelObserver()
        {
            _labelSubject.NotifyLabelObserver(label, isLabelVisible);
        }

        ValueSubject<bool> _valueSubject = new();

        public void Subscribe(IValueObserver<bool> observer)
        {
            _valueSubject.Subscribe(observer);
        }

        public void Unsubscribe(IValueObserver<bool> observer)
        {
            _valueSubject.Unsubscribe(observer);
        }

        void NotifyValueObserver()
        {
            _valueSubject.NotifyValueObserver(value);
        }

        #endregion

        /// <summary>
        /// Sets the label to the specified value and updates the visibility status.<br/>
        /// <br/>
        /// <example>
        /// Given a new label string, when setting the label, then the visibility status and label value are updated accordingly.
        /// <code>
        /// _toggleModel.SetLabel("Test Label");
        /// // isLabelVisible = true
        /// // label = "Test Label"
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newLabel">The new label string to set. If null or empty, the label will be hidden.</param>
        public void SetLabel(string newLabel)
        {
            isLabelVisible = !string.IsNullOrEmpty(newLabel);
            label = newLabel;
            NotifyLabelObserver();
        }

        /// <summary>
        /// Sets the value to the specified boolean value and notifies any observers.<br/>
        /// <br/>
        /// <example>
        /// Given a new boolean value, when setting the value, then the value is updated and observers are notified.
        /// <code>
        /// _toggleModel.SetValue(true);
        /// // value = true
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newValue">The new boolean value to set.</param>
        public void SetValue(bool newValue)
        {
            value = newValue;
            NotifyValueObserver();
        }
    }
}