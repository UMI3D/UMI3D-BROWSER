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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.ui.settings
{
    public class SettingsContent : MonoBehaviour
    {
        void Awake()
        {
            SetContentsTransform();
        }

        [ContextMenu("Set Size")]
        void SetContentsTransform()
        {
            RectTransform rt = transform.GetComponent<RectTransform>();

            rt.offsetMin = new Vector2(0f, 0f);
            rt.offsetMax = new Vector2(0f, -64f);


#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(rt);
#endif

            for (int i = 0; i < transform.childCount; i++)
            {
                rt = transform.GetChild(i).GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(1010f, 49f);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(rt);
#endif
            }

        }
    }
}