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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace umi3dVRBrowsersBase.connection
{
    /// <summary>
    /// This class is responsible for asking users to set up their avatar heights.
    /// </summary>
    public class AvatarHeightPanel : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        [Tooltip("Main gameobject of the panel")]
        private GameObject panel;

        [SerializeField]
        [Tooltip("Button to confirm avatar's height")]
        private UnityEngine.UI.Button validateButton;

        [SerializeField]
        [Tooltip("Text to display while resizing.")]
        private UnityEngine.UI.Text waitText;

        [SerializeField]
        [Tooltip("Text to display while resizing.")]
        private UnityEngine.UI.Button switchButton;

        #endregion

        #region Methods

        private void Start()
        {
            Assert.IsNotNull(panel);
            Assert.IsNotNull(validateButton);
            Assert.IsNotNull(waitText);
            Assert.IsNotNull(switchButton);

            switchButton.onClick.AddListener(SwitchVRMR);

            validateButton.onClick.AddListener(ResizeSkeleton);
            waitText.gameObject.SetActive(false);
        }

        [ContextMenu("Set Avatar Height")]
        void SwitchVRMR()
        {
            SetUpSkeleton setUp = GameObject.FindObjectOfType<SetUpSkeleton>();
            Debug.Assert(setUp != null, "No avatar found to set up height. Should not happen");

            setUp.SwitchARMR();
        }

        [ContextMenu("Set Avatar Height")]
        void ResizeSkeleton()
        {
            SetUpSkeleton setUp = GameObject.FindObjectOfType<SetUpSkeleton>();
            Debug.Assert(setUp != null, "No avatar found to set up height. Should not happen");
            
            if(setUp.TryResizeSkeleton())
            {
                StartCoroutine(WaitResize());
            }
        }

        IEnumerator WaitResize()
        {
            validateButton.gameObject.SetActive(false);
            waitText.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);
            waitText.gameObject.SetActive(false);
            validateButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// Displays panel to set up avatar's height.
        /// </summary>
        /// <param name="validationCallBack"></param>
        public void Display()
        {
            panel.SetActive(true);
        }

        /// <summary>
        /// Hides panel.
        /// </summary>
        public void Hide()
        {
            panel.SetActive(false);
        }

        #endregion
    }
}