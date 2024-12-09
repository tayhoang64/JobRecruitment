namespace CVRecruitment.Services
{
    public class FileService
    {
        private readonly string _basePublicPath;

        public FileService()
        {
            _basePublicPath = Path.Combine(Directory.GetCurrentDirectory(), "Public");
        }

        public async Task<string> UploadHtmlAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0 || file.ContentType != "text/html")
            {
                throw new ArgumentException("Invalid HTML file.");
            }

            string targetFolder = Path.Combine(_basePublicPath, folder);
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(targetFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/public/{folder}/{fileName}"; 
        }
        public bool DeleteFile(string folder, string fileName)
        {
            string filePath = Path.Combine(_basePublicPath, folder, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
        }
        public async Task<string> UpdateFileAsync(IFormFile newFile, string folder, string oldFileName)
        {
            DeleteFile(folder, oldFileName);
            return await UploadHtmlAsync(newFile, folder);
        }

        public async Task<string> ReadFileContentAsync(string folder, string fileName)
        {
            string filePath = Path.Combine(_basePublicPath, folder, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.", fileName);
            }

            using (var reader = new StreamReader(filePath))
            {
                return await reader.ReadToEndAsync();
            }
        }

    }
}
