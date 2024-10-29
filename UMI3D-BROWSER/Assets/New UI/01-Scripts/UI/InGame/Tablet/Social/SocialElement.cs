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

using System;
using System.Collections.Generic;
using TMPro;
using umi3d.baseBrowser.extension;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.inGame.tablet.social
{
    public class UserActionIcon : UserAction
    {
        public GameObject gameObject { get; private set; }
        private Button button;

        public UserActionIcon(GameObject gameObject, cdk.collaboration.UserAction action) : base(action)
        {
            this.gameObject = gameObject;
            button = this.gameObject.GetComponent<Button>();
            button?.onClick.AddListener(Call);
        }

        public override void Destroy() 
        {
            button?.onClick.RemoveListener(Call);
            button = null;
            GameObject.Destroy(gameObject);
            base.Destroy();
        }
    }

    public class UserAction
    {
        public cdk.collaboration.UserAction action { get; private set; }

        public UserAction(cdk.collaboration.UserAction action)
        {
            this.action = action;
        }

        public virtual void Destroy() { }

        public void Call() => this.action?.Call();
    }

    public class SocialElement : MonoBehaviour
    {
        const int MAX_ACTION_ICON = 3;

        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text placeText;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Image volumeImage;
        [SerializeField] private Button muteButton;
        [SerializeField] private Sprite volumeSprite;
        [SerializeField] private Sprite volumeMuteSprite;
        [SerializeField] private Transform actionContainer;
        [SerializeField] private GameObject actionIconPrefab;
        [SerializeField] private Button actionButton;

        public UMI3DUser User
        {
            get => _user;
            set {
                _user = value;
                UpdateUser();
            }
        }
        public string UserName => _user?.login;
        public float UserVolume
        {
            get => _volume;
            set {
                _volume = value;
                if (value > 0)
                    IsMute = false;
                SetVolume();
                UpdateDisplayVolume();
            }
        }

        public bool IsMute
        {
            get => _isMute || _volume <= 0;
            set {
                _isMute = value;
                if (!value && _volume <= 0)
                    _volume = 100;
                SetVolume();
                UpdateDisplayVolume();
            }
        }

        public bool MicroOpen => _user.microphoneStatus;

        private UMI3DUser _user;
        private float _volume = 100;
        private bool _isMute = false;

        private const float logBase = 1.5f;
        private const float factor = 5f / 2f;
        private const float factor2 = 5f / 2f;

        List<UserActionIcon> actionIcons = new();
        List<UserAction> actions = new();

        private void Awake()
        {
            volumeSlider.minValue = 0;
            volumeSlider.maxValue = 100;
            volumeSlider.onValueChanged.AddListener(newValue => {
                UserVolume = newValue;
            });

            UMI3DUser.OnUserActionsUpdated.AddListener(OnUserActionsUpdated);
        }

        private void OnDestroy()
        {
            UMI3DUser.OnUserActionsUpdated.RemoveListener(OnUserActionsUpdated);
        }

        private void OnUserActionsUpdated(UMI3DUser user)
        {
            UnityEngine.Debug.Log($"OnUserActionsUpdated {user != this.User}");
            if (user != this.User)
                return;

            foreach (UserAction item in actionIcons)
                item.Destroy();
            actionIcons.Clear();

            foreach (UserAction item in actions)
                item.Destroy();
            actions.Clear();

            int PrimaryCount = MAX_ACTION_ICON;
            foreach (cdk.collaboration.UserAction action in user.userActions)
            {
                if (PrimaryCount > 0 && action.isPrimary)
                {
                    PrimaryCount--;
                    var icon = Instantiate(this.actionIconPrefab, actionContainer);
                    actionIcons.Add(new(icon, action));
                    UnityEngine.Debug.Log($"Add icon {action.name}");
                    continue;
                }

                UnityEngine.Debug.Log($"Add {action.name}");
                actions.Add(new(action));
            }

            actionButton.gameObject.SetActive(actions.Count > 0);

        }

        private void UpdateUser()
        {
            nameText.text = UserName.CapitalizeAllWord();
            placeText.text = $"({UMI3DCollaborationClientServer.Environement?.name})";

            OnUserActionsUpdated(this.User);
        }

        public void ToggleMute()
        {
            IsMute = !IsMute;
        }

        private void SetVolume()
        {
            var volume = IsMute ? 0 : _volume;
            volumeSlider.SetValueWithoutNotify(volume);

            var vg = UserVolumeToVG(volume);
            AudioManager.Instance.SetGainForUser(_user, vg.gain);
            AudioManager.Instance.SetVolumeForUser(_user, vg.volume);
        }

        private void UpdateDisplayVolume()
        {
            volumeImage.sprite = IsMute ? volumeMuteSprite : volumeSprite;
        }

        private (float volume, float gain) UserVolumeToVG(float volume)
        {
            if (volume <= 100f)
                return (volume / 100f, 1);
            else
                return (1, GainFactor(volume / 100));
        }
        private float GainFactor(float gain) { return (Mathf.Pow(logBase, (gain - 1) * factor) - 1) * factor2 + 1; }
    }
}