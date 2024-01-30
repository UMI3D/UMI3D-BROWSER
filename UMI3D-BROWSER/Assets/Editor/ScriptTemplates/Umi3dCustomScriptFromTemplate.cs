/*
Copyright 2019 - 2023 Inetum

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
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace umi3d.browserEditor.utils
{
    /// <summary>
    /// Static class that provide methods to create a script from a template.
    /// </summary>
    public static class Umi3dCustomScriptFromTemplate
    {
        public static string path;
        public static string partialPath;

        static Umi3dCustomScriptFromTemplate()
        {
            string fileName = $"{nameof(Umi3dCustomScriptFromTemplate)}";
            var assets = AssetDatabase.FindAssets($"t:Script {fileName}");

            path = AssetDatabase.GUIDToAssetPath(assets[0]);
            partialPath = path.Substring(0, path.Length - fileName.Length - 3);
        }

        public static void CreateScriptFromTemplate(string templateName, string scriptName)
        {
            string templateFileName = templateName;
            string templatePath = $"{partialPath}/{templateFileName}";
            string templateContent = System.IO.File.ReadAllText(templatePath);
            string scriptContent = templateContent.Replace("#CURRENT_YEAR#", System.DateTime.Now.Year.ToString());
            string className = scriptName;

            scriptContent = scriptContent.Replace("#SCRIPTNAME#", className);
            scriptName = className + ".cs";
            string scriptPath = $"{partialPath}/{scriptName}";
            System.IO.File.WriteAllText(scriptPath, scriptContent);
            AssetDatabase.ImportAsset(scriptPath);
        }

        [MenuItem(itemName: "Assets/Create/Custom Script/UMI3D Class", isValidateFunction: false, priority: 51)]
        public static void CreateUmi3dClassTemplate()
        {
            CreateScriptFromTemplate("Umi3dClassTemplate.txt", "Umi3dClass");
        }

        [MenuItem(itemName: "Assets/Create/Custom Script/UMI3D Struct", isValidateFunction: false, priority: 51)]
        public static void CreateUmi3DStructTemplate()
        {
            CreateScriptFromTemplate("Umi3dStructTemplate.txt", "Umi3dStruct");
        }

        [MenuItem(itemName: "Assets/Create/Custom Script/UMI3D Enum", isValidateFunction: false, priority: 51)]
        public static void CreateUmi3DEnumTemplate()
        {
            CreateScriptFromTemplate("Umi3dEnumTemplate.txt", "Umi3dEnum");
        }

        [MenuItem(itemName: "Assets/Create/Custom Script/UMI3D Interface", isValidateFunction: false, priority: 51)]
        public static void CreateUmi3DInterfaceTemplate()
        {
            CreateScriptFromTemplate("Umi3dInterfaceTemplate.txt", "Umi3dInterface");
        }

        [MenuItem(itemName: "Assets/Create/Custom Script/Monobehaviour", isValidateFunction: false, priority: 51)]
        public static void CreateMonobehaviourTemplate()
        {
            CreateScriptFromTemplate("MonobehaviourTemplate.txt", "TestClass");
        }

    }
}
