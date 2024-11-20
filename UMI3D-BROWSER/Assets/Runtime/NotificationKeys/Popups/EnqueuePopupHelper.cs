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

using inetum.unityUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace umi3d.browserRuntime.ui.popup
{
    public class EnqueuePopupHelper 
    {
        Notifier notifier;
        PopupInfo popupInfo;

        public EnqueuePopupHelper(System.Object publisher, System.Guid guid)
        {
            this.notifier = NotificationHub.Default.GetNotifier<PopupNotificationKeys.EnqueuePopup>(publisher);
            popupInfo = new();
            popupInfo.id = guid;
        }

        #region Type

        bool isTypeSet = false;

        /// <summary>
        /// Set the type of the popup.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <remarks>
        /// If this method is not call before <see cref="Notify"/> then set <see cref="PopupType.Information"/> by default.
        /// </remarks>
        public EnqueuePopupHelper SetType(PopupType type)
        {
            popupInfo.type = type;
            isTypeSet = true;
            return this;
        }

        #endregion

        #region Title

        bool isTitleSet = false;

        /// <summary>
        /// Set the title of the popup.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        /// <remarks>
        /// If <see cref="SetTitle(string)"/> or <see cref="SetTitle(string, string)"/> are not call before <see cref="Notify"/> then set null by default.
        /// </remarks>
        public EnqueuePopupHelper SetTitle(string title)
        {
            popupInfo.title = title;
            isTitleSet = true;
            return this;
        }

        /// <summary>
        /// Set the localize title of the popup.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        ///  <remarks>
        /// If <see cref="SetTitle(string)"/> or <see cref="SetTitle(string, string)"/> are not call before <see cref="Notify"/> then set null by default.
        /// </remarks>
        public EnqueuePopupHelper SetTitle(string table, string entry)
        {
            popupInfo.title = (table, entry);
            isTitleSet = true;
            return this;
        }

        #endregion

        #region Description

        bool isDescriptionSet = false;

        /// <summary>
        /// Set the description of the popup.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <remarks>
        /// If <see cref="SetDescription(string)"/> or <see cref="SetDescription(string, string)"/> are not call before <see cref="Notify"/> then set null by default.
        /// </remarks>
        public EnqueuePopupHelper SetDescription(string description)
        {
            popupInfo.description = description;
            isDescriptionSet = true;
            return this;
        }

        /// <summary>
        /// Set the localize description of the popup.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        /// <remarks>
        /// If <see cref="SetDescription(string)"/> or <see cref="SetDescription(string, string)"/> are not call before <see cref="Notify"/> then set null by default.
        /// </remarks>
        public EnqueuePopupHelper SetDescription(string table, string entry)
        {
            popupInfo.description = (table, entry);
            isDescriptionSet = true;
            return this;
        }

        #endregion

        #region Buttons

        bool AreButtonsSet = false;

        /// <summary>
        /// Set the buttons text of the popup.<br/>
        /// <br/>
        /// Value can be <see cref="string"/> for a text or (<see cref="string"/> table, <see cref="string"/> entry) for a localized text.
        /// </summary>
        /// <param name="buttons"></param>
        /// <returns></returns>
        /// <remarks>
        /// If this method is not call before <see cref="Notify"/> then set null by default.
        /// </remarks>
        public EnqueuePopupHelper SetButtons(params System.Object[] buttons)
        {
            popupInfo.buttons = buttons?.ToList() ?? null;
            AreButtonsSet = true;
            return this;
        }

        #endregion

        #region ButtonsAction

        bool isButtonsActionSet = false;

        /// <summary>
        /// Set buttons action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <remarks>
        /// If this method is not call before <see cref="Notify"/> then set null by default.
        /// </remarks>
        public EnqueuePopupHelper SetButtonsAction(Action<int> action)
        {
            popupInfo.buttonActions = action;
            isButtonsActionSet = true;
            return this;
        }

        #endregion

        #region Arguments

        bool areArgumentsSet = false;

        /// <summary>
        /// Set the localized arguments of the popup.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        /// <remarks>
        /// If <see cref="SetArguments(Dictionary{string, object})"/> or <see cref="SetArguments(ValueTuple{string, object}[])"/> are not call before <see cref="Notify"/> then set null by default.
        /// </remarks>
        public EnqueuePopupHelper SetArguments(Dictionary<string, System.Object> arguments)
        {
            popupInfo.arguments = arguments;
            areArgumentsSet = true;
            return this;
        }

        /// <summary>
        /// Set the localized arguments of the popup.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        /// <remarks>
        /// If <see cref="SetArguments(Dictionary{string, object})"/> or <see cref="SetArguments(ValueTuple{string, object}[])"/> are not call before <see cref="Notify"/> then set null by default.
        /// </remarks>
        public EnqueuePopupHelper SetArguments(params (string, System.Object)[] arguments)
        {
            Dictionary<string, System.Object> args = new();
            foreach (var arg in arguments)
            {
                args.Add(arg.Item1, arg.Item2);
            }
            popupInfo.arguments = args;
            areArgumentsSet = true;
            return this;
        }

        #endregion

        #region CloseButtonVisibility

        bool isCloseButtonVisibilitySet = false;

        /// <summary>
        /// Set the visibility of the close button.
        /// </summary>
        /// <param name="hideCloseButton"></param>
        /// <returns></returns>
        /// <remarks>
        /// If this method is not call before <see cref="Notify"/> then set visible by default.
        /// </remarks>
        public EnqueuePopupHelper SetCloseButtonVisibility(bool hideCloseButton)
        {
            popupInfo.hideCloseButton = hideCloseButton;
            isCloseButtonVisibilitySet = true;
            return this;
        }

        #endregion

        /// <summary>
        /// Enqueue a new popup. 
        /// </summary>
        public void Notify()
        {
            if (!isTypeSet)
            {
                SetType(PopupType.Information);
            }
            if (!isTitleSet)
            {
                SetTitle(null);
            }
            if (!isDescriptionSet)
            {
                SetDescription(null);
            }
            if (!AreButtonsSet)
            {
                SetButtons(null);
            }
            if (!isButtonsActionSet)
            {
                SetButtonsAction(null);
            }
            if (!areArgumentsSet)
            {
                SetArguments(null as Dictionary<string, System.Object>);
            }
            if (!isCloseButtonVisibilitySet)
            {
                SetCloseButtonVisibility(false);
            }

            isTypeSet = false;
            isTitleSet = false;
            isDescriptionSet = false;
            AreButtonsSet = false;
            isButtonsActionSet = false;
            areArgumentsSet = false;
            isCloseButtonVisibilitySet = false;

            notifier[PopupNotificationKeys.EnqueuePopup.PopupInfo] = popupInfo;
            notifier.Notify();
        }
    }
}