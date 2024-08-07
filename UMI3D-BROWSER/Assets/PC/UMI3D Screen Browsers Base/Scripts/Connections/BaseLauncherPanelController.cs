/*
Copyright 2019 - 2022 Inetum

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
using umi3d.commonScreen;
using umi3d.commonScreen.Displayer;
using umi3d.commonScreen.menu;
using UnityEngine;
using UnityEngine.UIElements;

namespace umi3d.baseBrowser.connection
{
    public abstract class BaseLauncherPanelController : MonoBehaviour
    {
        public Launcher_C Launcher;

        [SerializeField]
        protected UIDocument document;
        protected VisualElement root => document.rootVisualElement;
        protected Dialoguebox_C m_connectionDialoguebox;

        protected virtual void Start()
        {
            Debug.Assert(document != null);

            Screen.sleepTimeout = SleepTimeout.SystemSetting;

            root.Add(TooltipsLayer_C.Instance);

            Launcher = root.Q<Launcher_C>();
#if !UNITY_STANDALONE
            Launcher.Version = Application.version;
#endif
            Launcher.Settings.Audio.SetAudio();
            Launcher.InitLibraries();
            Launcher.InitTips();
            Launcher.CurrentScreen = LauncherScreens.Home;

            m_connectionDialoguebox = new Dialoguebox_C();
            m_connectionDialoguebox.Type = DialogueboxType.Default;
            m_connectionDialoguebox.Size = ElementSize.Small;
        }

        private void OnDestroy()
        {
            Dialoguebox_C.ResetAllQueue();
        }
    }
}
