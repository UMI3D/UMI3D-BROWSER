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

        Notifier _setNotifier;
        Notifier _updateNotifier;

        public InputFieldModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier<InputFieldNotificationsKeys.InputFieldSet>(this);
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.IsLabelVisible] = isLabelVisible;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.Label] = label;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.Value] = value;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.Placeholder] = placeholder;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.NbrLine] = nbrLine;

            _updateNotifier = NotificationHub.Default.GetNotifier<InputFieldNotificationsKeys.InputFieldUpdated>(this);
        }

        /// <summary>
        /// Set the label of the input field.
        /// Send a <see cref="InputFieldNotificationsKeys.InputFieldSet"/> notification.
        /// </summary>
        /// <param name="newLabel"></param>
        public void SetTitle(string newLabel)
        {
            isLabelVisible = label == null || label == string.Empty;
            label = newLabel;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.IsLabelVisible] = isLabelVisible;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.Label] = label;
            _setNotifier.Notify();
        }

        /// <summary>
        /// Set the value of the input field.
        /// Send a <see cref="InputFieldNotificationsKeys.InputFieldSet"/> notification.
        /// </summary>
        /// <param name="newValue"></param>
        public void SetValue(string newValue)
        {
            value = newValue;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.Value] = value;
            _setNotifier.Notify();
        }

        /// <summary>
        /// Update the value of the input field.
        /// Send a <see cref="InputFieldNotificationsKeys.InputFieldUpdated"/> notification.
        /// </summary>
        /// <param name="newValue"></param>
        public void UpdateValue(string newValue)
        {
            value = newValue;
            _updateNotifier[InputFieldNotificationsKeys.InputFieldUpdated.Value] = value;
            _updateNotifier.Notify();
        }

        /// <summary>
        /// Set the placeholder of the input field.
        /// Send a <see cref="InputFieldNotificationsKeys.InputFieldSet"/> notification.
        /// </summary>
        /// <param name="newPlaceholder"></param>
        public void SetPlaceholder(string newPlaceholder)
        {
            placeholder = newPlaceholder;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.Placeholder] = placeholder;
            _setNotifier.Notify();
        }

        /// <summary>
        /// Set the number of the that the input field will display.
        /// Send a <see cref="InputFieldNotificationsKeys.InputFieldSet"/> notification.
        /// </summary>
        /// <param name="newNbrLine"></param>
        public void SetNbrLines(int newNbrLine)
        {
            nbrLine = newNbrLine;
            _setNotifier[InputFieldNotificationsKeys.InputFieldSet.NbrLine] = nbrLine;
            _setNotifier.Notify();
        }

        public int GetInputHeight(int heightOnLine)
        {
            // TODO: Calcul input height

            return 0;
        }
    }
}