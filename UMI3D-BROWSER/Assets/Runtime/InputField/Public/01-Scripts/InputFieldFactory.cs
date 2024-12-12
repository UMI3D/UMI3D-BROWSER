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

// Todo : Add Pooling
// Todo : Work with the model view

using System;
using UnityEngine;

namespace umi3d.browserRuntime.inputField
{
    [Serializable]
    public class InputFieldFactory 
    {
        [SerializeField] GameObject singleLinePrefab;
        [SerializeField] GameObject multiLinePrefab;

        public GameObject CreateInputField(Transform parent, string title, string value, string placeholder, bool isMultiline, int nbLine, Action<string> onTextSubmited = null)
        {
            if (nbLine < 1) nbLine = 1;

            var inputFieldGameobject = GameObject.Instantiate(isMultiline ? multiLinePrefab : singleLinePrefab);
            /*
            inputFieldGameobject.GetComponent<InputFieldTitleView>().SetTitle(title);
            inputFieldGameobject.GetComponent<InputFieldView>().Setup(value, placeholder, isMultiline ? nbLine : 1, onTextSubmited);

            inputFieldGameobject.transform.SetParent(parent, false);*/

            return inputFieldGameobject;
        }
    }
}