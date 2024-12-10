using NUnit.Framework;
using System;
using System.Collections.Generic;
using umi3d.browserRuntime.worldController;

public class WorldControllersFilteringAndSortingTests
{
    WorldControllersFilteringAndSorting filteringAndSorting;
    List<WorldController> worldControllers;
    WorldController worldController1;
    WorldController worldController2;
    WorldController worldController3;
    WorldController worldController4;
    WorldController worldController5;

    [SetUp]
    public void SetUp()
    {
        worldController1 = new()
        {
            url = "test/url1.com",
            name = "Favorite, Today, Today",
            isFavorite = true,
            firstConnection = DateTime.Today,
            lastConnection = DateTime.Now.AddHours(-5)
        };
        worldController2 = new()
        {
            url = "test/url2.com",
            name = "Not Favorite, 3D, Today",
            isFavorite = false,
            firstConnection = DateTime.Today.AddDays(-3),
            lastConnection = DateTime.Now
        };
        worldController3 = new()
        {
            url = "test/url3.com",
            name = "Favorite, 41D, 25D",
            isFavorite = true,
            firstConnection = DateTime.Today.AddDays(-41),
            lastConnection = DateTime.Today.AddDays(-25)
        };
        worldController4 = new()
        {
            url = "test/url4.com",
            name = "Not Favorite, 15D, 15D",
            isFavorite = false,
            firstConnection = DateTime.Today.AddDays(-15),
            lastConnection = DateTime.Today.AddDays(-15)
        };
        worldController5 = new()
        {
            url = "test/url5.com",
            name = "Not Favorite, 410D, 255D",
            isFavorite = false,
            firstConnection = DateTime.Today.AddDays(-410),
            lastConnection = DateTime.Today.AddMonths(-255)
        };
        worldControllers = new()
        {
            worldController1, worldController2, worldController3, worldController4, worldController5
        };
        filteringAndSorting = new(worldControllers);
    }

    [TearDown]
    public void TearDown()
    {
        filteringAndSorting = null;
        worldControllers.Clear();
        worldControllers = null;
    }

    public class FilteringTest : WorldControllersFilteringAndSortingTests
    {
        [Test]
        public void GivenNoFilters_WhenGetFiltered_ThenSameList()
        {
            Assert.IsNull(filteringAndSorting.isFavorite);
            Assert.AreEqual(filteringAndSorting.firstConnectionPeriod, Period.None);
            Assert.AreEqual(filteringAndSorting.lastConnectionPeriod, Period.None);

            List<WorldController> filtered = filteringAndSorting.filteredList;

            Assert.AreEqual(filtered.Count, worldControllers.Count);
            Assert.AreEqual(filtered.Count, 5);
            Assert.AreEqual(filtered[0], worldController1);
            Assert.AreEqual(filtered[1], worldController2);
            Assert.AreEqual(filtered[2], worldController3);
            Assert.AreEqual(filtered[3], worldController4);
            Assert.AreEqual(filtered[4], worldController5);
        }

        [Test]
        public void GivenIsFavorite_WhenGetFiltered_ThenOnlyFavorite()
        {
            filteringAndSorting.isFavorite = true;
            Assert.IsTrue(filteringAndSorting.isFavorite);
            Assert.AreEqual(filteringAndSorting.firstConnectionPeriod, Period.None);
            Assert.AreEqual(filteringAndSorting.lastConnectionPeriod, Period.None);

            List<WorldController> filtered = filteringAndSorting.filteredList;

            Assert.AreEqual(filtered.Count, 2);
            Assert.AreEqual(filtered[0], worldController1);
            Assert.AreEqual(filtered[1], worldController3);
        }

        [Test]
        public void GivenIsNotFavorite_WhenGetFiltered_ThenAllExceptFavorite()
        {
            filteringAndSorting.isFavorite = false;
            Assert.IsFalse(filteringAndSorting.isFavorite);
            Assert.AreEqual(filteringAndSorting.firstConnectionPeriod, Period.None);
            Assert.AreEqual(filteringAndSorting.lastConnectionPeriod, Period.None);

            List<WorldController> filtered = filteringAndSorting.filteredList;

            Assert.AreEqual(filtered.Count, 3);
            Assert.AreEqual(filtered[0], worldController2);
            Assert.AreEqual(filtered[1], worldController4);
            Assert.AreEqual(filtered[2], worldController5);
        }

        [Test]
        public void GivenFirstConnectionToday_WhenGetFiltered_ThenOnlyToday()
        {
            filteringAndSorting.firstConnectionPeriod = Period.Today;
            Assert.IsNull(filteringAndSorting.isFavorite);
            Assert.AreEqual(filteringAndSorting.firstConnectionPeriod, Period.Today);
            Assert.AreEqual(filteringAndSorting.lastConnectionPeriod, Period.None);

            List<WorldController> filtered = filteringAndSorting.filteredList;

            Assert.AreEqual(filtered.Count, 1);
            Assert.AreEqual(filtered[0], worldController1);
        }

