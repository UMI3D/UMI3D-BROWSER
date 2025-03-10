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
using System;

namespace umi3d.browserRuntime.ui.inputField
{
    /// <summary>
    /// Model of an input field element
    /// </summary>
    public class InputFieldModel
    {
        public bool isLabelVisible { get; private set; } = false;
        public string label { get; private set; }
        public string value { get; private set; }
        public string placeholder { get; private set; }
        public int nbrLine { get; private set; } = 1;
        public bool isPrivate { get; private set; } = false;

        Notifier _setNotifier;
        Notifier _updateNotifier;

        public InputFieldModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this, ID.FromType<InputFieldNotificationsKeys.InputFieldSet>());
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.IsLabelVisible] = isLabelVisible;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.Label] = label;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.Value] = value;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.Placeholder] = placeholder;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.NbrLine] = nbrLine;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.IsPrivate] = isPrivate;

            _updateNotifier = NotificationHub.Default.GetNotifier(this, ID.FromType<InputFieldNotificationsKeys.InputFieldUpdated>());
        }

        /// <summary>
        /// Sets the label and updates its visibility status.<br/>
        /// Send a <see cref="InputFieldNotificationsKeys.InputFieldSet"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a new label, when setting the label, then the label is updated and its visibility is set accordingly.
        /// <code>
        /// _model.SetLabel("Test Label"); // label = "Test Label", isLabelVisible = true
        /// _model.SetLabel(""); // label = "", isLabelVisible = false
        /// _model.SetLabel(null); // label = null, isLabelVisible = false
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newLabel">The new label to set.</param>
        public void SetLabel(string newLabel)
        {
            isLabelVisible = !string.IsNullOrEmpty(newLabel);
            label = newLabel;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.IsLabelVisible] = isLabelVisible;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.Label] = label;
            _setNotifier.Notify();
        }

        /// <summary>
        /// Sets the value of the input field and notifies the change.<br/>
        /// Send a <see cref="InputFieldNotificationsKeys.InputFieldSet"/> notification. <br/>
        /// <br/>
        /// <example>
        /// Given a new value when setting the value then the value is updated and notification is sent.
        /// <code>
        /// _model.SetValue("Test Value");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newValue">The new value to set.</param>
        public void SetValue(string newValue)
        {
            value = newValue;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.Value] = value;
            _setNotifier.Notify();
        }


        /// <summary>
        /// Sets the value of the input field and notifies the change.<br/>
        /// Send a <see cref="InputFieldNotificationsKeys.InputFieldUpdated"/> notification. <br/>
        /// <br/>
        /// <example>
        /// Given a new value when setting the value then the value is updated and notification is sent.
        /// <code>
        /// _model.SetValue("Test Value");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="newValue">The new value to set.</param>
        public void UpdateValue(string newValue)
        {
            value = newValue;
            _updateNotifier[InputFieldNotificationsKeys.InputFieldUpdated.Value] = value;
            _updateNotifier.Notify();
        }

        /// /// <summary>
        /// This method sets a new placeholder for the input field and notifies the change.<br/>
        /// Send a <see cref="InputFieldNotificationsKeys.InputFieldSet"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a new placeholder string when setting the placeholder then the placeholder is updated and notification is sent.
        /// <code>
        /// _model.SetPlaceholder("New Placeholder");
        /// </code> 
        /// </example>
        /// </summary>
        /// <param name="newPlaceholder">The new placeholder string to set.</param>
        public void SetPlaceholder(string newPlaceholder)
        {
            placeholder = newPlaceholder;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.Placeholder] = placeholder;
            _setNotifier.Notify();
        }

        /// /// <summary>
        /// This method sets a new number of lines for the input field and notifies the change.<br/>
        /// Send a <see cref="InputFieldNotificationsKeys.InputFieldSet"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a new number of lines when setting the number of lines then the number of lines is updated and notification is sent.
        /// <code>
        /// _model.SetNbrLines(2);
        /// </code> 
        /// </example>
        /// </summary>
        /// <param name="newNbrLine">The new number of lines to set.</param>
        public void SetNbrLines(int newNbrLine)
        {
            nbrLine = newNbrLine;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.NbrLine] = nbrLine;
            _setNotifier.Notify();
        }


        /// /// <summary>
        /// This method sets if the input field need to be shown with "*****" and notifies the change.<br/>
        /// Send a <see cref="InputFieldNotificationsKeys.InputFieldSet"/> notification.<br/>
        /// <br/>
        /// <example>
        /// Given a new isPrivate when setting the privacy then the isPrivate is updated and notification is sent.
        /// <code>
        /// _model.SetPrivate(true);
        /// </code> 
        /// </example>
        /// </summary>
        /// <param name="newNbrLine">The new number of lines to set.</param>
        public void SetPrivate(bool newIsPrivate)
        {
            isPrivate = newIsPrivate;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.IsPrivate] = isPrivate;
            _setNotifier.Notify();
        }
    }
}