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

public class PanelTuto : MonoBehaviour
{
    [SerializeField] private string localisedKey;
    [SerializeField] private Vector2 tutoPosition;

    [SerializeField] private List<PanelTuto> sameTutos;
    [SerializeField] private bool isPrimary = true;

    public string LocalisedKey => localisedKey;
    public Vector2 TutoPosition => tutoPosition;
    public bool IsPrimary => isPrimary;

    public void ShowElementOverlay()
    {
        gameObject.SetActive(true);
        foreach (var tuto in sameTutos)
        {
            tuto.ShowElementOverlay();
        }
    }

    public void HideElementOverlay() 
    { 
        gameObject.SetActive(false);
        foreach (var tuto in sameTutos)
        {
            tuto.HideElementOverlay();
        }
    }

    internal void Set(string localisedKey, Vector2 tutoPosition)
    {
        this.localisedKey = localisedKey;
        this.tutoPosition = tutoPosition;
    }
}
