/*
Copyright 2019 - 2025 Inetum

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
using UnityEngine;

namespace umi3d.browserRuntime.ui
{
    public interface IInputFieldPinSubject 
    {
        void Subscribe(IInputFieldPinObserver observer);
        void Unsubscribe(IInputFieldPinObserver observer);
    }

    public interface IInputFieldContentTypeSubject
    {
        void Subscribe(IInputFieldContentTypeObserver observer);
        void Unsubscribe(IInputFieldContentTypeObserver observer);
    }

    public interface IInputFieldNbLineSubject
    {
        void Subscribe(IInputFieldNbLineObserver observer);
        void Unsubscribe(IInputFieldNbLineObserver observer);
    }

    public interface IInputFieldPlaceholderSubject
    {
        void Subscribe(IInputFieldPlaceholderObserver observer);
        void Unsubscribe(IInputFieldPlaceholderObserver observer);
    }

    public interface IInputFieldPasswordSubject
    {
        void Subscribe(IInputFieldPasswordObserver observer);
        void Unsubscribe(IInputFieldPasswordObserver observer);
    }
}