using System.ComponentModel.DataAnnotations;

namespace FileUpload.Models
{
    public class FileUploadModel
    {
        [Required(ErrorMessage = "File is required")]
        [DataType(DataType.Upload)]
        [Display(Name = "File")]
        public IFormFile File { get; set; }
    }
    public class File
    {
        public int Id { get; set; } // Primary key
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadTime { get; set; }
        public DateTime? DownloadTime { get; set; } // Nullable
    }
}
