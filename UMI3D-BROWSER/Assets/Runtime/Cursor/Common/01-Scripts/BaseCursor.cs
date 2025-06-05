/*
Copyright 2019 - 2021 Inetum

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
//using umi3d.browserRuntime.ui.contextualMenu;
//using umi3d.browserRuntime.ui.tablet;
using ccs = umi3d.cdk.collaboration.UMI3DCollaborationClientServer;

namespace umi3d.browserRuntime.cursor
{
    public class BaseCursor : inetum.unityUtils.SingleBehaviour<BaseCursor>
    {
        public enum DrawingMode { Free, Center }
        public static DrawingMode Mode
        {
            get => Exists ? s_drawingMode : DrawingMode.Free;
            set
            {
                if (Exists && s_drawingMode != value)
                {
                    s_drawingMode = value;
                    s_drawingModeUpdated?.Invoke();
                }
            }
        }
        private static DrawingMode s_drawingMode;
        private static event System.Action s_drawingModeUpdated;


        protected override void Awake()
        {
            base.Awake();

            //// ========== Tablet
            //NotificationHub.Default.Subscribe(this,
            //    ID.FromType<TabletNotificationKeys.Opened>(),
            //    (Callback)FreeCursor);
            //NotificationHub.Default.Subscribe(this,
            //    ID.FromType<TabletNotificationKeys.Closed>(),
            //    (Callback)UnSetCursor);

            //// ========== Contextual Menu
            //NotificationHub.Default.Subscribe(this,
            //    ID.FromType<ContextualMenuNotificationKeys.Open>(),
            //    (Callback)FreeCursor);
            //NotificationHub.Default.Subscribe(this,
            //    ID.FromType<ContextualMenuNotificationKeys.Close>(),
            //    (Callback)UnSetCursor);
        }

        //public void FreeCursor(Notification notification)
        //{
        //    SetMovement(notification.Publisher, CursorMovement.Free);
        //}

        //public void UnSetCursor(Notification notification)
        //{
        //    UnSetMovement(notification.Publisher);
        //    State = CursorState.Default;
        //}

    }
}
