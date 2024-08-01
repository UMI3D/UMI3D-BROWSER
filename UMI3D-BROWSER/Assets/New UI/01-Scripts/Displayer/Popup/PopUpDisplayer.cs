/*
Copyright 2019 - 2024 Inetum

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
using UnityEngine.Events;
using UnityEngine.Localization.Components;

namespace umi3dBrowsers.displayer
{
    public class PopupDisplayer : MonoBehaviour
    {
        [SerializeField] private LocalizeStringEvent title;
        [SerializeField] private LocalizeStringEvent description;
        [SerializeField] private GameObject buttonGroup;

        [Header("Prefabs")]
        [SerializeField] private GameObject buttonPrefab;

        /// <remarks> Key for localisation </remarks>
        public string Title { set => title.SetEntry(value); }
        /// <remarks> Key for localisation </remarks>
        public string Description { set => description.SetEntry(value); }

        public void SetArguments(Dictionary<string, object> arguments)
        {
            if (arguments == null) 
            {
                title.StringReference.Arguments = null;
                description.StringReference.Arguments = null;
            }
            else
            {
                title.StringReference.Arguments = new object[] { arguments };
                description.StringReference.Arguments = new object[] { arguments };
            }
        }

        public void SetButtons(params (string, Action)[] buttons)
        {
            ClearButtons();

            foreach (var button in buttons)
                CreateButton(button);
        }

        private void ClearButtons()
        {
            for (var i = buttonGroup.transform.childCount - 1; i >= 0; i--)
                Destroy(buttonGroup.transform.GetChild(i).gameObject);
        }

        private GameObject CreateButton((string, Action) button)
        {
            var buttonObject = Instantiate(buttonPrefab, buttonGroup.transform).GetComponent<SimpleButton>();
            buttonObject.GetComponentInChildren<LocalizeStringEvent>().SetEntry(button.Item1);
            Action callback = () =>
            {
                button.Item2?.Invoke();
                gameObject.SetActive(false);
            };
            buttonObject.OnClick.AddListener(new UnityAction(callback));

            return buttonObject.gameObject;
        }
    }

}
