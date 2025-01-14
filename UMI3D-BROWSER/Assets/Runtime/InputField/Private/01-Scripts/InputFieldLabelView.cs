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
using TMPro;
using UnityEngine;

namespace umi3d.browserRuntime.ui.inputField
{
    [RequireComponent(typeof(TMP_Text))]
    public class InputFieldLabelView : MonoBehaviour
    {
        TMP_Text _text;

        InputFieldModelContainer _modelContainer;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _modelContainer = GetComponentInParent<InputFieldModelContainer>();

            NotificationHub.Default.Subscribe(this,
                ID.FromType<InputFieldNotificationsKeys.InputFieldSet>(), 
                (Callback)TitleSet,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == _modelContainer.model));
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        private void TitleSet(Notification notification)
        {
            if (!notification.TryGetInfoT(InputFieldNotificationsKeys.InputFieldSet.IsLabelVisible, out bool isActive))
            {
                gameObject.SetActive(isActive);
                if (!isActive)
                    return;
            }

            if (!notification.TryGetInfoT(InputFieldNotificationsKeys.InputFieldSet.Label, out string newTitle))
            {
                _text.text = newTitle;
            }
        }
    }
}