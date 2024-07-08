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
        [SerializeField] private bool m_displayCancel;
        [SerializeField] private bool m_canBeReturnedTo;
        [Header("Title")]
        [SerializeField] private string m_titlePrefab;
        [SerializeField] private string m_titleSuffix;
        [SerializeField] private TitleType m_titleType;

        public GameObject Prefab => m_prefab;

        public bool DisplayTop => m_displayTop;
        public bool DisplayNavbar => m_displayNavbar;
        public bool DisplayBack => m_displayBack;
        public bool DisplayCancel => m_displayCancel;
        public bool CanBeReturnedTo => m_canBeReturnedTo;

        public string TitlePrefix => m_titlePrefab;
        public string TitleSuffix => m_titleSuffix;
        public TitleType TitleType => m_titleType;
    }
}