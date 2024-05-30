using System;
using System.Collections;
using System.Collections.Generic;
using umi3dVRBrowsersBase.ui;
using UnityEngine;

namespace umi3dBrowsers.linker
{
    [CreateAssetMenu(menuName = "Linker/Sim")]
    public class SIM_Linker : ScriptableObject
    {
        public event Action<SelectedInteractableManager> OnSimReady;
        public void SimReady(SelectedInteractableManager sim) { OnSimReady?.Invoke(sim); }
    }
}

