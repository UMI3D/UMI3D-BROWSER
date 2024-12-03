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

using inetum.unityUtils.observation;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using umi3d.baseBrowser.inputs.interactions;
using umi3d.browserRuntime.notificationKeys;
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
        [SerializeField] private List<KeySprite> inputSprites;
        [SerializeField] private Sprite leftClickSprite;

        private Dictionary<string, InteractionMappingElement> _rows;

        private bool isQwerty => Keyboard.current.aKey.displayName.ToLower() == "a"; // In qwerty mode, it return an "a"

        private void Awake()
        {
            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<InteractionNotificationKeys.ParameterInputFound>(),
                (Callback)ParameterInputFound
            );

            NotificationHub.Default.Subscribe(
                this,
                ID.FromType<InteractionNotificationKeys.ToolReleased>(),
                (Callback)ToolReleased
            );

            KeyboardInteraction.Mapped += Show;
            KeyboardInteraction.Unmapped += Hide;

            _rows = new();

            Show(null, "", null);
            Hide();
        }

        private void OnDestroy()
        {
            NotificationHub.Default.Unsubscribe(this);

            KeyboardInteraction.Mapped -= Show;
            KeyboardInteraction.Unmapped -= Hide;
        }

        private void Show(KeyboardInteraction interaction, string name, InputAction action)
        {
            name ??= ""; // Ensure non null value

            gameObject.SetActive(true);
            interactionText.text = interaction?.name;

            if (_rows.ContainsKey(name))
                AddInteraction(action, _rows[name]);
            else
                AddRowWith(name, action);
            Reorder();
        }

        private void Reorder()
        {
            var rows = _rows.Values.ToList();
            rows.Sort();

            int minIndex = int.MaxValue;
            foreach(var e in rows)
            {
                var index = e.transform.GetSiblingIndex();
                if(index < minIndex)
                    minIndex = index;
            }
            foreach (var e in rows)
                e.transform.SetSiblingIndex(minIndex);



        }

        private void AddInteraction(InputAction action, InteractionMappingElement interactionMappingDisplayer)
        {
            if (action == null)
                return;
            var min = int.MaxValue;
            foreach (var inputControl in action.controls)
            {
                var index = inputKeys.FindIndex(a => a.controls.Contains(inputControl));
                if(index < min)
                    min = index;
                var keySprite = inputSprites[index];
                interactionMappingDisplayer.Add(isQwerty ? keySprite.Qwerty : keySprite.Azerty);
            }
            interactionMappingDisplayer.Order = min;
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

        private void Hide(KeyboardInteraction interaction, string key)
        {
            key ??= "";
            //Hide();
            if (_rows.ContainsKey(key))
            {
                var row = _rows[key];
                _rows.Remove(key);
                DestroyImmediate(row.gameObject);
            }

            if(_rows.Count == 0)
                gameObject.SetActive(false);

            //gameObject.SetActive(false);
            //foreach (var row in _rows)
            //    DestroyImmediate(row.Value.gameObject);
            //_rows.Clear();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            foreach (var row in _rows)
                DestroyImmediate(row.Value.gameObject);
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

        void ParameterInputFound(Notification notification)
        {
            if (!notification.TryGetInfoT(InteractionNotificationKeys.ParameterInputFound.parameterDto, out AbstractParameterDto dto))
            {
                return;
            }

            ShowParameters(dto);
        }

        void ToolReleased()
        {
            Hide();
        }
    }

    [Serializable]
    public struct KeySprite
    {
        public Sprite Azerty;
        public Sprite Qwerty;
    }
}