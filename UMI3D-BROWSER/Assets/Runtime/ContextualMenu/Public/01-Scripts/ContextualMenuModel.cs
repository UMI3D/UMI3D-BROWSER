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
using umi3d.browserRuntime.notificationKeys;
using umi3d.browserRuntime.ui.tablet;
using umi3d.common.interaction;

namespace umi3d.browserRuntime.ui.contextualMenu
{
    public class ContextualMenuModel
    {
        bool _isActive = false;

        Notifier _addParameterNotifier;

        public ContextualMenuModel()
        {
            _addParameterNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<ContextualMenuNotificationKeys.AddParameter>());

            NotificationHub.Default.Subscribe(this,
                ID.FromType<InteractionNotificationKeys.DisplayParameters>(), 
                (Callback)DisplayParameters);

            NotificationHub.Default.Subscribe(this,
                ID.FromType<ContextualMenuNotificationKeys.Close>(), 
                (Callback)Hide);
            NotificationHub.Default.Subscribe(this, 
                TabletNotificationKeys.Open, 
                (Callback)Hide);
        }

        ~ContextualMenuModel()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void DisplayParameters(Notification notification)
        {
            if (!notification.TryGetInfoT(InteractionNotificationKeys.DisplayParameters.parameters, out List<AbstractParameterDto> parameters))
                return;

            if (_isActive || parameters.Count <= 0)
                return;
            _isActive = true;

            var paramtersTemp = new List<AbstractParameterDto>(parameters);
            paramtersTemp.Reverse(); // Reverse to show element above in front (layout in the object is set to reverse too)

            foreach (var param in paramtersTemp)
            {
                _addParameterNotifier[ContextualMenuNotificationKeys.AddParameter.Parameter] = param;
                _addParameterNotifier.Notify();
            }

            NotificationHub.Default.Notify(this, ID.FromType<ContextualMenuNotificationKeys.Open>());
        }

        private void Hide(Notification notification)
        {
            _isActive = false;
        }
    }
}