        [Test]
        public void GivenFirstConnection7Days_WhenGetFiltered_ThenOnly7Days()
        {
            filteringAndSorting.firstConnectionPeriod = Period.Within7Days;
            Assert.IsNull(filteringAndSorting.isFavorite);
            Assert.AreEqual(filteringAndSorting.firstConnectionPeriod, Period.Within7Days);
            Assert.AreEqual(filteringAndSorting.lastConnectionPeriod, Period.None);

            List<WorldController> filtered = filteringAndSorting.filteredList;

            Assert.AreEqual(filtered.Count, 1);
            Assert.AreEqual(filtered[0], worldController2);
        }

        [Test]
        public void GivenFirstConnection30Days_WhenGetFiltered_ThenOnly30Days()
        {
            filteringAndSorting.firstConnectionPeriod = Period.Within30Days;
            Assert.IsNull(filteringAndSorting.isFavorite);
            Assert.AreEqual(filteringAndSorting.firstConnectionPeriod, Period.Within30Days);
            Assert.AreEqual(filteringAndSorting.lastConnectionPeriod, Period.None);

            List<WorldController> filtered = filteringAndSorting.filteredList;

            Assert.AreEqual(filtered.Count, 1);
            Assert.AreEqual(filtered[0], worldController4);
        }

        [Test]
        public void GivenFirstConnection365Days_WhenGetFiltered_ThenOnly365Days()
        {
            filteringAndSorting.firstConnectionPeriod = Period.Within365Days;
            Assert.IsNull(filteringAndSorting.isFavorite);
            Assert.AreEqual(filteringAndSorting.firstConnectionPeriod, Period.Within365Days);
            Assert.AreEqual(filteringAndSorting.lastConnectionPeriod, Period.None);

            List<WorldController> filtered = filteringAndSorting.filteredList;

            Assert.AreEqual(filtered.Count, 1);
            Assert.AreEqual(filtered[0], worldController3);
        }

        [Test]
        public void GivenFirstConnectionOlderThan365Days_WhenGetFiltered_ThenOnlyOlderThan365Days()
        {
            filteringAndSorting.firstConnectionPeriod = Period.OlderThan365Days;
            Assert.IsNull(filteringAndSorting.isFavorite);
            Assert.AreEqual(filteringAndSorting.firstConnectionPeriod, Period.OlderThan365Days);
            Assert.AreEqual(filteringAndSorting.lastConnectionPeriod, Period.None);

            List<WorldController> filtered = filteringAndSorting.filteredList;

            Assert.AreEqual(filtered.Count, 1);
            Assert.AreEqual(filtered[0], worldController5);
        }

        [Test]
        public void GivenFavoriteAndLastConnectionToday_WhenGetFiltered_ThenOnlyFavoriteAndToday()
        {
            filteringAndSorting.isFavorite = true;
            filteringAndSorting.lastConnectionPeriod = Period.Today;
            Assert.IsTrue(filteringAndSorting.isFavorite);
            Assert.AreEqual(filteringAndSorting.firstConnectionPeriod, Period.None);
            Assert.AreEqual(filteringAndSorting.lastConnectionPeriod, Period.Today);

            List<WorldController> filtered = filteringAndSorting.filteredList;

            Assert.AreEqual(filtered.Count, 1);
            Assert.AreEqual(filtered[0], worldController1);
        }
    }

    public class SortingTest : WorldControllersFilteringAndSortingTests
    {
        [Test]
        public void GivenSortByFirstConnectAsc_WhenGetSorted_ThenSameListButSorted()
        {
            Assert.AreEqual(filteringAndSorting.sortingField, SortingField.FirstConnection);
            Assert.AreEqual(filteringAndSorting.sorting, Sorting.Ascending);

            List<WorldController> sorted = filteringAndSorting.SortedList;

            Assert.AreEqual(sorted.Count, worldControllers.Count);
            Assert.AreEqual(sorted.Count, 5);
            Assert.AreEqual(sorted[0], worldController5);
            Assert.AreEqual(sorted[1], worldController3);
            Assert.AreEqual(sorted[2], worldController4);
            Assert.AreEqual(sorted[3], worldController2);
            Assert.AreEqual(sorted[4], worldController1);
        }

