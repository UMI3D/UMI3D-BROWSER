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

using UnityEngine.UI;

namespace umi3d.browserRuntime.interactions
{
    internal class NameView : Image
    {
        TMPro.TMP_Text textTMP;

        protected override void Awake()
        {
            base.Awake();

            SetTextView();
        }

        void Update()
        {
        }

        public void SetText(string text)
        {
            if (!SetTextView()) 
            {
                throw new System.Exception("Text view cannot be set."); 
            }

            textTMP.text = text;
        }

        bool SetTextView()
        {
            if (textTMP == null)
            {
                textTMP = transform.GetChild(0).GetComponent<TMPro.TMP_Text>();
            }

            return textTMP != null;
        }
    }
}