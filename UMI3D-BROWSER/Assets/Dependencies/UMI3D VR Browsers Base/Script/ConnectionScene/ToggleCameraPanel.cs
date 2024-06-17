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

using UnityEngine;
using UnityEngine.Assertions;

namespace umi3dVRBrowsersBase.connection
{
    /// <summary>
    /// This class is responsible for asking users to set up their avatar heights.
    /// </summary>
    public class ToggleCameraPanel : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        [Tooltip("Button to manage cameras.")]
        private UnityEngine.UI.Button switchButton;

        #endregion

        #region Methods

        private void Start()
        {
            Assert.IsNotNull(switchButton);

            switchButton.onClick.AddListener(SwitchVRMR);
        }

        [ContextMenu("Turn on/off devices cameras.")]
        void SwitchVRMR()
        {
            SetUpCamera setUp = GameObject.FindObjectOfType<SetUpCamera>();
            Debug.Assert(setUp != null, "No camera management found.");

            setUp.SwitchARMR();
        }
    }

    #endregion
}