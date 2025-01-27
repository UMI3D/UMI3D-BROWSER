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

namespace umi3d.browserRuntime.ui.tablet
{
    /// <summary>
    /// To permit all screen to call it awake
    /// </summary>
    public class ScreenStart : MonoBehaviour
    {
        [SerializeField] TabletMenu _startingMenu;

        TabletModelContainer _modelContainer;

        private void Awake()
        {
            _modelContainer = GetComponentInParent<TabletModelContainer>();

            foreach (Transform screen in transform)
                screen.gameObject.SetActive(true);
        }

        private void Start()
        {
            foreach (Transform screen in transform)
                screen.gameObject.SetActive(false);

            _modelContainer.Model.SetMenu(_startingMenu);
        }
    }
}