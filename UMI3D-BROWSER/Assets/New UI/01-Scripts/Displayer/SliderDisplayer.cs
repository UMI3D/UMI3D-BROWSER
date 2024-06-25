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
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderDisplayer : MonoBehaviour, IDisplayer
{
    [SerializeField] private TMP_Text m_Label;
    [SerializeField] private Slider m_Slider;

    public Slider Slider => m_Slider;

    public object GetValue(bool trim)
    {
        return m_Slider.value;
    }

    public void SetColor(Color color)
    {
    }

    public void SetPlaceHolder(List<string> placeHolder)
    {
    }

    public void SetResource(object resource)
    {
    }

    public void SetTitle(string title)
    {
        m_Label.text = title;
    }
}
