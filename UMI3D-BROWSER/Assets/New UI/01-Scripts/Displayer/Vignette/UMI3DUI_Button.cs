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
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3dBrowsers.displayer
{
    public class UMI3DUI_Button : Button
    {
        private ISubDisplayer _subDisplayer;
        public ISubDisplayer SubDisplayer => _subDisplayer;
        public event Action OnHoverEnter;
        public event Action OnHoverExit;
        public event Action OnDeselected;

        [Header("Selection")]
        [SerializeField] private List<GameObject> objectToEnableOnHover;
        [SerializeField] private bool autoUnHover;
        [SerializeField] private List<GameObject> objectToEnableOnClick;
        [SerializeField] private bool autoDeselect;

        [Header("Styles")]
        [SerializeField] private bool autoInitStyles = true;
        [SerializeField] private List<ButtonStyle> relatedButtonStyles;

        private bool isSelected;


        private int id;
        public int ID => id;    
        public void SetID(int id) { this.id = id; }

        protected override void Awake()
        {
            _subDisplayer = GetComponent<ISubDisplayer>();

            onClick.AddListener(() => Select());

            if (relatedButtonStyles != null && autoInitStyles)
                foreach (var style in relatedButtonStyles)
                    if (style != null)
                        style.Init(_subDisplayer as ButtonStyle);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            OnHoverEnter?.Invoke();
            if (_subDisplayer == null) return;
            _subDisplayer.HoverEnter(eventData);

            if (relatedButtonStyles != null) 
                foreach (var style in relatedButtonStyles)
                    if(style != null)
                        style.HoverEnter(eventData);

            ActivateList(objectToEnableOnHover, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            OnHoverExit?.Invoke();
            if (_subDisplayer == null) return;
            _subDisplayer.HoverExit(eventData);

            if (relatedButtonStyles != null && !isSelected)
                foreach (var style in relatedButtonStyles)
                    if (style != null)
                        style.HoverExit(eventData);

            if (!isSelected)
                ActivateList(objectToEnableOnHover, false);
            else
                ActivateListNoCrossOver(objectToEnableOnHover, objectToEnableOnClick, false);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if (autoDeselect)
            {
                isSelected = false;
                ActivateList(objectToEnableOnClick, false);
            }
        }

        public void Deselect()
        {
            isSelected = false;
            ActivateList(objectToEnableOnClick, false);
            _subDisplayer.Disable();
            if (relatedButtonStyles != null)
                foreach (var style in relatedButtonStyles)
                    if (style != null)
                        style.Disable();
        }

        public override void Select()
        {
            if (_subDisplayer != null)
                _subDisplayer.Click();

            isSelected = true;
            ActivateList(objectToEnableOnClick, true);

            if (autoUnHover)
                ActivateList(objectToEnableOnHover, false);
        }

        private void ActivateList(List<GameObject> list, bool isIt)
        {
            if (list != null)
                foreach (var obj in list)
                    if (obj != null)
                        obj.SetActive(isIt);
        }

        private void ActivateListNoCrossOver(List<GameObject> list1, List<GameObject> list2, bool isIt)
        {
            if (list1 == null) return;
            if (list2 == null)
            {
                ActivateList(list1, isIt); 
                return; 
            }

            foreach (var obj in list1)
            {

                if (obj != null)
                    if(!list2.Contains(obj))
                        obj.SetActive(isIt);

            }
        }

    }
}

