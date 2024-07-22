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
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Ui/PageTip")]
public class PageTipData : ScriptableObject
{
    [Serializable]
    public struct Tip
    {
        public string LocalizedString;
        public Vector2 Position;
    }

    [SerializeField] private List<Tip> localizedSequence;

    public Tip GetTip(int index) => localizedSequence[index];
    public int Count => localizedSequence.Count;
}
