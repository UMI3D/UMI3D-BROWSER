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
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class PageTipDisplayer : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent localizeStringEvent;
    [SerializeField] private Button buttonNext;
    [SerializeField] private Button buttonDone;
    [SerializeField] private Button buttonPrevious;

    private PageTipData _pageTipData;
    private int _pageIndex;

    private RectTransform _rectTransform;

    public void Show(PageTipData pageTipData)
    {
        _rectTransform = transform as RectTransform;
        gameObject.SetActive(true);
        Init(pageTipData);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Next()
    {
        ++_pageIndex;
        GoTo();
    }
    public void Done()
    {
        Hide();
    }
    public void Previous()
    {
        --_pageIndex;
        GoTo();
    }

    private void Init(PageTipData pageTipData)
    {
        _pageTipData = pageTipData;
        _pageIndex = 0;
        GoTo();
    }

    private void GoTo()
    {
        if (_pageIndex >= _pageTipData.Count - 1)
        {
            _pageIndex = _pageTipData.Count - 1;
            buttonNext.gameObject.SetActive(false);
            buttonDone.gameObject.SetActive(true);
            buttonPrevious.gameObject.SetActive(_pageTipData.Count > 1);
        }
        else if (_pageIndex <= 0)
        {
            _pageIndex = 0;
            buttonNext.gameObject.SetActive(_pageTipData.Count > 1);
            buttonDone.gameObject.SetActive(_pageTipData.Count == 1);
            buttonPrevious.gameObject.SetActive(false);
        }
        else
        {
            buttonNext.gameObject.SetActive(true);
            buttonDone.gameObject.SetActive(false);
            buttonPrevious.gameObject.SetActive(true);
        }

        var tip = _pageTipData.GetTip(_pageIndex);
        localizeStringEvent.SetEntry(tip.LocalizedString);
        _rectTransform.anchoredPosition = tip.Position;
    }
}
