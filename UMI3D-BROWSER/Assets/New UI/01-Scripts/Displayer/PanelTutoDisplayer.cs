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
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class PanelTutoDisplayer : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent localizeStringEvent;
    [SerializeField] private Button buttonNext;
    [SerializeField] private Button buttonDone;
    [SerializeField] private Button buttonPrevious;

    private PanelTutoManager _currentPanelTutoManager;
    private PanelTuto _currentPanelTuto;
    private int _tutoIndex;

    private RectTransform _rectTransform;

    public void Show(PanelTutoManager pageTipDataManager)
    {
        _rectTransform = transform as RectTransform;
        gameObject.SetActive(true);
        Init(pageTipDataManager);
    }
    public void Hide()
    {
        _currentPanelTuto?.HideElementOverlay();
        gameObject.SetActive(false);
    }

    public void Next()
    {
        ++_tutoIndex;
        GoTo();
    }
    public void Done()
    {
        Hide();
    }
    public void Previous()
    {
        --_tutoIndex;
        GoTo();
    }

    private void Init(PanelTutoManager pageTipDataManager)
    {
        _currentPanelTutoManager = pageTipDataManager;
        _tutoIndex = 0;
        GoTo();
    }

    private void GoTo()
    {
        _currentPanelTuto?.HideElementOverlay();

        if (_tutoIndex >= _currentPanelTutoManager.Count - 1)
        {
            _tutoIndex = _currentPanelTutoManager.Count - 1;
            buttonNext.gameObject.SetActive(false);
            buttonDone.gameObject.SetActive(true);
            buttonPrevious.gameObject.SetActive(_currentPanelTutoManager.Count > 1);
        }
        else if (_tutoIndex <= 0)
        {
            _tutoIndex = 0;
            buttonNext.gameObject.SetActive(_currentPanelTutoManager.Count > 1);
            buttonDone.gameObject.SetActive(_currentPanelTutoManager.Count == 1);
            buttonPrevious.gameObject.SetActive(false);
        }
        else
        {
            buttonNext.gameObject.SetActive(true);
            buttonDone.gameObject.SetActive(false);
            buttonPrevious.gameObject.SetActive(true);
        }
        EventSystem.current.SetSelectedGameObject(null);

        _currentPanelTuto = _currentPanelTutoManager.GetTuto(_tutoIndex);
        localizeStringEvent.SetEntry(_currentPanelTuto.LocalisedKey);
        _rectTransform.anchoredPosition = _currentPanelTuto.TutoPosition;
        _currentPanelTuto.ShowElementOverlay();
    }
}
