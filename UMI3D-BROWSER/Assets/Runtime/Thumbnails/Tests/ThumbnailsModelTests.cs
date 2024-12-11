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

    public class ContentLayoutTest: ThumbnailsModelTests
    {
        [Test]
        public void GivenSmall_WhenSetLarge_ThenLarge()
        {
            model.contentMode = ThumbnailContentMode.Small;
            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Small);

            model.SetContentMode(ThumbnailContentMode.Large);

            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Large);
        }

        [Test]
        public void GivenMiddle_WhenSetLarge_ThenLarge()
        {
            model.contentMode = ThumbnailContentMode.Middle;
            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Middle);

            model.SetContentMode(ThumbnailContentMode.Large);

            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Large);
        }

        [Test]
        public void GivenLarge_WhenSetSmall_ThenSmall()
        {
            model.contentMode = ThumbnailContentMode.Large;
            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Large);

            model.SetContentMode(ThumbnailContentMode.Small);

            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Small);
        }

        [Test]
        public void GivenSmallModeSmallAndLarge_WhenToggleMode_ThenLarge()
        {
            model.primaryContentMode = ThumbnailContentMode.Large;
            model.secondaryContentMode = ThumbnailContentMode.Small;
            model.contentMode = ThumbnailContentMode.Small;
            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Small);

            model.ToggleContentMode();

            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Large);
        }

        [Test]
        public void GivenLargeModeSmallAndLarge_WhenToggleMode_ThenSmall()
        {
            model.primaryContentMode = ThumbnailContentMode.Large;
            model.secondaryContentMode = ThumbnailContentMode.Small;
            model.contentMode = ThumbnailContentMode.Large;
            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Large);

            model.ToggleContentMode();

            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Small);
        }

        [Test]
        public void GivenSmallModeSmallAndMiddle_WhenToggleMode_ThenMiddle()
        {
            model.primaryContentMode = ThumbnailContentMode.Middle;
            model.secondaryContentMode = ThumbnailContentMode.Small;
            model.contentMode = ThumbnailContentMode.Small;
            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Small);

            model.ToggleContentMode();

            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Middle);
        }

        [Test]
        public void GivenMiddleModeSmallAndMiddle_WhenToggleMode_ThenSmall()
        {
            model.primaryContentMode = ThumbnailContentMode.Middle;
            model.secondaryContentMode = ThumbnailContentMode.Small;
            model.contentMode = ThumbnailContentMode.Middle;
            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Middle);

            model.ToggleContentMode();

            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Small);
        }

        [Test]
        public void GivenSmallAndMiddleButLarge_WhenToggleMode_ThenMiddle()
        {
            model.primaryContentMode = ThumbnailContentMode.Middle;
            model.secondaryContentMode = ThumbnailContentMode.Small;
            model.contentMode = ThumbnailContentMode.Large;
            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Large);

            model.ToggleContentMode();

            Assert.AreEqual(model.contentMode, ThumbnailContentMode.Middle);
        }

        [Test]
        public void Given0ThumbnailAndSmall_WhenNumberOfEmpties_Then8()
        {
            model.contentMode = ThumbnailContentMode.Small;

            int emptiesCount = model.NumberOfEmptyToDisplay();

            Assert.AreEqual(emptiesCount, 8);
        }

        [Test]
        public void Given0ThumbnailAndMedium_WhenNumberOfEmpties_Then3()
        {
            model.contentMode = ThumbnailContentMode.Middle;

            int emptiesCount = model.NumberOfEmptyToDisplay();

            Assert.AreEqual(emptiesCount, 3);
        }

        [Test]
        public void Given0ThumbnailAndLarge_WhenNumberOfEmpties_Then2()
        {
            model.contentMode = ThumbnailContentMode.Large;

            int emptiesCount = model.NumberOfEmptyToDisplay();

            Assert.AreEqual(emptiesCount, 2);
        }

        [Test]
        public void Given5ThumbnailsAndSmall_WhenNumberOfEmpties_Then3()
        {
            model.contentMode = ThumbnailContentMode.Small;
            for (int i = 0; i < 5; i++)
            {
                model.thumbnails.Add(new());
            }

            int emptiesCount = model.NumberOfEmptyToDisplay();

            Assert.AreEqual(emptiesCount, 3);
        }

        [Test]
        public void Given2ThumbnailsAndMedium_WhenNumberOfEmpties_Then1()
        {
            model.contentMode = ThumbnailContentMode.Middle;
            for (int i = 0; i < 2; i++)
            {
                model.thumbnails.Add(new());
            }

            int emptiesCount = model.NumberOfEmptyToDisplay();

            Assert.AreEqual(emptiesCount, 1);
        }

        [Test]
        public void Given1ThumbnailAndLarge_WhenNumberOfEmpties_Then1()
        {
            model.contentMode = ThumbnailContentMode.Large;
            model.thumbnails.Add(new());

            int emptiesCount = model.NumberOfEmptyToDisplay();

            Assert.AreEqual(emptiesCount, 1);
        }

        [Test]
        public void Given10ThumbnailsAndSmall_WhenNumberOfEmpties_Then0()
        {
            model.contentMode = ThumbnailContentMode.Small;
            for (int i = 0; i < 10; i++)
            {
                model.thumbnails.Add(new());
            }

            int emptiesCount = model.NumberOfEmptyToDisplay();

            Assert.AreEqual(emptiesCount, 0);
        }

        [Test]
        public void Given5ThumbnailsAndMedium_WhenNumberOfEmpties_Then0()
        {
            model.contentMode = ThumbnailContentMode.Middle;
            for (int i = 0; i < 5; i++)
            {
                model.thumbnails.Add(new());
            }

            int emptiesCount = model.NumberOfEmptyToDisplay();

            Assert.AreEqual(emptiesCount, 0);
        }

        [Test]
        public void Given3ThumbnailsAndLarge_WhenNumberOfEmpties_Then0()
        {
            model.contentMode = ThumbnailContentMode.Large;
            for (int i = 0; i < 3; i++)
            {
                model.thumbnails.Add(new());
            }

            int emptiesCount = model.NumberOfEmptyToDisplay();

            Assert.AreEqual(emptiesCount, 0);
        }

        [Test]
        public void GivenModel_WhenSetSmall_ThenSmallLayout()
        {
            Assert.AreEqual(model.gridSize, Vector2.zero);
            Assert.AreEqual(model.gridRowCount, 0);
            Assert.AreEqual(model.gridSpacing, Vector2.zero);

            model.SetContentMode(ThumbnailContentMode.Small);

            Assert.AreEqual(model.gridSize, model.smallContentModeLayout.size);
            Assert.AreEqual(model.gridRowCount, model.smallContentModeLayout.rowCount);
            Assert.AreEqual(model.gridSpacing, model.smallContentModeLayout.spacing);
        }

        [Test]
        public void GivenModel_WhenSetMiddle_ThenMiddleLayout()
        {
            Assert.AreEqual(model.gridSize, Vector2.zero);
            Assert.AreEqual(model.gridRowCount, 0);
            Assert.AreEqual(model.gridSpacing, Vector2.zero);

            model.SetContentMode(ThumbnailContentMode.Middle);

            Assert.AreEqual(model.gridSize, model.middleContentModeLayout.size);
            Assert.AreEqual(model.gridRowCount, model.middleContentModeLayout.rowCount);
            Assert.AreEqual(model.gridSpacing, model.middleContentModeLayout.spacing);
        }

        [Test]
        public void GivenModel_WhenSetLarge_ThenLargeLayout()
        {
            Assert.AreEqual(model.gridSize, Vector2.zero);
            Assert.AreEqual(model.gridRowCount, 0);
            Assert.AreEqual(model.gridSpacing, Vector2.zero);

            model.SetContentMode(ThumbnailContentMode.Large);

            Assert.AreEqual(model.gridSize, model.largeContentModeLayout.size);
            Assert.AreEqual(model.gridRowCount, model.largeContentModeLayout.rowCount);
            Assert.AreEqual(model.gridSpacing, model.largeContentModeLayout.spacing);
        }
    }

    public class ScrollingTest: ThumbnailsModelTests
    {
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
    }

    public class AddAndRemove : ThumbnailsModelTests
    {
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
}
