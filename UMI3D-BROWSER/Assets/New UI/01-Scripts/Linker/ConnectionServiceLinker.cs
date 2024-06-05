using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3dBrowsers.linker
{
    [CreateAssetMenu(menuName = "Linker/ConnectionService")]
    public class ConnectionServiceLinker : ScriptableObject
    {
        public event Action<string> OnTryToConnect;
        public void TriesToConnect(string url) { OnTryToConnect?.Invoke(url); }

        public event Action<string> OnConnectionFailure;
        public void ConnectionFailure(string url) { OnConnectionFailure?.Invoke(url); }
    }
}
