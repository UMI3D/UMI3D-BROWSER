using System;
using System.Collections.Generic;
using umi3dBrowsers.data.ui;
using umi3dBrowsers.services.title;
using UnityEngine;

namespace umi3dBrowsers.linker.ui
{
    [CreateAssetMenu(menuName = "Linker/Ui/Navigation")]
    public class MenuNavigationLinker : ScriptableObject
    {
        [SerializeField] private PanelData[] m_panelsData;
        [SerializeField] private PanelData m_StartPanel;
        [SerializeField] private bool m_forceLanguage;

        public event Action<bool> OnSetCancelButtonActive;
        public event Action OnReplacePlayerAndShowPanel;
        public void ReplacePlayerAndShowPanel() { OnReplacePlayerAndShowPanel?.Invoke(); }
        public event Action<PanelData, PanelTutoManager> OnPanelChanged;

        public bool ForceLanguage => m_forceLanguage;

        private Dictionary<PanelData, GameObject> m_panels;
        private PanelData m_currentPanelData;
        private PanelData m_lastPanelData;

        public void Initialize(Transform pPanelParent)
        {
            m_panels = new();
            foreach (var panelData in m_panelsData)
            {
                var panel = Instantiate(panelData.Prefab, pPanelParent);
                panel.SetActive(false);
                m_panels.Add(panelData, panel);
            }
        }

        public void ShowStartPanel()
        {
            ShowPanel(m_StartPanel);
        }

        public void ShowPanel(PanelData pPanelData)
        {
            if (pPanelData == null || m_currentPanelData == pPanelData)
                return;

            if (!m_panels.ContainsKey(pPanelData))
            {
                Debug.LogError("Trying to show a panel that's not registered!");
                return;
            }

            if (m_currentPanelData != null)
            {
                m_panels[m_currentPanelData].SetActive(false);

                if (m_currentPanelData.CanBeReturnedTo)
                    m_lastPanelData = m_currentPanelData;
            }

            m_currentPanelData = pPanelData;
            m_panels[m_currentPanelData].SetActive(true);

            OnPanelChanged?.Invoke(m_currentPanelData, m_panels[m_currentPanelData].GetComponentInChildren<PanelTutoManager>());
        }

        public void Back()
        {
            ShowPanel(m_lastPanelData);
            m_lastPanelData = null;
        }

        public void SetCancelButtonActive(bool pShow)
        {
            OnSetCancelButtonActive?.Invoke(pShow);
        }
    }
}