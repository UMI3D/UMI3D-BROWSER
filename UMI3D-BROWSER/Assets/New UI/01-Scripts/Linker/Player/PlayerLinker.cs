using System;
using System.Collections;
using System.Collections.Generic;
using umi3d.browserRuntime.player;
using UnityEngine;

namespace umi3dBrowsers.linker
{
    [CreateAssetMenu(menuName = "Linker/Player")]
    public class PlayerLinker : ScriptableObject
    {
        public event Action<UMI3DVRPlayer> OnPlayerReady;
        public void PlayerReady(UMI3DVRPlayer player) { OnPlayerReady?.Invoke(player); }
    }
}

