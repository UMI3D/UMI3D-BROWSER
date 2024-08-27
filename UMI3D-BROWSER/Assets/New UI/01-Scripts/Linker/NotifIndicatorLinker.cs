using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3dBrowsers.linker
{
    [CreateAssetMenu( menuName ="Linker/Notifs")]
    public class NotifIndicatorLinker : ScriptableObject
    {
        private bool _hasNotifications;
        public bool hasNotifications => _hasNotifications;
        public event Action OnHasNotifications;
        [ContextMenu("HasNotifs")]
        public void HasNotifications()
        {
            OnHasNotifications?.Invoke();
            _hasNotifications = true;
        }

        public event Action OnHasNoNotifications;
        [ContextMenu("NoNotifs")]
        public void HasNoNotifications()
        {
            OnHasNoNotifications?.Invoke();
            _hasNotifications = false;
        }
    }
}

