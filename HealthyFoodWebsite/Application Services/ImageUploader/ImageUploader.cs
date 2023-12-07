namespace HealthyFoodWebsite.ImageUploader
{
    public class ImageUploader
    {
        // Object Fields Zone
        private readonly IWebHostEnvironment webHostEnvironment;


        // Dependency Injection Zone
        public ImageUploader(IWebHostEnvironment webHostEnvironment) =>
            this.webHostEnvironment = webHostEnvironment;


        // Object Methods Zone
        public async Task<string> UploadImageToServerAsync(IFormFile imageFile, string containingfolderRelativePath)
        {
            string wwwRootPath = webHostEnvironment.WebRootPath;

            string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
            string fileExtension = Path.GetExtension(imageFile.FileName);
            string newFileName = $"{fileName}-{DateTime.Now:ffffff-ss-mm-dd-MM-yyyy}{fileExtension}";

            string fileDirectory = $"{wwwRootPath}{containingfolderRelativePath}";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
            string fileAbsolutePath = Path.Combine(fileDirectory, newFileName);
            using (var fileStream = new FileStream(fileAbsolutePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            string[] relativePathSegments = containingfolderRelativePath.Split("\\", StringSplitOptions.None);
            string fileRelativePath = $"/{relativePathSegments[1]}/{relativePathSegments[2]}/{newFileName}";

            return fileRelativePath;
        }

        public bool DeleteImageFromServerAsync(string imageUri)
        {
            try
            {
                string wwwRootPath = webHostEnvironment.WebRootPath;
                string[] imageUriSegments = imageUri.Split(@"/");
                string fileAbsolutePath = string.Concat(wwwRootPath, $"\\{imageUriSegments[1]}\\{imageUriSegments[2]}\\{imageUriSegments[3]}");

                System.IO.File.Delete(fileAbsolutePath);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
