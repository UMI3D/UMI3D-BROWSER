using System;
using System.Collections.Generic;
using umi3dBrowsers.data.ui;
using umi3dBrowsers.displayer;
using Unity.VisualScripting;
using UnityEngine;

namespace umi3dBrowsers.linker.ui
{
    [CreateAssetMenu(menuName = "Linker/Ui/Popup")]
    public class PopupLinker : ScriptableObject
    {
        [SerializeField] private PopupData[] m_popupData;

        private Dictionary<PopupData, PopupDisplayer> m_popups;
        private PopupData m_currentPopupData;

        public event Action OnPopupOpen;
        public event Action OnPopupClose;

        public void Initialize(Transform pPopupParent)
        {
            m_popups = new();
            foreach (var popupData in m_popupData)
            {
                var popup = Instantiate(popupData.Prefab, pPopupParent).GetComponent<PopupDisplayer>();
                popup.gameObject.SetActive(false);
                popup.OnDisabled += OnPopupClose;
                m_popups.Add(popupData, popup);
            }
        }

        public void Show(PopupData pPopupData, string pTitle, string pDescription, params (string, Action)[] pButtons)
        {
            if (pPopupData == null || !m_popups.ContainsKey(pPopupData))
                return;

            if (m_currentPopupData != null)
                m_popups[m_currentPopupData].gameObject.SetActive(false);

            m_currentPopupData = pPopupData;
            var popup = m_popups[m_currentPopupData];
            popup.Title = pTitle;
            popup.Description = pDescription;
            popup.SetButtons(pButtons);

            popup.gameObject.SetActive(true);
            OnPopupOpen?.Invoke();
        }

        public void SetArguments(PopupData pPopupData, Dictionary<string, object> pArguments)
        {
            if (pPopupData == null || !m_popups.ContainsKey(pPopupData))
                return;

            m_popups[pPopupData].SetArguments(pArguments);
        }

        public void CloseAll()
        {
            foreach (var popup in m_popups.Values)
                popup.gameObject.SetActive(false);
        }
    }
}