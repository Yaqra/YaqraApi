using YaqraApi.DTOs.Author;
using YaqraApi.DTOs;
using YaqraApi.Models;

namespace YaqraApi.Helpers
{
    public static class ImageHelpers
    {
        public static string AuthorsDir = "Authors";
        public static string ProfilePicturesDir= "ProfilePictures";
        public static string ProfileCoversDir= "ProfileCovers";
        public static string BooksDir= "Books";

        public static string? UploadImage(string dir, string oldPicPath, IFormFile pic, IWebHostEnvironment env)
        {
            var picName = Path.GetFileName(pic.FileName);
            var picExtension = Path.GetExtension(picName);
            var picWithGuid = $"{picName.TrimEnd(picExtension.ToArray())}{Guid.NewGuid().ToString()}{picExtension}";
            var fullDir = Path.Combine(env.WebRootPath, dir);
            if (Directory.Exists(fullDir) == false)
                Directory.CreateDirectory(fullDir);
            var picPath = Path.Combine(fullDir, picWithGuid);

            string? result = null;
            var createPic = Task.Run(async () =>
            {
                using (var stream = new FileStream(picPath, FileMode.Create, FileAccess.Write))
                {
                    await pic.CopyToAsync(stream);
                    result = $"/{dir}/{picWithGuid}";
                }

            });
            var deleteOldPic = Task.Run(() =>
            {
                if (string.IsNullOrEmpty(oldPicPath) == false && File.Exists(oldPicPath))
                    File.Delete(oldPicPath);
            });
            Task.WaitAll(createPic, deleteOldPic);

            return result;
        }
    }
}
