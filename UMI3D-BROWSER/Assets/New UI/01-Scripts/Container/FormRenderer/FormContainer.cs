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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace umi3dBrowsers.container.formrenderer
{
    public class FormContainer : MonoBehaviour
    {
        public FormContainer parentFormContainer;
        public GameObject container;
        public List<GameObject> contents = new();
        public List<FormContainer> childrenFormContainers = new();

        /// <summary>
        /// adds the content to this container and generates a new one with the content as container
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public FormContainer GetNextFormContainer(GameObject content)
        {
            contents.Add(content);
            FormContainer formContainer = new FormContainer();
            formContainer.container = content;
            childrenFormContainers.Add(formContainer);
            formContainer.parentFormContainer = parentFormContainer;
            return formContainer;
        }

        /// <summary>
        /// Replace the container with a prefab. WARNING :: it reparents all its children and deletes it.
        /// </summary>
        /// <param name="intermediaryContainer"></param>
        /// <returns></returns>
        public FormContainer ReplaceContainerWithPrefab(GameObject intermediaryContainer)
        {
            /// creare un nouveau qui contient la page 
            /// reremplit l'adresse de celui ci avec la le prefab


            GameObject newContainer = Instantiate(intermediaryContainer, parentFormContainer.container.transform); 
            parentFormContainer.contents.Remove(container);

            foreach (var content in contents)
            {
                content.transform.SetParent(newContainer.transform);
            }

            Destroy(container);

            container = newContainer;

            parentFormContainer.contents.Add(container);

            return this;
        }
    }
}

