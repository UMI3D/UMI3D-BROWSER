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

using System;
using System.Collections.Generic;
using UnityEngine;
using umi3dBrowsers.displayer;
using System.ComponentModel;

namespace umi3dBrowsers.container
{
    public class TabManager : MonoBehaviour
    {
        [Header("Tabs")]
        [SerializeField] private GameObject tabButtonPrefab;
        [SerializeField] private List<TabToContainerBinder> tabs;
        [Header("Content")]
        [SerializeField] private Transform contentRoot;

        public TabToContainerBinder currentActiveButton;

        private void Start()
        {
            foreach(var tab in tabs)
            {
                tab.Bind();
                tab.OnSelectionChanged += (tabBinder) =>
                {
                    currentActiveButton = tabBinder;
                    UpdateTabsVisual();
                };
            }
            InitSelectedButton();
        }

        private void InitSelectedButton()
        {
            foreach (var tab in tabs)
            {
                tab.SetActive(false);
            }

            tabs[0].SetActive(true);
            currentActiveButton = tabs[0];
        }

        private void UpdateTabsVisual()
        {
            foreach(var tab in tabs)
            {
                bool activate = tab == currentActiveButton;
                tab.SetActive(activate);
            }
        }

        public void Clear()
        {
            foreach(var tab in tabs)
            {
                tab.Clear();
            }
            tabs = null;
        }

        public int AddNewTab(string label)
        {
            TabToContainerBinder tabBinder = new TabToContainerBinder();

            Tab tabButton = Instantiate(tabButtonPrefab, transform).GetComponent<Tab>();
            tabButton.SetLabel(label);
            GameObject tab = new GameObject(label + "Container");
            tab.transform.SetParent(contentRoot);

            tabBinder.SetTabButton(tabButton);
            tabBinder.SetTabContainer(tab);
            tabBinder.Bind();
            tabBinder.OnSelectionChanged += (tabBinder) =>
            {
                currentActiveButton = tabBinder;
                UpdateTabsVisual();
            };

            if (tabs == null)
            {
                tabs = new();
                currentActiveButton = tabBinder;
                tabBinder.SetActive(true);
            }
            tabs.Add(tabBinder);

            return tabs.Count - 1;
        }

        public Transform GetTabContainerById(int id)
        {
            return tabs[id].TabTransform;
        }

        [Serializable]
        public class TabToContainerBinder
        {
            [SerializeField] private Tab tabButton;
            [SerializeField] private GameObject tab;
            public Transform TabTransform => tab.transform;
            public void SetTabContainer(GameObject tab)
            {
                this.tab = tab;
            }

            public void SetTabButton(Tab tabButton)
            {
                this.tabButton = tabButton;
            }

            public event Action<TabToContainerBinder> OnSelectionChanged;

            internal void Bind()
            {
                tabButton.OnClick.AddListener(() =>
                {
                    OnSelectionChanged?.Invoke(this);
                });
            }

            public void SetActive(bool isActive)
            {
                if (isActive)
                {
                    tabButton.label.color = tabButton.selectedLabelColor;
                    tabButton.hoverBar.color = new Color(tabButton.hoverBar.color.r, tabButton.hoverBar.color.g, tabButton.hoverBar.color.b, 1);
                }
                else
                {
                    tabButton.label.color = tabButton.labelBaseColor;
                    tabButton.hoverBar.color = new Color(tabButton.hoverBar.color.r, tabButton.hoverBar.color.g, tabButton.hoverBar.color.b, 0);
                }

                tab.SetActive(isActive);
                tabButton.isSelected = isActive;
            }

            internal void Clear()
            {
                DestroyImmediate(tabButton.gameObject);
                DestroyImmediate(tab);
            }
        }
    }
}

