/*
Copyright 2019 - 2022 Inetum

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

using umi3d.cdk.interaction;
using umi3d.picoBrowser;

using UnityEngine;
using UnityEngine.XR.Hands.Samples.GestureSample;

namespace umi3dVRBrowsersBase.interactions
{
    /// <summary>
    /// Observer for VR controller gestures.
    /// </summary>
    [RequireComponent(typeof(StaticHandGesture))]
    public class VRGestureObserver : MonoBehaviour
    {
        public bool IsGestureStaying { get; private set; }

        [SerializeField]
        private ActionType actionType;
        public ActionType ActionType => actionType;

        public event System.Action GestureStarted;
        public event System.Action GestureStopped;
        public event System.Action GestureStayed;

        private StaticHandGesture gestureObserved;

        #region Methods

        private void Start()
        {
            gestureObserved = GetComponent<StaticHandGesture>();
        }

        private void Update()
        {
            if (IsGestureStaying)
                GestureStay();
        }

        public void GestureStart()
        {
            IsGestureStaying = true;
            GestureStarted?.Invoke();
            Debug.Log("Started gesture " + gestureObserved.gameObject.name);
        }

        public void GestureStop()
        {
            IsGestureStaying = false;
            GestureStopped?.Invoke();
            Debug.Log("Stopped gesture " + gestureObserved.gameObject.name);
        }

        public void GestureStay()
        {
            GestureStayed?.Invoke();
        }

        #endregion Methods
    }
}