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

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace umi3d.browserRuntime.ui.social
{
    public class OptionUserActionContainer : MonoBehaviour, IPointerEnterHandler, ICancelHandler
    {
        [SerializeField] public TMP_Text _name;
        [SerializeField] public TMP_Text description;
        [SerializeField] public Image icon;

        public void SetData(string name, string description, Sprite icon)
        {
            this._name.text = name;
            this.description.text = description;
            if (icon != null)
                this.icon.sprite = icon;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public virtual void OnCancel(BaseEventData eventData)
        {
            UnityEngine.Debug.Log("Hide ?");
        }
    }
}