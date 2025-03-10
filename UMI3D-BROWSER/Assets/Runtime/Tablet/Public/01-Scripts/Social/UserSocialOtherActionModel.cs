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
using umi3d.cdk.collaboration;
using UnityEngine;

namespace umi3d.browserRuntime.ui.tablet.social
{
    public class UserSocialOtherActionModel
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Texture2D Texture { get; private set; }
        public Action Action { get; private set; }

        Notifier _setNotifier;

        public UserSocialOtherActionModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<UserSocialNotificationKeys.UserSocialOtherActionSet>());

            _setNotifier[UserSocialNotificationKeys.UserSocialOtherActionSet.Name] = Name;
            _setNotifier[UserSocialNotificationKeys.UserSocialOtherActionSet.Description] = Description;
            _setNotifier[UserSocialNotificationKeys.UserSocialOtherActionSet.Texture] = Texture;
            _setNotifier[UserSocialNotificationKeys.UserSocialOtherActionSet.Action] = Action;
        }

        /// <summary>
        /// Sets the user action properties.<br/>
        /// <br/>
        /// <example>
        /// Given a valid UserAction object, when SetUserAction is called, then the properties are set correctly.<br/>
        /// <code>
        ///  // var userAction;
        /// _model.SetUserAction(userAction);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="userAction">The UserAction object containing the action details.</param>
        public async void SetUserAction(UserAction userAction)
        {
            if (userAction == null)
            {
                Debug.LogError("UserAction is null");
                return;
            }

            Name = userAction.name;
            Description = userAction.description;
            Texture = await userAction.GetTexture();
            Action = userAction.Call;

            _setNotifier[UserSocialNotificationKeys.UserSocialOtherActionSet.Name] = Name;
            _setNotifier[UserSocialNotificationKeys.UserSocialOtherActionSet.Description] = Description;
            _setNotifier[UserSocialNotificationKeys.UserSocialOtherActionSet.Texture] = Texture;
            _setNotifier[UserSocialNotificationKeys.UserSocialOtherActionSet.Action] = Action;
            _setNotifier.Notify();
        }
    }
}