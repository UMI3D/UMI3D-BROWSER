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

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.settings
{
    public class SettingsPanel : MonoBehaviour
    {
        Transform tabs;
        Transform contents;
        Transform templates;

        void Awake()
        {
            tabs = transform.GetChild(0);
            contents = transform.GetChild(1);
            templates = transform.GetChild(2);

            SetTemplates();
        }

        void SetTemplates()
        {
            for (int i = 0; i < templates.childCount; i++)
            {
                Transform panel = templates.GetChild(i);
                panel.gameObject.SetActive(true);
                Transform tab = panel.GetChild(0);
                Transform content = panel.GetChild(1);

                tab.SetParent(tabs, true);
                content.SetParent(contents, true);

                tab.name = panel.name;
                content.name = panel.name;

                ApplySettings(i, tab, content);
            }
        }

        async void ApplySettings(int i, Transform tab, Transform content)
        {
            await Task.Yield();

            if (i != 0)
            {
                content.gameObject.SetActive(false);
            }
            else
            {
                tab.GetComponent<Button>().onClick?.Invoke();
            }
        }
    }
}