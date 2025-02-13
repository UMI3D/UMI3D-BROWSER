/*
Copyright 2019 - 2025 Inetum

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

namespace umi3d.browserRuntime.ui.tablet
{
    [RequireComponent(typeof(Button))]
    public class TabletMenuButton : MonoBehaviour
    {
        [SerializeField] TabletMenu _menu;

        Button _button;
        TabletModelContainer _modelContainer;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);

            _modelContainer = GetComponentInParent<TabletModelContainer>();
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        void OnClick()
        {
            _modelContainer.Model.UpdateMenu(_menu);
            NotificationHub.Default.Notify(this, ID.FromType<TabletNotificationKeys.Open>());
        }
    }
}