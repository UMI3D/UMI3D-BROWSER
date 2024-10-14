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

using umi3dBrowsers.services.title;
using UnityEngine;

namespace umi3dBrowsers.data.ui
{
    [CreateAssetMenu(menuName = "Data/Ui/Panel")]
    public class PanelData : ScriptableObject
    {
        [SerializeField] private GameObject m_prefab;
        [Header("Parameters")]
        [SerializeField] private bool m_displayTop;
        [SerializeField] private bool m_displayNavbar;
        [SerializeField] private bool m_displayBack;
        [SerializeField] private bool m_canBeReturnedTo;
        [Header("Title")]
        [SerializeField] private string m_titlePrefab;
        [SerializeField] private string m_titleSuffix;
        [SerializeField] private TitleType m_titleType;

        public GameObject Prefab => m_prefab;

        public bool DisplayTop => m_displayTop;
        public bool DisplayNavbar => m_displayNavbar;
        public bool DisplayBack => m_displayBack;
        public bool CanBeReturnedTo => m_canBeReturnedTo;

        public string TitlePrefix => m_titlePrefab;
        public string TitleSuffix => m_titleSuffix;
        public TitleType TitleType => m_titleType;
    }
}