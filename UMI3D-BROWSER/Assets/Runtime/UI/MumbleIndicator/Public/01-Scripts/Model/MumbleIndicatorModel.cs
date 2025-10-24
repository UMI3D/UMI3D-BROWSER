/*
Copyright 2019 - 2025 Inetum

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

namespace umi3d.browserRuntime.ui
{
    public class MumbleIndicatorModel : IMumbleIndicatorConnectedSubject
    {
        bool _isConnected = false;

        List<IMumbleIndicatorConnectedObserver> _connectedObserver = new();

        public void Subscribe(IMumbleIndicatorConnectedObserver observer)
        {
            if (!_connectedObserver.Contains(observer))
                _connectedObserver.Add(observer);
        }

        public void Unsubscribe(IMumbleIndicatorConnectedObserver observer)
        {
            if (_connectedObserver.Contains(observer))
                _connectedObserver.Remove(observer);
        }

        private void NotifyConnectedObservers()
        {
            foreach (var observer in _connectedObserver)
            {
                try
                {
                    observer.UpdateConnected(_isConnected);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        public void SetIsConnected(bool isConnected)
        {
            _isConnected = isConnected;
            NotifyConnectedObservers();
        }
    }
}