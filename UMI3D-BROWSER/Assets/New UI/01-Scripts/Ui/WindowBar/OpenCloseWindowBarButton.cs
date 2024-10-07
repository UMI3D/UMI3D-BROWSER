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

using UnityEngine;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.windowBar
{
    public class OpenCloseWindowBarButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private Transform WindowBarTransform;

        private void Awake()
        {
            button.onClick.AddListener(OpenOrClose);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OpenOrClose);
        }

        private void OpenOrClose()
        {
            WindowBarTransform.gameObject.SetActive(!WindowBarTransform.gameObject.activeInHierarchy);
            icon.transform.Rotate(0, 0, WindowBarTransform.gameObject.activeInHierarchy ? -180 : 180);
        }
    }
}