        [Test]
        public void GivenSortByFirstConnectDsc_WhenGetSorted_ThenSameListButSorted()
        {
            filteringAndSorting.sorting = Sorting.Descending;
            Assert.AreEqual(filteringAndSorting.sortingField, SortingField.FirstConnection);
            Assert.AreEqual(filteringAndSorting.sorting, Sorting.Descending);

            List<WorldController> sorted = filteringAndSorting.SortedList;

            Assert.AreEqual(sorted.Count, worldControllers.Count);
            Assert.AreEqual(sorted.Count, 5);
            Assert.AreEqual(sorted[0], worldController1);
            Assert.AreEqual(sorted[1], worldController2);
            Assert.AreEqual(sorted[2], worldController4);
            Assert.AreEqual(sorted[3], worldController3);
            Assert.AreEqual(sorted[4], worldController5);
        }

        [Test]
        public void GivenSortByLastConnectAsc_WhenGetSorted_ThenSameListButSorted()
        {
            filteringAndSorting.sortingField = SortingField.LastConnection;
            Assert.AreEqual(filteringAndSorting.sortingField, SortingField.LastConnection);
            Assert.AreEqual(filteringAndSorting.sorting, Sorting.Ascending);

            List<WorldController> sorted = filteringAndSorting.SortedList;

            Assert.AreEqual(sorted.Count, worldControllers.Count);
            Assert.AreEqual(sorted.Count, 5);
            Assert.AreEqual(sorted[0], worldController5);
            Assert.AreEqual(sorted[1], worldController3);
            Assert.AreEqual(sorted[2], worldController4);
            Assert.AreEqual(sorted[3], worldController1);
            Assert.AreEqual(sorted[4], worldController2);
        }

        [Test]
        public void GivenSortByLastConnectDsc_WhenGetSorted_ThenSameListButSorted()
        {
            filteringAndSorting.sortingField = SortingField.LastConnection;
            filteringAndSorting.sorting = Sorting.Descending;
            Assert.AreEqual(filteringAndSorting.sortingField, SortingField.LastConnection);
            Assert.AreEqual(filteringAndSorting.sorting, Sorting.Descending);

            List<WorldController> sorted = filteringAndSorting.SortedList;

            Assert.AreEqual(sorted.Count, worldControllers.Count);
            Assert.AreEqual(sorted.Count, 5);
            Assert.AreEqual(sorted[0], worldController2);
            Assert.AreEqual(sorted[1], worldController1);
            Assert.AreEqual(sorted[2], worldController4);
            Assert.AreEqual(sorted[3], worldController3);
            Assert.AreEqual(sorted[4], worldController5);
        }
    }

    public class FilteringAndSortingTest : WorldControllersFilteringAndSortingTests
    {
        [Test]
        public void GivenFavoriteAndSortedByFstConAsc_WhenGetFiltered_ThenOnlyFavoriteAndSorted()
        {
            filteringAndSorting.isFavorite = true;
            filteringAndSorting.sortingField = SortingField.FirstConnection;
            filteringAndSorting.sorting = Sorting.Ascending;

            List<WorldController> result = filteringAndSorting.filteredAndSortedList;

            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0], worldController3);
            Assert.AreEqual(result[1], worldController1);
        }

        [Test]
        public void GivenFavoriteAndSortedByFstConDsc_WhenGetFiltered_ThenOnlyFavoriteAndSorted()
        {
            filteringAndSorting.isFavorite = true;
            filteringAndSorting.sortingField = SortingField.FirstConnection;
            filteringAndSorting.sorting = Sorting.Descending;

            List<WorldController> result = filteringAndSorting.filteredAndSortedList;

            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0], worldController1);
            Assert.AreEqual(result[1], worldController3);
        }

        [Test]
        public void GivenNoFavoriteAndSortedByFstConAsc_WhenGetFiltered_ThenOnlyNotFavoriteAndSorted()
        {
            filteringAndSorting.isFavorite = false;
            filteringAndSorting.sortingField = SortingField.FirstConnection;
            filteringAndSorting.sorting = Sorting.Ascending;

            List<WorldController> result = filteringAndSorting.filteredAndSortedList;

            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual(result[0], worldController5);
            Assert.AreEqual(result[1], worldController4);
            Assert.AreEqual(result[2], worldController2);
        }

        [Test]
        public void GivenNoFavoriteAndSortedByFstConDsc_WhenGetFiltered_ThenOnlyNotFavoriteAndSorted()
        {
            filteringAndSorting.isFavorite = false;
            filteringAndSorting.sortingField = SortingField.FirstConnection;
            filteringAndSorting.sorting = Sorting.Descending;

            List<WorldController> result = filteringAndSorting.filteredAndSortedList;

            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual(result[0], worldController2);
            Assert.AreEqual(result[1], worldController4);
            Assert.AreEqual(result[2], worldController5);
        }
    }
}