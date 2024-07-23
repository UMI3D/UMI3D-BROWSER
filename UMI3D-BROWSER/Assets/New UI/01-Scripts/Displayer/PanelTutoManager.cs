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
using System.Linq;
using UnityEngine;

public class PanelTutoManager : MonoBehaviour
{
    [SerializeField] private List<PanelTuto> lstPanelTuto;

    public PanelTuto GetTuto(int index) => lstPanelTuto[index];
    public int Count => lstPanelTuto.Count;

    protected void Awake()
    {
        foreach (var tuto in lstPanelTuto)
            tuto.HideElementOverlay();
    }

#if UNITY_EDITOR
    [ContextMenu("Search for panel tuto")]
    private void SearchPanelTuto()
    {
        if (lstPanelTuto == null)
            lstPanelTuto = new List<PanelTuto>();

        var lst = gameObject.GetComponentsInChildren<PanelTuto>();

        foreach (var panelTuto in lst)
        {
            if (panelTuto.IsPrimary)
                if (!lstPanelTuto.Any(tuto => tuto.LocalisedKey == panelTuto.LocalisedKey))
                    lstPanelTuto.Add(panelTuto);
        }
    }
#endif
}
