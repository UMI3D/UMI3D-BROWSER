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
using System.Linq;
using TMPro;
using umi3d.baseBrowser.Controller;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.common.interaction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace umi3d.browserRuntime.ui.inGame.interactionMapping
{
    public class InteractionMappingPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text interactionText;
        [SerializeField] private Transform mappingContent;
        [SerializeField] private GameObject mappingPrefab;
        [SerializeField] private List<InputAction> inputKeys;
        [SerializeField] private List<Sprite> inputSprites;
        [SerializeField] private Sprite leftClickSprite;

        private Dictionary<string, InteractionMappingElement> _rows;

        private void Awake()
        {
            KeyboardInteraction.Mapped += Show;
            KeyboardInteraction.Unmapped += Hide;
            BaseController.Instance.OnAddParameter += ShowParameters;
            BaseController.Instance.OnRelease += Hide;

            _rows = new();

            Show(null, "", null);
            Hide();
        }

        private void OnDestroy()
        {
            KeyboardInteraction.Mapped -= Show;
            KeyboardInteraction.Unmapped -= Hide;
            BaseController.Instance.OnAddParameter -= ShowParameters;
            BaseController.Instance.OnRelease -= Hide;
        }

        private void Show(KeyboardInteraction interaction, string name, InputAction action)
        {
            gameObject.SetActive(true);
            interactionText.text = interaction?.name;

            if (_rows.ContainsKey(name))
                AddInteraction(action, _rows[name]);
            else
                AddRowWith(name, action);
        }

        private void AddInteraction(InputAction action, InteractionMappingElement interactionMappingDisplayer)
        {
            if (action == null)
                return;
            foreach (var inputControl in action.controls)
                interactionMappingDisplayer.Add(inputSprites[inputKeys.FindIndex(a => a.controls.Contains(inputControl))]);
        }

        private void AddRowWith(string name, InputAction action)
        {
            if (action == null)
                return;
            var interactionMappingDisplayer = Instantiate(mappingPrefab, mappingContent).GetComponent<InteractionMappingElement>();
            interactionMappingDisplayer.Initialize(name);
            AddInteraction(action, interactionMappingDisplayer);

            _rows.Add(name, interactionMappingDisplayer);
        }

        private void Hide(KeyboardInteraction interaction)
        {
            Hide();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            foreach (var row in _rows)
                Destroy(row.Value.gameObject);
            _rows.Clear();
        }

        private void ShowParameters(AbstractParameterDto dto)
        {
            if (gameObject.activeInHierarchy)
                return;

            gameObject.SetActive(true);
            interactionText.text = "Interaction";

            var interactionMappingDisplayer = Instantiate(mappingPrefab, mappingContent).GetComponent<InteractionMappingElement>();
            interactionMappingDisplayer.Initialize("Modify");
            interactionMappingDisplayer.Add(leftClickSprite);

            _rows.Add("Modify", interactionMappingDisplayer);
        }
    }
}