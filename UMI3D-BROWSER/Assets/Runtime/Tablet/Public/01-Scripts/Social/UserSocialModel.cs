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
using umi3d.cdk.collaboration;
using UnityEngine;

namespace umi3d.browserRuntime.ui.tablet.social
{
    public class UserSocialModel
    {
        public const int k_maxPrimaryAction = 3;

        const float k_logBase = 1.5f;
        const float k_factor = 5f / 2f;
        const float k_factor2 = 5f / 2f;

        public string Name { get; private set; }
        public string Place { get; private set; }
        public bool IsMute { get; private set; }
        public float Volume => IsMute ? 0 : _volume;
        public List<UserAction> PrimaryActions { get; private set; } = new List<UserAction>();
        public List<UserAction> OtherActions { get; private set; } = new List<UserAction>();

        private float _volume;

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

        /// <summary>
        /// Sets the user information and initializes various properties.<br/>
        /// <br/>
        /// <example>
        /// Given a valid user, when setting the user, then user properties are set correctly.
        /// <code>
        /// // UMI3DUser user;
        /// userSocial.SetUser(user);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="user">The user to set.</param>
        public void SetUser(UMI3DUser user)
        {
            if (user == null)
            {
                Debug.LogError("User cannot be null");
                return;
            }

            if (user.login == null)
            {
                Debug.LogError("User login cannot be null");
                return;
            }

            _user = user;

            Name = CapitalizeAllWord(_user.login);
            Place = $"({UMI3DCollaborationClientServer.Environement?.name})";
            _volume = 100;
            IsMute = false;
            PrimaryActions = new List<UserAction>();
            OtherActions = new List<UserAction>();

            if (user.userActions != null) 
                foreach (var action in user.userActions)
                {
                    if (action.isPrimary && PrimaryActions.Count < k_maxPrimaryAction)
                        PrimaryActions.Add(action);
                    else
                        OtherActions.Add(action);
                }

            _setNotifier[UserSocialNotificationKeys.UserSocialSet.Name] = Name;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.Place] = Place;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.Volume] = _volume;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.IsMute] = IsMute;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.PrimaryActions] = PrimaryActions;
            _setNotifier[UserSocialNotificationKeys.UserSocialSet.OtherActions] = OtherActions;
            _setNotifier.Notify();
        }

        private static string CapitalizeAllWord(string s)
        {
            if (s == null || s == string.Empty)
                return "";
            var words = s.Split(" ");
            var valueFormated = "";
            foreach (var word in words)
            {
                if (word.Length == 0)
                    continue;
                valueFormated += char.ToUpper(word[0]) + word.Substring(1) + " ";
            }
            return valueFormated.Substring(0, valueFormated.Length - 1);
        }

        /// <summary>
        /// Updates the volume for the user and adjusts the mute status accordingly.<br/>
        /// <br/>
        /// <example>
        /// Given a valid volume, when updating the volume, then the volume is updated correctly.
        /// <code>
        /// userSocial.UpdateVolume(75f);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="volume">The new volume to set.</param>
        public void UpdateVolume(float volume)
        {
            if (_user == null)
            {
                Debug.LogError("User cannot be null when updating volume.");
                return;
            }

            _volume = Mathf.Max(0, volume);
            IsMute = volume <= float.Epsilon;

            UpdateAudioManagerFor(_user, Volume);

            _updateNotifier[UserSocialNotificationKeys.UserSocialUpdate.Volume] = Volume;
            _updateNotifier[UserSocialNotificationKeys.UserSocialUpdate.IsMute] = IsMute;
            _updateNotifier.Notify();
        }

        /// <summary>
        /// Updates the mute status for the user.<br/>
        /// <br/>
        /// <example>
        /// Given a mute status, when updating the mute status, then the user's mute status is updated correctly.
        /// <code>
        /// userSocial.UpdateMute(true);
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="mute">The new mute status to set.</param>
        public void UpdateMute(bool mute)
        {
            if (_user == null)
            {
                Debug.LogError("User cannot be null when updating mute.");
                return;
            }

            IsMute = mute;

            UpdateAudioManagerFor(_user, Volume);

            _updateNotifier[UserSocialNotificationKeys.UserSocialUpdate.Volume] = Volume;
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