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

using inetum.unityUtils.observation;
using System;
using UnityEngine;

namespace inetum.unityUtils.lifeCycle
{
    public class Quitting 
    {
        public enum QuittingState
        {
            NotQuitting,
            WaitsForConfirmation,
            IsQuitting
        }

        public static Quitting instance => _instance.Value;
        static readonly Lazy<Quitting> _instance = new(() => new());

        Notifier askForConfirmationNotifier;
        Notifier applicationIsQuittingNotifier;

        QuittingState _state = QuittingState.NotQuitting;
        public QuittingState state
        {
            get => _state;
            private set => _state = value;
        }

        public bool isWaitingForConfirmation => state == QuittingState.WaitsForConfirmation;
        public bool isQuitting => state == QuittingState.IsQuitting;

        Quitting()
        {
            Application.wantsToQuit += WantsToQuit;

            askForConfirmationNotifier = NotificationHub.Default.GetNotifier(
                this,
                ID.FromType<QuittingNotificationKeys.AskForConfirmation>()
            );

            applicationIsQuittingNotifier = NotificationHub.Default.GetNotifier(
               this,
               ID.FromType<QuittingNotificationKeys.ApplicationIsQuitting>()
           );
        }

        public void Quit(object publisher, bool askForConfirmation = true)
        {
            state = askForConfirmation 
                ? QuittingState.WaitsForConfirmation
                : QuittingState.IsQuitting;

            _Quit();

            if (askForConfirmation)
            {
                askForConfirmationNotifier[QuittingNotificationKeys.AskForConfirmation.Publisher] = publisher;
                askForConfirmationNotifier.Notify();
            }
        }

        public void Confirmation(object publisher, bool isQuitting)
        {
            state = isQuitting
                ? QuittingState.IsQuitting
                : QuittingState.NotQuitting;

            if (isQuitting) 
            {
                applicationIsQuittingNotifier[QuittingNotificationKeys.AskForConfirmation.Publisher] = publisher;
                applicationIsQuittingNotifier.Notify();

                _Quit(); 
            }
        }

        public static implicit operator bool(Quitting quitting)
        {
            return quitting.isQuitting;
        }

        bool WantsToQuit()
        {
            switch (state)
            {
                case QuittingState.NotQuitting:
                    Debug.LogError($"[QuittingModel.WantsToQuit] Error: state should not have this value.");
                    return false;

                case QuittingState.WaitsForConfirmation:
                    return false;

                case QuittingState.IsQuitting:
                    return true;

                default:
                    Debug.LogError($"[QuittingModel.WantsToQuit] Error: Unhandled case.");
                    return true;
            }
        }

        void _Quit()
        {
            Application.Quit();
        }
    }
}