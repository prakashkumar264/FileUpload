using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;

namespace FileUpload.Models
{
    public class FilesContext : DbContext
    {
        public FilesContext(DbContextOptions<FilesContext> options) : base(options) { }

        public DbSet<FileData> files { get; set; }
    }
}
