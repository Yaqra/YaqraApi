using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaqraApi.Helpers;

namespace YaqraApi.Tests.HelperTests
{
    [TestFixture]
    public class ImageUploadServiceTests
    {
        private Mock<IWebHostEnvironment> _mockEnv;
        private Mock<IFormFile> _mockFormFile;
        [SetUp]
        public void SetUp()
        {
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockFormFile = new Mock<IFormFile>();
        }
        [Test]
        public void UploadImage_ShouldReturnNull_WhenPicIsNull()
        {
            //arrange
            string dir = "images";
            string oldPicPath = null;

            //act 
            var result = ImageHelpers.UploadImage(dir, oldPicPath, null, _mockEnv.Object);

            //assert
            Assert.IsNull(result);
        }
        [Test]
        public void UploadImage_ShouldCreateDir_WhenItDoesNotExist()
        {
            string dir = "images";
            string oldPicPath = null;
            string webRootPath = "wwwroot";
            string expectedDir = Path.Combine(webRootPath, dir);
            _mockEnv.Setup(env => env.WebRootPath).Returns(webRootPath);
            _mockFormFile.Setup(f => f.FileName).Returns("tests.jpg");

            var result = ImageHelpers.UploadImage(dir, oldPicPath, _mockFormFile.Object, _mockEnv.Object);

            Assert.IsTrue(Directory.Exists(expectedDir));
        }
        [Test]
        public void UploadImage_ShouldReturnCorrectPath_WhenImageIsUploaded()
        {
            string dir = "images";
            string oldPicPath = null;
            string webRootPath = "wwwroot";
            string picName = "test.jpg";
            string picExtension = ".jpg";
            string expectedDirectory = Path.Combine(webRootPath, dir);

            _mockEnv.Setup(env => env.WebRootPath).Returns(webRootPath);
            _mockFormFile.Setup(f => f.FileName).Returns(picName);
            _mockFormFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            var result = ImageHelpers.UploadImage(dir, oldPicPath, _mockFormFile.Object, _mockEnv.Object);

            Assert.IsNotNull(result);
            StringAssert.StartsWith($"/{dir}/", result);
            StringAssert.EndsWith(picExtension, result);

            // Cleanup
            var filePath = Path.Combine(webRootPath, result.TrimStart('/'));
            if (File.Exists(filePath)) File.Delete(filePath);
            Directory.Delete(expectedDirectory, true);
        }
        [Test]
        public void UploadImage_ShouldDeleteOldImage_WhenOldPicPathExists()
        {
            // Arrange
            string dir = "images";
            string oldPicPath = Path.Combine("wwwroot", "oldPic.jpg");
            string webRootPath = "wwwroot";

            _mockEnv.Setup(env => env.WebRootPath).Returns(webRootPath);
            _mockFormFile.Setup(f => f.FileName).Returns("newPic.jpg");
            _mockFormFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            File.WriteAllText(oldPicPath, "old image content");

            // Act
            var result = ImageHelpers.UploadImage(dir, oldPicPath, _mockFormFile.Object, _mockEnv.Object);

            // Assert
            Assert.IsFalse(File.Exists(oldPicPath));

            // Cleanup
            var expectedDirectory = Path.Combine(webRootPath, dir);
            if (Directory.Exists(expectedDirectory))
            {
                var newPicPath = Path.Combine(expectedDirectory, result.TrimStart('/'));
                if (File.Exists(newPicPath)) File.Delete(newPicPath);
                Directory.Delete(expectedDirectory, true);
            }
        }
    }
}
