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
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings
{
    internal class SettingsLabelControl : MonoBehaviour
    {
        [SerializeField] Color evenColor;
        [SerializeField] Color oddColor;

        Image image;
        int instanceID;

        void Awake()
        {
            image = GetComponent<Image>();

            instanceID = GetComponentInParent<SettingsContent>().GetInstanceID();

            NotificationHub.Default.Subscribe(
                this,
                SettingsNotificationKeys.UpdateChildVisibilitySelected + instanceID,
                SetColor
            );

            SetColor();
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this, SettingsNotificationKeys.UpdateChildVisibilitySelected + instanceID);
        }

        [ContextMenu("Set Even Color")]
        void SetEvenColor()
        {
            image.color = evenColor;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(image);
#endif
        }

        [ContextMenu("Set Odd Color")]
        void SetOddColor()
        {
            image.color = oddColor;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(image);
#endif
        }

        [ContextMenu("Set Color")]
        public void SetColor()
        {
            int siblingIndex = transform.parent.GetSiblingIndex();
            Transform parent = transform.parent.parent;
            int index = 0;
            for (int i = 0; i < siblingIndex; i++)
            {
                if (parent.GetChild(i).gameObject.activeSelf)
                {
                    index++;
                }
            }

            image.color = index % 2 == 0 ? oddColor : evenColor;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(image);
#endif
        }
    }
}