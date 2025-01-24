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
using System.Linq;
using TMPro;
using umi3d.baseBrowser.extension;
using umi3d.cdk.collaboration;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.social
{

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
        [HideInInspector] public RectTransform nonPrimaryActionContainer;
        [SerializeField] private GameObject actionIconPrefab;
        [SerializeField] private GameObject nonPrimaryActionPrefab;
        [SerializeField] private Toggle actionButton;
        [SerializeField] private TMP_Dropdown dropdown;

        public struct UserData
        {
            public bool isMute;
            public float volume;
        }

        public UserData Data => new() { isMute = IsMute, volume = UserVolume };

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

        public ToggleGroup ToggleGroup
        {
            get => actionButton.group;
            set => actionButton.group = value;
        }

        private UMI3DUser _user;
        private float _volume = 100;
        private bool _isMute = false;

        private const float logBase = 1.5f;
        private const float factor = 5f / 2f;
        private const float factor2 = 5f / 2f;

        List<PrimaryUserActionIcon> actionIcons = new();
        List<OptionUserActionIcon> actions = new();

        private void Awake()
        {
            volumeSlider.minValue = 0;
            volumeSlider.maxValue = 300;
            volumeSlider.onValueChanged.AddListener(newValue => {
                UserVolume = newValue;
            });

            dropdown.onValueChanged.AddListener(DropDown_OnValueChanged);
            actionButton.onValueChanged.AddListener(DisplayAction);
            UMI3DUser.OnUserActionsUpdated.AddListener(OnUserActionsUpdated);
        }

        private void Update()
        {
            if(isDisplayingAction)
            {
                Vector3[] v = new Vector3[4];
                (this.transform as RectTransform).GetWorldCorners(v);
                nonPrimaryActionContainer.position = v[3];
            }
        }
        private void OnDisable()
        {
            if (isDisplayingAction)
            {
                DisplayAction(false);
            }
        }

        bool isDisplayingAction = false;

        private void DisplayAction(bool display)
        {
            if (display)
            {
                foreach (Transform child in nonPrimaryActionContainer.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                float size = 0;
                
                foreach(var action in actions)
                {
                    var rect = Instantiate(nonPrimaryActionPrefab, nonPrimaryActionContainer).transform as RectTransform;
                    action.SetButton(rect.gameObject);
                    size += rect.sizeDelta.y;
                }

                Vector3[] v = new Vector3[4];
                (this.transform as RectTransform).GetWorldCorners(v);

                nonPrimaryActionContainer.position = v[3];//new Vector3(this.transform.position.x - nonPrimaryActionContainer.sizeDelta.x, this.transform.position.y );
                nonPrimaryActionContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
            }
            else
                foreach (Transform child in nonPrimaryActionContainer.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            isDisplayingAction = display;
            actionButton.SetIsOnWithoutNotify(display);
            nonPrimaryActionContainer.gameObject.SetActive(display);
        }

        private void DropDown_OnValueChanged(int value)
        {
            value -= 1;
            if (actions.Count < value && value >= 0)
                actions[value].Call();

            dropdown.SetValueWithoutNotify(0);
        }

        private void OnDestroy()
        {
            dropdown.onValueChanged.RemoveListener(DropDown_OnValueChanged);
            UMI3DUser.OnUserActionsUpdated.RemoveListener(OnUserActionsUpdated);
        }

        private async void OnUserActionsUpdated(UMI3DUser user)
        {
            if (user != this.User)
                return;

            foreach (PrimaryUserActionIcon item in actionIcons)
                item.Destroy();
            actionIcons.Clear();

            foreach (OptionUserActionIcon item in actions)
                item.Destroy();
            actions.Clear();

            int PrimaryCount = MAX_ACTION_ICON;

            dropdown.ClearOptions();
            foreach (cdk.collaboration.UserAction action in user.userActions.ToList())
            {
                if (PrimaryCount > 0 && action.isPrimary)
                {
                    PrimaryCount--;
                    PrimaryUserActionIcon displayer = await PrimaryUserActionIcon.Create(Instantiate(this.actionIconPrefab, actionContainer), action);
                    actionIcons.Add(displayer);
                }
                else
                {
                    var d = await OptionUserActionIcon.Create(action);
                    actions.Add(d);
                }
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