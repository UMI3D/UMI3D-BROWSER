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
using umi3d.common.interaction.form;
using UnityEngine;

namespace umi3dBrowsers.container.formrenderer
{
    public class FormContainer 
    {
        public FormContainer parentFormContainer;
        public GameObject container;
        public List<GameObject> contents = new();
        public List<FormContainer> childrenFormContainers = new();
        public List<StyleDto> Styles;

        /// <summary>
        /// adds the content to this container and generates a new one with the content as container
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public FormContainer GetNextFormContainer(GameObject content, List<StyleDto> styles)
        {
            contents.Add(content);
            FormContainer childContainer = new FormContainer();
            childContainer.container = content;
            childrenFormContainers.Add(childContainer);
            childContainer.parentFormContainer = this;
            childContainer.Styles = styles;
            return childContainer;
        }

        /// <summary>
        /// Replace the container with a prefab. WARNING :: it reparents all its children and deletes it.
        /// </summary>
        /// <param name="intermediaryContainer"></param>
        /// <returns></returns>
        public FormContainer ReplaceContainerWithPrefab(GameObject intermediaryContainer)
        {
            FormContainer replacementParentContainer = new FormContainer();
            replacementParentContainer.parentFormContainer = parentFormContainer;
            replacementParentContainer.container = container;
            replacementParentContainer.contents.AddRange(contents);
            replacementParentContainer.childrenFormContainers.AddRange(childrenFormContainers);

            replacementParentContainer.childrenFormContainers.Add(this);

            GameObject newContainer = GameObject.Instantiate(intermediaryContainer, container.transform);
            replacementParentContainer.contents.Add(newContainer);

            parentFormContainer.childrenFormContainers.Remove(this);
            parentFormContainer.childrenFormContainers.Add(replacementParentContainer);

            parentFormContainer = replacementParentContainer;

            container = newContainer;
            contents.Clear();
            childrenFormContainers.Clear();

            foreach (var content in contents)
            {
                content.transform.SetParent(newContainer.transform);
            }

            return this;
        }
    }
}

