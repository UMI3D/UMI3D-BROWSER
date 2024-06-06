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

        private Dictionary<PanelData, GameObject> m_panels;
        private PanelData m_currentPanelData;

        private GameObject m_top;
        private TitleManager m_titleManager;
        private GameObject m_navBar;

        public void Initialize(Transform pPanelParent, GameObject pTop, TitleManager pTitleManager, GameObject pNavBar)
        {
            m_top = pTop;
            m_titleManager = pTitleManager;
            m_navBar = pNavBar;

            foreach (var panelData in m_panelsData)
                m_panels.Add(panelData, Instantiate(panelData.Prefab, pPanelParent));

            ShowPanel(m_StartPanel);
        }

        public void ShowPanel(PanelData pPanelData)
        {
            if (m_currentPanelData == pPanelData)

            if (m_panelsData != null)
                m_panels[m_currentPanelData].SetActive(false);

            if (!m_panels.ContainsKey(pPanelData))
            {
                Debug.LogError("Trying to show a panel that's not registered!");
                return;
            }

            m_currentPanelData = pPanelData;
            m_panels[m_currentPanelData].SetActive(true);

            m_top.SetActive(m_currentPanelData.DisplayTop);
            m_titleManager.SetTitle(m_currentPanelData.TitleType, m_currentPanelData.TitlePrefix, m_currentPanelData.TitleSuffix);
            m_navBar.SetActive(m_currentPanelData.DisplayNavbar);
        }
    }
}