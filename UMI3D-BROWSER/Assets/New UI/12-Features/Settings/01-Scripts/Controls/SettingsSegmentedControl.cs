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
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings
{
    [RequireComponent(typeof(Button))]
    internal class SettingsSegmentedControl : MonoBehaviour
    {
        Button button;
        Image background;
        TMPro.TMP_Text text;

        string activeText = null;
        string inactiveText = null;

        internal bool isActive { get; private set; }

        
        [SerializeField] private LocalizeStringEvent IsActiveEvent;
        [Tooltip("If this field is not set, the IsActiveEvent is used")]
        [SerializeField] private LocalizeStringEvent IsInactiveEvent;

        GameObject parent;
        int instanceID;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);

            background = transform.GetChild(0).GetComponent<Image>();

            parent = transform.parent.gameObject;
            instanceID = parent.GetInstanceID();

            text = transform.GetChild(1).GetComponent<TMPro.TMP_Text>();

            IsActiveEvent?.OnUpdateString.AddListener(UpdateIsActiveString);
            IsInactiveEvent?.OnUpdateString.AddListener(UpdateIsInactiveString);

            NotificationHub.Default.Subscribe(
                this, 
                SettingsNotificationKeys.NewToggleCustomSelected + instanceID, 
                Deactivate
            );
        }

        private void OnEnable()
        {
            IsActiveEvent?.RefreshString();
            IsInactiveEvent?.RefreshString();
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this, SettingsNotificationKeys.NewToggleCustomSelected + instanceID);

            IsActiveEvent?.OnUpdateString.RemoveListener(UpdateIsActiveString);
            IsInactiveEvent?.OnUpdateString.RemoveListener(UpdateIsInactiveString);
        }

        void Click()
        {
            NotificationHub.Default.Notify(this, SettingsNotificationKeys.NewToggleCustomSelected + instanceID);
            Activate();
        }

        void Activate()
        {
            isActive = true;
            background.gameObject.SetActive(true);
            if (text && activeText != null)
                text.text = activeText;
        }

        void Deactivate()
        {
            isActive = false;
            background.gameObject.SetActive(false);
            if (text && inactiveText != null)
                text.text = inactiveText;
        }


        private void UpdateIsActiveString(string s)
        {
            activeText = s;
            if(text && isActive)
                text.text = activeText;

            if (IsInactiveEvent == null)
                UpdateIsInactiveString(s);
        }

        private void UpdateIsInactiveString(string s)
        {
            inactiveText = s;
            if (text && !isActive)
                text.text = inactiveText;
        }
    }
}
