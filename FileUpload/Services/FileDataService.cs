﻿using FileUpload.Models;
using FileUpload.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace FileUpload.Services
{
    public class FileDataService
    {
        private readonly FilesContext _context;

        public FileDataService(FilesContext context)
        {
            _context = context;
        }

        public async Task InsertFileDataAsync(string fileName, string fileType, long fileSize, string date, string time)
        {
            try
            {
                var fileData = new FileData
                {
                    filename = fileName,
                    filetype = fileType,
                    filesize = fileSize,
                    uploaddate = date,
                    uploadtime = time
                };

                _context.files.Add(fileData);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exception
                throw new Exception($"Failed to insert file data: {ex.Message}");
            }
        }



    }
}
