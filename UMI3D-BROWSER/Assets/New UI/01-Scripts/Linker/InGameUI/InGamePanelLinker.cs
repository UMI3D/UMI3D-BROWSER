using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3dBrowsers.linker.ingameui
{
    [CreateAssetMenu(menuName = "Linker/InGamePanel")]
    public class InGamePanelLinker : ScriptableObject
    {
        public event Action OnOpenClosePanel;
        public void OpenClosePanel() { OnOpenClosePanel?.Invoke(); }
            
    }
}
