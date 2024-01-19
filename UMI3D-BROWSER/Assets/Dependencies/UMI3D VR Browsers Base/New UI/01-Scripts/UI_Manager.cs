using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ui
{
    public class UI_Manager : MonoBehaviour
    {
        [SerializeField] PanelEnum currentScreen;
        public PanelEnum CurrentScreen => currentScreen;
    }
}
