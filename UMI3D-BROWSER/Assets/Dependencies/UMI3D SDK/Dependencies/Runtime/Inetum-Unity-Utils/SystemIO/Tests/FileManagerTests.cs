using System.Collections;
using System.Collections.Generic;
using inetum.unityUtils.systemIO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FileManagerTests
{
    public class FullPathFromPersistentDataPathTest
    {
        string companyName;
        string productName;

        [SetUp]
        public void SetUp()
        {
            companyName = Application.companyName;
            productName = Application.productName;
        }

        [Test]
        public void GivenNull_WhenFullPathFromPersistentDataPath_ThenFullPath()
        {
            // Null;

            string fullPath = FileManager.FullPathFromPersistentDataPath(null);
            
            Assert.True(fullPath.EndsWith($"/AppData/LocalLow/{companyName}/{productName}"));
        }

        [Test]
        public void GivenEmpty_WhenFullPathFromPersistentDataPath_ThenFullPath()
        {
            // Null;

            string fullPath = FileManager.FullPathFromPersistentDataPath("");

            Assert.True(fullPath.EndsWith($"/AppData/LocalLow/{companyName}/{productName}"));
        }

        [Test]
        public void GivenFile_WhenFullPathFromPersistentDataPath_ThenFullPath()
        {
            string fileNameWithExtension = "Test.txt";

            string fullPath = FileManager.FullPathFromPersistentDataPath(fileNameWithExtension);

            Assert.True(fullPath.EndsWith($"/AppData/LocalLow/{companyName}/{productName}/Test.txt"));
        }
    }

    public class ExistsTest
    {
        [Test]
        public void GivenNull_WhenExists_ThenFalse()
        {
            // Null;

            bool result = FileManager.Exists(null);

            Assert.False(result);
        }

        [Test]
        public void GivenEmpty_WhenExists_ThenFalse()
        {
            // Null;

            bool result = FileManager.Exists("");

            Assert.False(result);
        }

        [Test]
        public void GivenWrongValue_WhenExists_ThenFalse()
        {
            string value = "NotAFile";

            bool result = FileManager.Exists(value);

            Assert.False(result);
        }

        [Test]
        public void GivenFile_WhenExists_ThenTrue()
        {
            string directory = "TestDirectory";
            string file = "TestFile.txt";
            string content = "This is a test file";
            FileManager.WriteToFile(content, directory, file, out string path);

            bool result = FileManager.Exists("TestDirectory/TestFile.txt");

            Assert.True(result);

            // Teardown
            System.IO.Directory.Delete(FileManager.FullPathFromPersistentDataPath(directory), true);
        }
    }

    public class WriteToFileTest
    {
        [Test]
        public void GivenNullNullNull_WhenWriteToFile_ThenFalseAndLogError()
        {
            // Null;

            LogAssert.Expect(LogType.Error, $"[FileManager.WriteToFile] Invalide file name ''.");

            bool result = FileManager.WriteToFile(
                null, 
                null, 
                null, 
                out string path
            );

            Assert.False(result);
            Assert.IsNull(path);
        }

        [Test]
        public void GivenNullNullEmpty_WhenWriteToFile_ThenFalseAndLogError()
        {
            // Null;

            LogAssert.Expect(LogType.Error, $"[FileManager.WriteToFile] Invalide file name ''.");

            bool result = FileManager.WriteToFile(
                null,
                null,
                "",
                out string path
            );

            Assert.False(result);
            Assert.IsNull(path);
        }

        [Test]
        public void GivenNullNullValid_WhenWriteToFile_ThenTrue()
        {
            string companyName = Application.companyName;
            string productName = Application.productName;
            string fileName = "FileName.txt";

            bool result = FileManager.WriteToFile(
                null,
                null,
                fileName,
                out string path
            );

            Assert.True(result);
            Assert.True(path.EndsWith($"/AppData/LocalLow/{companyName}/{productName}/FileName.txt"));

            // Teardown
            FileManager.Delete(fileName);
        }

        [Test]
        public void GivenNullDirectoryFile_WhenWriteToFile_ThenFalseAndLogError()
        {
            string companyName = Application.companyName;
            string productName = Application.productName;
            string directory = "TestDirectory";

            bool result = FileManager.WriteToFile(
                null,
                directory,
                "FileName.txt",
                out string path
            );

            Assert.True(result);
            Assert.True(path.EndsWith($"/AppData/LocalLow/{companyName}/{productName}/TestDirectory/FileName.txt"));

            // Teardown
            System.IO.Directory.Delete(FileManager.FullPathFromPersistentDataPath(directory), true);
        }
    }

    public class LoadFromFileTest
    {
        [Test]
        public void GivenNullNull_WhenLoadFromFile_ThenFalseAndLogError()
        {
            // Null;

            LogAssert.Expect(LogType.Error, $"[FileManager.LoadFromFile] Invalide file name ''.");
            bool result = FileManager.LoadFromFile(
                null,
                null,
                out string path,
                out string content
            );

            Assert.False(result);
            Assert.IsNull(path);
            Assert.IsNull(content);
        }

        [Test]
        public void GivenNullFileThatDoesNotExist_WhenLoadFromFile_ThenFalseAndLogError()
        {
            string fileName = "File That Does not exist";

            LogAssert.Expect(LogType.Error, $"[FileManager.LoadFromFile] File at {fileName} doesn't exist.");
            bool result = FileManager.LoadFromFile(
                null,
                fileName,
                out string path,
                out string content
            );

            Assert.False(result);
            Assert.IsNull(path);
            Assert.IsNull(content);
        }

        [Test]
        public void GivenDirectoryFile_WhenLoadFromFile_ThenTrue()
        {
            string companyName = Application.companyName;
            string productName = Application.productName;
            string directory = "TestDirectory";
            string fileName = "TestFile.txt";
            FileManager.WriteToFile("This is a test.", directory, fileName, out string _);

            bool result = FileManager.LoadFromFile(
                directory,
                fileName,
                out string path,
                out string content
            );

            Assert.True(result);
            Assert.True(path.EndsWith($"/AppData/LocalLow/{companyName}/{productName}/TestDirectory/TestFile.txt"));
            Assert.AreEqual(content, "This is a test.");

            // Teardown
            System.IO.Directory.Delete(FileManager.FullPathFromPersistentDataPath(directory), true);
        }
    }

    public class MoveTest
    {

    }

    public class DeleteTest
    {
        [Test]
        public void GivenNull_WhenDelete_ThenFalseAndLogError()
        {
            string fullPath = FileManager.FullPathFromPersistentDataPath(null);

            LogAssert.Expect(LogType.Error, $"[FileManager.Delete] Try to delete a file (at: {fullPath}) that doesn't exist.");
            bool result = FileManager.Delete(null);

            Assert.False(result);
        }

        [Test]
        public void GivenFileThatDoesNotExist_WhenDelete_ThenFalse()
        {
            string fileName = "TestFile";
            string fullPath = FileManager.FullPathFromPersistentDataPath(fileName);
            Assert.False(FileManager.Exists(fileName));

            LogAssert.Expect(LogType.Error, $"[FileManager.Delete] Try to delete a file (at: {fullPath}) that doesn't exist.");
            bool result = FileManager.Delete(fileName);

            Assert.False(result);
        }

        [Test]
        public void GivenFileInDirectory_WhenDeleteDirectory_ThenFalse()
        {
            string directory = "TestDirectory";
            string fileName = "TestFile";
            FileManager.WriteToFile(null, directory, fileName, out string path);

            string fullPath = FileManager.FullPathFromPersistentDataPath(directory);
            LogAssert.Expect(LogType.Error, $"[FileManager.Delete] Try to delete a file but path point to a directory '{fullPath}'.");
            bool result = FileManager.Delete(directory);

            Assert.False(result);
            Assert.True(FileManager.Exists("TestDirectory/TestFile"));

            // Teardown
            System.IO.Directory.Delete(fullPath, true);
        }

        [Test]
        public void GivenFile_WhenDelete_ThenTrue()
        {
            string fileName = "TestFile";
            FileManager.WriteToFile(null, null, fileName, out string path);

            bool result = FileManager.Delete(fileName);

            Assert.True(result);
        }
    }
}
