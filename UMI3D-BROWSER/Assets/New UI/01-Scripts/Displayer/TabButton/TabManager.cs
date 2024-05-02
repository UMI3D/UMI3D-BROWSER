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
        [SerializeField] private Transform navigationRoot;
        [SerializeField] private GameObject tabButtonPrefab;
        [SerializeField] private GameObject containerPrefab;
        [SerializeField] private GameObject paramFormContainerPrefab;
        [SerializeField, Tooltip("To bind existing tabs ith existing container")] 
        private List<TabToContainerBinder> tabs;
        [Header("Content")]
        [SerializeField] private Transform contentRoot;

        public TabToContainerBinder currentActiveButton;

        [Header("Option")]
        [SerializeField, Tooltip("Enables the binding of the tabs on start : [Don't use if you are creating dynamic panel tab menu]")] 
        private bool autoInit = true;

        private void Start()
        {
            if (!autoInit) return;
            foreach(var tab in tabs)
            {
                tab.Bind();
                tab.OnSelectionChanged += (tabBinder) =>
                {
                    currentActiveButton = tabBinder;
                    UpdateTabsVisual();
                };
            }
            InitSelectedButtonById();
        }

        public void InitSelectedButtonById(int id = 0)
        {
            foreach (var tab in tabs)
            {
                tab.SetActive(false);
            }

            tabs[id].SetActive(true);
            currentActiveButton = tabs[id];
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

        /// <summary>
        /// Add A new tabs to the tab manager and its coresponding container
        /// </summary>
        /// <param name="label">The name of the new tab</param>
        /// <param name="container">the container you wish to associate with the new tab</param>
        /// <param name="useLocalization">Should the name of the that depend on the localization</param>
        /// <returns>The container of the tab</returns>
        public GameObject AddNewTab(string label, bool useLocalization = false, GameObject container = null)
        {
            TabToContainerBinder tabBinder = new TabToContainerBinder();

            Tab tabButton = Instantiate(tabButtonPrefab, navigationRoot).GetComponent<Tab>();
            tabButton.SetLabel(label, useLocalization);
            GameObject tab = container;

            if (tab == null) // Instantiate a container if none are given
                tab = Instantiate(containerPrefab, contentRoot);

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

            return tab;
        }

        public GameObject AddNewTabForParamForm(string label)
        {
            GameObject tabContainer = Instantiate(paramFormContainerPrefab, contentRoot);
            return AddNewTab(label, true, tabContainer);
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

