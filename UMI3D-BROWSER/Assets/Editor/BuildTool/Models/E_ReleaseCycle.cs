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
namespace umi3d.browserEditor.BuildTool
{
    public enum E_ReleaseCycle
    {
        Alpha,
        Beta,
        Production
    }

    public static class ReleaseCycleExt
    {
        public static string GetReleaseInitial(this E_ReleaseCycle releaseCycle)
        {
            return releaseCycle switch
            {
                E_ReleaseCycle.Alpha => "a",
                E_ReleaseCycle.Beta => "b",
                E_ReleaseCycle.Production => "p",
                _ => throw new System.NotImplementedException()
            };
        }
    }
}
