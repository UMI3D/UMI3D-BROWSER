/*
Copyright 2019 - 2023 Inetum

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
using umi3d;
using UnityEngine;
using umi3d.common.interaction.form;
using System.Linq;
using System;
using TMPro;
using UnityEditor.Build.Utilities;

namespace form_generator
{
    public class DivFormMaker : MonoBehaviour
    {
        [Header("Form reader")]
        [SerializeField] private MainContainer mainContainer;

        [Header("Data")]
        [SerializeField] private DivTypeTagger rootDiv;
        [SerializeField] private List<DivTypeTagger> divs = new();
        private List<DivDto> _divDtos = new();

        [ContextMenu("SendForm")]
        public void SendForm()
        {
            divs = rootDiv.GetComponentsInChildren<DivTypeTagger>().ToList();
            CleanDivsTagger(divs);
            CreateDevHierarchy(divs);
            ConnectionFormDto form = PrepareForm(divs[0]) as ConnectionFormDto;
            mainContainer.ToolAccessProcessForm(form);
        }

        private void CleanDivsTagger(List<DivTypeTagger> divs)
        {
            foreach (var div in divs)
            {
                if (div != null)
                {
                    div.Children.Clear();
                }
            }
        }

        private void CreateDevHierarchy(List<DivTypeTagger> divs)
        {
            if (divs[0].DivType != DivType.Form)
            {
                Debug.Log("The root of the form must be a form, it is actually a " + divs[0].DivType);
                return;
            }

            foreach (var parentDiv in divs)
            {
                List<DivTypeTagger> cycleExcludedDivs = new();
                List<DivTypeTagger> children = parentDiv.GetComponentsInChildren<DivTypeTagger>().ToList();
                children.RemoveAt(0);

                foreach (var childDiv in children)
                {
                    if (cycleExcludedDivs.Contains(childDiv)) continue;
                    if (IsChildDivParentOfParentDiv(childDiv, parentDiv)) continue;

                    List<DivTypeTagger> childs = childDiv.GetComponentsInChildren<DivTypeTagger>().ToList();
                    childs.RemoveAt(0);
                    cycleExcludedDivs.AddRange(childs);

                    parentDiv.Children.Add(childDiv);
                }
            }
        }

        private DivDto PrepareForm(DivTypeTagger divParent)
        {
            DivDto divDto = CreateDivInstance(divParent);
            divDto.FirstChildren = new();
            List<DivDto> children = new List<DivDto>();
            foreach (var div in divParent.Children)
            {
                children.Add(PrepareForm(div));
            }

            divDto.FirstChildren = children;

            //Debug.Log("__Parent____" + divDto.tooltip); 
            //children.ForEach(child => 
            //{
            //    Debug.Log("_____________Child____" + child.tooltip);
            //});

            return divDto;
        }

        private DivDto CreateDivInstance(DivTypeTagger divTagger)
        {
            switch (divTagger.DivType)
            {
                case DivType.Form:
                    ConnectionFormDto form = new ConnectionFormDto();
                    form.name = divTagger.Name;
                    form.tooltip = divTagger.ToolTip;
                    form.description = divTagger.Description;
                    return form;
                case DivType.Page:
                    PageDto page = new PageDto();
                    page.name = divTagger.Name;
                    page.tooltip = divTagger.ToolTip;
                    return page;
                case DivType.Group:
                    GroupDto group = new GroupDto();
                    group.tooltip = divTagger.ToolTip;
                    group.label = divTagger.Name;
                    return group;
                case DivType.Button:
                    ButtonDto button = new ButtonDto();
                    button.Name = divTagger.Name;
                    button.Text = divTagger.GetComponent<TextMeshPro>()?.text;
                    button.tooltip = divTagger.ToolTip;
                    return button;
                case DivType.Enum:
                    break;
                case DivType.Image:
                    ImageDto image = new ImageDto();
                    image.tooltip = divTagger.ToolTip;
                    return image;
                case DivType.Range:
                    break;
                case DivType.Input_Text:
                    break;
                case DivType.Label:
                    LabelDto label = new LabelDto();
                    label.text = divTagger.GetComponent<TextMeshPro>()?.text;
                    label.tooltip = divTagger.ToolTip;
                    return label;
            }

            return null;
        }

        private bool IsChildDivParentOfParentDiv(DivTypeTagger childDiv, DivTypeTagger parentDiv)
        {
            List<DivTypeTagger> divs = childDiv.GetComponentsInChildren<DivTypeTagger>().ToList();
            if (divs.Contains(parentDiv)) return true;
            return false;          
        }
    }
}

