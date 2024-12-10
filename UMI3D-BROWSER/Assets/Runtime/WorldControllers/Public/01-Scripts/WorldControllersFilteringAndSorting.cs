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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace umi3d.browserRuntime.worldController
{
    public class WorldControllersFilteringAndSorting : IEnumerable<WorldController>
    {
        List<WorldController> worldControllers;

        public List<WorldController> filteredList
        {
            get
            {
                return worldControllers
                    .Where(Filter)
                    .ToList();
            }
        }

        public List<WorldController> SortedList
        {
            get
            {
                List<WorldController> result = new(worldControllers);

                result.Sort(Sort);

                return result;
            }
        }

        public List<WorldController> filteredAndSortedList
        {
            get
            {
                List<WorldController> result = worldControllers
                    .Where(Filter)
                    .ToList();

                result.Sort(Sort);

                return result;
            }
        }

        #region Filtering

        public bool? isFavorite;

        public Period firstConnectionPeriod;
        public Period lastConnectionPeriod;


        public bool Filter(WorldController worldController)
        {
            bool result = true;
            result &= FilterByFavorite(worldController);
            result &= FilterByFirstConnection(worldController);
            result &= FilterByLastConnection(worldController);

            return result;
        }

        bool FilterByFavorite(WorldController worldController)
        {
            if (!isFavorite.HasValue)
            {
                return true;
            }

            if (isFavorite.Value && !worldController.isFavorite)
            {
                return false;
            }
            else if (!isFavorite.Value && worldController.isFavorite)
            {
                return false;
            }

            return true;
        }

        bool FilterByFirstConnection(WorldController worldController)
        {
            return FilterByDate(firstConnectionPeriod, worldController.firstConnection);
        }

        bool FilterByLastConnection(WorldController worldController)
        {
            return FilterByDate(lastConnectionPeriod, worldController.lastConnection);
        }

        bool FilterByDate(Period period, System.DateTime date)
        {
            if (period == Period.None)
            {
                return true;
            }

            bool hasTodayFlag = period.HasFlag(Period.Today);
            bool isToday = date.IsToday();
            if (hasTodayFlag && isToday)
            {
                return true;
            }

            bool hasWithin7DaysFlag = period.HasFlag(Period.Within7Days);
            bool isWithin7Days = date.IsWithin7Days();
            if (hasWithin7DaysFlag && isWithin7Days && !isToday)
            {
                return true;
            }

            bool hasWithin30DaysFlag = period.HasFlag(Period.Within30Days);
            bool isWithin30Days = date.IsWithin30Days();
            if (hasWithin30DaysFlag && isWithin30Days && !isWithin7Days && !isToday)
            {
                return true;
            }

            bool hasWithin365DaysFlag = period.HasFlag(Period.Within365Days);
            bool isWithin365Days = date.IsWithin365Days();
            if (hasWithin365DaysFlag && isWithin365Days && !isWithin30Days && !isWithin7Days && !isToday)
            {
                return true;
            }

            bool hasOlderThan365DaysFlag = period.HasFlag(Period.OlderThan365Days);
            bool isOlderThan365Days = date.IsOlderThan365Days();
            if (hasOlderThan365DaysFlag && isOlderThan365Days)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Sorting

        public SortingField sortingField;
        public Sorting sorting;

        public int Sort(WorldController wc1, WorldController w2)
        {
            switch (sortingField)
            {
                case SortingField.FirstConnection:
                    return SortByDate(wc1.firstConnection, w2.firstConnection);
                case SortingField.LastConnection:
                    return SortByDate(wc1.lastConnection, w2.lastConnection);
                default:
                    UnityEngine.Debug.Log($"Error: Unhandled case.");
                    return 0;
            }
        }

        int SortByDate(System.DateTime wc1, System.DateTime wc2)
        {
            if (wc1 < wc2)
            {
                return sorting == Sorting.Ascending ? -1 : 1;
            }
            else if (wc1 == wc2)
            {
                return 0;
            }
            else
            {
                return sorting == Sorting.Ascending ? 1 : -1;
            }
        }

        #endregion

        public WorldControllersFilteringAndSorting(List<WorldController> worldControllers) 
        {
            this.worldControllers = worldControllers;
        }

        public IEnumerator<WorldController> GetEnumerator()
        {
            return ((IEnumerable<WorldController>)filteredList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)filteredList).GetEnumerator();
        }
    }
}