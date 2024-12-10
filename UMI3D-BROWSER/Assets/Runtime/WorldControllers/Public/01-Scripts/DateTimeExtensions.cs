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
using UnityEngine;

namespace umi3d.browserRuntime.worldController
{
    public static class DateTimeExtensions 
    {
        public static bool IsToday(this DateTime date)
        {
            return date.Date == DateTime.Today;
        }

        public static bool IsWithin7Days(this DateTime date)
        {
            DateTime deadLine = DateTime.Now.AddDays(-7);
            return date >= deadLine && date <= DateTime.Today;
        }

        public static bool IsWithin30Days(this DateTime date)
        {
            DateTime deadLine = DateTime.Now.AddDays(-30);
            return date >= deadLine && date <= DateTime.Today;
        }

        public static bool IsWithin365Days(this DateTime date)
        {
            DateTime deadLine = DateTime.Now.AddDays(-365);
            return date >= deadLine && date <= DateTime.Today;
        }

        public static bool IsOlderThan365Days(this DateTime date)
        {
            DateTime deadLine = DateTime.Now.AddDays(-365);
            return date < deadLine;
        }
    }
}