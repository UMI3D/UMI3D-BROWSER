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
using System.Collections.Generic;

namespace umi3d.browserRuntime.ui.dropdown
{
    /// <summary>
    /// Model of a dropdown element
    /// </summary>

    public class DropdownModel
    {
        public bool isLabelVisible { get; private set; } = false;
        public string label { get; private set; }
        public string value { get; private set; }
        public List<string> options { get; private set; } = new();

        Notifier _setNotifier;
        Notifier _updateNotifier;

        public DropdownModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<DropdownNotificationKeys.DropdownSet>());
            _setNotifier[DropdownNotificationKeys.DropdownSet.IsLabelVisible] = isLabelVisible;
            _setNotifier[DropdownNotificationKeys.DropdownSet.Label] = label;
            _setNotifier[DropdownNotificationKeys.DropdownSet.Value] = value;
            _setNotifier[DropdownNotificationKeys.DropdownSet.Options] = options;

            _updateNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<DropdownNotificationKeys.DropdownUpdated>());
        }

        /// <summary>
        /// Sets the label of the dropdown and updates its visibility status.<br/>
        /// Send a <see cref="DropdownNotificationKeys.DropdownSet"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a new label when setting the label then the label visibility is updated accordingly.
        /// <code>
        /// dropdownModel.SetLabel("New Label");
        /// // isLabelVisible = true
        /// // label = "New Label"
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newLabel">The new label to set.</param>
        public void SetLabel(string newLabel)
        {
            isLabelVisible = !string.IsNullOrEmpty(newLabel);
            label = newLabel;
            _setNotifier[DropdownNotificationKeys.DropdownSet.IsLabelVisible] = isLabelVisible;
            _setNotifier[DropdownNotificationKeys.DropdownSet.Label] = label;
            _setNotifier.Notify();
        }

        /// <summary>
        /// Sets the value of the dropdown and notifies any listeners about the change.<br/>
        /// Send a <see cref="DropdownNotificationKeys.DropdownSet"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a new value when setting the value then the value is updated and listeners are notified.
        /// <code>
        /// dropdownModel.SetValue("New Value");
        /// // value = "New Value"
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newValue">The new value to set.</param>
        public void SetValue(string newValue)
        {
            value = newValue;
            _setNotifier[DropdownNotificationKeys.DropdownSet.Value] = value;
            _setNotifier.Notify();
        }

        /// <summary>
        /// Updates the value of the dropdown and notifies any listeners about the change.<br/>
        /// Send a <see cref="DropdownNotificationKeys.DropdownUpdated"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a new value when updating the value then the value is updated and listeners are notified.
        /// <code>
        /// dropdownModel.UpdateValue("New Value");
        /// // value = "New Value"
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newValue">The new value to update.</param>
        public void UpdateValue(string newValue)
        {
            value = newValue;
            _updateNotifier[DropdownNotificationKeys.DropdownUpdated.Value] = value;
            _updateNotifier.Notify();
        }

        /// <summary>
        /// Sets the options of the dropdown and notifies any listeners about the change.<br/>
        /// Send a <see cref="DropdownNotificationKeys.DropdownSet"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a new list of options when setting the options then the options are updated and listeners are notified.
        /// <code>
        /// dropdownModel.SetOptions(new List&lt;string> { "Option 1", "Option 2" });
        /// // options = new List&lt;string> { "Option 1", "Option 2" }
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newOptions">The new list of options to set.</param>
        public void SetOptions(List<string> newOptions)
        {
            options = newOptions;
            _setNotifier[DropdownNotificationKeys.DropdownSet.Options] = options;
            _setNotifier.Notify();
        }
    }
}