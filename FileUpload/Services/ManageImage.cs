using FileUpload.Helper;
using Microsoft.AspNetCore.StaticFiles;
using System.Runtime.CompilerServices;


namespace FileUpload.Services
{
    public class ManageImage : IManageImage
    {
        private readonly IConfiguration _configuration;
        private readonly FileDataService _fileDataService;

        public ManageImage(IConfiguration configuration, FileDataService fileDataService)
        {
            _configuration = configuration;
            _fileDataService = fileDataService;
        }


        public async Task<string> UploadFile(IFormFile file)
        {
            try
            {
                var allowedFileTypes = _configuration.GetSection("AllowedFileTypes").Get<string[]>();
                var maxFileSizeMB = _configuration.GetValue<int>("MaxFileSizeMB");
                var minFileSizeMB = _configuration.GetValue<int>("MinFileSizeMB");

                // Validate file type
                string fileExtension = Path.GetExtension(file.FileName);
                if (!allowedFileTypes.Contains(fileExtension))
                {
                    throw new ArgumentException("Uploaded file type is not allowed.");
                }

                // Validate file size
                long fileSizeMB = file.Length / (1024 * 1024); 
                if (fileSizeMB > maxFileSizeMB || fileSizeMB < minFileSizeMB)
                {
                    throw new ArgumentException($"Uploaded file size should be between {minFileSizeMB}MB and {maxFileSizeMB}MB.");
                }

                // Proceed with file upload
                var _Filename = file.FileName.Split('.');
                var currentTime = DateTime.Now;
                var datePart = currentTime.ToString("MM/dd/yyyy"); 
                var timePart = currentTime.ToString("HH:mm:ss");   
                var _time = currentTime.ToString("yyyyMMddHHmmss");
                string fileName = _Filename[0] + "_" + _time + fileExtension;
                var filePath = Common.GetFilePath(fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                await _fileDataService.InsertFileDataAsync(_Filename[0] + "_" + _time, _Filename[1], fileSizeMB, datePart, timePart);

                return fileName;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"An error occurred while uploading the file: {ex.Message}");
            }


        }

        public async Task<(byte[], string, string)> DownloadFile(string fileName)
        {
            try
            {
                var filePath = Common.GetFilePath(fileName);
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                return (fileBytes, contentType, Path.GetFileName(filePath));
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("File not found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while downloading the file: {ex.Message}");
            }
        }
    }

}
