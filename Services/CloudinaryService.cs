using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace CVRecruitment.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            Account account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "JobRecruitment/" + folder 
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }

        public async Task DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deleteParams);
        }

        public async Task<string> UploadHtmlAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0 || file.ContentType != "text/html")
            {
                throw new ArgumentException("Invalid HTML file.");
            }

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "JobRecruitment/" + folder,
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
        public async Task<string> SaveFileWithExtension(string extension, string folder, string content)
        {
            var fileBytes = System.Text.Encoding.UTF8.GetBytes(content);
            using var stream = new MemoryStream(fileBytes);
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription("file." + extension, stream),
                Folder = folder
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.Url.ToString();
            }

            throw new Exception("File upload failed");
        }

    }
}
