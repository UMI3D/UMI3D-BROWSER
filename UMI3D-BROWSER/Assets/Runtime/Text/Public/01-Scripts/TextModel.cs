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

namespace umi3d.browserRuntime.text
{
    public class TextModel 
    {
        public string Text { get; private set; } = "";

        Notifier _setNotifier;

        public TextModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<TextNotificationKeys.TextSet>());
            _setNotifier[TextNotificationKeys.TextSet.Text] = Text;
        }

        public void SetText(string text)
        {
            Text = text ?? "";
            _setNotifier[TextNotificationKeys.TextSet.Text] = Text;
            _setNotifier.Notify();
        }
    }
}