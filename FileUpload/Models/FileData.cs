using System;

namespace FileUpload.Models
{
    public class FileData
    {
        public int id { get; set; }
        public string filename { get; set; } // Change to lowercase
        public string filetype { get; set; } // Change to lowercase
        public long filesize { get; set; }   // Change to lowercase
        public string uploaddate { get; set; } // Change to lowercase
        public string uploadtime { get; set; } // Change to lowercase
    }
}