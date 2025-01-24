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

using System.Threading.Tasks;
using TMPro;
using umi3d.cdk;
using UnityEngine;
using UnityEngine.UI;
using umi3d.common.collaboration.dto.signaling;

namespace umi3d.browserRuntime.ui.social
{
    public class PrimaryUserActionIcon : UserActionIcon
    {
        private GameObject gameObject;
        private Button button;
        [SerializeField] private Image Icon;


        public static async Task<PrimaryUserActionIcon> Create(GameObject gameObject, cdk.collaboration.UserAction action)
        {
            var option = new PrimaryUserActionIcon(gameObject, action);
            var sprite = await LoadIcon(action);
            if (sprite != null)
                option.Icon.sprite = sprite;
            return option;
        }

        public PrimaryUserActionIcon(GameObject gameObject, cdk.collaboration.UserAction action) : base(action)
        {
            this.gameObject = gameObject;
            button = this.gameObject.GetComponent<Button>();
            button?.onClick.AddListener(Call);
            Icon = this.gameObject.GetComponent<Image>();
        }

        public override void Destroy()
        {
            button?.onClick.RemoveListener(Call);
            button = null;
            GameObject.Destroy(this.gameObject);
        }
    }

    public class OptionUserActionIcon : UserActionIcon
    {
        public TMP_Dropdown.OptionData data;
        private Button button;
        private Sprite sprite;

        public static async Task<OptionUserActionIcon> Create(cdk.collaboration.UserAction action)
        {
            var option = new OptionUserActionIcon(action);
            option.sprite = await LoadIcon(action);
            return option;
        }

        protected OptionUserActionIcon(cdk.collaboration.UserAction action) : base(action)
        {
            data = new TMP_Dropdown.OptionData()
            {
                text = action.name,
            };
        }

        public void SetButton(GameObject gameObject)
        {
            var displayer = gameObject.GetComponent<OptionUserActionContainer>();
            displayer.SetData(action.name, action.description, sprite);
            if(this.button != null)
                button.onClick.RemoveListener(Call);
            this.button = gameObject.GetComponent<Button>();
            this.button?.onClick.AddListener(Call);
        }

        public override void Destroy()
        {
        }
    }

    public class UserActionIcon
    {
        static Sprite defaultIcon;

        public cdk.collaboration.UserAction action { get; private set; }

        public UserActionIcon(cdk.collaboration.UserAction action)
        {
            this.action = action;
        }

        public virtual void Destroy()
        { }

        public void Call() => this.action?.Call();

        static protected async Task<Sprite> LoadIcon(UserActionDto dto)
        {
            if (dto?.icon2D?.variants == null)
                return null;

            Sprite icon;
            try
            {
                var iconResourceFile = UMI3DEnvironmentLoader.AbstractParameters.ChooseVariant(dto.icon2D.variants);
                if (iconResourceFile == null)
                    return null;
                IResourcesLoader loader = UMI3DEnvironmentLoader.AbstractParameters.SelectLoader(iconResourceFile.extension);
                Texture2D texture = (Texture2D)await UMI3DResourcesManager.LoadFile(dto.id, iconResourceFile, loader);
                icon = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
            catch (common.Umi3dException e)
            {
                // in that case, we use the default icon.
                UnityEngine.Debug.Log($"Unable to load icon for \"{dto.name}\" user action.\n${e.Message}");
                icon = defaultIcon;
            }
            return icon;
        }
    }
}