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

using System.Collections.Generic;
using System.Linq;
using TMPro;
using umi3d.baseBrowser.Controller;
using umi3d.baseBrowser.cursor;
using umi3d.baseBrowser.parameters;
using umi3d.common.interaction;
using umi3dBrowsers.displayer;
using UnityEngine;
using UnityEngine.UI;
using static umi3d.baseBrowser.cursor.BaseCursor;

public class ParameterMenuDisplayer : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject stringParameterDisplayPrefab;
    [SerializeField] private GameObject booleanParameterDisplayPrefab;
    [SerializeField] private GameObject sliderParameterDisplayPrefab;
    [SerializeField] private GameObject dropdownParameterDisplayPrefab;

    private void Awake()
    {
        BaseController.Instance.LeftClickParametersInteraction.OnClicked += Show;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        BaseController.Instance.LeftClickParametersInteraction.OnClicked -= Show;
    }

    private void Show(List<AbstractParameterDto> parameters)
    {
        if (gameObject.activeInHierarchy || parameters.Count <= 0)
            return;

        parameters.Reverse(); // Reverse to show element above in front (layout in the object is set to reverse too)

        gameObject.SetActive(true);
        BaseCursor.SetMovement(this, CursorMovement.Free);

        Debug.Log(parameters.Count);
        foreach (AbstractParameterDto parameter in parameters)
            AddParameter(parameter);
    }

    private void AddParameter(AbstractParameterDto parameter)
    {
        switch (parameter)
        {
            case StringParameterDto stringParameter:
            {
                var stringGameObject = Instantiate(stringParameterDisplayPrefab, content);
                stringGameObject.GetComponentInChildren<TMP_Text>().text = stringParameter.name;
                var inputfield = stringGameObject.GetComponentInChildren<TMP_InputField>();
                inputfield.text = stringParameter.value;
                inputfield.onSubmit.AddListener(newValue => {
                    stringParameter.value = newValue;
                });
                return;
            }
            case BooleanParameterDto booleanParameter:
            {
                GameObject booleanGameObject = Instantiate(booleanParameterDisplayPrefab, content);
                booleanGameObject.GetComponentInChildren<TMP_Text>().text = booleanParameter.name;
                var toggle = booleanGameObject.GetComponentInChildren<ToggleSwitch>();
                if (toggle.CurrentValue == booleanParameter.value)
                    toggle.Click();
                toggle.onToggleOn.AddListener(() => {
                    booleanParameter.value = true;
                });
                toggle.onToggleOff.AddListener(() => {
                    booleanParameter.value = false;
                });
                return;
            }
            case FloatRangeParameterDto sliderParameter:
            {
                var sliderGameObject = Instantiate(sliderParameterDisplayPrefab, content);
                sliderGameObject.GetComponentInChildren<TMP_Text>().text = sliderParameter.name;
                var slider = sliderGameObject.GetComponentInChildren<Slider>();
                slider.value = sliderParameter.value;
                slider.minValue = sliderParameter.min;
                slider.maxValue = sliderParameter.max;
                slider.wholeNumbers = false;
                slider.onValueChanged.AddListener(newValue => {
                    sliderParameter.value = newValue;
                });
                return;
            }
            case IntegerRangeParameterDto sliderParameter:
            {
                var sliderGameObject = Instantiate(sliderParameterDisplayPrefab, content);
                sliderGameObject.GetComponentInChildren<TMP_Text>().text = sliderParameter.name;
                var slider = sliderGameObject.GetComponentInChildren<Slider>();
                slider.value = sliderParameter.value;
                slider.minValue = sliderParameter.min;
                slider.maxValue = sliderParameter.max;
                slider.wholeNumbers = true;
                slider.onValueChanged.AddListener(newValue => {
                    sliderParameter.value = (int)newValue;
                });
                return;
            }
            case EnumParameterDto<string> dropdownParameter:
            {
                var dropdownGameObject = Instantiate(dropdownParameterDisplayPrefab, content);
                dropdownGameObject.GetComponentInChildren<TMP_Text>().text = dropdownParameter.name;
                var dropdown = dropdownGameObject.GetComponentInChildren<GridDropDown>();
                dropdown.Init(dropdownParameter.possibleValues.Select(v => new GridDropDownItemCell(v)).ToList(),
                    dropdownParameter.possibleValues.IndexOf(dropdownParameter.value));
                dropdown.OnClick += () => {
                    dropdownParameter.value = dropdown.GetValue();
                };
                return;
            }
            default:
                break;
        }
        Debug.LogWarning($"Parameter type not found : {parameter}", this);
    }
}
