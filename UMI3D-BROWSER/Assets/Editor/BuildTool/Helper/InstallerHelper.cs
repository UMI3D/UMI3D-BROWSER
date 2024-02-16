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
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace umi3d.browserEditor.BuildTool
{
    public static class InstallerHelper 
    {
        public static void UpdateInstaller(string InstallerPath, VersionDTO version, TargetDto target)
        {
            string appName = BuildToolHelper.GetApplicationName(target);
            string formattedVersion = version.GetFormattedVersion(DateTime.Now.ToString("yyMMdd"));
            string exeName = BuildToolHelper.GetExeName(target, version, true);

            string setupText = File.ReadAllText(InstallerPath);
            setupText = Regex.Replace(
                input: setupText,
                pattern: "#define MyAppName \"(.*)?\"",
                replacement: $"#define MyAppVersion \"{appName}\""
            );
            setupText = Regex.Replace(
                input: setupText, 
                pattern: "#define MyAppVersion \"(.*)?\"", 
                replacement: $"#define MyAppVersion \"{formattedVersion}\""
            );
            setupText = Regex.Replace(
                input: setupText,
                pattern: "#define MyAppExeName \"(.*)?\"",
                replacement: $"#define MyAppVersion \"{exeName}\""
            );
            //setupText = Regex.Replace(
            //    setupText, 
            //    "OutputBaseFilename=" + fileNamePattern, 
            //    $"OutputBaseFilename={fileName}"
            //);
            File.WriteAllText(InstallerPath, setupText);


            //Regex DirReg = new Regex(patternOutputDir, RegexOptions.Multiline | RegexOptions.Singleline);
            //var md = DirReg.Match(setupText);
            //string path = md.Groups[1].Captures[0].Value + "\\" + md.Groups[2].Captures[0].Value + ".exe";
            //return (path, md.Groups[2].Captures[0].Value + ".exe");
        }
    }
}