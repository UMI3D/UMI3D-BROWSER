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
using System.Collections.Generic;
using umi3d.baseBrowser.extension;
using umi3d.cdk.collaboration;
using UnityEngine;

namespace umi3d.browserRuntime.ui.tablet.social
{
    public class UserSocialModel
    {
        const int k_maxPrimaryAction = 3;

        const float k_logBase = 1.5f;
        const float k_factor = 5f / 2f;
        const float k_factor2 = 5f / 2f;

        public string Name { get; private set; }
        public string Place { get; private set; }
        public float Volume { get; private set; }
        public bool IsMute { get; private set; }
        public List<UserAction> PrimaryActions { get; private set; }
        public List<UserAction> OtherActions { get; private set; }

        UMI3DUser _user;

        Notifier _setNotifier;
        Notifier _updateNotifier;

        public UserSocialModel()
        {
            _setNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<UserSocialNotificationKeys.UserSocialSet>());
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.Name] = Name;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.Place] = Place;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.Volume] = Volume;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.IsMute] = IsMute;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.PrimaryActions] = PrimaryActions;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.OtherActions] = OtherActions;

            _updateNotifier = NotificationHub.Default.GetNotifier(this,
                ID.FromType<UserSocialNotificationKeys.UserSocialUpdate>());
        }

        public void SetUser(UMI3DUser user)
        {
            _user = user;

            Name = _user.login.CapitalizeAllWord();
            Place = $"({UMI3DCollaborationClientServer.Environement?.name})";
            Volume = 100;
            IsMute = false;
            PrimaryActions = new List<UserAction>();
            OtherActions = new List<UserAction>();

            foreach (var action in PrimaryActions)
            {
                if (action.isPrimary && PrimaryActions.Count < k_maxPrimaryAction)
                    PrimaryActions.Add(action);
                else
                    OtherActions.Add(action);
            }

            _setNotifier[UserSocialNotificationKeys.UserSocialSet.Name] = Name;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.Place] = Place;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.Volume] = Volume;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.IsMute] = IsMute;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.PrimaryActions] = PrimaryActions;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.OtherActions] = OtherActions;
            _setNotifier.Notify();
        }

        public void UpdateVolume(float volume)
        {
            Volume = volume;
            IsMute = volume <= float.Epsilon;

            UpdateAudioManagerFor(_user, IsMute ? 0 : Volume);

            _updateNotifier[UserSocialNotificationKeys.UserSocialUpdate.Volume] = IsMute ? 0 : Volume;
            _updateNotifier[UserSocialNotificationKeys.UserSocialUpdate.IsMute] = IsMute;
            _updateNotifier.Notify();
        }

        public void UpdateMute(bool mute)
        {
            IsMute = mute;

            UpdateAudioManagerFor(_user, IsMute ? 0 : Volume);

            _updateNotifier[UserSocialNotificationKeys.UserSocialUpdate.Volume] = IsMute ? 0 : Volume;
            _updateNotifier[UserSocialNotificationKeys.UserSocialUpdate.IsMute] = IsMute;
            _updateNotifier.Notify();
        }

        static void UpdateAudioManagerFor(UMI3DUser user, float volume)
        {
            var vg = UserVolumeToVG(volume);
            AudioManager.Instance.SetGainForUser(user, vg.gain);
            AudioManager.Instance.SetVolumeForUser(user, vg.volume);
        }

        static (float volume, float gain) UserVolumeToVG(float volume)
        {
            if (volume <= 100f)
                return (volume / 100f, 1);
            else
                return (1, GainFactor(volume / 100));
        }

        static float GainFactor(float gain) => (Mathf.Pow(k_logBase, (gain - 1) * k_factor) - 1) * k_factor2 + 1;
    }
}