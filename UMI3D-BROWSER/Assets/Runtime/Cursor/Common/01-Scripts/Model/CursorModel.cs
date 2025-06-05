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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.browserRuntime.cursor
{
    public abstract class CursorModel : ICursorLockModeSubject, ICursorVisibilitySubject, ICursorInteractingStateSubject
    {
        public CursorLockMode lockMode { get; protected set; }
        public bool isVisible {  get; protected set; }
        public CursorInteractingState interactingState { get; protected set; }

        #region Subject

        List<ICursorLockModeObserver> _lockModeObservers = new();

        public void Subscribe(ICursorLockModeObserver observer)
        {
            if (!_lockModeObservers.Contains(observer))
            {
                _lockModeObservers.Add(observer);
            }
        }

        public void Unsubscribe(ICursorLockModeObserver observer)
        {
            _lockModeObservers.Remove(observer);
        }

        protected void NotifyLockModeObserver()
        {
            foreach (var observer in _lockModeObservers)
            {
                try
                {
                    observer.UpdateLockMode(lockMode);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        List<ICursorVisibilityObserver> _visibilityObservers = new();

        public void Subscribe(ICursorVisibilityObserver observer)
        {
            if (!_visibilityObservers.Contains(observer))
            {
                _visibilityObservers.Add(observer);
            }
        }

        public void Unsubscribe(ICursorVisibilityObserver observer)
        {
            _visibilityObservers.Remove(observer);
        }

        protected void NotifyVisibilityObserver()
        {
            foreach (var observer in _visibilityObservers)
            {
                try
                {
                    observer.UpdateVisibility(isVisible);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        List<ICursorInteractingStateObserver> _interactingStateObservers = new();

        public void Subscribe(ICursorInteractingStateObserver observer)
        {
            if (!_interactingStateObservers.Contains(observer))
            {
                _interactingStateObservers.Add(observer);
            }
        }

        public void Unsubscribe(ICursorInteractingStateObserver observer)
        {
            _interactingStateObservers.Remove(observer);
        }

        protected void NotifyInteractingStateObserver()
        {
            foreach (var observer in _interactingStateObservers)
            {
                try
                {
                    observer.UpdateInteractingState(interactingState);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        #endregion

        public abstract void SetLockMode(object context, CursorLockMode state);
        public abstract void SetLockMode(CursorLockMode lockMode);
        public abstract void SetVisible(bool visible);

        public abstract void SetInteractingState(CursorInteractingState interactingState);

        public abstract void Reset();
        public abstract void Clear();
    }
}