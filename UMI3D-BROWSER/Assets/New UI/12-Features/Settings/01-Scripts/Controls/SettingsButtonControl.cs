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
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings
{
    internal class SettingsButtonControl : MonoBehaviour
    {
        [SerializeField] Color evenColor;
        [SerializeField] Color oddColor;

        Button button;
        int instanceID;

        void Awake()
        {
            button = GetComponent<Button>();

            instanceID = GetComponentInParent<SettingsContent>().GetInstanceID();

            NotificationHub.Default.Subscribe(
                this,
                SettingsNotificationKeys.UpdateChildVisibilitySelected + instanceID,
                (Callback)SetColor
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
            var color = button.colors;
            color.normalColor = evenColor;
            button.colors = color;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(button);
#endif
        }

        [ContextMenu("Set Odd Color")]
        void SetOddColor()
        {
            var color = button.colors;
            color.normalColor = oddColor;
            button.colors = color;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(button);
#endif
        }

        [ContextMenu("Set Color")]
        public void SetColor()
        {
            // Global index
            int superSiblingIndex = transform.parent.parent.GetSiblingIndex();
            Transform superParent = transform.parent.parent.parent;
            int superIndex = 0;
            for (int i = 0; i < superSiblingIndex; i++)
            {
                if (superParent.GetChild(i).gameObject.activeSelf)
                {
                    superIndex++;
                }
            }

            // Local index
            int siblingIndex = transform.GetSiblingIndex();
            Transform parent = transform.parent;
            int index = 0;
            for (int i = 0; i < siblingIndex; i++)
            {
                if (parent.GetChild(i).gameObject.activeSelf)
                {
                    index++;
                }
            }

            var color = button.colors;
            color.normalColor = superIndex % 2 == 0 ? (index % 2 == 0 ? oddColor : evenColor) : (index % 2 == 0 ? evenColor : oddColor);
            button.colors = color;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(button);
#endif
        }
    }
}