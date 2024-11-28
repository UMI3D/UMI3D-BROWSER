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

using inetum.unityUtils;
using UnityEngine;

namespace umi3d.browserRuntime.ui.thumbnails
{
    [RequireComponent(typeof(TMPro.TMP_InputField))]
    internal class ThumbnailInputField : MonoBehaviour
    {
        TMPro.TMP_InputField inputField;

        ThumbnailModelContainer model;

        void Awake()
        {
            inputField = GetComponent<TMPro.TMP_InputField>();
            inputField.onValueChanged.AddListener(OnValueChanged);

            model = GetComponentInParent<ThumbnailModelContainer>();

            NotificationHub.Default.Subscribe<ThumbnailsNotificationKeys.NameSet>(
                this,
                new FilterByCondition(FilterType.AcceptOnly, publisher => publisher == model.model),
                NameSet
            );
        }

        void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);
        }

        void OnValueChanged(string value)
        {
            model.model.UpdateName(value);
        }

        void NameSet(Notification notification)
        {
            if (!notification.TryGetInfoT(ThumbnailsNotificationKeys.NameSet.Name, out string name))
            {
                return;
            }

            inputField.SetTextWithoutNotify(name);
        }
    }
}