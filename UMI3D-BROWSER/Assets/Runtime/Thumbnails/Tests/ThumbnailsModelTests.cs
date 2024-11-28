using NUnit.Framework;
using umi3d.browserRuntime.ui.thumbnails;
using UnityEngine;

public class ThumbnailsModelTests
{
    ThumbnailsModel model;

    [SetUp]
    public void SetUp()
    {
        model = new ThumbnailsModel();
    }

    [TearDown]
    public void TearDown()
    {
        model = null;
    }

    [Test]
    public void GivenSmallMode_WhenToggleMode_ThenLarge()
    {
        Assert.AreEqual(model.contentMode, ThumbnailContentMode.Small);

        model.ToggleContentMode();

        Assert.AreEqual(model.contentMode, ThumbnailContentMode.Large);
    }

    [Test]
    public void GivenLargeMode_WhenToggleMode_ThenSmall()
    {
        model.contentMode = ThumbnailContentMode.Large;
        Assert.AreEqual(model.contentMode, ThumbnailContentMode.Large);

        model.ToggleContentMode();

        Assert.AreEqual(model.contentMode, ThumbnailContentMode.Small);
    }

    [Test]
    public void GivenScroll0_WhenSlideLeft_Then0()
    {
        Assert.AreEqual(model.horizontalSliderValue, 0f);

        model.SlideTowardLeft();

        Assert.AreEqual(model.horizontalSliderValue, 0f);
    }

    [Test]
    public void GivenScroll0_WhenSlideRight_Then()
    {
        Assert.AreEqual(model.horizontalSliderValue, 0f);

        model.SlideTowardRight();

        Assert.AreEqual(model.horizontalSliderValue, 0f);

        Assert.Fail();
    }

    [Test]
    public void GivenScrollNot0_WhenReset_Then0()
    {
        model.horizontalSliderValue = 10f;
        Assert.AreNotEqual(model.horizontalSliderValue, 0f);

        model.ResetSlider();

        Assert.AreEqual(model.horizontalSliderValue, 0f);
    }

    [Test]
    public void GivenScroll0_WhenReset_Then0()
    {
        Assert.AreEqual(model.horizontalSliderValue, 0f);

        model.ResetSlider();

        Assert.AreEqual(model.horizontalSliderValue, 0f);
    }

    [Test]
    public void GivenScroll0_WhenSet_ThenNot0()
    {
        Assert.AreEqual(model.horizontalSliderValue, 0f);

        model.SetHorizontalSliderValue(10f);

        Assert.AreEqual(model.horizontalSliderValue, 10f);
    }

    [Test]
    public void GivenSlideButtonHide_WhenDisplay_ThenDisplay()
    {
        Assert.False(model.areSlideButtonVisible);

        model.DisplaySlideButton(true);

        Assert.True(model.areSlideButtonVisible);
    }

    [Test]
    public void GivenSlideButtonDisplay_WhenHide_ThenHide()
    {
        model.areSlideButtonVisible = true;
        Assert.True(model.areSlideButtonVisible);

        model.DisplaySlideButton(false);

        Assert.False(model.areSlideButtonVisible);
    }

    [Test]
    public void GivenLayout0_WhenSetGrid_ThenNot0()
    {
        Assert.AreEqual(model.gridSize, Vector2.zero);
        Assert.AreEqual(model.gridRowCount, 0);
        Assert.AreEqual(model.gridSpacing, Vector2.zero);

        Vector2 size = new(100f, 100f);
        int rowCount = 1;
        Vector2 spacing = new(30f, 30f);
        model.SetGridProperties(size, rowCount, spacing);

        Assert.AreEqual(model.gridSize, size);
        Assert.AreEqual(model.gridRowCount, rowCount);
        Assert.AreEqual(model.gridSpacing, spacing);
    }

    [Test]
    public void GivenNoThumbnail_WhenAdd_Then1Thumbnail()
    {
        Assert.AreEqual(model.thumbnails.Count, 0);

        ThumbnailModel thumbnail = new();
        model.Add(thumbnail);

        Assert.AreEqual(model.thumbnails.Count, 1);
        Assert.AreEqual(model.thumbnails[0], thumbnail);
    }

    [Test]
    public void Given1Thumbnail_WhenAddSame_Then1Thumbnail()
    {
        ThumbnailModel thumbnail = new();
        model.Add(thumbnail);
        Assert.AreEqual(model.thumbnails.Count, 1);
        Assert.AreEqual(model.thumbnails[0], thumbnail);

        model.Add(thumbnail);

        Assert.AreEqual(model.thumbnails.Count, 1);
        Assert.AreEqual(model.thumbnails[0], thumbnail);
    }

    [Test]
    public void Given1Thumbnail_WhenDelete_Then0Thumbnail()
    {
        ThumbnailModel thumbnail = new();
        model.Add(thumbnail);
        Assert.AreEqual(model.thumbnails.Count, 1);
        Assert.AreEqual(model.thumbnails[0], thumbnail);

        model.Delete(thumbnail);

        Assert.AreEqual(model.thumbnails.Count, 0);
    }
}
