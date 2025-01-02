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
using umi3d.browserRuntime.ui.toggle;

namespace umi3d
{
    public class ToggleModel 
    {
        public bool isLabelVisible { get; private set; } = false;
        public string label { get; private set; }
        public bool value { get; private set; }

        Notifier _setNotifier;
        Notifier _updateNotifier;

        public ToggleModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier<ToggleNotificationKeys.ToggleSet>(this);
            _setNotifier[ToggleNotificationKeys.ToggleSet.IsLabelVisible] = isLabelVisible;
            _setNotifier[ToggleNotificationKeys.ToggleSet.Label] = label;
            _setNotifier[ToggleNotificationKeys.ToggleSet.Value] = value;

            _updateNotifier = NotificationHub.Default.GetNotifier<ToggleNotificationKeys.ToggleUpdated>(this);
        }

        public void SetLabel(string newLabel)
        {
            isLabelVisible = !string.IsNullOrEmpty(newLabel);
            label = newLabel;
            _setNotifier[ToggleNotificationKeys.ToggleSet.IsLabelVisible] = isLabelVisible;
            _setNotifier[ToggleNotificationKeys.ToggleSet.Label] = label;
            _setNotifier.Notify();
        }

        public void SetValue(bool newValue)
        {
            value = newValue;
            _setNotifier[ToggleNotificationKeys.ToggleSet.Value] = value;
            _setNotifier.Notify();
        }

        public void ToggleValue()
        {
            SetValue(!value);
        }
    }
}