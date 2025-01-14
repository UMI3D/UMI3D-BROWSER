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

namespace umi3d.browserRuntime.ui.toggle
{
    /// <summary>
    /// Model of a toggle element
    /// </summary>

    public class ToggleModel 
    {
        public bool isLabelVisible { get; private set; } = false;
        public string label { get; private set; }
        public bool value { get; private set; }

        Notifier _setNotifier;
        Notifier _updateNotifier;

        public ToggleModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<ToggleNotificationKeys.ToggleSet>());
            _setNotifier[ToggleNotificationKeys.ToggleSet.IsLabelVisible] = isLabelVisible;
            _setNotifier[ToggleNotificationKeys.ToggleSet.Label] = label;
            _setNotifier[ToggleNotificationKeys.ToggleSet.Value] = value;

            _updateNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<ToggleNotificationKeys.ToggleUpdated>());
        }

        /// <summary>
        /// Sets the label to the specified value and updates the visibility status.<br/>
        /// Send a <see cref="ToggleNotificationKeys.ToggleSet"/> notification.<br/>
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
            _setNotifier[ToggleNotificationKeys.ToggleSet.IsLabelVisible] = isLabelVisible;
            _setNotifier[ToggleNotificationKeys.ToggleSet.Label] = label;
            _setNotifier.Notify();
        }

        /// <summary>
        /// Sets the value to the specified boolean value and notifies any observers.<br/>
        /// Send a <see cref="ToggleNotificationKeys.ToggleSet"/> notification.<br/>
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
            _setNotifier[ToggleNotificationKeys.ToggleSet.Value] = value;
            _setNotifier.Notify();
        }

        /// <summary>
        /// Toggles the current boolean value and notifies any observers.<br/>
        /// Send a <see cref="ToggleNotificationKeys.ToggleUpdated"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given the current value, when toggling the value, then the value is inverted and observers are notified.
        /// <code>
        /// _toggleModel.ToggleValue();
        /// // if value was true, it becomes false
        /// // if value was false, it becomes true
        /// </code>
        /// </example>
        /// </summary>
        public void ToggleValue()
        {
            value = !value;
            _updateNotifier[ToggleNotificationKeys.ToggleUpdated.Value] = value;
            _updateNotifier.Notify();
        }
    }
}