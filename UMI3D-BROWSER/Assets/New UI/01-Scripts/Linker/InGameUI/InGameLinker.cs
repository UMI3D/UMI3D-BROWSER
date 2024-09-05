using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3dBrowsers.linker.ingameui
{
    [CreateAssetMenu(menuName = "Linker/InGame")]
    public class InGameLinker : ScriptableObject
    {
        public event Action<bool> OnEnableDisableInGameUI;
        public void EnableDisableInGameUI(bool isEnable) 
        { 
            OnEnableDisableInGameUI?.Invoke(isEnable); 
            this.isEnable = isEnable;
        }
        private bool isEnable;
        public bool IsEnable => isEnable;
    }
}

