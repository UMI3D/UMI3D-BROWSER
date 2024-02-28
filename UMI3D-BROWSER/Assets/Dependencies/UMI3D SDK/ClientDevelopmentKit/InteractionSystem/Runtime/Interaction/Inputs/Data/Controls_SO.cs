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

using inetum.unityUtils.saveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace umi3d.cdk.interaction
{
    public class Controls_SO : SerializableScriptableObject
    {
        public List<AbstractControlData> actions = new();


        public List<AbstractButtonControlData> buttonControls = new();
        public List<AbstractManipulationControlData> manipulationControls = new();
        public List<UIButtonControlData> uIControls = new();


        public List<AbstractControlData> shortcuts = new();

        public virtual AbstractControlData this[int index, AbstractControlType type]
        {
            get
            {
                return type switch
                {
                    ActionControlType => actions[index],
                    ShortcutControlType => shortcuts[index],
                    _ => throw new NoInputFoundException()
                };
            }
        }

        public AbstractControlData this[Guid id, AbstractControlType type]
        {
            get
            {
                return type switch
                {
                    ActionControlType => actions.Find(control =>
                    {
                        return control.id == id;
                    }),
                    ShortcutControlType => shortcuts.Find(control =>
                    {
                        return control.id == id;
                    }),
                    _ => throw new NoInputFoundException()
                };
            }
        }

        public int IndexOf(Guid id, AbstractControlType type)
        {
            return type switch
            {
                ActionControlType => actions.FindIndex(control =>
                {
                    return control.id == id;
                }),
                ShortcutControlType => shortcuts.FindIndex(control =>
                {
                    return control.id == id;
                }),
                _ => throw new NoInputFoundException()
            };
        }
    }
}