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

using inetum.unityUtils.observation;
using System;

namespace umi3d.browserRuntime.button
{
    public class ButtonModel 
    {
        public string Label { get; private set; } = string.Empty;

        internal Action _callback;

        Notifier _setNotifier;

        public ButtonModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<ButtonNotificationKeys.ButtonSet>());
            _setNotifier[ButtonNotificationKeys.ButtonSet.Label] = Label;
        }

        public void SetLabel(string label)
        {
            Label = label ?? string.Empty;
            _setNotifier[ButtonNotificationKeys.ButtonSet.Label] = Label;
            _setNotifier.Notify();
        }

        public void SetCallback(Action callback)
        {
            _callback = callback;
        }

        public void Click()
        {
            _callback?.Invoke();
        }
    }
}